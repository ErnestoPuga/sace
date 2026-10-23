using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Text.RegularExpressions;
using Microsoft.EntityFrameworkCore;
using Sace.Api.Contracts;
using Sace.Api.Domain;
using Sace.Api.Infrastructure;

namespace Sace.Api.Services;

public interface IRegulatoryCatalogImportService {
  Task<RegulatoryCatalogImportResult> DryRunAsync(RegulatoryCatalogPackage package, Guid userId, CancellationToken ct = default);
  Task<RegulatoryCatalogImportResult> ImportAsync(RegulatoryCatalogPackage package, Guid userId, CancellationToken ct = default);
}
public interface IRegulatoryCatalogDiffService {
  Task<RegulatoryCatalogDiffResult> CompareAsync(Guid fromVersionId, Guid toVersionId, CancellationToken ct = default);
}

public static partial class RegulatoryCatalogRules {
  [GeneratedRegex("^[0-9]{8}$")] private static partial Regex TariffRegex();
  [GeneratedRegex("^[0-9]{2}$")] private static partial Regex NicoRegex();
  public static string NormalizeTariff(string value) => Regex.Replace(value ?? "", @"[\.\-/\s]", "");
  public static bool IsValidTariff(string value) => TariffRegex().IsMatch(NormalizeTariff(value));
  public static bool IsValidNico(string? value) => string.IsNullOrWhiteSpace(value) || NicoRegex().IsMatch(value.Trim());
  public static bool IsOfficialUrl(string value) {
    if (!Uri.TryCreate(value, UriKind.Absolute, out var uri) || uri.Scheme != Uri.UriSchemeHttps) return false;
    return uri.Host.EndsWith(".gob.mx", StringComparison.OrdinalIgnoreCase) || uri.Host.Equals("gob.mx", StringComparison.OrdinalIgnoreCase);
  }
}

public class RegulatoryCatalogImportService(SaceDbContext db) : IRegulatoryCatalogImportService {
  static readonly JsonSerializerOptions JsonOptions = new() { PropertyNamingPolicy = JsonNamingPolicy.CamelCase, Converters = { new JsonStringEnumConverter() } };

  public async Task<RegulatoryCatalogImportResult> DryRunAsync(RegulatoryCatalogPackage package, Guid userId, CancellationToken ct = default) {
    var result = await AnalyzeAsync(package, ct);
    db.SystemAuditLogs.Add(Log(userId, "REGULATORY_CATALOG_DRY_RUN", package.PackageId, $"Hash {result.PackageHash}; errores {result.Errors.Count}; altas {result.Added}; cambios {result.Modified}."));
    await db.SaveChangesAsync(ct);
    return result;
  }

