using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;
using Microsoft.AspNetCore.Http;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Sace.Api.Domain;
using Sace.Api.Infrastructure;
using Sace.Api.Services;
using Xunit;

namespace Sace.Api.Tests;

public class DocumentIntelligenceLayerTests {
  [Theory]
  [InlineData("Qty (PCS): 6500","6500","PZA")]
  [InlineData("Quantity (Pieces): 6500","6500","PZA")]
  [InlineData("Cantidad (PZA): 6500","6500","PZA")]
  [InlineData("Quantity (Pairs): 500","500","PAR")]
  public void Quantity_labels_and_units_are_canonical_without_fixed_coordinates(string text,string expected,string unit){
    var fields=Resolver().Resolve(Layout(Line(1,text,5,10)),"COMMERCIAL_INVOICE",Aliases(),Units(),[],null,false);
    var quantity=Assert.Single(fields,x=>x.CanonicalCode==DocumentFieldCodes.Quantity);
    Assert.Equal(expected,quantity.NormalizedCandidate);
    Assert.Equal(unit,quantity.UnitCandidate);
    Assert.Equal(unit,Assert.Single(fields,x=>x.CanonicalCode==DocumentFieldCodes.Unit).NormalizedCandidate);
    Assert.NotNull(quantity.Region);
  }

  [Fact]
  public void Context_separates_line_total_invoice_total_and_rejects_ambiguous_amount(){
    var fields=Resolver().Resolve(Layout(Line(1,"Line Amount: 120.50"),Line(1,"Invoice Total: 500.25"),Line(1,"Amount: 999")),"COMMERCIAL_INVOICE",Aliases(),Units(),[],null,false);
    Assert.Equal("120.5",Assert.Single(fields,x=>x.CanonicalCode==DocumentFieldCodes.LineTotal).NormalizedCandidate);
    Assert.Equal("500.25",Assert.Single(fields,x=>x.CanonicalCode==DocumentFieldCodes.CommercialTotal).NormalizedCandidate);
    Assert.DoesNotContain(fields,x=>x.RawValue=="999");
  }

  [Fact]
  public void Controlled_inference_is_low_confidence_reviewable_and_keeps_reason(){
    var inferred=Assert.Single(Resolver().Resolve(Layout(Line(1,"Shipment arranged as Ocean Freight")),"COMMERCIAL_INVOICE",Aliases(),Units(),[],null,false));
    Assert.Equal(DocumentFieldCodes.ShipmentMethod,inferred.CanonicalCode);Assert.Equal("SEA",inferred.NormalizedCandidate);Assert.Equal(DocumentFieldSourceKind.INFERRED,inferred.SourceKind);Assert.Equal(DocumentIntelligenceConfidence.LOW,inferred.ConfidenceLevel);Assert.True(inferred.RequiresReview);Assert.Contains(inferred.Signals,x=>x.Contains("Inferencia controlada"));
  }

  [Fact]
  public void Line_items_are_extracted_from_pages_two_and_three_without_assuming_same_order(){
    var header=new[]{"Part No.","Description","Qty","Unit","Line Total"};
    DocumentLayoutPage Page(int number,string part,string description)=>new(number,$"{part} {description}",[
      new(number,string.Join(" | ",header),new(1,10,90,20),0,0,header),
      new(number,$"{part} | {description} | 10 | PCS | 25",new(1,1,90,9),0,1,[part,description,"10","PCS","25"])
    ]);
    var items=new SemanticLineItemExtractor().Extract(Guid.NewGuid(),new([Page(2,"B-200","SECOND"),Page(3,"A-100","THIRD")],"TEST"),Units(),100,false);
    Assert.Equal(2,items.Count);
    Assert.Collection(items,x=>{Assert.Equal(2,x.PageNumber);Assert.Equal("B-200",x.PartNumber);},x=>{Assert.Equal(3,x.PageNumber);Assert.Equal("A-100",x.PartNumber);});
    Assert.All(items,x=>Assert.Equal("PZA",x.Unit));
  }

