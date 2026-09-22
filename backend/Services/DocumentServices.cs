using System.IO.Compression;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using System.Text.RegularExpressions;
using System.Xml.Linq;
using Microsoft.EntityFrameworkCore;
using Sace.Api.Contracts;
using Sace.Api.Domain;
using Sace.Api.Infrastructure;

namespace Sace.Api.Services;

public interface IFileStorageService {
  Task<(string storedName, string relativePath)> SaveAsync(Guid operationId, string documentCode, IFormFile file, CancellationToken ct);
  Task<Stream?> OpenAsync(string relativePath, CancellationToken ct);
}

public sealed class LocalFileStorageService(IConfiguration configuration, IWebHostEnvironment environment) : IFileStorageService {
  private readonly string root = Path.GetFullPath(Path.Combine(environment.ContentRootPath, configuration["FileStorage:RootPath"] ?? "../storage"));
  public async Task<(string storedName, string relativePath)> SaveAsync(Guid operationId, string documentCode, IFormFile file, CancellationToken ct) {
    var extension = Path.GetExtension(file.FileName).ToLowerInvariant();
    var stored = $"{Guid.NewGuid():N}{extension}";
    var relative = Path.Combine("operations", operationId.ToString(), Safe(documentCode), stored);
    var full = Path.GetFullPath(Path.Combine(root, relative));
    if (!full.StartsWith(root, StringComparison.OrdinalIgnoreCase)) throw new InvalidOperationException("Ruta de almacenamiento inválida.");
    Directory.CreateDirectory(Path.GetDirectoryName(full)!);
    await using var output = File.Create(full);
    await file.CopyToAsync(output, ct);
    return (stored, relative.Replace('\\', '/'));
  }
  public Task<Stream?> OpenAsync(string relativePath, CancellationToken ct) {
    var full = Path.GetFullPath(Path.Combine(root, relativePath));
    if (!full.StartsWith(root, StringComparison.OrdinalIgnoreCase) || !File.Exists(full)) return Task.FromResult<Stream?>(null);
    return Task.FromResult<Stream?>(File.OpenRead(full));
  }
  private static string Safe(string value) => string.Concat(value.ToLowerInvariant().Select(c => char.IsLetterOrDigit(c) || c == '-' ? c : '-'));
}

public record ExtractedReference(string ReferenceType, string Value, string Source);
public record XmlDocumentAnalysis(bool Valid, string Format, string? ErrorCode, string? ErrorMessage, DateTime? DocumentDate, string? DocumentNumber, IReadOnlyList<ExtractedReference> References, IReadOnlyDictionary<string, string> Metadata);

public interface IPedimentoParser { Task<bool> IsValidAsync(Stream stream, CancellationToken ct); }
public interface IXmlDocumentProcessor { Task<bool> ValidateAsync(Stream stream, CancellationToken ct); Task<XmlDocumentAnalysis> AnalyzeAsync(Stream stream, CancellationToken ct); }
public interface IPdfDocumentProcessor { Task<bool> ValidateAsync(Stream stream, CancellationToken ct); }
public interface IExcelDocumentProcessor { Task<bool> ValidateAsync(Stream stream, CancellationToken ct); }

