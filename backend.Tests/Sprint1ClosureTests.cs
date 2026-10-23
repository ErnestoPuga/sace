using System.Data;
using System.Net;
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

public class Sprint1MigrationTests {
  private const string PreviousMigration="20260917025444_NormativeEngineV1";
  private static readonly string[] ExpectedMigrations=["20260907190536_InitialCreate","20260911001711_RealImportDocuments",PreviousMigration,"20261016183617_CrossValidationEngineV1","20261016222705_CustomsValuationEngineV1","20261019183747_RegulatoryRequirementsEngineV1","20261022195429_OfficialRegulatoryCatalogV1"];
  private static readonly string[] ExpectedRules=["CROSS-INV-PED-INVOICE-001","CROSS-INV-PED-QTY-001","CROSS-INV-PED-UNIT-001","CROSS-INV-PED-VALUE-001","CROSS-INV-PED-CURRENCY-001","CROSS-INV-PED-INCOTERM-001","CROSS-INV-COVE-INVOICE-001","CROSS-INV-COVE-DESC-001","CROSS-INV-COVE-QTY-001","CROSS-INV-COVE-UNIT-001","CROSS-INV-COVE-VALUE-001","CROSS-INV-COVE-CURRENCY-001","CROSS-PL-PED-DESC-001","CROSS-PL-PED-QTY-001","CROSS-PL-PED-UNIT-001","CROSS-PL-PED-WEIGHT-001","CROSS-PL-TR-PACKAGES-001","CROSS-PL-TR-WEIGHT-001","CROSS-FREIGHT-INV-PED-001","CROSS-FREIGHT-PAY-INV-001","CROSS-FREIGHT-TR-INV-001","CROSS-INS-POL-PED-001","CROSS-INS-INV-PED-001","CROSS-INS-PAY-INV-001","CROSS-INC-INV-PED-001","CROSS-INC-PAY-INV-001","CROSS-VAL-MV-PED-001","CROSS-VAL-COMMERCIAL-001","CROSS-VAL-COVE-001"];

  [Fact]
  public async Task Empty_database_applies_all_migrations_with_new_constraints(){
    var path=TempDatabase();
    try {
      await using(var db=Context(path)){
        await db.Database.MigrateAsync();
        Assert.Equal(ExpectedMigrations,await db.Database.GetAppliedMigrationsAsync());
        Assert.Equal(1,await SchemaCount(db,"table","DocumentFields"));
        Assert.Equal(1,await SchemaCount(db,"table","CrossValidationRules"));
        Assert.Equal(1,await SchemaCount(db,"table","CrossValidationResults"));
        Assert.Equal(1,await SchemaCount(db,"table","CustomsValuationCalculations"));
        Assert.Equal(1,await SchemaCount(db,"table","CustomsValuationComponents"));
        Assert.Equal(1,await SchemaCount(db,"table","ValuationComponentEvidence"));
        Assert.Equal(1,await SchemaCount(db,"table","ContributionCalculations"));
        Assert.Equal(1,await SchemaCount(db,"table","ContributionLines"));
        Assert.Equal(1,await SchemaCount(db,"table","RegulatoryRequirements"));
        Assert.Equal(1,await SchemaCount(db,"table","RegulatoryRequirementConditions"));
        Assert.Equal(1,await SchemaCount(db,"table","RegulatoryEvidenceRequirements"));
        Assert.Equal(1,await SchemaCount(db,"table","RegulatoryRequirementEvaluations"));
        Assert.Equal(1,await SchemaCount(db,"table","RegulatoryRequirementEvaluationEvidence"));
        Assert.Equal(1,await SchemaCount(db,"table","OfficialLegalSources"));
        Assert.Equal(1,await SchemaCount(db,"table","OfficialLegalSourceVersions"));
        Assert.Equal(1,await SchemaCount(db,"table","OfficialSourceCitations"));
        Assert.Equal(1,await SchemaCount(db,"table","RegulatoryTariffEntries"));
        Assert.Equal(1,await SchemaCount(db,"table","RegulatoryCatalogImports"));
        Assert.Equal(1,await SchemaCount(db,"index","IX_CrossValidationRules_Code_Version"));
        Assert.True(await ForeignKeyCount(db,"DocumentFields")>=1);
        Assert.True(await ForeignKeyCount(db,"CrossValidationResults")>=7);
      }
    } finally { DeleteDatabase(path); }
  }

