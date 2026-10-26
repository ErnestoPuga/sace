using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using Sace.Api.Contracts;
using Sace.Api.Domain;
using Sace.Api.Infrastructure;

namespace Sace.Api.Services;

public sealed record RiskCandidate(RiskFactorDefinition Definition,string SourceEntityType,Guid? SourceEntityId,string SourceOutcome,string Title,string Explanation,string? CorrelationKey=null);

public interface IOperationRiskScoringService {
  Task<OperationRiskScore> CalculateAsync(Guid operationId,Guid? auditId,Guid userId,CancellationToken ct=default);
}
public interface IReviewQueueService {
  Task<ReviewQueueItem> AssignAsync(Guid id,Guid? assignedTo,Guid userId,CancellationToken ct=default);
  Task<ReviewQueueItem> ChangeStatusAsync(Guid id,ReviewQueueStatus status,ReviewResolution? resolution,string? notes,Guid userId,CancellationToken ct=default);
  Task<ReviewQueueItem> ChangePriorityAsync(Guid id,RiskLevel priority,string reason,Guid userId,CancellationToken ct=default);
}
public interface IRiskDashboardService { Task<RiskDashboardDto> GetAsync(int page,int pageSize,string sort,string direction,CancellationToken ct=default); }

public static class RiskScoringMath {
  public static IReadOnlyList<(RiskCandidate Candidate,decimal Points)> Apply(IReadOnlyList<RiskCandidate> candidates,IReadOnlyDictionary<RiskCategory,decimal> caps){
    var correlated=new List<(RiskCandidate Candidate,decimal Points)>();
    foreach(var group in candidates.GroupBy(x=>string.IsNullOrWhiteSpace(x.CorrelationKey)?$"INDEPENDENT:{x.Definition.Code}:{x.SourceEntityId}":x.CorrelationKey!,StringComparer.OrdinalIgnoreCase)){
      var rows=group.Select(x=>(Candidate:x,Points:Math.Max(0,Math.Min(x.Definition.Weight,x.Definition.MaxPoints)))).ToList();
      if(group.Any(x=>x.Definition.CorrelationMode==RiskCorrelationMode.MAX))correlated.Add(rows.OrderByDescending(x=>x.Points).First());else correlated.AddRange(rows);
    }
    var capped=new List<(RiskCandidate Candidate,decimal Points)>();
    foreach(var category in correlated.GroupBy(x=>x.Candidate.Definition.Category)){
      var remaining=caps.TryGetValue(category.Key,out var cap)?Math.Max(0,cap):decimal.MaxValue;
      foreach(var row in category.OrderByDescending(x=>x.Points)){var points=Math.Min(row.Points,remaining);if(points>0)capped.Add((row.Candidate,points));remaining-=points;if(remaining<=0)break;}
    }
    return capped;
  }
}

