using System.Globalization;
using System.Text.Json;
using System.Text.RegularExpressions;
using Microsoft.EntityFrameworkCore;
using Sace.Api.Domain;
using Sace.Api.Infrastructure;

namespace Sace.Api.Services;

public interface ILegalAuditEngine {
  Task<LegalAuditSummary> ExecuteAsync(Guid operationId, Guid userId, DateTime? evaluationDate = null, CancellationToken ct = default);
}

public sealed record LegalAuditSummary(Guid AuditId, DateTime EvaluationDate, string NormativeVersion, int Compliant, int NonCompliant, int NotApplicable, int InsufficientEvidence, int ManualReview, double? AutomatedCompliancePercent, double CoveragePercent);

public sealed class LegalAuditEngine(SaceDbContext db) : ILegalAuditEngine {
  private sealed record Evaluation(LegalOutcome Outcome, string Explanation, object Evidence, object Compared, object Calculation, bool Manual = false, string? DocumentCode = null);

  public async Task<LegalAuditSummary> ExecuteAsync(Guid operationId, Guid userId, DateTime? evaluationDate = null, CancellationToken ct = default) {
    var op = await db.TradeOperations
      .Include(x => x.Documents).ThenInclude(x => x.DocumentType)
      .Include(x => x.Documents).ThenInclude(x => x.References)
      .Include(x => x.Adjustments)
      .SingleOrDefaultAsync(x => x.Id == operationId, ct) ?? throw new KeyNotFoundException("Operación no localizada.");
    var date = (evaluationDate ?? op.OperationDate).Date;
    var candidates = await db.NormativeRules.Where(x => x.IsActive && x.IsLegalRule && x.RuleCategory == RuleCategory.LEGAL && x.EffectiveFrom <= date && (x.EffectiveTo == null || date <= x.EffectiveTo)).OrderByDescending(x => x.Version).ToListAsync(ct);
    var rules = candidates.GroupBy(x => x.Code).Select(x => x.First()).OrderBy(x => x.Code).ToList();
    var audit = new Audit {
      TradeOperationId = op.Id, Period = op.Period, AuditType = AuditType.Legal, Status = AuditStatus.Running,
      CreatedBy = userId, EvaluationDate = date, EvaluationDateBasis = EvaluationDateBasis.OPERATION_DATE,
      NormativeVersionLabel = $"RGCE 2026 + modificaciones vigentes al {date:yyyy-MM-dd}", Number = $"AUD-LEGAL-{await db.Audits.CountAsync(ct) + 1:D6}"
    };
    db.Audits.Add(audit);
    await db.SaveChangesAsync(ct);

    foreach (var rule in rules) {
      var evaluation = Evaluate(rule, op);
      var result = new AuditResult {
        AuditId = audit.Id, RuleId = rule.Code, NormativeRuleId = rule.Id, NormativeRuleVersion = rule.Version,
        Outcome = evaluation.Outcome, Status = MapStatus(evaluation.Outcome), Message = evaluation.Explanation, Explanation = evaluation.Explanation,
        EvidenceJson = JsonSerializer.Serialize(evaluation.Evidence), ComparedValuesJson = JsonSerializer.Serialize(evaluation.Compared), CalculationJson = JsonSerializer.Serialize(evaluation.Calculation),
        RequiresManualReview = evaluation.Manual || rule.RequiresManualReview || evaluation.Outcome == LegalOutcome.MANUAL_REVIEW,
        EvaluatedAt = DateTime.UtcNow, IsIncludedInLegalCompliance = rule.IsIncludedInLegalCompliance,
        DocumentTypeId = ResolveDocumentTypeId(evaluation.DocumentCode, op)
      };
      db.AuditResults.Add(result);
      db.SystemAuditLogs.Add(new SystemAuditLog { UserId = userId, Action = "LEGAL_RULE_EVALUATED", EntityType = "AuditResult", EntityId = result.Id.ToString(), Details = $"{rule.Code} v{rule.Version}: {evaluation.Outcome}." });
      if (evaluation.Outcome == LegalOutcome.NON_COMPLIANT) await CreateFindingAsync(op, audit, rule, result, userId, ct);
    }

    audit.Status = AuditStatus.Completed; audit.CompletedAt = DateTime.UtcNow;
    if (audit.Results.Any(x => x.Outcome == LegalOutcome.NON_COMPLIANT)) op.Status = OperationStatus.WithFindings;
    op.UpdatedAt = DateTime.UtcNow;
    db.SystemAuditLogs.Add(new SystemAuditLog { UserId = userId, Action = "LEGAL_AUDIT_EXECUTED", EntityType = "TradeOperation", EntityId = op.Id.ToString(), Details = $"{audit.Number}; fecha evaluada {date:yyyy-MM-dd}; {rules.Count} reglas LEGAL." });
    await db.SaveChangesAsync(ct);
    return Summarize(audit);
  }

