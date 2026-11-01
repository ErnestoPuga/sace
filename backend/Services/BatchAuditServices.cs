using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using Sace.Api.Contracts;
using Sace.Api.Domain;
using Sace.Api.Infrastructure;

namespace Sace.Api.Services;

public sealed record BatchOperationExecutionResult(Guid? AuditId, Guid? RiskScoreId, IReadOnlyList<string> Errors);

public interface IBatchOperationPipeline {
  Task<BatchOperationExecutionResult> ExecuteAsync(Guid operationId, Guid userId, CancellationToken ct = default);
}

public interface IBatchAuditService {
  Task<BatchAudit> CreateAsync(CreateBatchAuditRequest request, Guid userId, CancellationToken ct = default);
  Task<BatchAudit> ExecuteAsync(Guid batchId, Guid userId, bool retryFailed = false, CancellationToken ct = default);
  Task<BatchAudit> CancelAsync(Guid batchId, Guid userId, CancellationToken ct = default);
}

public sealed class BatchOperationPipeline(
  IAuditEngine auditEngine,
  ICrossValidationEngine crossValidation,
  ICustomsValuationEngine valuation,
  IContributionReconciliationEngine contributions,
  IRegulatoryRequirementEngine regulatory,
  ILegalAuditEngineV2 legal,
  IOperationRiskScoringService riskScoring) : IBatchOperationPipeline {
  public async Task<BatchOperationExecutionResult> ExecuteAsync(Guid operationId, Guid userId, CancellationToken ct = default) {
    var errors = new List<string>();
    Guid? auditId = null;
    Guid? riskScoreId = null;
    await Run("DOCUMENT_ANALYSIS", async () => auditId = (await auditEngine.ExecuteAsync(operationId, AuditType.Daily, userId, ct)).Id);
    await Run("CROSS_VALIDATION", async () => await crossValidation.ExecuteAsync(operationId, auditId, userId, ct));
    await Run("VALUATION", async () => await valuation.CalculateAsync(operationId, auditId, userId, ct));
    await Run("CONTRIBUTIONS", async () => await contributions.ReconcileAsync(operationId, auditId, userId, ct));
    await Run("RRNA", async () => await regulatory.EvaluateAsync(operationId, auditId, userId, ct));
    await Run("LEGAL", async () => await legal.ExecuteAsync(operationId, userId, ct: ct));
    await Run("RISK_SCORING", async () => riskScoreId = (await riskScoring.CalculateAsync(operationId, auditId, userId, ct)).Id);
    return new(auditId, riskScoreId, errors);

    async Task Run(string stage, Func<Task> action) {
      try { await action(); }
      catch (Exception exception) when (exception is not OperationCanceledException) { errors.Add($"{stage}: {exception.Message}"); }
    }
  }
}

public sealed class BatchAuditService(SaceDbContext db, IBatchOperationPipeline pipeline, IConfiguration configuration) : IBatchAuditService {
  public async Task<BatchAudit> CreateAsync(CreateBatchAuditRequest request, Guid userId, CancellationToken ct = default) {
    if (string.IsNullOrWhiteSpace(request.Name)) throw new InvalidOperationException("BATCH_NAME_REQUIRED");
    if (request.DateFrom.HasValue && request.DateTo.HasValue && request.DateFrom > request.DateTo) throw new InvalidOperationException("BATCH_DATE_RANGE_INVALID");
    var query = db.TradeOperations.AsNoTracking().AsQueryable();
    if (request.DateFrom.HasValue) query = query.Where(x => x.OperationDate >= request.DateFrom.Value.Date);
    if (request.DateTo.HasValue) query = query.Where(x => x.OperationDate < request.DateTo.Value.Date.AddDays(1));
    if (request.OperationType.HasValue) query = query.Where(x => x.OperationType == request.OperationType);
    if (!string.IsNullOrWhiteSpace(request.CustomsOffice)) query = query.Where(x => x.CustomsOffice == request.CustomsOffice.Trim());
    if (!string.IsNullOrWhiteSpace(request.TariffFraction)) query = query.Where(x => x.TariffFraction == request.TariffFraction.Trim());
    if (request.OperationStatus.HasValue) query = query.Where(x => x.Status == request.OperationStatus);
    if (request.OperationIds is { Count: > 0 }) query = query.Where(x => request.OperationIds.Contains(x.Id));
    var max = Math.Max(1, configuration.GetValue("BatchAudit:MaxOperationsPerBatch", 500));
    var ids = await query.OrderBy(x => x.OperationDate).ThenBy(x => x.Id).Select(x => x.Id).Take(max + 1).ToListAsync(ct);
    if (ids.Count > max) throw new InvalidOperationException("BATCH_TOO_LARGE");
    var batch = new BatchAudit {
      Name = request.Name.Trim(), Description = request.Description?.Trim(), PeriodStart = request.DateFrom?.Date, PeriodEnd = request.DateTo?.Date,
      CreatedBy = userId, TotalOperations = ids.Count,
      ParametersJson = JsonSerializer.Serialize(new { request.DateFrom, request.DateTo, request.OperationType, request.CustomsOffice, request.TariffFraction, request.OperationStatus, ExplicitOperationIds = request.OperationIds?.Count ?? 0 }),
      Operations = ids.Select(id => new BatchAuditOperation { TradeOperationId = id }).ToList()
    };
    db.BatchAudits.Add(batch);
    db.SystemAuditLogs.Add(Log(userId, "BATCH_AUDIT_CREATED", batch.Id, $"Snapshot de {ids.Count} operación(es)."));
    await db.SaveChangesAsync(ct);
    return batch;
  }

