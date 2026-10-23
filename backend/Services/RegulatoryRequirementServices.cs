using System.Globalization;
using System.Text.Json;
using System.Text.RegularExpressions;
using Microsoft.EntityFrameworkCore;
using Sace.Api.Domain;
using Sace.Api.Infrastructure;

namespace Sace.Api.Services;

public interface IRegulatoryRequirementEngine {
  Task<IReadOnlyList<RegulatoryRequirementEvaluation>> EvaluateAsync(Guid operationId, Guid? auditId, Guid userId, CancellationToken ct = default);
  Task<RegulatoryRequirementEvaluation> ReviewAsync(Guid operationId, Guid evaluationId, RegulatoryManualOutcome outcome, string note, Guid userId, CancellationToken ct = default);
}

public class RegulatoryRequirementEngine(SaceDbContext db) : IRegulatoryRequirementEngine {
  static readonly HashSet<string> SupportedFields = new(StringComparer.OrdinalIgnoreCase) {
    DocumentFieldCodes.TariffFraction, DocumentFieldCodes.Nico, DocumentFieldCodes.OperationType, "CUSTOMS_REGIME", DocumentFieldCodes.OriginCountry,
    DocumentFieldCodes.DestinationCountry, DocumentFieldCodes.CustomsOffice, DocumentFieldCodes.Description, DocumentFieldCodes.Quantity, DocumentFieldCodes.Unit,
    DocumentFieldCodes.CommercialValue, DocumentFieldCodes.Currency
  };

  public async Task<IReadOnlyList<RegulatoryRequirementEvaluation>> EvaluateAsync(Guid operationId, Guid? auditId, Guid userId, CancellationToken ct = default) {
    var operation = await db.TradeOperations.AsNoTracking().AsSplitQuery().Include(x => x.Documents).ThenInclude(x => x.DocumentType).Include(x => x.Documents).ThenInclude(x => x.Fields).FirstOrDefaultAsync(x => x.Id == operationId, ct) ?? throw new KeyNotFoundException("Operación no encontrada.");
    if (auditId is not null && !await db.Audits.AnyAsync(x => x.Id == auditId && x.TradeOperationId == operationId, ct)) throw new InvalidOperationException("La auditoría no corresponde a la operación.");
    var validUser = await db.Users.AnyAsync(x => x.Id == userId, ct) ? userId : (Guid?)null;
    db.SystemAuditLogs.Add(Log(validUser, "REGULATORY_EVALUATION_STARTED", "TradeOperation", operationId, $"Fecha de operación: {operation.OperationDate:yyyy-MM-dd}."));

    var all = await db.RegulatoryRequirements.AsNoTracking().AsSplitQuery().Include(x => x.Conditions).Include(x => x.EvidenceRequirements).Include(x=>x.TariffEntries).Include(x=>x.SourceCitations).ThenInclude(x=>x.OfficialSourceCitation).ThenInclude(x=>x.OfficialLegalSourceVersion).ThenInclude(x=>x.OfficialLegalSource)
      .Where(x => x.Status != RegulatoryRequirementStatus.RETIRED && (x.EffectiveFrom == null || x.EffectiveFrom <= operation.OperationDate) && (x.EffectiveTo == null || x.EffectiveTo >= operation.OperationDate)).ToListAsync(ct);
    var selected = all.GroupBy(x => x.Code, StringComparer.OrdinalIgnoreCase).Select(g => g.Where(x => x.Status == RegulatoryRequirementStatus.VERIFIED).OrderByDescending(x => x.Version).FirstOrDefault() ?? g.OrderByDescending(x => x.Version).First()).OrderBy(x => x.Code).ToList();
    var results = selected.Select(requirement => Evaluate(operation, requirement, auditId)).ToList();
    db.RegulatoryRequirementEvaluations.AddRange(results);
    db.SystemAuditLogs.Add(Log(validUser, "REGULATORY_EVALUATION_COMPLETED", "TradeOperation", operationId, $"{results.Count} requisito(s) evaluado(s) con vigencia a la fecha de operación."));
    await db.SaveChangesAsync(ct);
    return results;
  }