public class OperationRiskScoringService(SaceDbContext db) : IOperationRiskScoringService {
  public async Task<OperationRiskScore> CalculateAsync(Guid operationId,Guid? auditId,Guid userId,CancellationToken ct=default){
    if(!await db.TradeOperations.AnyAsync(x=>x.Id==operationId,ct))throw new KeyNotFoundException("Operación no encontrada.");
    var now=DateTime.UtcNow;var profile=await db.RiskProfiles.AsNoTracking().Include(x=>x.Factors).Where(x=>x.Status==RiskProfileStatus.ACTIVE&&(x.EffectiveFrom==null||x.EffectiveFrom<=now)&&(x.EffectiveTo==null||x.EffectiveTo>=now)).OrderByDescending(x=>x.Version).FirstOrDefaultAsync(ct)??throw new InvalidOperationException("RISK_PROFILE_NOT_FOUND");
    var candidates=await CollectAsync(operationId,profile,ct);var caps=ParseCaps(profile.CategoryCapsJson);var selected=RiskScoringMath.Apply(candidates,caps);
    var raw=Math.Max(0,selected.Sum(x=>x.Points));var normalized=Math.Clamp(raw,0,100);var level=normalized<=profile.LowMax?RiskLevel.LOW:normalized<=profile.MediumMax?RiskLevel.MEDIUM:normalized<=profile.HighMax?RiskLevel.HIGH:RiskLevel.CRITICAL;
    var version=(await db.OperationRiskScores.Where(x=>x.TradeOperationId==operationId).MaxAsync(x=>(int?)x.Version,ct)??0)+1;
    var score=new OperationRiskScore{TradeOperationId=operationId,AuditId=auditId,RiskProfileId=profile.Id,RiskProfileCode=profile.Code,RiskProfileVersion=profile.Version,Version=version,RawScore=raw,NormalizedScore=normalized,RiskLevel=level,FactorCount=selected.Count,CriticalFactorCount=selected.Count(x=>x.Candidate.Definition.Severity==RiskSeverity.CRITICAL),HighFactorCount=selected.Count(x=>x.Candidate.Definition.Severity==RiskSeverity.HIGH),Explanation=selected.Count==0?"Sin factores operativos activos.":string.Join(Environment.NewLine,selected.OrderByDescending(x=>x.Points).Select(x=>$"+{x.Points:0.##} {x.Candidate.Title}")),SnapshotJson=JsonSerializer.Serialize(new{profile=new{profile.Code,profile.Version,profile.LowMax,profile.MediumMax,profile.HighMax},categoryCaps=caps,factors=selected.Select(x=>new{x.Candidate.Definition.Code,x.Candidate.Definition.Category,x.Points,x.Candidate.SourceEntityType,x.Candidate.SourceEntityId,x.Candidate.SourceOutcome,x.Candidate.CorrelationKey})})};
    foreach(var row in selected)score.Factors.Add(new(){FactorCode=row.Candidate.Definition.Code,Category=row.Candidate.Definition.Category,SourceEntityType=row.Candidate.SourceEntityType,SourceEntityId=row.Candidate.SourceEntityId,SourceOutcome=row.Candidate.SourceOutcome,Points=row.Points,Weight=row.Candidate.Definition.Weight,Severity=row.Candidate.Definition.Severity,Title=row.Candidate.Title,Explanation=row.Candidate.Explanation,CorrelationKey=row.Candidate.CorrelationKey??row.Candidate.Definition.CorrelationKey});
    db.OperationRiskScores.Add(score);await UpsertQueueAsync(score,userId,ct);db.SystemAuditLogs.Add(Log(userId,"RISK_SCORE_CALCULATED",nameof(OperationRiskScore),score.Id,$"{score.NormalizedScore:0.##}/100; prioridad {score.RiskLevel}; perfil {profile.Code} v{profile.Version}."));await db.SaveChangesAsync(ct);return score;
  }