  [Fact]
  public async Task Existing_database_preserves_data_seed_is_idempotent_and_down_is_scoped(){
    var path=TempDatabase();
    try {
      Guid operationId,documentId,auditId,resultId;
      await using(var db=Context(path)){
        var migrator=db.Database.GetService<IMigrator>();await migrator.MigrateAsync(PreviousMigration);
        var user=new User{Name="Synthetic migration user",Email="migration@example.invalid",PasswordHash="synthetic"};
        var operation=new TradeOperation{Folio="MIG-SYNTHETIC",PedimentoNumber="26",Period="2026-10",OperationDate=new DateTime(2026,10,1,0,0,0,DateTimeKind.Utc),TariffFraction="40169304"};
        var type=new DocumentType{Code="MIG_DOC",Name="Synthetic document"};
        var document=new OperationDocument{TradeOperation=operation,DocumentType=type,OriginalFileName="synthetic.pdf",StoredFileName="synthetic.pdf",MimeType="application/pdf",Extension=".pdf",FilePath="synthetic/none",FileSize=1,UploadedBy=user.Id};
        var audit=new Audit{Number="AUD-MIGRATION",TradeOperation=operation,AuditType=AuditType.Legal,Status=AuditStatus.Completed,CreatedBy=user.Id};
        var result=new AuditResult{Audit=audit,RuleId="LEGAL-SYNTHETIC",Status=ResultStatus.Valid,Outcome=LegalOutcome.COMPLIANT,IsIncludedInLegalCompliance=true};
        db.AddRange(user,operation,type,document,audit,result);await db.SaveChangesAsync();
        operationId=operation.Id;documentId=document.Id;auditId=audit.Id;resultId=result.Id;
        await migrator.MigrateAsync();
      }

      var services=new ServiceCollection();
      services.AddSingleton<IConfiguration>(new ConfigurationBuilder().AddInMemoryCollection(new Dictionary<string,string?>{{"Seed:DemoData","false"},{"PilotUser:Enabled","false"}}).Build());
      services.AddScoped<IPasswordService,PasswordService>();
      services.AddDbContext<SaceDbContext>(o=>o.UseSqlite($"Data Source={path}"));
      using(var provider=services.BuildServiceProvider()){await DbSeeder.SeedAsync(provider);await DbSeeder.SeedAsync(provider);}

      await using(var db=Context(path)){
        Assert.True(await db.TradeOperations.AnyAsync(x=>x.Id==operationId));
        Assert.True(await db.OperationDocuments.AnyAsync(x=>x.Id==documentId));
        Assert.True(await db.Audits.AnyAsync(x=>x.Id==auditId));
        Assert.Equal(LegalOutcome.COMPLIANT,(await db.AuditResults.SingleAsync(x=>x.Id==resultId)).Outcome);
        var rules=await db.CrossValidationRules.OrderBy(x=>x.Code).ToListAsync();
        Assert.Equal(29,rules.Count);Assert.Equal(ExpectedRules.Order(),rules.Select(x=>x.Code));
        Assert.Equal(29,rules.Select(x=>new{x.Code,x.Version}).Distinct().Count());
        Assert.All(rules,x=>Assert.Equal(MissingEvidenceBehavior.INSUFFICIENT_EVIDENCE,x.MissingEvidenceBehavior));
        Assert.All(rules.Where(x=>x.Code is "CROSS-PL-PED-WEIGHT-001" or "CROSS-PL-TR-WEIGHT-001"),x=>Assert.Equal(2m,x.PercentTolerance));
        Assert.Equal(4,await db.RegulatoryRequirements.CountAsync());Assert.Equal(2,await db.RegulatoryRequirements.CountAsync(x=>x.Status==RegulatoryRequirementStatus.DRAFT));Assert.Equal(2,await db.RegulatoryRequirements.CountAsync(x=>x.Status==RegulatoryRequirementStatus.VERIFIED));
        Assert.Equal(8,await db.DocumentTypes.CountAsync(x=>new[]{"NOM_EVIDENCE","PERMISO_PREVIO","AVISO_AUTOMATICO","COFEPRIS_DOCUMENT","SEMARNAT_DOCUMENT","SEDENA_DOCUMENT","FITOSANITARIO_CERTIFICATE","RRNA_OTHER"}.Contains(x.Code)));
        await db.Database.GetService<IMigrator>().MigrateAsync(PreviousMigration);
        Assert.Equal(0,await SchemaCount(db,"table","DocumentFields"));
        Assert.Equal(0,await SchemaCount(db,"table","CrossValidationRules"));
        Assert.Equal(0,await SchemaCount(db,"table","CrossValidationResults"));
        Assert.True(await db.TradeOperations.AnyAsync(x=>x.Id==operationId));
        Assert.True(await db.OperationDocuments.AnyAsync(x=>x.Id==documentId));
        Assert.True(await db.Audits.AnyAsync(x=>x.Id==auditId));
        Assert.Equal(LegalOutcome.COMPLIANT,(await db.AuditResults.SingleAsync(x=>x.Id==resultId)).Outcome);
      }
    } finally { DeleteDatabase(path); }
  }