  public async Task<RegulatoryRequirementEvaluation> ReviewAsync(Guid operationId, Guid evaluationId, RegulatoryManualOutcome outcome, string note, Guid userId, CancellationToken ct = default) {
    if (string.IsNullOrWhiteSpace(note)) throw new InvalidOperationException("La nota de revisión es obligatoria.");
    if (!await db.Users.AnyAsync(x => x.Id == userId, ct)) throw new InvalidOperationException("El usuario revisor no existe.");
    var evaluation = await db.RegulatoryRequirementEvaluations.Include(x => x.RegulatoryRequirement).Include(x => x.Evidence).ThenInclude(x => x.OperationDocument).FirstOrDefaultAsync(x => x.Id == evaluationId && x.TradeOperationId == operationId, ct) ?? throw new KeyNotFoundException("Evaluación regulatoria no encontrada.");
    evaluation.ManualOutcome = outcome; evaluation.ManualNote = note.Trim(); evaluation.ReviewedBy = userId; evaluation.ReviewedAt = DateTime.UtcNow;
    db.SystemAuditLogs.Add(Log(userId, "REGULATORY_MANUAL_REVIEW", nameof(RegulatoryRequirementEvaluation), evaluation.Id, $"Resultado manual: {outcome}. Resultado automático conservado: {evaluation.AutomatedOutcome}."));
    await db.SaveChangesAsync(ct); return evaluation;
  }