  private Evaluation Evaluate(NormativeRule rule, TradeOperation op) => rule.Code switch {
    "LEGAL-LA36-001" => Pedimento(op),
    "LEGAL-LA36A-VAL-001" => ValueDocument(op),
    "LEGAL-RGCE-318-001" => ThresholdInvoice(op, ConfigDecimal(rule, "thresholdUsd", 300m)),
    "LEGAL-RGCE-318-002" => InvoiceFields(op),
    "LEGAL-ANEXO22-INV-001" => InvoiceConsistency(op, ConfigDecimal(rule, "amountTolerance", .01m)),
    "LEGAL-RGCE-1916-001" => CoveConsistency(op),
    "LEGAL-LA36A-TRANSPORT-001" => Transport(op),
    "LEGAL-LA65-001" => CustomsValue(op, ConfigDecimal(rule, "toleranceMxn", 1m)),
    "LEGAL-ORIGIN-001" => Origin(op),
    "LEGAL-EDOC-001" => EDocument(op),
    "LEGAL-RRNA-001" => Rrna(op),
    _ => new(LegalOutcome.MANUAL_REVIEW, "La regla legal configurada no tiene evaluador automático en esta versión.", new { }, new { }, new { }, true)
  };

  private static Evaluation Pedimento(TradeOperation op) {
    var docs = Docs(op, "PEDIMENTO");
    if (docs.Count == 0) return Insufficient("No se localizó evidencia documental del pedimento.", "PEDIMENTO");
    var numbers = docs.Where(x => !string.IsNullOrWhiteSpace(x.DocumentNumber)).Select(x => new { x.Id, x.DocumentNumber }).ToList();
    if (numbers.Count == 0) return Manual("El archivo de pedimento existe, pero no hay número estructurado para cotejar.", new { documents = docs.Select(x => x.Id) }, "PEDIMENTO");
    var matches = numbers.Any(x => Normalize(x.DocumentNumber) == Normalize(op.PedimentoNumber));
    return new(matches ? LegalOutcome.COMPLIANT : LegalOutcome.NON_COMPLIANT, matches ? "El número de pedimento del expediente coincide con la evidencia estructurada." : "El número de pedimento del expediente no coincide con la evidencia estructurada.", new { documents = numbers }, new { operation = op.PedimentoNumber, documents = numbers.Select(x => x.DocumentNumber) }, new { }, false, "PEDIMENTO");
  }

  private static Evaluation ValueDocument(TradeOperation op) {
    var docs = Docs(op, "COMMERCIAL_INVOICE", "INVOICE", "COVE_ACK");
    var coveRefs = op.Documents.SelectMany(x => x.References).Where(x => x.ReferenceType.Equals("COVE", StringComparison.OrdinalIgnoreCase)).ToList();
    if (docs.Count == 0 && coveRefs.Count == 0) return Insufficient("No se localizó factura, documento equivalente ni acuse COVE para acreditar el valor.", "COMMERCIAL_INVOICE");
    return new(LegalOutcome.COMPLIANT, "Se localizó evidencia documental de valor; el contenido detallado se evalúa en reglas complementarias.", new { documents = docs.Select(x => new { x.Id, x.DocumentType.Code }), coveReferences = coveRefs.Select(x => x.Value) }, new { }, new { }, false, "COMMERCIAL_INVOICE");
  }