public sealed class PedimentoXmlParser : IPedimentoParser, IXmlDocumentProcessor {
  private static readonly Regex Cove = new(@"\bCOVE[A-Z0-9]{6,24}\b", RegexOptions.IgnoreCase | RegexOptions.Compiled);
  private static readonly Regex EDocument = new(@"\b[0-9]{7}[A-Z0-9]{6}\b", RegexOptions.IgnoreCase | RegexOptions.Compiled);
  public async Task<bool> IsValidAsync(Stream stream, CancellationToken ct) => await ValidateAsync(stream, ct);
  public async Task<bool> ValidateAsync(Stream stream, CancellationToken ct) => (await AnalyzeAsync(stream, ct)).Valid;
  public async Task<XmlDocumentAnalysis> AnalyzeAsync(Stream stream, CancellationToken ct) {
    if (stream.CanSeek && stream.Length == 0) return Invalid("EMPTY_FILE", "El archivo se encuentra vacío y no puede ser procesado.");
    XDocument doc;
    try { doc = await XDocument.LoadAsync(stream, LoadOptions.None, ct); }
    catch { return Invalid("MALFORMED_XML", "El XML está malformado o no puede ser interpretado."); }
    var root = doc.Root;
    if (root is null) return Invalid("MALFORMED_XML", "El XML no contiene un elemento raíz.");
    var namespaceUris = root.DescendantsAndSelf().Select(x => x.Name.NamespaceName).Where(x => x.Length > 0).Distinct(StringComparer.OrdinalIgnoreCase).ToArray();
    var allText = string.Join(' ', root.DescendantsAndSelf().SelectMany(x => x.Attributes().Select(a => a.Value).Append(x.Value))).ToUpperInvariant();
    var refs = new List<ExtractedReference>();
    var metadata = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
    var isCfdi = namespaceUris.Any(x => x.Contains("sat.gob.mx/cfd", StringComparison.OrdinalIgnoreCase)) || root.Name.LocalName.Equals("Comprobante", StringComparison.OrdinalIgnoreCase);
    var format = isCfdi ? "CFDI" : allText.Contains("DIG_ENVIO") ? "DIG_ENVIO" : allText.Contains("DIG_RECIBO") ? "DIG_RECIBO" : root.Name.LocalName.Contains("DODA", StringComparison.OrdinalIgnoreCase) ? "DODA" : Cove.IsMatch(allText) ? "COVE" : "GENERIC_XML";
    DateTime? date = null; string? number = null;
    if (isCfdi) {
      var uuid = root.DescendantsAndSelf().SelectMany(x => x.Attributes()).FirstOrDefault(a => a.Name.LocalName.Equals("UUID", StringComparison.OrdinalIgnoreCase))?.Value;
      if (!string.IsNullOrWhiteSpace(uuid)) { number = uuid; AddRef("CFDI_UUID", uuid, "CFDI XML"); metadata["UUID"] = uuid; }
      AddAttribute("Fecha", "Fecha"); AddAttribute("Total", "Total"); AddAttribute("Moneda", "Moneda");
      var emisor = root.Descendants().FirstOrDefault(x => x.Name.LocalName.Equals("Emisor", StringComparison.OrdinalIgnoreCase));
      var receptor = root.Descendants().FirstOrDefault(x => x.Name.LocalName.Equals("Receptor", StringComparison.OrdinalIgnoreCase));
      var rfcEmisor = emisor?.Attributes().FirstOrDefault(a => a.Name.LocalName.Equals("Rfc", StringComparison.OrdinalIgnoreCase))?.Value;
      var rfcReceptor = receptor?.Attributes().FirstOrDefault(a => a.Name.LocalName.Equals("Rfc", StringComparison.OrdinalIgnoreCase))?.Value;
      if (!string.IsNullOrWhiteSpace(rfcEmisor)) metadata["RfcEmisor"] = rfcEmisor;
      if (!string.IsNullOrWhiteSpace(rfcReceptor)) metadata["RfcReceptor"] = rfcReceptor;
      if (metadata.TryGetValue("Fecha", out var rawDate) && DateTime.TryParse(rawDate, out var parsed)) date = parsed.ToUniversalTime();
    }
    foreach (Match match in Cove.Matches(allText)) AddRef("COVE", match.Value, "XML");
    foreach (var candidate in root.DescendantsAndSelf().SelectMany(x => x.Attributes().Where(a => IsEDocumentName(a.Name.LocalName)).Select(a => a.Value).Concat(IsEDocumentName(x.Name.LocalName) ? [x.Value] : [])))
      foreach (Match match in EDocument.Matches(candidate.ToUpperInvariant())) AddRef("EDOCUMENT", match.Value, "XML");
    if ((format is "DIG_ENVIO" or "DIG_RECIBO") && !refs.Any(x => x.ReferenceType == "EDOCUMENT")) foreach (Match match in EDocument.Matches(allText)) AddRef("EDOCUMENT", match.Value, format);
    if (format == "DODA") foreach (var value in root.DescendantsAndSelf().Where(x => x.Name.LocalName.Contains("DODA", StringComparison.OrdinalIgnoreCase)).Select(x => x.Value).Where(x => !string.IsNullOrWhiteSpace(x))) AddRef("DODA", value.Trim(), "XML");
    return new(true, format, null, null, date, number, refs, metadata);
    void AddAttribute(string attribute, string key) { var value = root.Attributes().FirstOrDefault(a => a.Name.LocalName.Equals(attribute, StringComparison.OrdinalIgnoreCase))?.Value; if (!string.IsNullOrWhiteSpace(value)) metadata[key] = value; }
    void AddRef(string type, string value, string source) { value = value.Trim().ToUpperInvariant(); if (!refs.Any(x => x.ReferenceType == type && x.Value == value)) refs.Add(new(type, value, source)); }
    static bool IsEDocumentName(string name) => name.Contains("edocument", StringComparison.OrdinalIgnoreCase) || name.Contains("e_document", StringComparison.OrdinalIgnoreCase) || name.Contains("numerooperacion", StringComparison.OrdinalIgnoreCase);
  }
  private static XmlDocumentAnalysis Invalid(string code, string message) => new(false, "INVALID_XML", code, message, null, null, [], new Dictionary<string, string>());
}