  static RegulatoryRequirementEvaluation Evaluate(TradeOperation operation, RegulatoryRequirement requirement, Guid? auditId) {
    var conditionResults = new List<ConditionResult>();
    RegulatoryApplicabilityOutcome outcome;
    var scopeMatches = requirement.OperationScope == RegulatoryOperationScope.BOTH || requirement.OperationScope == RegulatoryOperationScope.IMPORT && operation.OperationType == OperationType.Importacion || requirement.OperationScope == RegulatoryOperationScope.EXPORT && operation.OperationType == OperationType.Exportacion;
    var tariffMatches=requirement.TariffEntries.Where(x=>TariffMatches(operation,x)).ToList();
    if (!scopeMatches) outcome = RegulatoryApplicabilityOutcome.DOES_NOT_APPLY;
    else if(requirement.TariffEntries.Count>0&&tariffMatches.Count==0)outcome=RegulatoryApplicabilityOutcome.DOES_NOT_APPLY;
    else if(tariffMatches.Any(x=>x.QualificationType!=QualificationType.NONE))outcome=RegulatoryApplicabilityOutcome.MANUAL_REVIEW;
    else if (requirement.Conditions.Count == 0) outcome = requirement.TariffEntries.Count>0?RegulatoryApplicabilityOutcome.APPLIES:RegulatoryApplicabilityOutcome.MANUAL_REVIEW;
    else {
      foreach (var condition in requirement.Conditions.OrderBy(x => x.ConditionGroup).ThenBy(x => x.Sequence)) conditionResults.Add(EvaluateCondition(operation, condition));
      var groups = conditionResults.GroupBy(x => x.Group, StringComparer.OrdinalIgnoreCase).Select(g => g.ToList()).ToList();
      if (groups.Any(g => g.All(x => x.State == ConditionState.Match))) outcome = RegulatoryApplicabilityOutcome.APPLIES;
      else if (groups.All(g => g.Any(x => x.State == ConditionState.NoMatch))) outcome = RegulatoryApplicabilityOutcome.DOES_NOT_APPLY;
      else if (conditionResults.Any(x => x.State == ConditionState.Manual)) outcome = RegulatoryApplicabilityOutcome.MANUAL_REVIEW;
      else outcome = RegulatoryApplicabilityOutcome.INSUFFICIENT_DATA;
    }
    if (requirement.Status == RegulatoryRequirementStatus.DRAFT || requirement.RequiresManualReview && outcome == RegulatoryApplicabilityOutcome.APPLIES) outcome = RegulatoryApplicabilityOutcome.MANUAL_REVIEW;

    var evidenceRows = BuildEvidence(operation, requirement, outcome);
    var evidenceStatus = EvidenceStatus(requirement, evidenceRows, outcome);
    var explanation = Explanation(requirement, outcome, evidenceStatus, scopeMatches, conditionResults,tariffMatches);
    var snapshot = JsonSerializer.Serialize(new {
      operationId = operation.Id, operationDate = operation.OperationDate.Date, requirement = new { requirement.Code, requirement.Version, requirement.Status },
      conditions = conditionResults.Select(x => new { x.FieldCode, @operator = x.Operator.ToString(), x.RawValue, x.NormalizedValue, x.ExpectedValue, x.ExpectedValue2, x.Group, result = x.State.ToString() }),
      tariffEntries=tariffMatches.Select(x=>new{x.RawTariffFraction,x.NormalizedTariffFraction,x.Nico,x.QualificationType,x.QualificationText}),
      officialSources=requirement.SourceCitations.Select(x=>new{x.OfficialSourceCitation.CitationLabel,x.OfficialSourceCitation.OfficialUrl,source=x.OfficialSourceCitation.OfficialLegalSourceVersion.OfficialLegalSource.Title,version=x.OfficialSourceCitation.OfficialLegalSourceVersion.VersionLabel,status=x.OfficialSourceCitation.OfficialLegalSourceVersion.VerificationStatus}),
      expectedEvidence = requirement.EvidenceRequirements.Select(x => new { x.DocumentTypeCode, x.EvidenceRole, x.IsRequired, x.AlternativeGroup }),
      evidence = evidenceRows.Select(x => new { x.DocumentTypeCode, x.OperationDocumentId, x.IsPresent, x.IsRequired, x.AlternativeGroup })
    });
    var evaluation = new RegulatoryRequirementEvaluation { TradeOperationId = operation.Id, AuditId = auditId, RegulatoryRequirementId = requirement.Id, RequirementCode = requirement.Code, RequirementVersion = requirement.Version, EvaluationDate = operation.OperationDate.Date, AutomatedOutcome = outcome, EvidenceStatus = evidenceStatus, Explanation = explanation, Confidence = Confidence(outcome, conditionResults), ManualReviewRequired = outcome is RegulatoryApplicabilityOutcome.MANUAL_REVIEW or RegulatoryApplicabilityOutcome.INSUFFICIENT_DATA || evidenceStatus is RegulatoryEvidenceStatus.PARTIAL or RegulatoryEvidenceStatus.MISSING_EVIDENCE, SnapshotJson = snapshot };
    evaluation.Evidence.AddRange(evidenceRows); return evaluation;
  }

  static ConditionResult EvaluateCondition(TradeOperation operation, RegulatoryRequirementCondition condition) {
    if (!SupportedFields.Contains(condition.FieldCode)) return Result(condition, null, null, ConditionState.Manual);
    var raw = GetValue(operation, condition.FieldCode);
    var normalized = Normalize(condition.FieldCode, raw);
    if (condition.Operator == RegulatoryConditionOperator.PRESENT) return Result(condition, raw, normalized, string.IsNullOrWhiteSpace(raw) ? ConditionState.NoMatch : ConditionState.Match);
    if (condition.Operator == RegulatoryConditionOperator.NOT_PRESENT) return Result(condition, raw, normalized, string.IsNullOrWhiteSpace(raw) ? ConditionState.Match : ConditionState.NoMatch);
    if (string.IsNullOrWhiteSpace(raw) || normalized is null) return Result(condition, raw, normalized, ConditionState.Missing);
    var expected = Normalize(condition.FieldCode, condition.ExpectedValue);
    if (condition.Operator != RegulatoryConditionOperator.NUMERIC_RANGE && expected is null) return Result(condition, raw, normalized, ConditionState.Missing);
    var state = condition.Operator switch {
      RegulatoryConditionOperator.EQUALS => normalized == expected ? ConditionState.Match : ConditionState.NoMatch,
      RegulatoryConditionOperator.NOT_EQUALS => normalized != expected ? ConditionState.Match : ConditionState.NoMatch,
      RegulatoryConditionOperator.IN => Split(condition.ExpectedValue).Select(x => Normalize(condition.FieldCode, x)).Contains(normalized) ? ConditionState.Match : ConditionState.NoMatch,
      RegulatoryConditionOperator.NOT_IN => !Split(condition.ExpectedValue).Select(x => Normalize(condition.FieldCode, x)).Contains(normalized) ? ConditionState.Match : ConditionState.NoMatch,
      RegulatoryConditionOperator.PREFIX => expected is not null && normalized.StartsWith(expected, StringComparison.Ordinal) ? ConditionState.Match : ConditionState.NoMatch,
      RegulatoryConditionOperator.NUMERIC_RANGE => NumericRange(raw, condition.ExpectedValue, condition.ExpectedValue2),
      RegulatoryConditionOperator.CONTAINS_NORMALIZED => expected is null ? ConditionState.Missing : normalized == expected ? ConditionState.Match : normalized.Contains(expected, StringComparison.Ordinal) ? ConditionState.Manual : ConditionState.NoMatch,
      _ => ConditionState.Manual
    };
    return Result(condition, raw, normalized, state);
  }