  public async Task<RegulatoryCatalogImportResult> ImportAsync(RegulatoryCatalogPackage package, Guid userId, CancellationToken ct = default) {
    var analysis = await AnalyzeAsync(package, ct);
    if (analysis.AlreadyImported || !analysis.CanImport) return analysis;
    await using var transaction = await db.Database.BeginTransactionAsync(ct);
    try {
      var source = await db.OfficialLegalSources.Include(x => x.Versions).FirstOrDefaultAsync(x => x.Code == package.SourceCode.Trim().ToUpper(), ct);
      if (source is null) {
        source = new OfficialLegalSource { Code=package.SourceCode.Trim().ToUpperInvariant(), Title=package.SourceTitle.Trim(), SourceType=package.SourceType, Authority=package.Authority.Trim(), OfficialUrl=package.OfficialUrl.Trim(), Status=SourceVerificationStatus.VERIFIED };
        db.OfficialLegalSources.Add(source);
      }
      var previousVersion = source.Versions.Where(x=>x.VerificationStatus==SourceVerificationStatus.VERIFIED).OrderByDescending(x=>x.Version).FirstOrDefault();
      if (previousVersion is not null && previousVersion.EffectiveTo is null) { previousVersion.EffectiveTo=package.EffectiveFrom.ToUniversalTime().AddTicks(-1); previousVersion.VerificationStatus=SourceVerificationStatus.SUPERSEDED; }
      var version = new OfficialLegalSourceVersion { OfficialLegalSource=source, Version=(source.Versions.Count==0?0:source.Versions.Max(x=>x.Version))+1, VersionLabel=package.VersionLabel.Trim(), DofPublicationCode=package.DofPublicationCode?.Trim(), PublicationDate=package.PublicationDate.ToUniversalTime(), EffectiveFrom=package.EffectiveFrom.ToUniversalTime(), EffectiveTo=package.EffectiveTo?.ToUniversalTime(), OfficialUrl=package.OfficialUrl.Trim(), VerificationStatus=SourceVerificationStatus.VERIFIED, VerifiedBy=userId, VerifiedAt=DateTime.UtcNow, VerificationNotes="Importado desde paquete validado." };
      source.Versions.Add(version); source.Status=SourceVerificationStatus.VERIFIED; source.UpdatedAt=DateTime.UtcNow;
      var citations = new Dictionary<string,OfficialSourceCitation>(StringComparer.OrdinalIgnoreCase);
      foreach (var item in package.Citations) {
        var citation=new OfficialSourceCitation { OfficialLegalSourceVersion=version, CitationType=item.CitationType, CitationLabel=item.CitationLabel.Trim(), Article=item.Article?.Trim(), RuleNumber=item.RuleNumber?.Trim(), Annex=item.Annex?.Trim(), Section=item.Section?.Trim(), Subsection=item.Subsection?.Trim(), Numeral=item.Numeral?.Trim(), ExactReferenceText=item.ExactReferenceText?.Trim(), OfficialUrl=(item.OfficialUrl??package.OfficialUrl).Trim() };
        version.Citations.Add(citation); citations[item.Key]=citation;
      }
      foreach (var item in package.Requirements) {
        var old = await db.RegulatoryRequirements.Where(x=>x.Code==item.Code.Trim().ToUpper()&&x.Status==RegulatoryRequirementStatus.VERIFIED).OrderByDescending(x=>x.Version).FirstOrDefaultAsync(ct);
        if(old is not null && old.EffectiveTo is null)old.EffectiveTo=(item.EffectiveFrom??package.EffectiveFrom).ToUniversalTime().AddTicks(-1);
        var next=(await db.RegulatoryRequirements.Where(x=>x.Code==item.Code.Trim().ToUpper()).MaxAsync(x=>(int?)x.Version,ct)??0)+1;
        var requirement=new RegulatoryRequirement { Code=item.Code.Trim().ToUpperInvariant(), Version=next, Name=item.Name.Trim(), Description=item.Description?.Trim(), RequirementCategory=item.RequirementCategory, AuthorityCode=item.AuthorityCode, AuthorityName=item.AuthorityName?.Trim(), OperationScope=item.OperationScope, LegalBasis=item.LegalBasis?.Trim(), OfficialSourceUrl=package.OfficialUrl.Trim(), PublicationDate=package.PublicationDate.ToUniversalTime(), EffectiveFrom=(item.EffectiveFrom??package.EffectiveFrom).ToUniversalTime(), EffectiveTo=item.EffectiveTo?.ToUniversalTime(), Status=RegulatoryRequirementStatus.VERIFIED, VerificationLevel=RegulatoryVerificationLevel.OFFICIAL_SOURCE, VerifiedBy=userId, VerifiedAt=DateTime.UtcNow, IsAutomatable=item.IsAutomatable, RequiresManualReview=item.RequiresManualReview };
        foreach(var condition in item.Conditions??[])requirement.Conditions.Add(new(){FieldCode=condition.FieldCode.Trim().ToUpperInvariant(),Operator=condition.Operator,ExpectedValue=condition.ExpectedValue?.Trim(),ExpectedValue2=condition.ExpectedValue2?.Trim(),ConditionGroup=condition.ConditionGroup.Trim().ToUpperInvariant(),Sequence=condition.Sequence});
        foreach(var evidence in item.EvidenceRequirements)requirement.EvidenceRequirements.Add(new(){DocumentTypeCode=evidence.DocumentTypeCode.Trim().ToUpperInvariant(),EvidenceRole=evidence.EvidenceRole,IsRequired=evidence.IsRequired,Description=evidence.Description?.Trim(),AlternativeGroup=evidence.AlternativeGroup?.Trim().ToUpperInvariant()});
        foreach(var key in item.CitationKeys)requirement.SourceCitations.Add(new(){OfficialSourceCitation=citations[key],IsPrimary=key.Equals(item.CitationKeys.FirstOrDefault(),StringComparison.OrdinalIgnoreCase)});
        foreach(var tariff in item.TariffEntries)requirement.TariffEntries.Add(new(){RawTariffFraction=tariff.RawTariffFraction.Trim(),NormalizedTariffFraction=RegulatoryCatalogRules.NormalizeTariff(tariff.RawTariffFraction),Nico=tariff.Nico?.Trim(),OperationScope=tariff.OperationScope,CustomsRegime=tariff.CustomsRegime?.Trim(),GoodsDescription=tariff.GoodsDescription?.Trim(),QualificationType=tariff.QualificationType,QualificationText=tariff.QualificationText?.Trim(),OriginCountryCodes=tariff.OriginCountryCodes?.Trim().ToUpperInvariant(),DestinationCountryCodes=tariff.DestinationCountryCodes?.Trim().ToUpperInvariant(),EffectiveFrom=tariff.EffectiveFrom.ToUniversalTime(),EffectiveTo=tariff.EffectiveTo?.ToUniversalTime(),OfficialSourceCitation=citations[tariff.CitationKey],Status=SourceVerificationStatus.VERIFIED});
        db.RegulatoryRequirements.Add(requirement);
      }
      var payload=JsonSerializer.Serialize(package,JsonOptions);
      var import=new RegulatoryCatalogImport { PackageId=package.PackageId.Trim(),PackageVersion=package.SchemaVersion,PackageHash=analysis.PackageHash,SourceCode=source.Code,SourceVersionLabel=package.VersionLabel.Trim(),PublicationDate=package.PublicationDate.ToUniversalTime(),EffectiveFrom=package.EffectiveFrom.ToUniversalTime(),Status=RegulatoryCatalogImportStatus.IMPORTED,SummaryJson=JsonSerializer.Serialize(new{analysis.Added,analysis.Modified,analysis.Removed},JsonOptions),PayloadJson=payload,ImportedBy=userId };
      db.RegulatoryCatalogImports.Add(import); db.SystemAuditLogs.Add(Log(userId,"REGULATORY_CATALOG_IMPORTED",import.Id.ToString(),$"{package.PackageId}; hash {analysis.PackageHash}."));
      if(previousVersion is not null)db.SystemAuditLogs.Add(Log(userId,"REGULATORY_SOURCE_SUPERSEDED",previousVersion.Id.ToString(),$"Sustituida por {package.VersionLabel}."));
      await db.SaveChangesAsync(ct); await transaction.CommitAsync(ct);
      return analysis with { ImportId=import.Id };
    } catch { await transaction.RollbackAsync(ct); throw; }
  }