  private static Evaluation ThresholdInvoice(TradeOperation op, decimal threshold) {
    if (!string.Equals(op.Currency, "USD", StringComparison.OrdinalIgnoreCase) || op.CommercialValue is null) return Insufficient("No existe valor comercial expresado en USD suficiente para aplicar el umbral de la regla 3.1.8.", "COMMERCIAL_INVOICE");
    if (op.CommercialValue <= threshold) return new(LegalOutcome.NOT_APPLICABLE, $"El valor comercial no excede USD {threshold}; el supuesto configurado no aplica.", new { }, new { op.CommercialValue, op.Currency }, new { thresholdUsd = threshold }, false, "COMMERCIAL_INVOICE");
    var docs = Docs(op, "COMMERCIAL_INVOICE", "INVOICE");
    return docs.Count > 0
      ? new(LegalOutcome.COMPLIANT, $"El valor excede USD {threshold} y se localizó factura o documento equivalente.", new { documents = docs.Select(x => x.Id) }, new { op.CommercialValue, op.Currency }, new { thresholdUsd = threshold }, false, "COMMERCIAL_INVOICE")
      : Insufficient("El valor excede USD 300, pero no se localizó factura o documento equivalente.", "COMMERCIAL_INVOICE", new { op.CommercialValue, op.Currency });
  }

  private static Evaluation InvoiceFields(TradeOperation op) {
    var docs = Docs(op, "COMMERCIAL_INVOICE", "INVOICE");
    if (docs.Count == 0) return Insufficient("No se localizó factura o documento equivalente.", "COMMERCIAL_INVOICE");
    var metadata = docs.Select(Metadata).FirstOrDefault(x => x.Count > 0);
    if (metadata is null || metadata.Count == 0) return Manual("La factura existe, pero el PDF/archivo no ofrece datos estructurados suficientes para revisar los campos de la regla 3.1.8.", new { documents = docs.Select(x => x.Id) }, "COMMERCIAL_INVOICE");
    var required = new[] { "documentnumber", "date", "recipient", "description", "quantity", "supplier", "total" };
    var missing = required.Where(x => !HasAny(metadata, x switch { "documentnumber" => ["documentnumber", "invoice", "folio"], "date" => ["date", "documentdate", "fecha"], "recipient" => ["recipient", "receptor"], "description" => ["description", "descripcion"], "quantity" => ["quantity", "cantidad"], "supplier" => ["supplier", "emisor"], _ => ["total", "commercialvalue", "valor"] })).ToList();
    return missing.Count == 0
      ? new(LegalOutcome.COMPLIANT, "Los campos estructurados mínimos configurados para la factura están presentes.", new { document = docs[0].Id, fields = metadata.Keys }, new { missing }, new { }, false, "COMMERCIAL_INVOICE")
      : Manual($"La factura requiere revisión humana; faltan o no pudieron extraerse: {string.Join(", ", missing)}.", new { document = docs[0].Id, fields = metadata.Keys }, "COMMERCIAL_INVOICE", new { missing });
  }

  private static Evaluation InvoiceConsistency(TradeOperation op, decimal tolerance) {
    var docs = Docs(op, "COMMERCIAL_INVOICE", "INVOICE");
    if (docs.Count == 0) return Insufficient("No hay factura para cotejar contra los datos declarados.", "COMMERCIAL_INVOICE");
    var doc = docs[0]; var meta = Metadata(doc);
    var docNumber = First(meta, "documentnumber", "invoice", "folio") ?? doc.DocumentNumber;
    var currency = First(meta, "currency", "moneda");
    var totalText = First(meta, "total", "commercialvalue", "valor");
    decimal? total = decimal.TryParse(totalText, NumberStyles.Any, CultureInfo.InvariantCulture, out var parsed) ? parsed : null;
    if (string.IsNullOrWhiteSpace(op.InvoiceNumber) || string.IsNullOrWhiteSpace(op.Currency) || op.CommercialValue is null || string.IsNullOrWhiteSpace(docNumber) || string.IsNullOrWhiteSpace(currency) || total is null)
      return Insufficient("Faltan datos estructurados de número, moneda o valor para realizar el cotejo completo de factura.", "COMMERCIAL_INVOICE", new { operation = new { op.InvoiceNumber, op.Currency, op.CommercialValue }, document = new { docNumber, currency, total } });
    var numberOk = Normalize(op.InvoiceNumber) == Normalize(docNumber); var currencyOk = Normalize(op.Currency) == Normalize(currency); var valueOk = Math.Abs(op.CommercialValue.Value - total.Value) <= tolerance;
    var ok = numberOk && currencyOk && valueOk;
    return new(ok ? LegalOutcome.COMPLIANT : LegalOutcome.NON_COMPLIANT, ok ? "Número, moneda y valor de factura coinciden con la operación." : "Existe discrepancia entre la factura y los datos declarados de la operación.", new { document = doc.Id }, new { number = new { operation = op.InvoiceNumber, document = docNumber, matches = numberOk }, currency = new { operation = op.Currency, document = currency, matches = currencyOk }, value = new { operation = op.CommercialValue, document = total, matches = valueOk }, op.Incoterm }, new { tolerance }, false, "COMMERCIAL_INVOICE");
  }

