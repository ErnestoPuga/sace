using System.Globalization;
using System.Text;
using System.Text.Json;
using System.Text.RegularExpressions;
using Microsoft.EntityFrameworkCore;
using Sace.Api.Domain;
using Sace.Api.Infrastructure;
using UglyToad.PdfPig;
using UglyToad.PdfPig.DocumentLayoutAnalysis.TextExtractor;

namespace Sace.Api.Services;

public record NormalizedFieldValue(string? Value, string? Unit = null, string? Currency = null);
public interface IFieldNormalizationService { NormalizedFieldValue Normalize(string fieldCode, string? rawValue, DocumentFieldDataType dataType, string? unit = null, string? currency = null); }
public sealed class FieldNormalizationService : IFieldNormalizationService {
  private static readonly Regex Spaces = new(@"\s+", RegexOptions.Compiled);
  public NormalizedFieldValue Normalize(string fieldCode, string? rawValue, DocumentFieldDataType dataType, string? unit = null, string? currency = null) {
    if (string.IsNullOrWhiteSpace(rawValue)) return new(null, NormalizeUnit(unit), NormalizeCurrency(currency));
    var raw = Spaces.Replace(rawValue.Trim(), " ");
    return dataType switch {
      DocumentFieldDataType.DECIMAL or DocumentFieldDataType.QUANTITY => new(ParseDecimal(raw), NormalizeUnit(unit), NormalizeCurrency(currency)),
      DocumentFieldDataType.WEIGHT => NormalizeWeight(raw, unit),
      DocumentFieldDataType.CURRENCY => new(NormalizeCurrency(raw), null, NormalizeCurrency(raw)),
      DocumentFieldDataType.DATE => new(NormalizeDate(raw)),
      DocumentFieldDataType.IDENTIFIER => new(NormalizeIdentifier(fieldCode, raw)),
      _ => new(raw.ToUpperInvariant(), NormalizeUnit(unit), NormalizeCurrency(currency))
    };
  }
  private static string? ParseDecimal(string raw) {
    var cleaned = Regex.Replace(raw, @"[^0-9,\.\-]", ""); if (cleaned.Length == 0) return null;
    if (cleaned.Contains(',') && cleaned.Contains('.')) { var decimalSep = cleaned.LastIndexOf(',') > cleaned.LastIndexOf('.') ? ',' : '.'; cleaned = decimalSep == ',' ? cleaned.Replace(".", "").Replace(',', '.') : cleaned.Replace(",", ""); }
    else if (cleaned.Count(c => c == ',') == 1) { var tail = cleaned[(cleaned.IndexOf(',') + 1)..]; cleaned = tail.Length == 3 ? cleaned.Replace(",", "") : cleaned.Replace(',', '.'); }
    else if (cleaned.Count(c => c == '.') > 1) cleaned = cleaned.Replace(".", "");
    return decimal.TryParse(cleaned, NumberStyles.Number | NumberStyles.AllowLeadingSign, CultureInfo.InvariantCulture, out var value) ? value.ToString("0.############################", CultureInfo.InvariantCulture) : null;
  }
  private static NormalizedFieldValue NormalizeWeight(string raw, string? unit) {
    var value = ParseDecimal(raw); var normalizedUnit = NormalizeUnit(unit ?? Regex.Match(raw, @"(?i)\b(kg|kgs|kilogramos?|g|gr|gramos?|lb|lbs)\b").Value);
    if (value is null) return new(null, normalizedUnit);
    var number = decimal.Parse(value, CultureInfo.InvariantCulture);
    if (normalizedUnit == "G") { number /= 1000m; normalizedUnit = "KG"; }
    return new(number.ToString("0.############################", CultureInfo.InvariantCulture), normalizedUnit);
  }
  private static string? NormalizeDate(string raw) => DateTime.TryParse(raw, CultureInfo.GetCultureInfo("es-MX"), DateTimeStyles.AllowWhiteSpaces, out var date) || DateTime.TryParse(raw, CultureInfo.InvariantCulture, DateTimeStyles.AllowWhiteSpaces, out date) ? date.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture) : null;
  private static string NormalizeIdentifier(string fieldCode, string raw) { var upper = raw.ToUpperInvariant().Trim(); return fieldCode is DocumentFieldCodes.PedimentoNumber or DocumentFieldCodes.TariffFraction or DocumentFieldCodes.Nico ? Regex.Replace(upper, @"[\s.\-/]", "") : Regex.Replace(upper, @"\s+", ""); }
  private static string? NormalizeCurrency(string? value) { if (string.IsNullOrWhiteSpace(value)) return null; var v=value.Trim().ToUpperInvariant(); return v switch { "US$" or "$USD" or "DOLARES" or "DÓLARES" => "USD", "$" or "PESOS" or "MX$" => "MXN", _ => v };
  }
  private static string? NormalizeUnit(string? value) { if (string.IsNullOrWhiteSpace(value)) return null; var v=value.Trim().ToUpperInvariant(); return v switch { "PIEZA" or "PIEZAS" or "PZ" or "PZS" => "PZA", "KGS" or "KILOGRAMO" or "KILOGRAMOS" => "KG", "GRAMO" or "GRAMOS" or "GR" => "G", "LIBRA" or "LIBRAS" or "LBS" => "LB", _ => v }; }
}