  async Task<List<RiskCandidate>> CollectAsync(Guid operationId,RiskProfile profile,CancellationToken ct){var rows=new List<RiskCandidate>();
    var crosses=(await db.CrossValidationResults.AsNoTracking().Where(x=>x.TradeOperationId==operationId&&!x.RuleCode.StartsWith("DEMO-")).ToListAsync(ct)).GroupBy(x=>x.RuleCode).Select(x=>x.OrderByDescending(y=>y.EvaluatedAt).First()).Where(x=>x.Outcome==CrossValidationOutcome.MISMATCH);
    foreach(var x in crosses)Add(rows,profile,RiskSourceType.CROSS_VALIDATION,x.Outcome.ToString(),nameof(CrossValidationResult),x.Id,x.RuleCode,x.Explanation);
    var valuation=await db.CustomsValuationCalculations.AsNoTracking().Where(x=>x.TradeOperationId==operationId).OrderByDescending(x=>x.CalculationVersion).FirstOrDefaultAsync(ct);if(valuation is not null&&valuation.Outcome is ValuationOutcome.MISMATCH or ValuationOutcome.MANUAL_REVIEW)Add(rows,profile,RiskSourceType.VALUATION,valuation.Outcome.ToString(),nameof(CustomsValuationCalculation),valuation.Id,"Valoración aduanera",valuation.Explanation);
    var contribution=await db.ContributionCalculations.AsNoTracking().Where(x=>x.TradeOperationId==operationId).OrderByDescending(x=>x.Version).FirstOrDefaultAsync(ct);if(contribution is not null&&contribution.Status is ContributionOutcome.MISMATCH or ContributionOutcome.MANUAL_REVIEW)Add(rows,profile,RiskSourceType.CONTRIBUTIONS,contribution.Status.ToString(),nameof(ContributionCalculation),contribution.Id,"Conciliación de contribuciones",contribution.Explanation);
    var rrna=(await db.RegulatoryRequirementEvaluations.AsNoTracking().Include(x=>x.RegulatoryRequirement).Where(x=>x.TradeOperationId==operationId).ToListAsync(ct)).GroupBy(x=>x.RequirementCode).Select(x=>x.OrderByDescending(y=>y.EvaluatedAt).First()).Where(x=>x.RegulatoryRequirement.Status==RegulatoryRequirementStatus.VERIFIED&&!x.RequirementCode.StartsWith("DEMO-",StringComparison.OrdinalIgnoreCase));
    foreach(var x in rrna){if(x.EvidenceStatus==RegulatoryEvidenceStatus.MISSING_EVIDENCE)Add(rows,profile,RiskSourceType.RRNA,"MISSING_EVIDENCE",nameof(RegulatoryRequirementEvaluation),x.Id,$"RRNA {x.RequirementCode}: evidencia faltante",x.Explanation);if(x.AutomatedOutcome==RegulatoryApplicabilityOutcome.MANUAL_REVIEW)Add(rows,profile,RiskSourceType.RRNA,"MANUAL_REVIEW",nameof(RegulatoryRequirementEvaluation),x.Id,$"RRNA {x.RequirementCode}: revisión manual",x.Explanation);}
    var legalAudit=await db.Audits.AsNoTracking().Where(x=>x.TradeOperationId==operationId&&x.AuditType==AuditType.Legal).OrderByDescending(x=>x.StartedAt).Select(x=>x.Id).FirstOrDefaultAsync(ct);if(legalAudit!=Guid.Empty){var legal=await db.AuditResults.AsNoTracking().Include(x=>x.NormativeRule).Where(x=>x.AuditId==legalAudit&&x.IsIncludedInLegalCompliance&&(x.NormativeRule==null||x.NormativeRule.RuleCategory==RuleCategory.LEGAL)).ToListAsync(ct);foreach(var x in legal.Where(x=>x.Outcome is LegalOutcome.NON_COMPLIANT or LegalOutcome.MANUAL_REVIEW))Add(rows,profile,RiskSourceType.LEGAL,x.Outcome!.Value.ToString(),nameof(AuditResult),x.Id,$"LEGAL {x.RuleId}",x.Explanation??x.Message);}
    var technicalAudit=await db.Audits.AsNoTracking().Where(x=>x.TradeOperationId==operationId&&x.AuditType!=AuditType.Legal&&x.AuditType!=AuditType.Monthly).OrderByDescending(x=>x.StartedAt).Select(x=>x.Id).FirstOrDefaultAsync(ct);if(technicalAudit!=Guid.Empty){var missing=await db.AuditResults.AsNoTracking().Where(x=>x.AuditId==technicalAudit&&x.Status==ResultStatus.Missing&&!x.RuleId.StartsWith("DEMO-")).ToListAsync(ct);foreach(var x in missing)Add(rows,profile,RiskSourceType.DOCUMENTATION,"MISSING",nameof(AuditResult),x.Id,"Documento obligatorio faltante",x.Message);}
    return rows;
  }
  static void Add(List<RiskCandidate> rows,RiskProfile profile,RiskSourceType source,string outcome,string entity,Guid id,string title,string explanation){foreach(var factor in profile.Factors.Where(x=>x.IsActive&&x.SourceType==source&&(x.SourceOutcome==null||x.SourceOutcome.Equals(outcome,StringComparison.OrdinalIgnoreCase))))rows.Add(new(factor,entity,id,outcome.ToUpperInvariant(),title,string.IsNullOrWhiteSpace(factor.ExplanationTemplate)?explanation:factor.ExplanationTemplate!,factor.CorrelationKey));}
  async Task UpsertQueueAsync(OperationRiskScore score,Guid userId,CancellationToken ct){if(score.RiskLevel is not (RiskLevel.HIGH or RiskLevel.CRITICAL))return;var open=await db.ReviewQueueItems.Include(x=>x.History).FirstOrDefaultAsync(x=>x.TradeOperationId==score.TradeOperationId&&x.Status!=ReviewQueueStatus.RESOLVED&&x.Status!=ReviewQueueStatus.DISMISSED,ct);if(open is null){open=new(){TradeOperationId=score.TradeOperationId,RiskScore=score,OriginalPriority=score.RiskLevel,CurrentPriority=score.RiskLevel};open.History.Add(new(){Action="CREATED",NewStatus=ReviewQueueStatus.OPEN,UserId=userId,Notes="Creado por score operativo."});db.ReviewQueueItems.Add(open);db.SystemAuditLogs.Add(Log(userId,"REVIEW_ITEM_CREATED",nameof(ReviewQueueItem),open.Id,$"Prioridad {score.RiskLevel}."));}else{open.RiskScore=score;open.OriginalPriority=score.RiskLevel;if(open.PriorityChangedAt is null)open.CurrentPriority=score.RiskLevel;}}
  static Dictionary<RiskCategory,decimal> ParseCaps(string json){try{var values=JsonSerializer.Deserialize<Dictionary<string,decimal>>(json)??[];return values.Where(x=>Enum.TryParse<RiskCategory>(x.Key,true,out _)).ToDictionary(x=>Enum.Parse<RiskCategory>(x.Key,true),x=>x.Value);}catch(JsonException){return[];}}
  static SystemAuditLog Log(Guid user,string action,string entity,Guid id,string details)=>new(){UserId=user==Guid.Empty?null:user,Action=action,EntityType=entity,EntityId=id.ToString(),Details=details};
}

