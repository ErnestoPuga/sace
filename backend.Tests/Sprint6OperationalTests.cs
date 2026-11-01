using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.Extensions.DependencyInjection;
using Sace.Api.Domain;
using Sace.Api.Infrastructure;
using Sace.Api.Services;
using Xunit;

namespace Sace.Api.Tests;

public class Sprint6OperationalTests {
  [Fact]
  public async Task Critical_immex_risk_reuses_review_queue_and_never_changes_legal(){await using var connection=new SqliteConnection("DataSource=:memory:");await connection.OpenAsync();await using var db=new SaceDbContext(new DbContextOptionsBuilder<SaceDbContext>().UseSqlite(connection).Options);await db.Database.EnsureCreatedAsync();var user=new User{Name="Risk user",Email="risk@example.invalid",PasswordHash="x"};var program=new ImmexProgram{ProgramNumber="RISK-S6",CompanyName="Synthetic",Rfc="AAA010101AAA",Status=ImmexProgramStatus.ACTIVE};var material=new ImmexMaterial{ImmexProgram=program,MaterialCode="MAT-RISK",Description="Synthetic",TariffFraction="85011010",TigieUnit="PZA"};var operation=new TradeOperation{Folio="RISK-S6",PedimentoNumber="26-RISK",Period="2026-01",OperationDate=new(2026,1,1),TariffFraction="85011010",ImmexProgram=program,IsTemporaryImport=true};var profile=new RiskProfile{Code="RISK-S6-TEST",Version=99,Name="Synthetic IMMEX",Description="Test",Status=RiskProfileStatus.ACTIVE,EffectiveFrom=new(2026,1,1),LowMax=5,MediumMax=10,HighMax=20,CategoryCapsJson="{\"IMMEX\":100}"};profile.Factors.Add(new(){Code="TEMPORALITY_PAST_DUE",Name="Past due",Category=RiskCategory.IMMEX,SourceType=RiskSourceType.IMMEX,SourceOutcome="TEMPORALITY_PAST_DUE",Weight=35,MaxPoints=35,Severity=RiskSeverity.CRITICAL,CorrelationMode=RiskCorrelationMode.MAX});var entry=new ImmexInventoryEntry{ImmexProgram=program,TradeOperation=operation,Material=material,PedimentoNumber="26-RISK",PedimentoKey="IN",EntryDate=new(2026,1,1),TariffFraction="85011010",Description="Synthetic",OriginalQuantity=100,AvailableQuantity=100,TigieUnit="PZA",InventoryClassification=ImmexInventoryClassification.COMPONENT,TemporalityOutcome=ImmexTemporalityOutcome.PAST_DUE,Status=ImmexInventoryEntryStatus.PAST_DUE};db.AddRange(user,profile,entry);await db.SaveChangesAsync();var score=await new OperationRiskScoringService(db).CalculateAsync(operation.Id,null,user.Id);Assert.Equal(RiskLevel.CRITICAL,score.RiskLevel);var queue=Assert.Single(await db.ReviewQueueItems.Include(x=>x.History).ToListAsync());Assert.Equal(operation.Id,queue.TradeOperationId);Assert.Equal(ReviewQueueStatus.OPEN,queue.Status);Assert.All(queue.History,x=>Assert.Equal(user.Id,x.UserId));Assert.Empty(await db.Findings.ToListAsync());Assert.Empty(await db.AuditResults.ToListAsync());}

  [Fact]
  public async Task Known_immex_errors_are_structured_without_stack_trace(){using var factory=new SaceFactory();var client=await Authenticated(factory);var entry=await client.GetAsync($"/api/immex/inventory/{Guid.NewGuid()}");Assert.Equal(HttpStatusCode.NotFound,entry.StatusCode);var entryBody=await entry.Content.ReadAsStringAsync();Assert.Contains("ENTRY_NOT_FOUND",entryBody);Assert.DoesNotContain("StackTrace",entryBody,StringComparison.OrdinalIgnoreCase);var exit=await client.PostAsJsonAsync($"/api/immex/discharges/{Guid.NewGuid()}/reverse",new{reason="Synthetic"});Assert.Equal(HttpStatusCode.NotFound,exit.StatusCode);var exitBody=await exit.Content.ReadAsStringAsync();Assert.Contains("EXIT_NOT_FOUND",exitBody);Assert.DoesNotContain("StackTrace",exitBody,StringComparison.OrdinalIgnoreCase);}

  [Fact]
  public async Task Fresh_database_applies_every_migration_through_immex_v1(){var directory=Path.Combine(Path.GetTempPath(),$"sace-s6-fresh-{Guid.NewGuid():N}");Directory.CreateDirectory(directory);var path=Path.Combine(directory,"fresh.db");try{await using var db=new SaceDbContext(new DbContextOptionsBuilder<SaceDbContext>().UseSqlite($"Data Source={path}").Options);await db.Database.GetService<IMigrator>().MigrateAsync();var applied=await db.Database.GetAppliedMigrationsAsync();Assert.Contains("20261026231346_ImmexInventoryControlV1",applied);Assert.True(await Table(db,"ImmexInventoryEntries"));Assert.True(await Table(db,"ImmexInventoryDischarges"));Assert.True(await Table(db,"ImmexTemporalityRules"));Assert.True(await Table(db,"RiskProfiles"));Assert.True(await Table(db,"ReviewQueueItems"));Assert.True(await Table(db,"AuditResults"));}finally{SqliteConnection.ClearAllPools();if(Directory.Exists(directory)&&Path.GetFullPath(directory).StartsWith(Path.GetFullPath(Path.GetTempPath()),StringComparison.OrdinalIgnoreCase))Directory.Delete(directory,true);}}

  static async Task<HttpClient> Authenticated(SaceFactory factory){var client=factory.CreateClient();var login=await client.PostAsJsonAsync("/api/auth/login",new{email="admin@sace.local",password="Admin123!"});login.EnsureSuccessStatusCode();using var json=JsonDocument.Parse(await login.Content.ReadAsStringAsync());client.DefaultRequestHeaders.Authorization=new AuthenticationHeaderValue("Bearer",json.RootElement.GetProperty("token").GetString());return client;}
  static async Task<bool> Table(SaceDbContext db,string name){var connection=(SqliteConnection)db.Database.GetDbConnection();if(connection.State!=System.Data.ConnectionState.Open)await connection.OpenAsync();await using var command=connection.CreateCommand();command.CommandText="SELECT COUNT(*) FROM sqlite_master WHERE type='table' AND name=$name";command.Parameters.AddWithValue("$name",name);return Convert.ToInt64(await command.ExecuteScalarAsync())==1;}
}