  private static Evaluation CoveConsistency(TradeOperation op) {
    var evidence = op.Documents.SelectMany(x => x.References.Where(r => r.ReferenceType.Equals("COVE", StringComparison.OrdinalIgnoreCase)).Select(r => new { r.OperationDocumentId, r.Value, r.Source })).Where(x => !string.IsNullOrWhiteSpace(x.Value)).ToList();
    var values = evidence.Select(x => Normalize(x.Value)).Append(Normalize(op.CoveNumber)).Where(x => x.Length > 0).Distinct().ToList();
    if (string.IsNullOrWhiteSpace(op.CoveNumber) || evidence.Count == 0) return Insufficient("Se requieren el COVE capturado en la operación y al menos una referencia documental para cotejar.", "COVE_ACK", new { op.CoveNumber, references = evidence });
    var ok = values.Count == 1;
    return new(ok ? LegalOutcome.COMPLIANT : LegalOutcome.NON_COMPLIANT, ok ? "Las referencias COVE disponibles son consistentes." : "Las referencias COVE disponibles no coinciden.", new { references = evidence }, new { operation = op.CoveNumber, values }, new { }, false, "COVE_ACK");
  }

  private static Evaluation Transport(TradeOperation op) {
    var docs = Docs(op, "TRANSPORT_DOCUMENT", "PACKING_LIST");
    if (docs.Count == 0) return Insufficient("No se localizó evidencia de transporte en el expediente.", "TRANSPORT_DOCUMENT");
    var structured = docs.Any(x => !string.IsNullOrWhiteSpace(x.DocumentNumber) || Metadata(x).Count > 0);
    return structured
      ? new(LegalOutcome.COMPLIANT, "Se localizó documento de transporte con referencia estructurada; su pertinencia jurídica permanece sujeta al modo de transporte.", new { documents = docs.Select(x => x.Id) }, new { }, new { }, true, "TRANSPORT_DOCUMENT")
      : Manual("Existe un documento de transporte, pero su tipo y pertinencia requieren revisión humana.", new { documents = docs.Select(x => x.Id) }, "TRANSPORT_DOCUMENT");
  }

  private static Evaluation CustomsValue(TradeOperation op, decimal toleranceMxn) {
    if (op.CommercialValue is null || string.IsNullOrWhiteSpace(op.Currency)) return Insufficient("Falta valor comercial o moneda para calcular el valor en aduana.", "INCREMENTABLES_DECLARATION");
    if (op.Adjustments.Any(x => x.AdjustmentType == AdjustmentType.UNKNOWN || x.LegalQualificationStatus == LegalQualificationStatus.PENDING_REVIEW)) return Manual("Hay conceptos cuya calificación como incrementable está pendiente de revisión.", new { adjustments = op.Adjustments.Select(x => new { x.Concept, x.Amount, x.AdjustmentType, x.LegalQualificationStatus }) }, "INCREMENTABLES_DECLARATION");
    var incrementables = op.Adjustments.Where(x => x.AdjustmentType == AdjustmentType.INCREMENTABLE).ToList();
    if (incrementables.Any(x => !string.Equals(x.Currency, op.Currency, StringComparison.OrdinalIgnoreCase))) return Manual("Existen incrementables en una moneda distinta y no hay tipo de cambio por concepto.", new { incrementables }, "INCREMENTABLES_DECLARATION");
    if (op.ExchangeRate is null || op.DeclaredCustomsValueMxn is null) return Insufficient("Falta tipo de cambio o valor en aduana declarado para completar el cotejo aritmético.", "INCREMENTABLES_DECLARATION");
    var additions = incrementables.Sum(x => x.Amount); var baseValue = op.CommercialValue.Value + additions; var calculated = decimal.Round(baseValue * op.ExchangeRate.Value, 2); var difference = Math.Abs(calculated - op.DeclaredCustomsValueMxn.Value); var ok = difference <= toleranceMxn;
    return new(ok ? LegalOutcome.COMPLIANT : LegalOutcome.NON_COMPLIANT, ok ? "El valor en aduana declarado coincide con el cálculo dentro de la tolerancia configurada." : "El valor en aduana declarado difiere del cálculo fuera de la tolerancia configurada.", new { adjustments = incrementables.Select(x => new { x.Concept, x.Amount, x.Currency }) }, new { op.CommercialValue, op.ExchangeRate, op.DeclaredCustomsValueMxn }, new { additions, baseValue, calculated, difference, toleranceMxn }, false, "INCREMENTABLES_DECLARATION");
  }