public interface IPdfTextExtractor { Task<string> ExtractAsync(Stream stream, CancellationToken ct); }
public sealed class PdfPigTextExtractor : IPdfTextExtractor {
  public Task<string> ExtractAsync(Stream stream, CancellationToken ct) { try { using var memory = new MemoryStream(); stream.CopyTo(memory); using var document = PdfDocument.Open(memory.ToArray()); var text = new StringBuilder(); foreach (var page in document.GetPages()) { ct.ThrowIfCancellationRequested(); text.AppendLine(ContentOrderTextExtractor.GetText(page)); } return Task.FromResult(text.ToString().Trim()); } catch { return Task.FromResult(""); } }
}

public interface IDocumentFieldExtractor { Task<int> ExtractDocumentAsync(Guid documentId, Guid userId, CancellationToken ct = default); Task<int> ExtractOperationAsync(Guid operationId, Guid userId, CancellationToken ct = default); }
public sealed class DocumentFieldExtractor(SaceDbContext db, IFileStorageService storage, IPdfTextExtractor pdf, IFieldNormalizationService normalizer) : IDocumentFieldExtractor {
  private static readonly RegexOptions Rx = RegexOptions.IgnoreCase | RegexOptions.Multiline | RegexOptions.Compiled;
  public async Task<int> ExtractOperationAsync(Guid operationId, Guid userId, CancellationToken ct = default) { var ids=await db.OperationDocuments.Where(x=>x.TradeOperationId==operationId).Select(x=>x.Id).ToListAsync(ct); var count=0; foreach(var id in ids) count+=await ExtractDocumentAsync(id,userId,ct); return count; }
  public async Task<int> ExtractDocumentAsync(Guid documentId, Guid userId, CancellationToken ct = default) {
    var doc=await db.OperationDocuments.Include(x=>x.DocumentType).Include(x=>x.TradeOperation).Include(x=>x.References).Include(x=>x.Fields).SingleOrDefaultAsync(x=>x.Id==documentId,ct) ?? throw new KeyNotFoundException("Documento no localizado.");
    var extracted=new List<DocumentField>(); string text="";
    if(doc.Extension.Equals(".pdf",StringComparison.OrdinalIgnoreCase)) { await using var input=await storage.OpenAsync(doc.FilePath,ct); if(input is not null) text=await pdf.ExtractAsync(input,ct); }
    var metadata=ParseMetadata(doc.ExtractedMetadataJson);
    void Add(string code,string? raw,DocumentFieldDataType type,double confidence,ExtractionMethod method,string locator,string? unit=null,string? currency=null,string? groupKey=null,int? sequence=null) { if(string.IsNullOrWhiteSpace(raw))return; var n=normalizer.Normalize(code,raw,type,unit,currency); if(n.Value is null)return; extracted.Add(new DocumentField { OperationDocumentId=doc.Id,FieldCode=code,RawValue=raw.Trim(),NormalizedValue=n.Value,DataType=type,Unit=n.Unit,Currency=n.Currency,Confidence=confidence,ExtractionMethod=method,SourceLocator=locator,GroupKey=groupKey,Sequence=sequence }); }
    void Match(string code,DocumentFieldDataType type,double confidence,params string[] patterns){foreach(var pattern in patterns){var m=Regex.Match(text,pattern,Rx);if(m.Success){var value=m.Groups["value"].Success?m.Groups["value"].Value:m.Groups[1].Value;Add(code,value,type,confidence,ExtractionMethod.PDF_TEXT,$"PDF text / {code}",m.Groups["unit"].Success?m.Groups["unit"].Value:null,m.Groups["currency"].Success?m.Groups["currency"].Value:null);break;}}}
    void MatchGrouped(string code,DocumentFieldDataType type,double confidence,string groupKey,int sequence,string pattern){var m=Regex.Match(text,pattern,Rx);if(!m.Success)return;var value=m.Groups["value"].Success?m.Groups["value"].Value:m.Groups[1].Value;Add(code,value,type,confidence,ExtractionMethod.PDF_TEXT,$"PDF text / {code}",m.Groups["unit"].Success?m.Groups["unit"].Value:null,m.Groups["currency"].Success?m.Groups["currency"].Value:null,groupKey,sequence);}
    if(doc.DocumentType.Code=="PEDIMENTO") AddOperationFields(doc.TradeOperation,(code,raw,type,confidence,method,locator,unit,currency)=>Add(code,raw,type,confidence,method,locator,unit,currency));
    if(metadata.TryGetValue("UUID",out var uuid))Add(DocumentFieldCodes.InvoiceNumber,uuid,DocumentFieldDataType.IDENTIFIER,.98,ExtractionMethod.XML,"CFDI/@UUID");
    if(metadata.TryGetValue("Fecha",out var date))Add(DocumentFieldCodes.Date,date,DocumentFieldDataType.DATE,.98,ExtractionMethod.XML,"CFDI/@Fecha");
    if(metadata.TryGetValue("Total",out var total))Add(DocumentFieldCodes.TotalAmount,total,DocumentFieldDataType.DECIMAL,.98,ExtractionMethod.XML,"CFDI/@Total",currency:metadata.GetValueOrDefault("Moneda"));
    if(metadata.TryGetValue("Moneda",out var currency))Add(DocumentFieldCodes.Currency,currency,DocumentFieldDataType.CURRENCY,.98,ExtractionMethod.XML,"CFDI/@Moneda");
    if(metadata.TryGetValue("RfcEmisor",out var supplier))Add(DocumentFieldCodes.SupplierName,supplier,DocumentFieldDataType.IDENTIFIER,.9,ExtractionMethod.XML,"CFDI/Emisor/@Rfc");
    if(metadata.TryGetValue("RfcReceptor",out var buyer))Add(DocumentFieldCodes.BuyerName,buyer,DocumentFieldDataType.IDENTIFIER,.9,ExtractionMethod.XML,"CFDI/Receptor/@Rfc");
    foreach(var reference in doc.References.Where(x=>x.ReferenceType=="COVE"))Add(DocumentFieldCodes.CoveNumber,reference.Value,DocumentFieldDataType.IDENTIFIER,.98,ExtractionMethod.XML,reference.Source??"DocumentReference");
    if(text.Length>0){
      Match(DocumentFieldCodes.InvoiceNumber,DocumentFieldDataType.IDENTIFIER,.82,@"(?:factura|invoice|folio)\s*(?:n[oú]m(?:ero)?|no\.?|#)?\s*[:\-]?\s*([A-Z0-9\-/]{3,})");
      Match(DocumentFieldCodes.Date,DocumentFieldDataType.DATE,.8,@"(?:fecha|date)\s*[:\-]?\s*([0-9]{1,4}[\-/][0-9]{1,2}[\-/][0-9]{1,4})");
      Match(DocumentFieldCodes.Quantity,DocumentFieldDataType.QUANTITY,.85,@"(?:cantidad|quantity)\s*[:\-]?\s*([0-9.,]+)\s*(?<unit>PZA|PIEZAS?|PCS|KG|KGS|LB|LBS)?");
      Match(DocumentFieldCodes.Unit,DocumentFieldDataType.IDENTIFIER,.82,@"(?:unidad|unit)\s*[:\-]?\s*([A-ZÁÉÍÓÚÑ]{1,20})");
      Match(DocumentFieldCodes.TotalAmount,DocumentFieldDataType.DECIMAL,.86,@"(?:total|importe total|valor comercial)\s*[:\-]?\s*(?<currency>USD|MXN|EUR|US\$|\$)?\s*(?<value>[0-9.,]+)");
      Match(DocumentFieldCodes.Currency,DocumentFieldDataType.CURRENCY,.86,@"(?:moneda|currency)\s*[:\-]?\s*([A-Z]{3})");
      Match(DocumentFieldCodes.Incoterm,DocumentFieldDataType.IDENTIFIER,.8,@"(?:incoterm)\s*[:\-]?\s*([A-Z]{3})");
      Match(DocumentFieldCodes.Description,DocumentFieldDataType.TEXT,.72,@"(?:descripci[oó]n|description|mercanc[ií]a)\s*[:\-]?\s*([^\r\n]{3,160})");
      Match(DocumentFieldCodes.PackageCount,DocumentFieldDataType.DECIMAL,.8,@"(?:bultos?|packages?|paquetes?)\s*[:\-]?\s*([0-9.,]+)");
      Match(DocumentFieldCodes.NetWeight,DocumentFieldDataType.WEIGHT,.82,@"(?:peso neto|net weight)\s*[:\-]?\s*([0-9.,]+)\s*(?<unit>KG|KGS|G|LB|LBS)?");
      Match(DocumentFieldCodes.GrossWeight,DocumentFieldDataType.WEIGHT,.82,@"(?:peso bruto|gross weight)\s*[:\-]?\s*([0-9.,]+)\s*(?<unit>KG|KGS|G|LB|LBS)?");
      Match(DocumentFieldCodes.TransportReference,DocumentFieldDataType.IDENTIFIER,.8,@"(?:gu[ií]a|awb|b\/l|bill of lading|referencia de transporte)\s*[:#\-]?\s*([A-Z0-9\-/]{3,})");
      Match(DocumentFieldCodes.SupplierName,DocumentFieldDataType.TEXT,.72,@"(?:proveedor|supplier|vendedor)\s*[:\-]?\s*([^\r\n]{3,120})"); Match(DocumentFieldCodes.BuyerName,DocumentFieldDataType.TEXT,.72,@"(?:importador|comprador|buyer|destinatario)\s*[:\-]?\s*([^\r\n]{3,120})");
      Match(DocumentFieldCodes.DocumentNumber,DocumentFieldDataType.IDENTIFIER,.82,@"(?:n[uú]mero\s+de\s+documento|document\s+(?:number|no\.?))\s*[:#\-]?\s*([A-Z0-9\-/]{3,})");
      Match(DocumentFieldCodes.ServiceProvider,DocumentFieldDataType.TEXT,.78,@"(?:prestador\s+de\s+servicio|service\s+provider|transportista)\s*[:\-]?\s*([^\r\n]{3,120})");
      Match(DocumentFieldCodes.ServiceConcept,DocumentFieldDataType.TEXT,.78,@"(?:concepto\s+de\s+servicio|service\s+concept|concepto)\s*[:\-]?\s*([^\r\n]{3,160})");
      Match(DocumentFieldCodes.FreightAmount,DocumentFieldDataType.DECIMAL,.86,@"(?:importe\s+(?:de\s+)?flete|flete|freight(?:\s+amount)?)\s*[:\-]?\s*(?<currency>USD|MXN|EUR|US\$|\$)?\s*(?<value>[0-9.,]+)");
      Match(DocumentFieldCodes.InsuranceAmount,DocumentFieldDataType.DECIMAL,.86,@"(?:prima\s+atribuible|prima\s+(?:de\s+)?seguro|importe\s+(?:de\s+)?seguro|insurance(?:\s+premium|\s+amount)?)\s*[:\-]?\s*(?<currency>USD|MXN|EUR|US\$|\$)?\s*(?<value>[0-9.,]+)");
      Match(DocumentFieldCodes.PackagingAmount,DocumentFieldDataType.DECIMAL,.84,@"(?:importe\s+(?:de\s+)?embalaje|embalaje|packaging(?:\s+amount)?)\s*[:\-]?\s*(?<currency>USD|MXN|EUR|US\$|\$)?\s*(?<value>[0-9.,]+)");
      Match(DocumentFieldCodes.OtherIncrementableAmount,DocumentFieldDataType.DECIMAL,.78,@"(?:otros?\s+incrementables?|other\s+incrementables?)\s*[:\-]?\s*(?<currency>USD|MXN|EUR|US\$|\$)?\s*(?<value>[0-9.,]+)");
      Match(DocumentFieldCodes.IncrementableTotal,DocumentFieldDataType.DECIMAL,.86,@"(?:total\s+(?:de\s+)?incrementables?|incrementables?\s+total)\s*[:\-]?\s*(?<currency>USD|MXN|EUR|US\$|\$)?\s*(?<value>[0-9.,]+)");
      Match(DocumentFieldCodes.TransactionValue,DocumentFieldDataType.DECIMAL,.86,@"(?:valor\s+de\s+transacci[oó]n|transaction\s+value)\s*[:\-]?\s*(?<currency>USD|MXN|EUR|US\$|\$)?\s*(?<value>[0-9.,]+)");
      Match(DocumentFieldCodes.PricePaid,DocumentFieldDataType.DECIMAL,.84,@"(?:precio\s+pagado|price\s+paid)\s*[:\-]?\s*(?<currency>USD|MXN|EUR|US\$|\$)?\s*(?<value>[0-9.,]+)");
      Match(DocumentFieldCodes.PricePayable,DocumentFieldDataType.DECIMAL,.84,@"(?:precio\s+por\s+pagar|price\s+payable)\s*[:\-]?\s*(?<currency>USD|MXN|EUR|US\$|\$)?\s*(?<value>[0-9.,]+)");
      Match(DocumentFieldCodes.DeclaredFreight,DocumentFieldDataType.DECIMAL,.88,@"(?:flete\s+declarado|declared\s+freight)\s*[:\-]?\s*(?<currency>USD|MXN|EUR|US\$|\$)?\s*(?<value>[0-9.,]+)");
      Match(DocumentFieldCodes.DeclaredInsurance,DocumentFieldDataType.DECIMAL,.88,@"(?:seguro\s+declarado|declared\s+insurance)\s*[:\-]?\s*(?<currency>USD|MXN|EUR|US\$|\$)?\s*(?<value>[0-9.,]+)");
      Match(DocumentFieldCodes.DeclaredPackaging,DocumentFieldDataType.DECIMAL,.88,@"(?:embalaje\s+declarado|declared\s+packaging)\s*[:\-]?\s*(?<currency>USD|MXN|EUR|US\$|\$)?\s*(?<value>[0-9.,]+)");
      Match(DocumentFieldCodes.DeclaredOtherIncrementables,DocumentFieldDataType.DECIMAL,.86,@"(?:otros?\s+incrementables?\s+declarados?|declared\s+other\s+incrementables?)\s*[:\-]?\s*(?<currency>USD|MXN|EUR|US\$|\$)?\s*(?<value>[0-9.,]+)");
      Match(DocumentFieldCodes.DeclaredCustomsValue,DocumentFieldDataType.DECIMAL,.9,@"(?:valor\s+en\s+aduana(?:\s+declarado)?|declared\s+customs\s+value)\s*[:\-]?\s*(?<currency>USD|MXN|EUR|US\$|\$)?\s*(?<value>[0-9.,]+)");
      Match(DocumentFieldCodes.PaymentBeneficiary,DocumentFieldDataType.TEXT,.82,@"(?:beneficiario|payment\s+beneficiary)\s*[:\-]?\s*([^\r\n]{3,120})");
      Match(DocumentFieldCodes.PaymentAmount,DocumentFieldDataType.DECIMAL,.88,@"(?:importe\s+(?:del\s+)?pago|payment\s+amount|monto\s+pagado)\s*[:\-]?\s*(?<currency>USD|MXN|EUR|US\$|\$)?\s*(?<value>[0-9.,]+)");
      Match(DocumentFieldCodes.PaymentDate,DocumentFieldDataType.DATE,.84,@"(?:fecha\s+(?:del\s+)?pago|payment\s+date)\s*[:\-]?\s*([0-9]{1,4}[\-/][0-9]{1,2}[\-/][0-9]{1,4})");
      Match(DocumentFieldCodes.PaymentReference,DocumentFieldDataType.IDENTIFIER,.84,@"(?:referencia\s+(?:del\s+)?pago|payment\s+reference)\s*[:#\-]?\s*([A-Z0-9\-/]{3,})");
      Match(DocumentFieldCodes.PolicyNumber,DocumentFieldDataType.IDENTIFIER,.88,@"(?:n[uú]mero\s+de\s+p[oó]liza|policy\s+(?:number|no\.?))\s*[:#\-]?\s*([A-Z0-9\-/]{3,})");
      Match(DocumentFieldCodes.ValidFrom,DocumentFieldDataType.DATE,.8,@"(?:vigencia\s+desde|v[aá]lida\s+desde|valid\s+from)\s*[:\-]?\s*([0-9]{1,4}[\-/][0-9]{1,2}[\-/][0-9]{1,4})");
      Match(DocumentFieldCodes.ValidTo,DocumentFieldDataType.DATE,.8,@"(?:vigencia\s+hasta|v[aá]lida\s+hasta|valid\s+to)\s*[:\-]?\s*([0-9]{1,4}[\-/][0-9]{1,2}[\-/][0-9]{1,4})");
      Match(DocumentFieldCodes.InsuredName,DocumentFieldDataType.TEXT,.8,@"(?:asegurado|insured(?:\s+name)?)\s*[:\-]?\s*([^\r\n]{3,120})");
      Match(DocumentFieldCodes.InsuredGoods,DocumentFieldDataType.TEXT,.76,@"(?:bienes\s+asegurados|mercanc[ií]a\s+asegurada|insured\s+goods)\s*[:\-]?\s*([^\r\n]{3,160})");
      Match(DocumentFieldCodes.ValuationMethod,DocumentFieldDataType.TEXT,.8,@"(?:m[eé]todo\s+de\s+valoraci[oó]n|valuation\s+method)\s*[:\-]?\s*([^\r\n]{2,100})");
      Match(DocumentFieldCodes.ExchangeRate,DocumentFieldDataType.DECIMAL,.9,@"(?:tipo\s+de\s+cambio|exchange\s+rate)\s*[:\-]?\s*([0-9.,]+)");
      var contribution=Regex.Match(text,@"(?im)^(?:contribuci[oó]n|contribution)\s*[:\-]?\s*(?<code>[A-Z0-9\-]{2,20})(?:\s+[^\r\n]*)?$");if(contribution.Success){const string group="CONTRIBUTION-1";Add(DocumentFieldCodes.ContributionCode,contribution.Groups["code"].Value,DocumentFieldDataType.IDENTIFIER,.82,ExtractionMethod.PDF_TEXT,"PDF text / contribution code",groupKey:group,sequence:1);MatchGrouped(DocumentFieldCodes.ContributionBase,DocumentFieldDataType.DECIMAL,.8,group,1,@"(?:base\s+(?:de\s+)?contribuci[oó]n|tax\s+base)\s*[:\-]?\s*([0-9.,]+)");MatchGrouped(DocumentFieldCodes.ContributionRate,DocumentFieldDataType.DECIMAL,.8,group,1,@"(?:tasa|rate)\s*[:\-]?\s*([0-9.,]+)\s*%?");MatchGrouped(DocumentFieldCodes.ContributionDeclaredAmount,DocumentFieldDataType.DECIMAL,.84,group,1,@"(?:contribuci[oó]n\s+declarada|declared\s+(?:tax|amount))\s*[:\-]?\s*([0-9.,]+)");MatchGrouped(DocumentFieldCodes.ContributionPaidAmount,DocumentFieldDataType.DECIMAL,.84,group,1,@"(?:contribuci[oó]n\s+pagada|paid\s+(?:tax|amount))\s*[:\-]?\s*([0-9.,]+)");}
    }
    var automatic=doc.Fields.Where(x=>!x.IsManuallyConfirmed).ToList(); db.DocumentFields.RemoveRange(automatic); db.DocumentFields.AddRange(extracted);
    db.SystemAuditLogs.Add(new SystemAuditLog{UserId=await ValidUser(userId,ct),Action="DOCUMENT_FIELDS_EXTRACTED",EntityType="OperationDocument",EntityId=doc.Id.ToString(),Details=$"{extracted.Count} campos automáticos extraídos; se preservaron {doc.Fields.Count(x=>x.IsManuallyConfirmed)} confirmaciones manuales."}); await db.SaveChangesAsync(ct); return extracted.Count;
  }
  private static Dictionary<string,string> ParseMetadata(string? json){try{return json is null?new():JsonSerializer.Deserialize<Dictionary<string,string>>(json)??new();}catch{return new();}}
  private static void AddOperationFields(TradeOperation op,Action<string,string?,DocumentFieldDataType,double,ExtractionMethod,string,string?,string?> add){
    void A(string code,object? value,DocumentFieldDataType type,string? unit=null,string? currency=null)=>add(code,value?.ToString(),type,.99,ExtractionMethod.EXISTING_METADATA,$"TradeOperation.{code}",unit,currency);
    A(DocumentFieldCodes.PedimentoNumber,op.PedimentoNumber,DocumentFieldDataType.IDENTIFIER); A(DocumentFieldCodes.OperationType,op.OperationType,DocumentFieldDataType.IDENTIFIER); A(DocumentFieldCodes.EntryDate,op.OperationDate.ToString("O"),DocumentFieldDataType.DATE); A(DocumentFieldCodes.CustomsOffice,op.CustomsOffice,DocumentFieldDataType.TEXT); A(DocumentFieldCodes.InvoiceNumber,op.InvoiceNumber,DocumentFieldDataType.IDENTIFIER); A(DocumentFieldCodes.CoveNumber,op.CoveNumber,DocumentFieldDataType.IDENTIFIER); A(DocumentFieldCodes.TariffFraction,op.TariffFraction,DocumentFieldDataType.IDENTIFIER); A(DocumentFieldCodes.Nico,op.Nico,DocumentFieldDataType.IDENTIFIER); A(DocumentFieldCodes.Description,op.GoodsDescription,DocumentFieldDataType.TEXT); A(DocumentFieldCodes.Quantity,op.Quantity,DocumentFieldDataType.QUANTITY,op.Unit); A(DocumentFieldCodes.Unit,op.Unit,DocumentFieldDataType.IDENTIFIER); A(DocumentFieldCodes.CommercialValue,op.CommercialValue,DocumentFieldDataType.DECIMAL,currency:op.Currency); A(DocumentFieldCodes.Currency,op.Currency,DocumentFieldDataType.CURRENCY); A(DocumentFieldCodes.GrossWeight,op.GrossWeight,DocumentFieldDataType.WEIGHT,"KG"); A(DocumentFieldCodes.Incoterm,op.Incoterm,DocumentFieldDataType.IDENTIFIER); A(DocumentFieldCodes.ExchangeRate,op.ExchangeRate,DocumentFieldDataType.DECIMAL); A(DocumentFieldCodes.DeclaredCustomsValue,op.DeclaredCustomsValueMxn,DocumentFieldDataType.DECIMAL,currency:"MXN");
  }
  private async Task<Guid?> ValidUser(Guid userId,CancellationToken ct)=>userId!=Guid.Empty&&await db.Users.AnyAsync(x=>x.Id==userId,ct)?userId:null;
}