  async Task<RegulatoryCatalogImportResult> AnalyzeAsync(RegulatoryCatalogPackage package,CancellationToken ct) {
    var json=JsonSerializer.Serialize(package,JsonOptions); var hash=Convert.ToHexString(SHA256.HashData(Encoding.UTF8.GetBytes(json))).ToLowerInvariant();
    if(await db.RegulatoryCatalogImports.AnyAsync(x=>x.PackageHash==hash&&x.Status==RegulatoryCatalogImportStatus.IMPORTED,ct))return new(false,true,hash,0,0,0,[],[new("DUPLICATE_PACKAGE","El paquete ya fue importado.")]);
    var errors=new List<CatalogValidationMessage>();var warnings=new List<CatalogValidationMessage>();
    if(package.SchemaVersion!=1)errors.Add(new("UNSUPPORTED_SCHEMA","Sólo se admite schemaVersion 1.","schemaVersion"));
    if(string.IsNullOrWhiteSpace(package.PackageId))errors.Add(new("REQUIRED","packageId es obligatorio.","packageId"));
    if(string.IsNullOrWhiteSpace(package.SourceCode)||string.IsNullOrWhiteSpace(package.SourceTitle))errors.Add(new("REQUIRED","Código y título de fuente son obligatorios.","source"));
    if(!RegulatoryCatalogRules.IsOfficialUrl(package.OfficialUrl))errors.Add(new("UNOFFICIAL_URL","La URL debe ser HTTPS y pertenecer a un dominio gob.mx.","officialUrl"));
    if(package.VerificationStatus!=SourceVerificationStatus.VERIFIED)errors.Add(new("SOURCE_NOT_VERIFIED","El paquete importable debe declarar una versión VERIFIED.","verificationStatus"));
    if(package.EffectiveTo.HasValue&&package.EffectiveTo<package.EffectiveFrom)errors.Add(new("INVALID_PERIOD","La vigencia de la fuente es inválida.","effectiveTo"));
    var citationKeys=package.Citations.Select(x=>x.Key).ToList();
    foreach(var duplicate in citationKeys.GroupBy(x=>x,StringComparer.OrdinalIgnoreCase).Where(x=>x.Count()>1))errors.Add(new("DUPLICATE_CITATION",$"Cita duplicada: {duplicate.Key}.","citations"));
    foreach(var requirement in package.Requirements) {
      if(string.IsNullOrWhiteSpace(requirement.Code)||string.IsNullOrWhiteSpace(requirement.Name))errors.Add(new("REQUIRED","Código y nombre de requisito son obligatorios.","requirements"));
      if(requirement.CitationKeys.Count==0)errors.Add(new("MISSING_CITATION",$"{requirement.Code} no contiene cita oficial.",requirement.Code));
      foreach(var key in requirement.CitationKeys.Where(x=>!citationKeys.Contains(x,StringComparer.OrdinalIgnoreCase)))errors.Add(new("UNKNOWN_CITATION",$"{requirement.Code} referencia la cita inexistente {key}.",requirement.Code));
      foreach(var tariff in requirement.TariffEntries) {
        if(!RegulatoryCatalogRules.IsValidTariff(tariff.RawTariffFraction))errors.Add(new("INVALID_TARIFF",$"Fracción inválida: {tariff.RawTariffFraction}.",requirement.Code));
        if(!RegulatoryCatalogRules.IsValidNico(tariff.Nico))errors.Add(new("INVALID_NICO",$"NICO inválido: {tariff.Nico}.",requirement.Code));
        if(!citationKeys.Contains(tariff.CitationKey,StringComparer.OrdinalIgnoreCase))errors.Add(new("UNKNOWN_CITATION",$"La fracción {tariff.RawTariffFraction} referencia una cita inexistente.",requirement.Code));
        if(tariff.EffectiveTo.HasValue&&tariff.EffectiveTo<tariff.EffectiveFrom)errors.Add(new("INVALID_PERIOD",$"Vigencia arancelaria inválida en {requirement.Code}.",requirement.Code));
        if(tariff.QualificationType!=QualificationType.NONE&&string.IsNullOrWhiteSpace(tariff.QualificationText))errors.Add(new("MISSING_QUALIFICATION",$"{tariff.QualificationType} requiere texto de acotación.",requirement.Code));
      }
      foreach(var duplicate in requirement.TariffEntries.GroupBy(x=>$"{RegulatoryCatalogRules.NormalizeTariff(x.RawTariffFraction)}|{x.Nico}|{x.OperationScope}",StringComparer.OrdinalIgnoreCase).Where(x=>x.Count()>1))errors.Add(new("DUPLICATE_TARIFF",$"Entrada arancelaria duplicada: {duplicate.Key}.",requirement.Code));
    }
    foreach(var duplicate in package.Requirements.GroupBy(x=>x.Code,StringComparer.OrdinalIgnoreCase).Where(x=>x.Count()>1))errors.Add(new("DUPLICATE_REQUIREMENT",$"Requisito duplicado: {duplicate.Key}.","requirements"));
    var codes=package.Requirements.Select(x=>x.Code.Trim().ToUpper()).ToList();var existing=await db.RegulatoryRequirements.Where(x=>codes.Contains(x.Code)).Select(x=>x.Code).Distinct().ToListAsync(ct);
    var added=codes.Count(x=>!existing.Contains(x,StringComparer.OrdinalIgnoreCase));var modified=codes.Count-added;
    return new(errors.Count==0,false,hash,added,modified,0,errors,warnings);
  }
  static SystemAuditLog Log(Guid user,string action,string id,string details)=>new(){UserId=user==Guid.Empty?null:user,Action=action,EntityType=nameof(RegulatoryCatalogImport),EntityId=id,Details=details};
}