  public async Task<BatchAudit> ExecuteAsync(Guid batchId, Guid userId, bool retryFailed = false, CancellationToken ct = default) {
    var batch = await db.BatchAudits.Include(x => x.Operations).FirstOrDefaultAsync(x => x.Id == batchId, ct) ?? throw new KeyNotFoundException("BATCH_NOT_FOUND");
    if (batch.Status == BatchAuditStatus.RUNNING) throw new InvalidOperationException("BATCH_ALREADY_RUNNING");
    if (!retryFailed && batch.Status is BatchAuditStatus.COMPLETED or BatchAuditStatus.COMPLETED_WITH_ERRORS or BatchAuditStatus.CANCELLED) throw new InvalidOperationException("BATCH_ALREADY_FINISHED");
    var work = (retryFailed ? batch.Operations.Where(x => x.Status == BatchOperationStatus.FAILED) : batch.Operations.Where(x => x.Status == BatchOperationStatus.PENDING)).OrderBy(x => x.CreatedAt).ToList();
    if (retryFailed && work.Count == 0) throw new InvalidOperationException("BATCH_NO_FAILED_OPERATIONS");
    batch.Status = BatchAuditStatus.RUNNING; batch.StartedAt ??= DateTime.UtcNow; batch.CompletedAt = null;
    db.SystemAuditLogs.Add(Log(userId, "BATCH_AUDIT_STARTED", batch.Id, retryFailed ? $"Reintento de {work.Count} operación(es)." : $"Ejecución de {work.Count} operación(es)."));
    await db.SaveChangesAsync(ct);

    foreach (var row in work) {
      ct.ThrowIfCancellationRequested();
      var currentStatus = await db.BatchAudits.AsNoTracking().Where(x => x.Id == batchId).Select(x => x.Status).SingleAsync(ct);
      if (currentStatus == BatchAuditStatus.CANCELLED) break;
      row.Status = BatchOperationStatus.RUNNING; row.StartedAt = DateTime.UtcNow; row.CompletedAt = null; row.ErrorCode = null; row.ErrorMessage = null;
      if (retryFailed) row.RetryCount++;
      await db.SaveChangesAsync(ct);
      try {
        var result = await pipeline.ExecuteAsync(row.TradeOperationId, userId, ct);
        row.AuditId = result.AuditId; row.RiskScoreId = result.RiskScoreId; row.CompletedAt = DateTime.UtcNow;
        if (result.Errors.Count == 0) row.Status = BatchOperationStatus.COMPLETED;
        else { row.Status = BatchOperationStatus.FAILED; row.ErrorCode = "BATCH_OPERATION_FAILED"; row.ErrorMessage = string.Join(" | ", result.Errors); db.SystemAuditLogs.Add(Log(userId, "BATCH_OPERATION_FAILED", row.Id, row.ErrorMessage)); }
      } catch (Exception exception) when (exception is not OperationCanceledException) {
        row.Status = BatchOperationStatus.FAILED; row.CompletedAt = DateTime.UtcNow; row.ErrorCode = "BATCH_OPERATION_FAILED"; row.ErrorMessage = exception.Message;
        db.SystemAuditLogs.Add(Log(userId, "BATCH_OPERATION_FAILED", row.Id, exception.Message));
      }
      RefreshCounts(batch);
      await db.SaveChangesAsync(ct);
    }

    await db.Entry(batch).ReloadAsync(ct);
    if (batch.Status == BatchAuditStatus.CANCELLED) {
      foreach (var row in batch.Operations.Where(x => x.Status is BatchOperationStatus.PENDING or BatchOperationStatus.RUNNING)) row.Status = BatchOperationStatus.CANCELLED;
    } else {
      RefreshCounts(batch); batch.Status = batch.FailedOperations > 0 ? BatchAuditStatus.COMPLETED_WITH_ERRORS : BatchAuditStatus.COMPLETED; batch.CompletedAt = DateTime.UtcNow;
      db.SystemAuditLogs.Add(Log(userId, "BATCH_AUDIT_COMPLETED", batch.Id, $"{batch.SuccessfulOperations} exitosas; {batch.FailedOperations} fallidas."));
    }
    RefreshCounts(batch);
    await db.SaveChangesAsync(ct);
    return batch;
  }

  public async Task<BatchAudit> CancelAsync(Guid batchId, Guid userId, CancellationToken ct = default) {
    var batch = await db.BatchAudits.FirstOrDefaultAsync(x => x.Id == batchId, ct) ?? throw new KeyNotFoundException("BATCH_NOT_FOUND");
    if (batch.Status != BatchAuditStatus.RUNNING) throw new InvalidOperationException("BATCH_NOT_RUNNING");
    batch.Status = BatchAuditStatus.CANCELLED; batch.CompletedAt = DateTime.UtcNow;
    db.SystemAuditLogs.Add(Log(userId, "BATCH_AUDIT_CANCELLED", batch.Id, "Cancelación solicitada; se conservan resultados terminados."));
    await db.SaveChangesAsync(ct);
    return batch;
  }

  static void RefreshCounts(BatchAudit batch) {
    batch.ProcessedOperations = batch.Operations.Count(x => x.Status is BatchOperationStatus.COMPLETED or BatchOperationStatus.FAILED);
    batch.SuccessfulOperations = batch.Operations.Count(x => x.Status == BatchOperationStatus.COMPLETED);
    batch.FailedOperations = batch.Operations.Count(x => x.Status == BatchOperationStatus.FAILED);
  }
  static SystemAuditLog Log(Guid user, string action, Guid id, string details) => new() { UserId = user == Guid.Empty ? null : user, Action = action, EntityType = nameof(BatchAudit), EntityId = id.ToString(), Details = details };
}