  [Fact]
  public void Unit_price_is_calculated_only_when_absent_and_extracted_value_wins_with_consistency_flag(){
    var documentId=Guid.NewGuid();
    var fields=new List<DocumentField>{Field(documentId,DocumentFieldCodes.Quantity,"4","ROW-1"),Field(documentId,DocumentFieldCodes.LineTotal,"100","ROW-1")};
    var calculated=Assert.Single(new CalculatedFieldService().Apply(documentId,1,fields));
    Assert.Equal("25",calculated.NormalizedValue);Assert.Equal(DocumentFieldSourceKind.CALCULATED,calculated.SourceKind);Assert.Equal("LINE_TOTAL / QUANTITY",calculated.Formula);
    var extracted=Field(documentId,DocumentFieldCodes.UnitPrice,"24","ROW-1");fields.Add(extracted);
    Assert.Empty(new CalculatedFieldService().Apply(documentId,2,fields));
    Assert.True(extracted.RequiresReview);Assert.Contains("25",extracted.RecognitionReason);
  }

  [Fact]
  public void Volume_is_calculated_only_from_three_verifiable_homogeneous_dimensions(){
    var documentId=Guid.NewGuid();var fields=new List<DocumentField>{Field(documentId,DocumentFieldCodes.Length,"100","ROW-1"),Field(documentId,DocumentFieldCodes.Width,"50","ROW-1"),Field(documentId,DocumentFieldCodes.Height,"20","ROW-1")};foreach(var field in fields)field.Unit="CM";
    var volume=Assert.Single(new CalculatedFieldService().Apply(documentId,1,fields));
    Assert.Equal(DocumentFieldCodes.VolumeCbm,volume.CanonicalFieldCode);Assert.Equal("0.1",volume.NormalizedValue);Assert.Equal("M3",volume.Unit);Assert.Equal(DocumentFieldSourceKind.CALCULATED,volume.SourceKind);
    fields[2].Unit="IN";Assert.Empty(new CalculatedFieldService().Apply(documentId,2,fields));
  }

  [Fact]
  public void Provider_mapping_is_only_a_hint_when_exact_layout_fingerprint_matches(){
    var profile=new ProviderDocumentProfile{ProviderKey="ACME",ProviderName="ACME",DocumentTypeCode="COMMERCIAL_INVOICE",Status=ProviderProfileStatus.ACTIVE,LayoutFingerprint="FP-1"};
    var mapping=new ApprovedFieldMapping{ProviderProfile=profile,ProviderProfileId=profile.Id,CanonicalFieldCode=DocumentFieldCodes.Quantity,OriginalLabel="Custom Qty",ContextFingerprint="CTX",DocumentTypeCode="COMMERCIAL_INVOICE",ApprovedBy=Guid.NewGuid(),Evidence="human"};
    var layout=Layout(Line(1,"Custom Qty: 12 PCS"));
    Assert.Single(Resolver().Resolve(layout,"COMMERCIAL_INVOICE",[],Units(),[mapping],"FP-1",false),x=>x.CanonicalCode==DocumentFieldCodes.Quantity);
    Assert.Empty(Resolver().Resolve(layout,"COMMERCIAL_INVOICE",[],Units(),[mapping],"DIFFERENT",false));
  }

  [Fact]
  public async Task Ocr_fallback_populates_fields_roles_and_history_while_unavailable_ocr_degrades_safely(){
    await using var fixture=await PipelineFixture.Create();
    var success=await fixture.Analyze(new FakeOcr(true,true,null,[new(1,"Invoice\nSeller: ACME\nBuyer: BUYER MX\nShip To: WAREHOUSE\nManufacturer: MAKER\nQty (PCS): 6500",[
      Line(1,"Invoice Number: OCR-123"),Line(1,"Seller: ACME"),Line(1,"Buyer: BUYER MX"),Line(1,"Ship To: WAREHOUSE"),Line(1,"Manufacturer: MAKER"),Line(1,"Qty (PCS): 6500")])],[1],"ok"));
    Assert.True(success.OcrUsed);Assert.True(success.FieldCount>=6);
    Assert.Equal(4,await fixture.Db.DocumentParticipants.CountAsync(x=>x.OperationDocumentId==success.DocumentId));
    var snapshot=await fixture.Db.DocumentIntelligenceAnalyses.SingleAsync(x=>x.OperationDocumentId==success.DocumentId);
    Assert.Contains("OCR-123",snapshot.SnapshotJson);Assert.Equal("OCR",snapshot.ReadingMethod);

    var failed=await fixture.NewDocument("unreadable.pdf");
    var unavailable=await fixture.Analyze(new FakeOcr(false,false,"OCR_UNAVAILABLE",[],[],"not installed"),failed.Id);
    Assert.Equal(DocumentIntelligenceStatus.OCR_UNAVAILABLE,unavailable.Status);Assert.False(unavailable.OcrUsed);Assert.Equal(0,unavailable.FieldCount);
  }