public class ReviewQueueService(SaceDbContext db) : IReviewQueueService {
  public async Task<ReviewQueueItem> AssignAsync(Guid id,Guid? assignedTo,Guid userId,CancellationToken ct=default){
    var item=await Item(id,ct);if(assignedTo.HasValue&&!await db.Users.AnyAsync(x=>x.Id==assignedTo&&x.IsActive,ct))throw new InvalidOperationException("El usuario asignado no existe o está inactivo.");var previous=item.Status;var next=assignedTo.HasValue&&previous==ReviewQueueStatus.OPEN?ReviewQueueStatus.ASSIGNED:previous;var now=DateTime.UtcNow;
    await Update(id,db.ReviewQueueItems.Where(x=>x.Id==id).ExecuteUpdateAsync(set=>set.SetProperty(x=>x.AssignedTo,assignedTo).SetProperty(x=>x.AssignedAt,assignedTo.HasValue?now:null).SetProperty(x=>x.Status,next),ct));
    db.ReviewQueueHistory.Add(History(item,"ASSIGNED",previous,next,userId,assignedTo.HasValue?"Revisión asignada.":"Asignación retirada."));db.SystemAuditLogs.Add(Log(userId,"REVIEW_ITEM_ASSIGNED",item.Id,$"Asignado a {assignedTo?.ToString()??"sin asignar"}."));await db.SaveChangesAsync(ct);return await Item(id,ct);
  }
  public async Task<ReviewQueueItem> ChangeStatusAsync(Guid id,ReviewQueueStatus status,ReviewResolution? resolution,string? notes,Guid userId,CancellationToken ct=default){
    var item=await Item(id,ct);var previous=item.Status;if(!Valid(previous,status))throw new InvalidOperationException("INVALID_REVIEW_TRANSITION");var closing=status is ReviewQueueStatus.RESOLVED or ReviewQueueStatus.DISMISSED;if(closing&&(!resolution.HasValue||string.IsNullOrWhiteSpace(notes)))throw new InvalidOperationException("Resolution y notes son obligatorios para cerrar una revisión.");var now=DateTime.UtcNow;var started=status==ReviewQueueStatus.IN_REVIEW?item.StartedAt??now:item.StartedAt;var completed=closing?now:(DateTime?)null;var trimmed=notes?.Trim();
    await Update(id,db.ReviewQueueItems.Where(x=>x.Id==id).ExecuteUpdateAsync(set=>set.SetProperty(x=>x.Status,status).SetProperty(x=>x.Resolution,closing?resolution:null).SetProperty(x=>x.Notes,trimmed).SetProperty(x=>x.StartedAt,started).SetProperty(x=>x.CompletedAt,completed),ct));
    db.ReviewQueueHistory.Add(History(item,closing?"RESOLVED":"STATUS_CHANGED",previous,status,userId,notes));db.SystemAuditLogs.Add(Log(userId,closing?"REVIEW_ITEM_RESOLVED":"REVIEW_STATUS_CHANGED",item.Id,$"{previous} → {status}."));await db.SaveChangesAsync(ct);return await Item(id,ct);
  }
  public async Task<ReviewQueueItem> ChangePriorityAsync(Guid id,RiskLevel priority,string reason,Guid userId,CancellationToken ct=default){
    if(string.IsNullOrWhiteSpace(reason))throw new InvalidOperationException("La razón del cambio de prioridad es obligatoria.");var item=await Item(id,ct);var previous=item.CurrentPriority;var trimmed=reason.Trim();var now=DateTime.UtcNow;
    await Update(id,db.ReviewQueueItems.Where(x=>x.Id==id).ExecuteUpdateAsync(set=>set.SetProperty(x=>x.CurrentPriority,priority).SetProperty(x=>x.PriorityChangedBy,userId).SetProperty(x=>x.PriorityChangedAt,now).SetProperty(x=>x.PriorityChangeReason,trimmed),ct));
    db.ReviewQueueHistory.Add(History(item,"PRIORITY_CHANGED",item.Status,item.Status,userId,$"{previous} → {priority}. {trimmed}"));db.SystemAuditLogs.Add(Log(userId,"REVIEW_PRIORITY_CHANGED",item.Id,$"{previous} → {priority}."));await db.SaveChangesAsync(ct);return await Item(id,ct);
  }
  async Task<ReviewQueueItem> Item(Guid id,CancellationToken ct)=>await db.ReviewQueueItems.AsNoTracking().Include(x=>x.History).FirstOrDefaultAsync(x=>x.Id==id,ct)??throw new KeyNotFoundException("REVIEW_ITEM_NOT_FOUND");
  static async Task Update(Guid id,Task<int> update){if(await update!=1)throw new DbUpdateConcurrencyException($"La revisión {id} cambió durante la operación.");}
  static bool Valid(ReviewQueueStatus from,ReviewQueueStatus to)=>(from,to) is (ReviewQueueStatus.OPEN,ReviewQueueStatus.ASSIGNED) or (ReviewQueueStatus.OPEN,ReviewQueueStatus.IN_REVIEW) or (ReviewQueueStatus.ASSIGNED,ReviewQueueStatus.IN_REVIEW) or (ReviewQueueStatus.IN_REVIEW,ReviewQueueStatus.WAITING_INFORMATION) or (ReviewQueueStatus.WAITING_INFORMATION,ReviewQueueStatus.IN_REVIEW) or (ReviewQueueStatus.IN_REVIEW,ReviewQueueStatus.RESOLVED) or (ReviewQueueStatus.IN_REVIEW,ReviewQueueStatus.DISMISSED);
  static ReviewQueueHistory History(ReviewQueueItem item,string action,ReviewQueueStatus? previous,ReviewQueueStatus? next,Guid user,string? notes)=>new(){ReviewQueueItemId=item.Id,Action=action,PreviousStatus=previous,NewStatus=next,UserId=user,Notes=notes};
  static SystemAuditLog Log(Guid user,string action,Guid id,string details)=>new(){UserId=user,Action=action,EntityType=nameof(ReviewQueueItem),EntityId=id.ToString(),Details=details};
}