  static List<RegulatoryRequirementEvaluationEvidence> BuildEvidence(TradeOperation operation, RegulatoryRequirement requirement, RegulatoryApplicabilityOutcome outcome) {
    if (outcome != RegulatoryApplicabilityOutcome.APPLIES) return [];
    var rows = new List<RegulatoryRequirementEvaluationEvidence>();
    foreach (var expected in requirement.EvidenceRequirements) {
      var documents = operation.Documents.Where(x => x.DocumentType.Code.Equals(expected.DocumentTypeCode, StringComparison.OrdinalIgnoreCase)).ToList();
      if (documents.Count == 0) rows.Add(new() { RegulatoryEvidenceRequirementId = expected.Id, DocumentTypeCode = expected.DocumentTypeCode, EvidenceRole = expected.EvidenceRole, IsRequired = expected.IsRequired, IsPresent = false, AlternativeGroup = expected.AlternativeGroup, Notes = "No se encontró evidencia documental clasificada." });
      else foreach (var document in documents) rows.Add(new() { RegulatoryEvidenceRequirementId = expected.Id, OperationDocumentId = document.Id, DocumentTypeCode = expected.DocumentTypeCode, EvidenceRole = expected.EvidenceRole, IsRequired = expected.IsRequired, IsPresent = true, AlternativeGroup = expected.AlternativeGroup, Notes = "Documento clasificado disponible; su suficiencia jurídica no se determina en esta capa." });
    }
    return rows;
  }

  static RegulatoryEvidenceStatus EvidenceStatus(RegulatoryRequirement requirement, List<RegulatoryRequirementEvaluationEvidence> rows, RegulatoryApplicabilityOutcome outcome) {
    if (outcome == RegulatoryApplicabilityOutcome.DOES_NOT_APPLY) return RegulatoryEvidenceStatus.NOT_REQUIRED;
    if (outcome == RegulatoryApplicabilityOutcome.INSUFFICIENT_DATA) return RegulatoryEvidenceStatus.INSUFFICIENT_DATA;
    if (outcome == RegulatoryApplicabilityOutcome.MANUAL_REVIEW) return RegulatoryEvidenceStatus.MANUAL_REVIEW;
    var required = requirement.EvidenceRequirements.Where(x => x.IsRequired).ToList(); if (required.Count == 0) return RegulatoryEvidenceStatus.NOT_REQUIRED;
    var independentOk = required.Where(x => string.IsNullOrWhiteSpace(x.AlternativeGroup)).All(x => rows.Any(r => r.RegulatoryEvidenceRequirementId == x.Id && r.IsPresent));
    var alternativesOk = required.Where(x => !string.IsNullOrWhiteSpace(x.AlternativeGroup)).GroupBy(x => x.AlternativeGroup).All(g => g.Any(x => rows.Any(r => r.RegulatoryEvidenceRequirementId == x.Id && r.IsPresent)));
    if (independentOk && alternativesOk) return RegulatoryEvidenceStatus.COMPLETE;
    return rows.Any(x => x.IsPresent) ? RegulatoryEvidenceStatus.PARTIAL : RegulatoryEvidenceStatus.MISSING_EVIDENCE;
  }