  [Fact]
  public async Task Manual_correction_preserves_history_and_survives_reanalysis(){
    await using var fixture=await PipelineFixture.Create();
    var result=await fixture.Analyze(new FakeOcr(true,true,null,[new(1,"Invoice\nQty (PCS): 6500",[Line(1,"Invoice Number: OCR-123"),Line(1,"Qty (PCS): 6500")])],[1],"ok"));
    var field=await fixture.Db.DocumentFields.FirstAsync(x=>x.OperationDocumentId==result.DocumentId&&x.CanonicalFieldCode==DocumentFieldCodes.Quantity);
    var service=new DocumentFieldCorrectionService(fixture.Db,new FieldNormalizationService());
    await service.CorrectAsync(field.Id,new(DocumentFieldCodes.Quantity,"6400","PZA","Corrección contra documento fuente","página 1"),fixture.User.Id);
    Assert.Single(await fixture.Db.DocumentFieldCorrections.Where(x=>x.DocumentFieldId==field.Id).ToListAsync());
    var second=await fixture.Analyze(new FakeOcr(true,true,null,[new(1,"Invoice\nQty (PCS): 6500",[Line(1,"Invoice Number: OCR-123"),Line(1,"Qty (PCS): 6500")])],[1],"ok"),result.DocumentId);
    var preserved=await fixture.Db.DocumentFields.SingleAsync(x=>x.Id==field.Id);
    Assert.Equal("6400",preserved.NormalizedValue);Assert.True(preserved.IsManuallyConfirmed);Assert.Equal(DocumentFieldSourceKind.MANUAL,preserved.SourceKind);
    Assert.Equal(2,second.Version);Assert.Equal(2,await fixture.Db.DocumentIntelligenceAnalyses.CountAsync(x=>x.OperationDocumentId==result.DocumentId));
  }

  [Fact]
  public async Task Seed_is_idempotent_and_http_diagnostics_are_authenticated(){
    using var factory=new SaceFactory();var anonymous=factory.CreateClient();
    Assert.Equal(HttpStatusCode.Unauthorized,(await anonymous.GetAsync($"/api/documents/{Guid.NewGuid()}/intelligence")).StatusCode);
    await DbSeeder.SeedAsync(factory.Services);
    await using(var first=factory.Services.CreateAsyncScope()){
      var db=first.ServiceProvider.GetRequiredService<SaceDbContext>();
      var counts=new[]{await db.CanonicalFieldDefinitions.CountAsync(),await db.FieldAliases.CountAsync(),await db.UnitAliases.CountAsync()};
      await DbSeeder.SeedAsync(factory.Services);
      Assert.Equal(counts[0],await db.CanonicalFieldDefinitions.CountAsync());Assert.Equal(counts[1],await db.FieldAliases.CountAsync());Assert.Equal(counts[2],await db.UnitAliases.CountAsync());
      Assert.Equal(await db.CanonicalFieldDefinitions.CountAsync(),await db.CanonicalFieldDefinitions.Select(x=>x.Code).Distinct().CountAsync());
    }
    var client=await Client(factory);var catalog=await client.GetAsync("/api/document-intelligence/canonical-fields");catalog.EnsureSuccessStatusCode();
    using var json=JsonDocument.Parse(await catalog.Content.ReadAsStringAsync());Assert.True(json.RootElement.GetArrayLength()>50);
  }

