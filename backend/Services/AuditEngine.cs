using Microsoft.EntityFrameworkCore;
using Sace.Api.Contracts;
using Sace.Api.Domain;
using Sace.Api.Infrastructure;

namespace Sace.Api.Services;

public interface IAuditEngine { Task<Audit> ExecuteAsync(Guid operationId, AuditType type, Guid userId, CancellationToken ct = default); Task<List<RequirementDto>> GetRequirementsAsync(Guid operationId, CancellationToken ct = default); }
public sealed class AuditEngine(SaceDbContext db) : IAuditEngine {
  public async Task<List<RequirementDto>> GetRequirementsAsync(Guid operationId, CancellationToken ct = default) {
    var op = await db.TradeOperations.Include(x => x.Documents).ThenInclude(x => x.DocumentType).SingleOrDefaultAsync(x => x.Id == operationId, ct) ?? throw new KeyNotFoundException("Operación no localizada.");
    var rules = await db.NormativeRules.Where(x => x.IsActive && x.EffectiveFrom <= op.OperationDate && (x.EffectiveTo == null || op.OperationDate <= x.EffectiveTo)).OrderByDescending(x => x.Version).ToListAsync(ct);
    var active = rules.GroupBy(x => x.Code).Select(x => x.First()).ToDictionary(x => x.Code); var types = await db.DocumentTypes.Where(x => x.IsActive).ToDictionaryAsync(x => x.Code, ct); var result = new List<RequirementDto>();
    Add(["PEDIMENTO"], "DEMO-001", true); Add(["COMMERCIAL_INVOICE", "INVOICE"], "DEMO-002", true); Add(["ORIGIN_CERT"], "DEMO-003", op.PreferentialTreatment); Add(["IMMEX_DOCUMENT", "IMMEX_DOC"], "DEMO-004", op.IsImmex);
    return result;
    void Add(string[] typeCodes, string ruleCode, bool condition) {
      var candidates = typeCodes.Where(types.ContainsKey).Select(x => types[x]).ToList(); if (candidates.Count == 0 || !active.ContainsKey(ruleCode)) return; var type = candidates[0];
      var ids = candidates.Select(x => x.Id).ToHashSet(); var latest = op.Documents.Where(x => ids.Contains(x.DocumentTypeId)).OrderByDescending(x => x.UploadedAt).ThenByDescending(x => x.Version).FirstOrDefault();
      var status = !condition ? ResultStatus.NotApplicable : latest is null ? ResultStatus.Missing : latest.ValidationStatus == ValidationStatus.Invalid ? ResultStatus.Invalid : latest.ValidationStatus == ValidationStatus.Pending ? ResultStatus.Warning : ResultStatus.Valid;
      var message = status switch { ResultStatus.Valid => "Documento localizado y válido.", ResultStatus.Missing => "Documento requerido no localizado.", ResultStatus.Invalid => "Documento presente con error.", ResultStatus.Warning => "Documento pendiente de validación.", _ => "La regla no aplica a esta operación." };
      result.Add(new(type.Id, type.Name, type.Code, condition, status, ruleCode, message, latest?.Version, latest?.Id));
    }
  }