  private static Evaluation Origin(TradeOperation op) => !op.PreferentialTreatment
    ? new(LegalOutcome.NOT_APPLICABLE, "La operación no declara trato arancelario preferencial; la revisión de origen preferencial no aplica.", new { }, new { op.PreferentialTreatment }, new { }, false, "ORIGIN_CERT")
    : Manual("La operación declara trato preferencial; la validez material de la prueba de origen requiere revisión humana.", new { documents = Docs(op, "ORIGIN_CERT").Select(x => x.Id) }, "ORIGIN_CERT");

  private static Evaluation EDocument(TradeOperation op) {
    var refs = op.Documents.SelectMany(x => x.References.Where(r => r.ReferenceType.Equals("EDOCUMENT", StringComparison.OrdinalIgnoreCase)).Select(r => new { documentId = x.Id, r.Value, r.Source })).ToList();
    if (refs.Select(x => x.documentId).Distinct().Count() < 2) return Insufficient("La referencia e-Document no está presente en dos documentos relacionados para poder cotejarla.", "EDOCUMENT_ACK", new { references = refs });
    var values = refs.Select(x => Normalize(x.Value)).Distinct().ToList(); var ok = values.Count == 1;
    return new(ok ? LegalOutcome.COMPLIANT : LegalOutcome.NON_COMPLIANT, ok ? "La referencia e-Document coincide entre los documentos relacionados." : "La referencia e-Document difiere entre los documentos relacionados.", new { references = refs }, new { values }, new { }, false, "EDOCUMENT_ACK");
  }

  private static Evaluation Rrna(TradeOperation op) => new(LegalOutcome.MANUAL_REVIEW, "La determinación de regulaciones y restricciones no arancelarias requiere revisión humana con la fracción, NICO, descripción, origen, régimen e identificadores disponibles.", new { documents = op.Documents.Select(x => new { x.Id, x.DocumentType.Code }) }, new { op.TariffFraction, op.Nico, op.GoodsDescription, op.OriginCountry, op.CustomsRegime, identifiers = op.Documents.SelectMany(x => x.References).Select(x => new { x.ReferenceType, x.Value }) }, new { }, true);

  private async Task CreateFindingAsync(TradeOperation op, Audit audit, NormativeRule rule, AuditResult result, Guid userId, CancellationToken ct) {
    if (await db.Findings.AnyAsync(x => x.TradeOperationId == op.Id && x.RuleId == rule.Code && x.Status != FindingStatus.Resolved, ct)) return;
    var documentTypeId = result.DocumentTypeId ?? await db.DocumentTypes.Where(x => x.Code == "OTHER").Select(x => x.Id).FirstAsync(ct);
    var responsible = await db.DocumentTypes.Where(x => x.Id == documentTypeId).Select(x => x.CorrectionResponsible).FirstAsync(ct);
    var finding = new Finding { AuditId = audit.Id, TradeOperationId = op.Id, DocumentTypeId = documentTypeId, RuleId = rule.Code, Number = $"HAL-{await db.Findings.CountAsync(ct) + 1:D6}", Severity = Severity.High, Description = result.Explanation ?? result.Message, CorrectionResponsible = responsible };
    db.Findings.Add(finding); db.FindingHistories.Add(new FindingHistory { FindingId = finding.Id, NewStatus = FindingStatus.Pending, Comment = $"Hallazgo creado por regla LEGAL {rule.Code} v{rule.Version}.", ChangedBy = userId });
  }