  [Fact]
  public async Task Migration_is_fresh_incremental_and_down_removes_only_document_intelligence(){
    const string previous="20261108221726_DocumentReconciliationEngineV1";var dir=Path.Combine(Path.GetTempPath(),$"sace-doc-int-{Guid.NewGuid():N}");Directory.CreateDirectory(dir);var path=Path.Combine(dir,"test.db");
    try{await using(var db=Context(path)){var migrator=db.Database.GetService<IMigrator>();await migrator.MigrateAsync(previous);Assert.False(await Table(db,"CanonicalFieldDefinitions"));Assert.True(await Table(db,"DocumentReconciliationProfiles"));await migrator.MigrateAsync();Assert.True(await Table(db,"CanonicalFieldDefinitions"));Assert.True(await Table(db,"DocumentIntelligenceAnalyses"));Assert.True(await Table(db,"DocumentFieldCorrections"));
        var type=new DocumentType{Code="DI_TEST",Name="Document intelligence test"};var user=new User{Name="Migration",Email=$"migration-{Guid.NewGuid():N}@example.invalid",PasswordHash="x"};var operation=new TradeOperation{Folio=$"DI-MIG-{Guid.NewGuid():N}",PedimentoNumber="26",Period="2026-11",OperationDate=new(2026,11,12)};var doc=new OperationDocument{TradeOperation=operation,DocumentType=type,OriginalFileName="legacy.pdf",StoredFileName="legacy.pdf",FilePath="legacy.pdf",MimeType="application/pdf",Extension=".pdf",UploadedBy=user.Id};doc.Fields.Add(new(){FieldCode="LEGACY",RawValue="kept",NormalizedValue="KEPT",DataType=DocumentFieldDataType.TEXT,ExtractionMethod=ExtractionMethod.PDF_TEXT});db.AddRange(user,doc);await db.SaveChangesAsync();db.ChangeTracker.Clear();Assert.Equal("EXTRACTED",await ScalarText(db,"SELECT SourceKind FROM DocumentFields WHERE RawValue='kept'"));
        await migrator.MigrateAsync(previous);Assert.False(await Table(db,"CanonicalFieldDefinitions"));Assert.True(await Table(db,"DocumentReconciliationProfiles"));Assert.Equal(1,await Scalar(db,"SELECT COUNT(*) FROM DocumentFields WHERE RawValue='kept'"));}
      await using(var fresh=Context(Path.Combine(dir,"fresh.db"))){await fresh.Database.MigrateAsync();Assert.True(await Table(fresh,"DocumentIntelligenceAnalyses"));Assert.Contains("20261113020737_DocumentIntelligenceLayerV1",await fresh.Database.GetAppliedMigrationsAsync());}
    }finally{SqliteConnection.ClearAllPools();if(Directory.Exists(dir))Directory.Delete(dir,true);}
  }