  public async Task<Audit> ExecuteAsync(Guid operationId, AuditType type, Guid userId, CancellationToken ct = default) {
    var op = await db.TradeOperations.SingleOrDefaultAsync(x => x.Id == operationId, ct) ?? throw new KeyNotFoundException("Operación no localizada."); var requirements = await GetRequirementsAsync(operationId, ct);
    var audit = new Audit { TradeOperationId = operationId, AuditType = type, CreatedBy = userId, Number = $"AUD-{(await db.Audits.CountAsync(ct) + 1):D6}" }; db.Audits.Add(audit); await db.SaveChangesAsync(ct);
    foreach (var requirement in requirements) {
      var auditResult = new AuditResult { AuditId = audit.Id, DocumentTypeId = requirement.DocumentTypeId, Status = requirement.Status, Message = requirement.Message, RuleId = requirement.RuleCode }; db.AuditResults.Add(auditResult); audit.Results.Add(auditResult);
      if (requirement.Status is ResultStatus.Missing or ResultStatus.Invalid) {
        var findingRule = requirement.Status == ResultStatus.Invalid ? "DEMO-005" : requirement.RuleCode;
        var exists = await db.Findings.AnyAsync(x => x.TradeOperationId == operationId && x.DocumentTypeId == requirement.DocumentTypeId && x.RuleId == findingRule && x.Status != FindingStatus.Resolved, ct);
        if (!exists) { var docType = await db.DocumentTypes.FindAsync([requirement.DocumentTypeId], ct); var finding = new Finding { AuditId = audit.Id, TradeOperationId = operationId, DocumentTypeId = requirement.DocumentTypeId, RuleId = findingRule, Number = $"HAL-{(await db.Findings.CountAsync(ct) + 1):D6}", Severity = findingRule is "DEMO-003" or "DEMO-005" ? Severity.High : Severity.Medium, Description = requirement.Message, CorrectionResponsible = docType!.CorrectionResponsible }; db.Findings.Add(finding); db.FindingHistories.Add(new FindingHistory { FindingId = finding.Id, NewStatus = FindingStatus.Pending, Comment = "Hallazgo creado por regla DEMO.", ChangedBy = userId }); db.SystemAuditLogs.Add(new SystemAuditLog { UserId = userId, Action = "FINDING_CREATED", EntityType = "Finding", EntityId = finding.Id.ToString(), Details = $"{finding.Number}: {findingRule} para {docType.Name}." }); }
      } else if (requirement.Status == ResultStatus.Valid) {
        var findings = await db.Findings.Where(x => x.TradeOperationId == operationId && x.DocumentTypeId == requirement.DocumentTypeId && x.Status != FindingStatus.Resolved).ToListAsync(ct);
        foreach (var f in findings) { var previous = f.Status; f.Status = FindingStatus.Resolved; f.ResolvedAt = DateTime.UtcNow; f.UpdatedAt = DateTime.UtcNow; db.FindingHistories.Add(new FindingHistory { FindingId = f.Id, PreviousStatus = previous, NewStatus = FindingStatus.Resolved, Comment = "Resuelto automáticamente después de re-auditoría satisfactoria.", ChangedBy = userId }); }
      }
    }
    var eDocumentRefs = await db.DocumentReferences.AsNoTracking().Where(x => x.OperationDocument.TradeOperationId == operationId && x.ReferenceType == "EDOCUMENT").Include(x => x.OperationDocument).ThenInclude(x => x.DocumentType).ToListAsync(ct);
    foreach (var group in eDocumentRefs.GroupBy(x => x.Value, StringComparer.OrdinalIgnoreCase)) {
      var linked = group.Select(x => x.OperationDocumentId).Distinct().Count() > 1;
      var result = new AuditResult { AuditId = audit.Id, DocumentTypeId = group.First().OperationDocument.DocumentTypeId, Status = linked ? ResultStatus.Valid : ResultStatus.Warning, RuleId = "TECH-005", Message = linked ? $"La referencia e-Document {group.Key} coincide en documentos relacionados del expediente." : $"Se encontró la referencia e-Document {group.Key}, pero no se localizó su documento relacionado dentro del expediente. Control técnico; no implica incumplimiento legal." };
      db.AuditResults.Add(result); audit.Results.Add(result);
    }
    audit.Status = AuditStatus.Completed; audit.CompletedAt = DateTime.UtcNow; var hasFindings = requirements.Any(x => x.Status is ResultStatus.Missing or ResultStatus.Invalid); op.Status = hasFindings ? OperationStatus.WithFindings : OperationStatus.Completed; op.UpdatedAt = DateTime.UtcNow;
    db.SystemAuditLogs.Add(new SystemAuditLog { UserId = userId, Action = type == AuditType.Reaudit ? "REAUDIT_EXECUTED" : "AUDIT_EXECUTED", EntityType = "TradeOperation", EntityId = operationId.ToString(), Details = $"{type}: {requirements.Count} requisitos; {requirements.Count(x => x.Status is ResultStatus.Missing or ResultStatus.Invalid)} hallazgos." });
    try { await db.SaveChangesAsync(ct); }
    catch (DbUpdateConcurrencyException ex) { var entities = string.Join(", ", ex.Entries.Select(x => x.Metadata.ClrType.Name)); throw new InvalidOperationException($"Conflicto al persistir la auditoría. Entidades afectadas: {entities}.", ex); }
    return audit;
  }
}

public interface IMonthlyAuditRule { string Code { get; } Task<object> EvaluateAsync(string period, CancellationToken ct); }
public sealed class DemoMonthlyConsolidationRule(SaceDbContext db) : IMonthlyAuditRule {
  public string Code => "DEMO-MONTHLY-001";
  public async Task<object> EvaluateAsync(string period, CancellationToken ct) { var ops = await db.TradeOperations.Where(x => x.Period == period).Include(x => x.Documents).Include(x => x.Findings).ToListAsync(ct); var correct = ops.Count(x => x.Status == OperationStatus.Completed || x.Status == OperationStatus.Audited); var findings = ops.Sum(x => x.Findings.Count(f => f.Status != FindingStatus.Resolved)); return new { ruleCode = Code, isDemo = true, period, operations = ops.Count, correct, withFindings = ops.Count(x => x.Findings.Any(f => f.Status != FindingStatus.Resolved)), findings, compliance = ops.Count == 0 ? 0 : Math.Round(correct * 100d / ops.Count, 1), rows = ops.Select(x => new { x.Id, x.Folio, x.PedimentoNumber, result = x.Status, documents = x.Documents.Count, findings = x.Findings.Count(f => f.Status != FindingStatus.Resolved) }) }; }
}