public interface ICrossValidationEngine { Task<IReadOnlyList<CrossValidationResult>> ExecuteAsync(Guid operationId, Guid? auditId, Guid userId, CancellationToken ct = default); }
public sealed class CrossValidationEngine(SaceDbContext db, IDocumentFieldExtractor extractor) : ICrossValidationEngine {
  private static readonly Dictionary<string,string[]> Aliases=new(StringComparer.OrdinalIgnoreCase){["COMMERCIAL_INVOICE"]=["COMMERCIAL_INVOICE","INVOICE"],["COVE_ACK"]=["COVE_ACK"],["PACKING_LIST"]=["PACKING_LIST"],["TRANSPORT_DOCUMENT"]=["TRANSPORT_DOCUMENT"],["PEDIMENTO"]=["PEDIMENTO"]};
  public async Task<IReadOnlyList<CrossValidationResult>> ExecuteAsync(Guid operationId,Guid? auditId,Guid userId,CancellationToken ct=default){
    var operation=await db.TradeOperations.SingleOrDefaultAsync(x=>x.Id==operationId,ct)??throw new KeyNotFoundException("Operación no localizada."); db.SystemAuditLogs.Add(new SystemAuditLog{UserId=await ValidUser(userId,ct),Action="CROSS_VALIDATION_STARTED",EntityType="TradeOperation",EntityId=operationId.ToString(),Details="Inició el cruce técnico/documental."}); await db.SaveChangesAsync(ct);
    await extractor.ExtractOperationAsync(operationId,userId,ct);
    var documents=await db.OperationDocuments.AsNoTracking().Where(x=>x.TradeOperationId==operationId).Include(x=>x.DocumentType).Include(x=>x.Fields).ToListAsync(ct);
    var allRules=await db.CrossValidationRules.Where(x=>x.IsActive&&(x.EffectiveFrom==null||x.EffectiveFrom<=operation.OperationDate)&&(x.EffectiveTo==null||operation.OperationDate<=x.EffectiveTo)).OrderByDescending(x=>x.Version).ToListAsync(ct); var rules=allRules.GroupBy(x=>x.Code).Select(x=>x.First()).ToList(); var results=new List<CrossValidationResult>();
    foreach(var rule in rules){var source=Latest(documents,rule.SourceDocumentTypeCode);var target=Latest(documents,rule.TargetDocumentTypeCode);var sf=source?.Fields.FirstOrDefault(x=>x.FieldCode==rule.SourceFieldCode);var tf=target?.Fields.FirstOrDefault(x=>x.FieldCode==rule.TargetFieldCode); var targetSnapshot=tf is null&&rule.TargetDocumentTypeCode=="PEDIMENTO"?OperationValue(operation,rule.TargetFieldCode):null; var result=Evaluate(operation,auditId,rule,source,target,sf,tf,targetSnapshot);db.CrossValidationResults.Add(result);results.Add(result);}
    db.SystemAuditLogs.Add(new SystemAuditLog{UserId=await ValidUser(userId,ct),Action="CROSS_VALIDATION_COMPLETED",EntityType="TradeOperation",EntityId=operationId.ToString(),Details=$"{results.Count} reglas: {results.Count(x=>x.Outcome==CrossValidationOutcome.MATCH)} coincidencias, {results.Count(x=>x.Outcome==CrossValidationOutcome.MISMATCH)} diferencias."}); await db.SaveChangesAsync(ct); return results;
  }
  private static OperationDocument? Latest(List<OperationDocument> docs,string code){var codes=Aliases.GetValueOrDefault(code)??[code];return docs.Where(x=>codes.Contains(x.DocumentType.Code,StringComparer.OrdinalIgnoreCase)).OrderByDescending(x=>x.Version).ThenByDescending(x=>x.UploadedAt).FirstOrDefault();}
  private static CrossValidationResult Evaluate(TradeOperation op,Guid? auditId,CrossValidationRule rule,OperationDocument? source,OperationDocument? target,DocumentField? sf,DocumentField? tf,string? targetFallback){
    var s=sf?.NormalizedValue;var t=tf?.NormalizedValue??targetFallback;var outcome=CrossValidationOutcome.MATCH;decimal? diff=null,pct=null;string explanation;
    if(string.IsNullOrWhiteSpace(s)||string.IsNullOrWhiteSpace(t)){outcome=rule.MissingEvidenceBehavior switch{MissingEvidenceBehavior.MANUAL_REVIEW=>CrossValidationOutcome.MANUAL_REVIEW,MissingEvidenceBehavior.NOT_APPLICABLE=>CrossValidationOutcome.NOT_APPLICABLE,_=>CrossValidationOutcome.INSUFFICIENT_EVIDENCE};explanation=$"No existen ambas evidencias comparables para {rule.SourceFieldCode} ↔ {rule.TargetFieldCode}.";}
    else if(rule.ComparisonType is CrossComparisonType.EXACT_NUMERIC or CrossComparisonType.NUMERIC_TOLERANCE or CrossComparisonType.PERCENT_TOLERANCE or CrossComparisonType.CURRENCY_AMOUNT){if(!decimal.TryParse(s,NumberStyles.Number,CultureInfo.InvariantCulture,out var sv)||!decimal.TryParse(t,NumberStyles.Number,CultureInfo.InvariantCulture,out var tv)){outcome=CrossValidationOutcome.MANUAL_REVIEW;explanation="Los valores existen, pero no pudieron interpretarse de forma numérica inequívoca.";}else{diff=Math.Abs(sv-tv);pct=tv==0?(sv==0?0:null):Math.Abs((sv-tv)/tv*100m);var match=rule.ComparisonType switch{CrossComparisonType.EXACT_NUMERIC or CrossComparisonType.CURRENCY_AMOUNT=>diff==0,CrossComparisonType.NUMERIC_TOLERANCE=>diff<=(rule.AbsoluteTolerance??0),CrossComparisonType.PERCENT_TOLERANCE=>pct.HasValue&&pct.Value<=(rule.PercentTolerance??0),_=>false};outcome=match?CrossValidationOutcome.MATCH:CrossValidationOutcome.MISMATCH;explanation=match?$"Los valores coinciden dentro de la tolerancia configurada ({Tolerance(rule)}).":$"Diferencia documental detectada: {diff.Value.ToString(CultureInfo.InvariantCulture)}; tolerancia {Tolerance(rule)}.";}}
    else{var match=rule.ComparisonType==CrossComparisonType.EXACT_TEXT?string.Equals(sf?.RawValue,tf?.RawValue??targetFallback,StringComparison.Ordinal):string.Equals(s,t,StringComparison.OrdinalIgnoreCase);outcome=match?CrossValidationOutcome.MATCH:CrossValidationOutcome.MISMATCH;explanation=match?"Los valores documentales comparados coinciden.":"Los valores documentales comparados son diferentes; este resultado no constituye una conclusión legal.";}
    if(tf is null&&!string.IsNullOrWhiteSpace(targetFallback))explanation+=" El valor destino proviene de TradeOperation y se identifica como EXISTING_METADATA; no fue extraído del archivo.";
    return new CrossValidationResult{TradeOperationId=op.Id,AuditId=auditId,CrossValidationRuleId=rule.Id,RuleCode=rule.Code,RuleVersion=rule.Version,SourceDocumentId=source?.Id,TargetDocumentId=target?.Id,SourceFieldId=sf?.Id,TargetFieldId=tf?.Id,SourceRawValue=sf?.RawValue,TargetRawValue=tf?.RawValue??targetFallback,SourceNormalizedValue=s,TargetNormalizedValue=t,Outcome=outcome,NumericDifference=diff,PercentDifference=pct,Confidence=Min(sf?.Confidence,tf?.Confidence),Explanation=explanation};
  }
  private static string? OperationValue(TradeOperation op,string code)=>code switch{DocumentFieldCodes.InvoiceNumber=>op.InvoiceNumber,DocumentFieldCodes.Quantity=>op.Quantity?.ToString(CultureInfo.InvariantCulture),DocumentFieldCodes.Unit=>NormalizeSimpleUnit(op.Unit),DocumentFieldCodes.CommercialValue=>op.CommercialValue?.ToString(CultureInfo.InvariantCulture),DocumentFieldCodes.DeclaredCustomsValue=>op.DeclaredCustomsValueMxn?.ToString(CultureInfo.InvariantCulture),DocumentFieldCodes.ExchangeRate=>op.ExchangeRate?.ToString(CultureInfo.InvariantCulture),DocumentFieldCodes.Currency=>op.Currency?.ToUpperInvariant(),DocumentFieldCodes.Incoterm=>op.Incoterm?.ToUpperInvariant(),DocumentFieldCodes.Description=>op.GoodsDescription?.Trim().ToUpperInvariant(),DocumentFieldCodes.GrossWeight=>op.GrossWeight?.ToString(CultureInfo.InvariantCulture),_=>null};
  private static string? NormalizeSimpleUnit(string? unit)=>unit?.Trim().ToUpperInvariant() switch{"PIEZA" or "PIEZAS" or "PZ" or "PZS"=>"PZA",var value=>value}; private static double? Min(double? a,double? b)=>a.HasValue&&b.HasValue?Math.Min(a.Value,b.Value):a??b; private static string Tolerance(CrossValidationRule r)=>r.ComparisonType==CrossComparisonType.PERCENT_TOLERANCE?$"{r.PercentTolerance??0}%":(r.AbsoluteTolerance??0).ToString(CultureInfo.InvariantCulture);
  private async Task<Guid?> ValidUser(Guid userId,CancellationToken ct)=>userId!=Guid.Empty&&await db.Users.AnyAsync(x=>x.Id==userId,ct)?userId:null;
}