  private static SemanticFieldResolver Resolver()=>new(new FieldNormalizationService());
  private static DocumentLayout Layout(params DocumentLayoutLine[] lines)=>new([new(1,string.Join('\n',lines.Select(x=>x.Text)),lines)],"TEST");
  private static DocumentLayoutLine Line(int page,string text,decimal left=0,decimal bottom=0)=>new(page,text,new(left,bottom,left+100,bottom+10));
  private static List<FieldAlias> Aliases()=>[
    Alias(DocumentFieldCodes.Quantity,"Qty"),Alias(DocumentFieldCodes.Quantity,"Quantity"),Alias(DocumentFieldCodes.Quantity,"Cantidad","ES"),Alias(DocumentFieldCodes.LineTotal,"Line Amount"),Alias(DocumentFieldCodes.CommercialTotal,"Invoice Total"),Alias(DocumentFieldCodes.CommercialTotal,"Amount"),Alias(DocumentFieldCodes.InvoiceNumber,"Invoice Number"),Alias(DocumentFieldCodes.SellerName,"Seller"),Alias(DocumentFieldCodes.BuyerName,"Buyer"),Alias(DocumentFieldCodes.ShipToName,"Ship To"),Alias(DocumentFieldCodes.ManufacturerName,"Manufacturer")
  ];
  private static FieldAlias Alias(string code,string label,string language="EN")=>new(){CanonicalFieldCode=code,Alias=label,Language=language,Priority=100};
  private static List<UnitAlias> Units()=>[new(){Alias="PCS",CanonicalUnit="PZA"},new(){Alias="PIECES",CanonicalUnit="PZA"},new(){Alias="PZA",CanonicalUnit="PZA",Language="ES"},new(){Alias="PAIRS",CanonicalUnit="PAR"},new(){Alias="PAR",CanonicalUnit="PAR",Language="ES"}];
  private static DocumentField Field(Guid documentId,string code,string value,string group)=>new(){OperationDocumentId=documentId,FieldCode=code,CanonicalFieldCode=code,RawValue=value,NormalizedValue=value,DataType=DocumentFieldDataType.DECIMAL,GroupKey=group,SourceKind=DocumentFieldSourceKind.EXTRACTED};
  private static SaceDbContext Context(string path)=>new(new DbContextOptionsBuilder<SaceDbContext>().UseSqlite($"Data Source={path}").Options);
  private static async Task<bool> Table(SaceDbContext db,string name){var connection=db.Database.GetDbConnection();if(connection.State!=System.Data.ConnectionState.Open)await connection.OpenAsync();await using var command=connection.CreateCommand();command.CommandText="SELECT COUNT(*) FROM sqlite_master WHERE type='table' AND name=$name";var p=command.CreateParameter();p.ParameterName="$name";p.Value=name;command.Parameters.Add(p);return Convert.ToInt64(await command.ExecuteScalarAsync())==1;}
  private static async Task<long> Scalar(SaceDbContext db,string sql){var connection=db.Database.GetDbConnection();if(connection.State!=System.Data.ConnectionState.Open)await connection.OpenAsync();await using var command=connection.CreateCommand();command.CommandText=sql;return Convert.ToInt64(await command.ExecuteScalarAsync());}
  private static async Task<string> ScalarText(SaceDbContext db,string sql){var connection=db.Database.GetDbConnection();if(connection.State!=System.Data.ConnectionState.Open)await connection.OpenAsync();await using var command=connection.CreateCommand();command.CommandText=sql;return Convert.ToString(await command.ExecuteScalarAsync())!;}
  private static async Task<HttpClient> Client(SaceFactory factory){var client=factory.CreateClient();var login=await client.PostAsJsonAsync("/api/auth/login",new{email="admin@sace.local",password="Admin123!"});login.EnsureSuccessStatusCode();using var json=JsonDocument.Parse(await login.Content.ReadAsStringAsync());client.DefaultRequestHeaders.Authorization=new AuthenticationHeaderValue("Bearer",json.RootElement.GetProperty("token").GetString());return client;}