  static string Explanation(RegulatoryRequirement requirement, RegulatoryApplicabilityOutcome outcome, RegulatoryEvidenceStatus evidence, bool scopeMatches, List<ConditionResult> results,List<RegulatoryTariffEntry> tariffMatches) {
    var lines = new List<string> { $"Requisito {requirement.Code} v{requirement.Version} ({requirement.Status}).", $"Aplicabilidad automática: {outcome}. Evidencia documental: {evidence}." };
    if (!scopeMatches) lines.Add("El alcance IMPORT/EXPORT no corresponde a la operación.");
    foreach (var group in results.GroupBy(x => x.Group)) lines.Add($"Grupo {group.Key}: {string.Join(", ", group.Select(x => $"{x.FieldCode}={x.State}"))}.");
    foreach(var entry in tariffMatches.Where(x=>x.QualificationType!=QualificationType.NONE))lines.Add($"Acotación oficial {entry.QualificationType}: {entry.QualificationText} Requiere determinación humana.");
    foreach(var citation in requirement.SourceCitations.Select(x=>x.OfficialSourceCitation))lines.Add($"Fuente oficial: {citation.OfficialLegalSourceVersion.OfficialLegalSource.Title}, {citation.OfficialLegalSourceVersion.VersionLabel}, {citation.CitationLabel}, {citation.OfficialUrl}.");
    if(requirement.SourceCitations.Any(x=>x.OfficialSourceCitation.OfficialLegalSourceVersion.VerificationStatus==SourceVerificationStatus.SUPERSEDED))lines.Add("Advertencia: la fuente está SUPERSEDED; se conserva por vigencia histórica.");
    if (requirement.Status == RegulatoryRequirementStatus.DRAFT) lines.Add("Regla DEMO/DRAFT: requiere revisión humana y no constituye una determinación jurídica.");
    lines.Add("Resultado LEGAL: No evaluado en esta capa."); return string.Join(Environment.NewLine, lines);
  }