public sealed class PdfDocumentProcessor : IPdfDocumentProcessor { public async Task<bool> ValidateAsync(Stream stream, CancellationToken ct) { var b = new byte[5]; return await stream.ReadAsync(b, ct) == 5 && Encoding.ASCII.GetString(b) == "%PDF-"; } }
public sealed class ExcelDocumentProcessor : IExcelDocumentProcessor { public Task<bool> ValidateAsync(Stream stream, CancellationToken ct) { try { using var zip = new ZipArchive(stream, ZipArchiveMode.Read, true); return Task.FromResult(zip.GetEntry("[Content_Types].xml") is not null && zip.GetEntry("xl/workbook.xml") is not null); } catch { return Task.FromResult(false); } } }

public record DocumentValidationResult(bool Valid, string? Code, string? Message);
public interface IDocumentValidationService { Task<DocumentValidationResult> ValidateAsync(IFormFile file, CancellationToken ct); }
public sealed class DocumentValidationService(IConfiguration config, IXmlDocumentProcessor xml, IPdfDocumentProcessor pdf, IExcelDocumentProcessor excel) : IDocumentValidationService {
  private static readonly Dictionary<string, string[]> Allowed = new(StringComparer.OrdinalIgnoreCase) { [".pdf"] = ["application/pdf", "application/octet-stream"], [".xml"] = ["application/xml", "text/xml", "application/octet-stream"], [".xlsx"] = ["application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "application/octet-stream"] };
  public async Task<DocumentValidationResult> ValidateAsync(IFormFile file, CancellationToken ct) {
    if (file.Length <= 0) return new(false, "EMPTY_FILE", "El archivo se encuentra vacío y no puede ser procesado.");
    var max = config.GetValue<long>("FileStorage:MaxFileSizeMb", 10) * 1024 * 1024;
    if (file.Length > max) return new(false, "FILE_TOO_LARGE", $"El archivo excede el límite de {max / 1024 / 1024} MB.");
    var ext = Path.GetExtension(file.FileName).ToLowerInvariant();
    if (!Allowed.TryGetValue(ext, out var mimes)) return new(false, "UNSUPPORTED_FILE_TYPE", $"El formato {(string.IsNullOrWhiteSpace(ext) ? "sin extensión" : ext.ToUpperInvariant())} todavía no está soportado por SACE.");
    if (!mimes.Contains(file.ContentType, StringComparer.OrdinalIgnoreCase)) return new(false, "MIME_MISMATCH", "El tipo MIME no corresponde con la extensión del archivo.");
    await using var stream = file.OpenReadStream();
    if (ext == ".xml") { var analysis = await xml.AnalyzeAsync(stream, ct); return analysis.Valid ? new(true, null, null) : new(false, analysis.ErrorCode, analysis.ErrorMessage); }
    var ok = ext == ".pdf" ? await pdf.ValidateAsync(stream, ct) : await excel.ValidateAsync(stream, ct);
    return ok ? new(true, null, null) : new(false, "INVALID_FILE_CONTENT", $"El contenido {ext.ToUpperInvariant()} no es válido o está dañado.");
  }
}

public record DocumentClassification(string DocumentTypeCode, double Confidence, string Reason) { public bool RequiresManualSelection => Confidence < .60 || DocumentTypeCode == "OTHER"; }
public interface IDocumentClassifier { Task<DocumentClassification> ClassifyAsync(IFormFile file, CancellationToken ct); }
public sealed class DocumentClassifier(IXmlDocumentProcessor xml) : IDocumentClassifier {
  private static readonly (Regex Pattern, string Code, double Confidence, string Reason)[] Rules = [
    (new(@"PS01", RegexOptions.IgnoreCase | RegexOptions.Compiled), "PEDIMENTO", .95, "Patrón de nombre PS01"), (new(@"FC01", RegexOptions.IgnoreCase | RegexOptions.Compiled), "COMMERCIAL_INVOICE", .95, "Patrón de nombre FC01"),
    (new(@"PL01", RegexOptions.IgnoreCase | RegexOptions.Compiled), "PACKING_LIST", .95, "Patrón de nombre PL01"), (new(@"TR01", RegexOptions.IgnoreCase | RegexOptions.Compiled), "TRANSPORT_DOCUMENT", .95, "Patrón de nombre TR01"),
    (new(@"COVE01", RegexOptions.IgnoreCase | RegexOptions.Compiled), "COVE_ACK", .95, "Patrón de nombre COVE01"), (new(@"QR01", RegexOptions.IgnoreCase | RegexOptions.Compiled), "DODA", .95, "Patrón de nombre QR01"),
    (new(@"DIG_ACUSE", RegexOptions.IgnoreCase | RegexOptions.Compiled), "EDOCUMENT_ACK", .93, "Patrón de nombre DIG_ACUSE")
  ];
  public async Task<DocumentClassification> ClassifyAsync(IFormFile file, CancellationToken ct) {
    var safeName = Path.GetFileNameWithoutExtension(file.FileName);
    if (Regex.IsMatch(safeName, @"^CARTAS\d+$", RegexOptions.IgnoreCase)) return new("OTHER", .20, "Las cartas requieren selección manual; el número no determina su naturaleza.");
    foreach (var rule in Rules) if (rule.Pattern.IsMatch(safeName)) return new(rule.Code, rule.Confidence, rule.Reason);
    if (Path.GetExtension(file.FileName).Equals(".xml", StringComparison.OrdinalIgnoreCase) && file.Length > 0) {
      await using var stream = file.OpenReadStream(); var result = await xml.AnalyzeAsync(stream, ct);
      return result.Format switch { "CFDI" => new("CUSTOMS_BROKER_INVOICE", .92, "Namespace y estructura CFDI reconocidos"), "COVE" => new("COVE_ACK", .90, "Referencia COVE reconocida en XML"), "DODA" => new("DODA", .90, "Estructura DODA reconocida en XML"), "DIG_ENVIO" or "DIG_RECIBO" => new("EDOCUMENT_ACK", .85, $"Estructura {result.Format} reconocida"), _ => new("OTHER", .30, "Formato XML válido, pero sin estructura conocida") };
    }
    return new("OTHER", .25, "No se encontró una regla de clasificación confiable.");
  }
}

public record DocumentAnalysisItem(string FileName, long FileSize, string Format, string Status, string? Code, string? Message, DocumentClassificationDto Classification, string FileHash, string? DuplicateOf, string? XmlFormat, IReadOnlyList<ExtractedReference> References, IReadOnlyDictionary<string, string> Metadata);
public record BulkFileResult(string FileName, string Status, Guid? DocumentId, string? Code, string? Message, string? DuplicateOf);
public record BulkUploadResult(int Total, int Accepted, int Rejected, int DuplicatesSkipped, IReadOnlyList<BulkFileResult> Files);
public interface IDocumentProcessingService { Task<IReadOnlyList<DocumentAnalysisItem>> AnalyzeAsync(Guid operationId, IReadOnlyList<IFormFile> files, CancellationToken ct); Task<BulkUploadResult> UploadBulkAsync(Guid operationId, IReadOnlyList<IFormFile> files, IReadOnlyList<string> documentTypeCodes, bool skipDuplicate, Guid userId, CancellationToken ct); }

public sealed class DocumentProcessingService(SaceDbContext db, IDocumentValidationService validation, IDocumentClassifier classifier, IXmlDocumentProcessor xml, IFileStorageService storage) : IDocumentProcessingService {
  public async Task<IReadOnlyList<DocumentAnalysisItem>> AnalyzeAsync(Guid operationId, IReadOnlyList<IFormFile> files, CancellationToken ct) {
    if (!await db.TradeOperations.AnyAsync(x => x.Id == operationId, ct)) throw new KeyNotFoundException("Operación no localizada.");
    var existing = await db.OperationDocuments.Where(x => x.TradeOperationId == operationId && x.FileHash != null).Select(x => new { x.FileHash, x.OriginalFileName }).ToListAsync(ct);
    var result = new List<DocumentAnalysisItem>();
    foreach (var file in files) {
      var check = await validation.ValidateAsync(file, ct); var hash = file.Length > 0 ? await HashAsync(file, ct) : "";
      var duplicate = hash.Length == 0 ? null : existing.FirstOrDefault(x => x.FileHash == hash)?.OriginalFileName;
      var classification = await classifier.ClassifyAsync(file, ct); XmlDocumentAnalysis? xmlInfo = null;
      if (check.Valid && Path.GetExtension(file.FileName).Equals(".xml", StringComparison.OrdinalIgnoreCase)) { await using var stream = file.OpenReadStream(); xmlInfo = await xml.AnalyzeAsync(stream, ct); }
      result.Add(new(Path.GetFileName(file.FileName), file.Length, Path.GetExtension(file.FileName).TrimStart('.').ToUpperInvariant(), check.Valid ? duplicate is null ? "Ready" : "PossibleDuplicate" : "Rejected", check.Code, check.Message, new(classification.DocumentTypeCode, classification.Confidence, classification.Reason, classification.RequiresManualSelection), hash, duplicate, xmlInfo?.Format, xmlInfo?.References ?? [], xmlInfo?.Metadata ?? new Dictionary<string, string>()));
    }
    return result;
  }
  public async Task<BulkUploadResult> UploadBulkAsync(Guid operationId, IReadOnlyList<IFormFile> files, IReadOnlyList<string> documentTypeCodes, bool skipDuplicate, Guid userId, CancellationToken ct) {
    var operation = await db.TradeOperations.FindAsync([operationId], ct) ?? throw new KeyNotFoundException("Operación no localizada.");
    var types = await db.DocumentTypes.Where(x => x.IsActive).ToDictionaryAsync(x => x.Code, StringComparer.OrdinalIgnoreCase, ct); var results = new List<BulkFileResult>();
    for (var i = 0; i < files.Count; i++) {
      var file = files[i]; var safeName = Path.GetFileName(file.FileName);
      try {
        var check = await validation.ValidateAsync(file, ct);
        if (!check.Valid) { await RejectedLog(operationId, userId, safeName, check.Code!, ct); results.Add(new(safeName, "Rejected", null, check.Code, check.Message, null)); continue; }
        var hash = await HashAsync(file, ct); var duplicate = await db.OperationDocuments.Where(x => x.TradeOperationId == operationId && x.FileHash == hash).Select(x => x.OriginalFileName).FirstOrDefaultAsync(ct);
        if (duplicate is not null && skipDuplicate) { await RejectedLog(operationId, userId, safeName, "TECH-004", ct); results.Add(new(safeName, "DuplicateSkipped", null, "DUPLICATE_FILE", $"Este archivo tiene el mismo contenido que {duplicate}.", duplicate)); continue; }
        var requestedCode = i < documentTypeCodes.Count ? documentTypeCodes[i]?.Trim() : null; var classification = await classifier.ClassifyAsync(file, ct); var code = string.IsNullOrWhiteSpace(requestedCode) ? classification.DocumentTypeCode : requestedCode;
        if (!types.TryGetValue(code!, out var type)) { results.Add(new(safeName, "Rejected", null, "UNKNOWN_DOCUMENT_TYPE", "Seleccione un tipo documental válido.", null)); continue; }
        XmlDocumentAnalysis? xmlInfo = null; if (Path.GetExtension(safeName).Equals(".xml", StringComparison.OrdinalIgnoreCase)) { await using var input = file.OpenReadStream(); xmlInfo = await xml.AnalyzeAsync(input, ct); }
        var stored = await storage.SaveAsync(operationId, type.Code, file, ct); var version = (await db.OperationDocuments.Where(x => x.TradeOperationId == operationId && x.DocumentTypeId == type.Id).MaxAsync(x => (int?)x.Version, ct) ?? 0) + 1;
        var doc = new OperationDocument { TradeOperationId = operationId, DocumentTypeId = type.Id, OriginalFileName = safeName, StoredFileName = stored.storedName, FilePath = stored.relativePath, MimeType = file.ContentType, Extension = Path.GetExtension(safeName).ToLowerInvariant(), FileSize = file.Length, Version = version, ValidationStatus = ValidationStatus.Valid, UploadedBy = userId, FileHash = hash, DocumentNumber = xmlInfo?.DocumentNumber, DocumentDate = xmlInfo?.DocumentDate, SourceSystem = xmlInfo?.Format, ExtractedMetadataJson = xmlInfo is null || xmlInfo.Metadata.Count == 0 ? null : JsonSerializer.Serialize(xmlInfo.Metadata) };
        foreach (var reference in xmlInfo?.References ?? []) doc.References.Add(new DocumentReference { ReferenceType = reference.ReferenceType, Value = reference.Value, Source = reference.Source });
        db.OperationDocuments.Add(doc); db.SystemAuditLogs.Add(new SystemAuditLog { UserId = userId, Action = duplicate is null ? "DOCUMENT_UPLOADED" : "DUPLICATE_DOCUMENT_SAVED", EntityType = "OperationDocument", EntityId = doc.Id.ToString(), Details = $"{safeName}; tipo {type.Code}; versión {version}." }); await db.SaveChangesAsync(ct);
        results.Add(new(safeName, "Accepted", doc.Id, duplicate is null ? null : "DUPLICATE_SAVED", duplicate is null ? null : $"Guardado aunque coincide con {duplicate}.", duplicate));
      } catch (Exception ex) when (ex is not OperationCanceledException) { db.ChangeTracker.Clear(); results.Add(new(safeName, "Rejected", null, "PROCESSING_ERROR", "No fue posible procesar este archivo.", null)); }
    }
    operation.UpdatedAt = DateTime.UtcNow; await db.SaveChangesAsync(ct); return new(results.Count, results.Count(x => x.Status == "Accepted"), results.Count(x => x.Status == "Rejected"), results.Count(x => x.Status == "DuplicateSkipped"), results);
  }
  private async Task RejectedLog(Guid operationId, Guid userId, string fileName, string code, CancellationToken ct) { db.SystemAuditLogs.Add(new SystemAuditLog { UserId = userId, Action = "DOCUMENT_UPLOAD_REJECTED", EntityType = "TradeOperation", EntityId = operationId.ToString(), Details = $"{Path.GetFileName(fileName)}; {code}." }); await db.SaveChangesAsync(ct); }
  public static async Task<string> HashAsync(IFormFile file, CancellationToken ct) { await using var input = file.OpenReadStream(); var hash = await SHA256.HashDataAsync(input, ct); return Convert.ToHexString(hash); }
}