  private sealed class PipelineFixture(SqliteConnection connection,SaceDbContext db,User user,OperationDocument document,MemoryStorage storage):IAsyncDisposable{
    public SaceDbContext Db=>db;public User User=>user;
    public static async Task<PipelineFixture>Create(){var connection=new SqliteConnection("DataSource=:memory:");await connection.OpenAsync();var db=new SaceDbContext(new DbContextOptionsBuilder<SaceDbContext>().UseSqlite(connection).Options);await db.Database.EnsureCreatedAsync();var user=new User{Name="Document intelligence",Email=$"doc-int-{Guid.NewGuid():N}@example.invalid",PasswordHash="x"};var operation=new TradeOperation{Folio=$"DI-{Guid.NewGuid():N}",PedimentoNumber="26",Period="2026-11",OperationDate=new(2026,11,12)};var type=new DocumentType{Code="COMMERCIAL_INVOICE",Name="Factura"};var document=new OperationDocument{TradeOperation=operation,DocumentType=type,OriginalFileName="scan.pdf",StoredFileName="scan.pdf",FilePath="scan.pdf",MimeType="application/pdf",Extension=".pdf",FileSize=10,UploadedBy=user.Id};db.AddRange(user,document);foreach(var alias in Aliases()){var definition=await db.CanonicalFieldDefinitions.SingleOrDefaultAsync(x=>x.Code==alias.CanonicalFieldCode);if(definition is null){definition=new(){Code=alias.CanonicalFieldCode,Name=alias.CanonicalFieldCode,DataType=alias.CanonicalFieldCode==DocumentFieldCodes.Quantity?DocumentFieldDataType.QUANTITY:DocumentFieldDataType.TEXT};db.Add(definition);await db.SaveChangesAsync();}alias.CanonicalFieldDefinitionId=definition.Id;db.Add(alias);}db.AddRange(Units());await db.SaveChangesAsync();var storage=new MemoryStorage();storage.Files[document.FilePath]=[1,2,3];return new(connection,db,user,document,storage);}
    public async Task<OperationDocument>NewDocument(string name){var current=await db.OperationDocuments.Include(x=>x.TradeOperation).Include(x=>x.DocumentType).FirstAsync();var doc=new OperationDocument{TradeOperationId=current.TradeOperationId,DocumentTypeId=current.DocumentTypeId,OriginalFileName=name,StoredFileName=name,FilePath=name,MimeType="application/pdf",Extension=".pdf",FileSize=3,UploadedBy=user.Id};db.Add(doc);await db.SaveChangesAsync();storage.Files[name]=[4,5,6];return doc;}
    public Task<DocumentIntelligenceResult>Analyze(IOcrTextExtractor ocr,Guid? documentId=null){var config=new ConfigurationBuilder().AddInMemoryCollection(new Dictionary<string,string?>{{"DocumentIntelligence:MinimumNativeTextCharacters","40"},{"DocumentIntelligence:MaxPages","100"},{"DocumentIntelligence:OcrMaxPages","20"},{"DocumentIntelligence:MaxLineItems","500"}}).Build();var service=new DocumentIntelligencePipeline(db,storage,new NoopLegacy(),new EmptyPdf(),ocr,new DocumentSemanticClassifier(),new SemanticFieldResolver(new FieldNormalizationService()),new SemanticLineItemExtractor(),new CalculatedFieldService(),config);return service.AnalyzeAsync(documentId??document.Id,user.Id);}
    public async ValueTask DisposeAsync(){await db.DisposeAsync();await connection.DisposeAsync();}
  }
  private sealed class MemoryStorage:IFileStorageService{public Dictionary<string,byte[]>Files{get;}=[];public Task<(string storedName,string relativePath)>SaveAsync(Guid operationId,string documentCode,IFormFile file,CancellationToken ct)=>throw new NotSupportedException();public Task<Stream?>OpenAsync(string relativePath,CancellationToken ct)=>Task.FromResult<Stream?>(Files.TryGetValue(relativePath,out var bytes)?new MemoryStream(bytes):null);}
  private sealed class NoopLegacy:IDocumentFieldExtractor{public Task<int>ExtractDocumentAsync(Guid documentId,Guid userId,CancellationToken ct=default)=>Task.FromResult(0);public Task<int>ExtractOperationAsync(Guid operationId,Guid userId,CancellationToken ct=default)=>Task.FromResult(0);}
  private sealed class EmptyPdf:IPdfLayoutExtractor{public Task<DocumentLayout>ExtractAsync(Stream stream,int maxPages,CancellationToken ct)=>Task.FromResult(new DocumentLayout([],"PDF_NATIVE_LAYOUT"));}
  private sealed class FakeOcr(bool available,bool success,string? error,IReadOnlyList<DocumentLayoutPage> pages,IReadOnlyList<int> ocrPages,string explanation):IOcrTextExtractor{public Task<OcrExtractionResult>ExtractAsync(string inputPath,string mimeType,int maxPages,IReadOnlyList<string>languages,CancellationToken ct)=>Task.FromResult(new OcrExtractionResult(available,success,error,pages,ocrPages,explanation));}
}