  static string? GetValue(TradeOperation o, string field) => field.ToUpperInvariant() switch {
    "TARIFF_FRACTION" => o.TariffFraction, "NICO" => o.Nico, "OPERATION_TYPE" => o.OperationType == OperationType.Importacion ? "IMPORT" : "EXPORT", "CUSTOMS_REGIME" => o.CustomsRegime,
    "ORIGIN_COUNTRY" => o.OriginCountry, "DESTINATION_COUNTRY" => o.DestinationCountry, "CUSTOMS_OFFICE" => o.CustomsOffice, "DESCRIPTION" => o.GoodsDescription,
    "QUANTITY" => o.Quantity?.ToString(CultureInfo.InvariantCulture), "UNIT" => o.Unit, "COMMERCIAL_VALUE" => o.CommercialValue?.ToString(CultureInfo.InvariantCulture), "CURRENCY" => o.Currency,
    _ => o.Documents.SelectMany(x => x.Fields).Where(x => x.FieldCode.Equals(field, StringComparison.OrdinalIgnoreCase)).OrderByDescending(x => x.UpdatedAt).Select(x => x.NormalizedValue ?? x.RawValue).FirstOrDefault()
  };
  static bool TariffMatches(TradeOperation operation,RegulatoryTariffEntry entry){
    var date=operation.OperationDate; if(entry.Status is SourceVerificationStatus.REJECTED or SourceVerificationStatus.STAGED||entry.EffectiveFrom>date||entry.EffectiveTo.HasValue&&entry.EffectiveTo<date)return false;
    if(entry.OperationScope==RegulatoryOperationScope.IMPORT&&operation.OperationType!=OperationType.Importacion||entry.OperationScope==RegulatoryOperationScope.EXPORT&&operation.OperationType!=OperationType.Exportacion)return false;
    if(RegulatoryCatalogRules.NormalizeTariff(operation.TariffFraction)!=entry.NormalizedTariffFraction)return false;
    if(!string.IsNullOrWhiteSpace(entry.Nico)&&!entry.Nico.Equals(operation.Nico?.Trim(),StringComparison.OrdinalIgnoreCase))return false;
    if(!string.IsNullOrWhiteSpace(entry.CustomsRegime)&&!entry.CustomsRegime.Equals(operation.CustomsRegime,StringComparison.OrdinalIgnoreCase))return false;
    if(!MatchesCountry(entry.OriginCountryCodes,operation.OriginCountry)||!MatchesCountry(entry.DestinationCountryCodes,operation.DestinationCountry))return false;return true;
  }
  static bool MatchesCountry(string? allowed,string value)=>string.IsNullOrWhiteSpace(allowed)||Split(allowed).Select(Country).Where(x=>x!=null).Contains(Country(value));
  static string? Normalize(string field, string? value) {
    if (string.IsNullOrWhiteSpace(value)) return null; var normalized = Collapse(value).ToUpperInvariant();
    if (field.Equals(DocumentFieldCodes.TariffFraction, StringComparison.OrdinalIgnoreCase) || field.Equals(DocumentFieldCodes.Nico, StringComparison.OrdinalIgnoreCase)) return Regex.Replace(normalized, @"[\.\-/\s]", "");
    if (field.Equals(DocumentFieldCodes.OriginCountry, StringComparison.OrdinalIgnoreCase) || field.Equals(DocumentFieldCodes.DestinationCountry, StringComparison.OrdinalIgnoreCase)) return Country(normalized);
    return normalized;
  }
  static string? Country(string value) => value switch { "MX" or "MEX" or "MEXICO" or "MÉXICO" => "MX", "US" or "USA" or "ESTADOS UNIDOS" => "US", "CA" or "CAN" or "CANADA" or "CANADÁ" => "CA", "CN" or "CHN" or "CHINA" => "CN", "DE" or "DEU" or "ALEMANIA" => "DE", _ when Regex.IsMatch(value, "^[A-Z]{2}$") => value, _ => null };
  static string Collapse(string value) => Regex.Replace(value.Trim(), @"\s+", " ");
  static IEnumerable<string> Split(string? value) => (value ?? "").Split([',', ';', '|'], StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries);
  static ConditionState NumericRange(string raw, string? low, string? high) => decimal.TryParse(raw, NumberStyles.Any, CultureInfo.InvariantCulture, out var n) && decimal.TryParse(low, NumberStyles.Any, CultureInfo.InvariantCulture, out var a) && decimal.TryParse(high, NumberStyles.Any, CultureInfo.InvariantCulture, out var b) ? n >= a && n <= b ? ConditionState.Match : ConditionState.NoMatch : ConditionState.Missing;
  static double Confidence(RegulatoryApplicabilityOutcome outcome, List<ConditionResult> results) => outcome switch { RegulatoryApplicabilityOutcome.APPLIES or RegulatoryApplicabilityOutcome.DOES_NOT_APPLY => results.Count == 0 ? .75 : 1, RegulatoryApplicabilityOutcome.MANUAL_REVIEW => .5, _ => .25 };
  static ConditionResult Result(RegulatoryRequirementCondition c, string? raw, string? normalized, ConditionState state) => new(c.FieldCode, c.Operator, raw, normalized, c.ExpectedValue, c.ExpectedValue2, string.IsNullOrWhiteSpace(c.ConditionGroup) ? "DEFAULT" : c.ConditionGroup, state);
  static SystemAuditLog Log(Guid? user, string action, string entity, Guid id, string details) => new() { UserId = user, Action = action, EntityType = entity, EntityId = id.ToString(), Details = details };
  enum ConditionState { Match, NoMatch, Missing, Manual }
  sealed record ConditionResult(string FieldCode, RegulatoryConditionOperator Operator, string? RawValue, string? NormalizedValue, string? ExpectedValue, string? ExpectedValue2, string Group, ConditionState State);
}