public class RiskDashboardService(SaceDbContext db,IConfiguration configuration) : IRiskDashboardService {
  public async Task<RiskDashboardDto> GetAsync(int page,int pageSize,string sort,string direction,CancellationToken ct=default){
    page=Math.Max(1,page);var max=configuration.GetValue("Pagination:MaxPageSize",100);pageSize=Math.Clamp(pageSize==0?20:pageSize,1,max);
    var latest=db.OperationRiskScores.AsNoTracking().Where(x=>!db.OperationRiskScores.Any(n=>n.TradeOperationId==x.TradeOperationId&&n.Version>x.Version));
    var total=await latest.CountAsync(ct);
    var distribution=await latest.GroupBy(x=>x.RiskLevel).Select(x=>new RiskDistributionDto(x.Key,x.Count())).ToListAsync(ct);
    var critical=distribution.FirstOrDefault(x=>x.Level==RiskLevel.CRITICAL)?.Count??0;var high=distribution.FirstOrDefault(x=>x.Level==RiskLevel.HIGH)?.Count??0;
    var latestIds=await latest.Select(x=>x.Id).ToListAsync(ct);
    var factorRows=await db.OperationRiskFactors.AsNoTracking().Where(x=>latestIds.Contains(x.OperationRiskScoreId)).Select(x=>new{x.OperationRiskScoreId,x.FactorCode,x.Title,x.Category,x.Points,x.OperationRiskScore.TradeOperationId}).ToListAsync(ct);
    var topFactors=factorRows.GroupBy(x=>new{x.FactorCode,x.Title,x.Category}).Select(x=>new RiskFactorAggregateDto(x.Key.FactorCode,x.Key.Title,x.Key.Category,x.Select(y=>y.TradeOperationId).Distinct().Count(),x.Average(y=>y.Points))).OrderByDescending(x=>x.Operations).Take(10).ToList();
    var dimensionRows=await latest.Select(x=>new{x.TradeOperation.TariffFraction,x.TradeOperation.CustomsOffice,x.RiskLevel,x.NormalizedScore}).ToListAsync(ct);
    var tariff=dimensionRows.GroupBy(x=>x.TariffFraction).Select(x=>new RiskDimensionAggregateDto(x.Key,x.Count(),x.Count(y=>y.RiskLevel is RiskLevel.HIGH or RiskLevel.CRITICAL),x.Average(y=>y.NormalizedScore))).OrderByDescending(x=>x.HighCritical).Take(10).ToList();
    var customs=dimensionRows.GroupBy(x=>x.CustomsOffice??"Sin aduana").Select(x=>new RiskDimensionAggregateDto(x.Key,x.Count(),x.Count(y=>y.RiskLevel is RiskLevel.HIGH or RiskLevel.CRITICAL),x.Average(y=>y.NormalizedScore))).OrderByDescending(x=>x.HighCritical).Take(10).ToList();
    var query=latest.Include(x=>x.TradeOperation).Include(x=>x.Factors).AsQueryable();query=(sort.ToLowerInvariant(),direction.ToLowerInvariant()) switch{("date","asc")=>query.OrderBy(x=>x.TradeOperation.OperationDate),("date",_)=>query.OrderByDescending(x=>x.TradeOperation.OperationDate),("riskscore","asc")=>query.OrderBy(x=>x.NormalizedScore),_=>query.OrderByDescending(x=>x.NormalizedScore)};
    var scores=await query.Skip((page-1)*pageSize).Take(pageSize).ToListAsync(ct);var operationIds=scores.Select(x=>x.TradeOperationId).ToList();var reviews=await db.ReviewQueueItems.AsNoTracking().Where(x=>operationIds.Contains(x.TradeOperationId)&&x.Status!=ReviewQueueStatus.RESOLVED&&x.Status!=ReviewQueueStatus.DISMISSED).ToDictionaryAsync(x=>x.TradeOperationId,x=>(ReviewQueueStatus?)x.Status,ct);
    var rows=scores.Select(x=>new RiskOperationRowDto(x.TradeOperationId,x.TradeOperation.Folio,x.TradeOperation.PedimentoNumber,x.TradeOperation.OperationDate,x.TradeOperation.OperationType,x.TradeOperation.CustomsOffice,x.TradeOperation.TariffFraction,x.NormalizedScore,x.RiskLevel,string.Join(", ",x.Factors.OrderByDescending(f=>f.Points).Take(3).Select(f=>f.Title)),reviews.GetValueOrDefault(x.TradeOperationId))).ToList();
    var manual=await db.ReviewQueueItems.CountAsync(x=>x.Status==ReviewQueueStatus.IN_REVIEW||x.Status==ReviewQueueStatus.WAITING_INFORMATION,ct);var rrna=factorRows.Count(x=>x.FactorCode=="RRNA_MISSING_EVIDENCE");var valuations=factorRows.Count(x=>x.FactorCode=="VALUATION_MISMATCH");var contributions=factorRows.Count(x=>x.FactorCode=="CONTRIBUTION_MISMATCH");var incomplete=factorRows.Count(x=>x.Category==RiskCategory.DOCUMENTATION);
    return new(total,critical,high,manual,rrna,valuations,contributions,incomplete,distribution,topFactors,tariff,customs,rows,page,pageSize,total);
  }
}