  private static LegalAuditSummary Summarize(Audit audit) {
    var included = audit.Results.Where(x => x.IsIncludedInLegalCompliance).ToList();
    var compliant = included.Count(x => x.Outcome == LegalOutcome.COMPLIANT); var non = included.Count(x => x.Outcome == LegalOutcome.NON_COMPLIANT); var na = included.Count(x => x.Outcome == LegalOutcome.NOT_APPLICABLE); var insufficient = included.Count(x => x.Outcome == LegalOutcome.INSUFFICIENT_EVIDENCE); var manual = included.Count(x => x.Outcome == LegalOutcome.MANUAL_REVIEW);
    var decided = compliant + non; var applicable = included.Count - na;
    return new(audit.Id, audit.EvaluationDate!.Value, audit.NormativeVersionLabel!, compliant, non, na, insufficient, manual, decided == 0 ? null : Math.Round(compliant * 100d / decided, 1), applicable == 0 ? 0 : Math.Round(decided * 100d / applicable, 1));
  }

  private static Guid? ResolveDocumentTypeId(string? code, TradeOperation op) => string.IsNullOrWhiteSpace(code) ? null : op.Documents.FirstOrDefault(x => x.DocumentType.Code == code)?.DocumentTypeId;
  private static List<OperationDocument> Docs(TradeOperation op, params string[] codes) => op.Documents.Where(x => codes.Contains(x.DocumentType.Code, StringComparer.OrdinalIgnoreCase)).OrderByDescending(x => x.UploadedAt).ToList();
  private static string Normalize(string? value) => Regex.Replace(value ?? "", "[^A-Za-z0-9]", "").ToUpperInvariant();
  private static ResultStatus MapStatus(LegalOutcome value) => value switch { LegalOutcome.COMPLIANT => ResultStatus.Valid, LegalOutcome.NON_COMPLIANT => ResultStatus.Invalid, LegalOutcome.NOT_APPLICABLE => ResultStatus.NotApplicable, LegalOutcome.INSUFFICIENT_EVIDENCE => ResultStatus.Missing, _ => ResultStatus.Warning };
  private static Evaluation Insufficient(string explanation, string? code = null, object? compared = null) => new(LegalOutcome.INSUFFICIENT_EVIDENCE, explanation, new { }, compared ?? new { }, new { }, false, code);
  private static Evaluation Manual(string explanation, object evidence, string? code = null, object? compared = null) => new(LegalOutcome.MANUAL_REVIEW, explanation, evidence, compared ?? new { }, new { }, true, code);
  private static Dictionary<string, string> Metadata(OperationDocument doc) {
    if (string.IsNullOrWhiteSpace(doc.ExtractedMetadataJson)) return [];
    try { using var json = JsonDocument.Parse(doc.ExtractedMetadataJson); return json.RootElement.ValueKind == JsonValueKind.Object ? json.RootElement.EnumerateObject().ToDictionary(x => NormalizeKey(x.Name), x => x.Value.ToString(), StringComparer.OrdinalIgnoreCase) : []; } catch (JsonException) { return []; }
  }
  private static string NormalizeKey(string value) => Regex.Replace(value, "[^A-Za-z0-9]", "").ToLowerInvariant();
  private static bool HasAny(Dictionary<string, string> values, params string[] keys) => keys.Any(x => values.TryGetValue(NormalizeKey(x), out var value) && !string.IsNullOrWhiteSpace(value));
  private static string? First(Dictionary<string, string> values, params string[] keys) => keys.Select(NormalizeKey).Where(values.ContainsKey).Select(x => values[x]).FirstOrDefault(x => !string.IsNullOrWhiteSpace(x));
  private static decimal ConfigDecimal(NormativeRule rule, string name, decimal fallback) { try { using var json = JsonDocument.Parse(rule.ConfigurationJson); return json.RootElement.TryGetProperty(name, out var value) && value.TryGetDecimal(out var parsed) ? parsed : fallback; } catch (JsonException) { return fallback; } }
}