  private static SaceDbContext Context(string path)=>new(new DbContextOptionsBuilder<SaceDbContext>().UseSqlite($"Data Source={path}").Options);
  private static string TempDatabase(){var directory=Path.Combine(Path.GetTempPath(),$"sace-migration-{Guid.NewGuid():N}");Directory.CreateDirectory(directory);return Path.Combine(directory,"validation.db");}
  private static void DeleteDatabase(string path){SqliteConnection.ClearAllPools();var directory=Path.GetDirectoryName(path)!;if(Directory.Exists(directory)&&Path.GetFileName(directory).StartsWith("sace-migration-",StringComparison.Ordinal))Directory.Delete(directory,true);}
  private static async Task<long> SchemaCount(SaceDbContext db,string type,string name){var connection=(SqliteConnection)db.Database.GetDbConnection();if(connection.State!=ConnectionState.Open)await connection.OpenAsync();await using var command=connection.CreateCommand();command.CommandText="SELECT COUNT(*) FROM sqlite_master WHERE type=$type AND name=$name";command.Parameters.AddWithValue("$type",type);command.Parameters.AddWithValue("$name",name);return Convert.ToInt64(await command.ExecuteScalarAsync());}
  private static async Task<long> ForeignKeyCount(SaceDbContext db,string table){var connection=(SqliteConnection)db.Database.GetDbConnection();if(connection.State!=ConnectionState.Open)await connection.OpenAsync();await using var command=connection.CreateCommand();command.CommandText=$"PRAGMA foreign_key_list([{table}])";await using var reader=await command.ExecuteReaderAsync();long count=0;while(await reader.ReadAsync())count++;return count;}
}

public class Sprint1SecurityTests {
  [Theory]
  [InlineData("GET","/api/operations/00000000-0000-0000-0000-000000000001/document-fields")]
  [InlineData("POST","/api/operations/00000000-0000-0000-0000-000000000001/document-fields/reextract")]
  [InlineData("GET","/api/operations/00000000-0000-0000-0000-000000000001/cross-validations")]
  [InlineData("POST","/api/operations/00000000-0000-0000-0000-000000000001/cross-validations/run")]
  public async Task Cross_validation_endpoints_require_authentication(string method,string path){using var factory=new SaceFactory();var response=await factory.CreateClient().SendAsync(new HttpRequestMessage(new HttpMethod(method),path));Assert.Equal(HttpStatusCode.Unauthorized,response.StatusCode);}
}