public class RegulatoryCatalogDiffService(SaceDbContext db) : IRegulatoryCatalogDiffService {
  public async Task<RegulatoryCatalogDiffResult> CompareAsync(Guid fromVersionId,Guid toVersionId,CancellationToken ct=default) {
    var from=await Snapshot(fromVersionId,ct);var to=await Snapshot(toVersionId,ct);
    var keys=from.Keys.Union(to.Keys,StringComparer.OrdinalIgnoreCase).OrderBy(x=>x).ToList();var items=new List<RegulatoryDiffItem>();
    foreach(var key in keys){var hasBefore=from.TryGetValue(key,out var before);var hasAfter=to.TryGetValue(key,out var after);var kind=!hasBefore?RegulatoryDiffKind.ADDED:!hasAfter?RegulatoryDiffKind.REMOVED:before==after?RegulatoryDiffKind.UNCHANGED:RegulatoryDiffKind.MODIFIED;items.Add(new(kind,"REQUIREMENT",key,before,after));}
    var toVersion=await db.OfficialLegalSourceVersions.AsNoTracking().SingleAsync(x=>x.Id==toVersionId,ct);var affected=items.Where(x=>x.Kind!=RegulatoryDiffKind.UNCHANGED).Select(x=>x.Key).ToList();
    var tariffs=await db.RegulatoryTariffEntries.AsNoTracking().Where(x=>x.RegulatoryRequirement.SourceCitations.Any(c=>c.OfficialSourceCitation.OfficialLegalSourceVersionId==fromVersionId||c.OfficialSourceCitation.OfficialLegalSourceVersionId==toVersionId)&&affected.Contains(x.RegulatoryRequirement.Code)).Select(x=>new{x.NormalizedTariffFraction,x.Nico}).Distinct().ToListAsync(ct);
    var operations=await db.TradeOperations.AsNoTracking().Select(x=>new{x.Id,x.OperationDate,x.TariffFraction,x.Nico}).ToListAsync(ct);var matches=operations.Where(o=>tariffs.Any(t=>RegulatoryCatalogRules.NormalizeTariff(o.TariffFraction)==t.NormalizedTariffFraction&&(t.Nico==null||t.Nico==o.Nico))).ToList();
    var impact=new RegulatoryImpactAnalysis(matches.Count(x=>x.OperationDate<toVersion.EffectiveFrom),matches.Count(x=>x.OperationDate>=toVersion.EffectiveFrom),matches.Select(x=>x.Id).ToList(),tariffs.Select(x=>x.NormalizedTariffFraction).Distinct().ToList(),tariffs.Where(x=>x.Nico!=null).Select(x=>x.Nico!).Distinct().ToList());
    return new(fromVersionId,toVersionId,items,impact);
  }
  async Task<Dictionary<string,string>> Snapshot(Guid versionId,CancellationToken ct){var rows=await db.RegulatoryRequirements.AsNoTracking().AsSplitQuery().Where(x=>x.SourceCitations.Any(c=>c.OfficialSourceCitation.OfficialLegalSourceVersionId==versionId)).Include(x=>x.Conditions).Include(x=>x.EvidenceRequirements).Include(x=>x.TariffEntries).ToListAsync(ct);return rows.ToDictionary(x=>x.Code,x=>JsonSerializer.Serialize(new{x.Name,x.RequirementCategory,x.OperationScope,x.EffectiveFrom,x.EffectiveTo,conditions=x.Conditions.Select(c=>new{c.FieldCode,c.Operator,c.ExpectedValue,c.ExpectedValue2,c.ConditionGroup}),evidence=x.EvidenceRequirements.Select(e=>new{e.DocumentTypeCode,e.EvidenceRole,e.IsRequired,e.AlternativeGroup}),tariffs=x.TariffEntries.Select(t=>new{t.NormalizedTariffFraction,t.Nico,t.QualificationType,t.QualificationText,t.EffectiveFrom,t.EffectiveTo})}),StringComparer.OrdinalIgnoreCase);}
}
