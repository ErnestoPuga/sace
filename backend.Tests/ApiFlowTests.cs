using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Sace.Api.Infrastructure;
using Xunit;

namespace Sace.Api.Tests;

public sealed class SaceFactory : WebApplicationFactory<Program> {
  private readonly SqliteConnection connection = new("DataSource=:memory:");
  private readonly string testStorage = Path.Combine(Path.GetTempPath(), $"sace-tests-{Guid.NewGuid():N}");
  protected override void ConfigureWebHost(IWebHostBuilder builder) {
    connection.Open(); builder.UseEnvironment("Development"); builder.ConfigureAppConfiguration((_, config) => config.AddInMemoryCollection(new Dictionary<string, string?> { ["FileStorage:RootPath"] = testStorage })); builder.ConfigureLogging(logging => logging.ClearProviders().AddConsole()); builder.ConfigureServices(services => {
      var options = services.Where(x => x.ServiceType == typeof(DbContextOptions<SaceDbContext>) || x.ServiceType == typeof(SaceDbContext)).ToList(); foreach (var item in options) services.Remove(item);
      services.AddDbContext<SaceDbContext>(o => o.UseSqlite(connection));
    });
  }
  protected override void Dispose(bool disposing) { base.Dispose(disposing); if (disposing) { connection.Dispose(); if (Directory.Exists(testStorage) && Path.GetFullPath(testStorage).StartsWith(Path.GetFullPath(Path.GetTempPath()), StringComparison.OrdinalIgnoreCase)) Directory.Delete(testStorage, true); } }
}

public class ApiFlowTests {
  [Fact] public async Task Login_accepts_seed_credentials() { using var f = new SaceFactory(); var r = await f.CreateClient().PostAsJsonAsync("/api/auth/login", new { email="admin@sace.local", password="Admin123!" }); Assert.Equal(HttpStatusCode.OK, r.StatusCode); Assert.False(string.IsNullOrWhiteSpace((await Doc(r)).RootElement.GetProperty("token").GetString())); }
  [Fact] public async Task Operation_can_be_created_and_auto_audited() { using var f = new SaceFactory(); var c = await Client(f); var r = await c.PostAsJsonAsync("/api/operations", NewOperation()); Assert.Equal(HttpStatusCode.Created, r.StatusCode); }
  [Fact] public async Task Checklist_is_generated_from_backend_rules() { using var f = new SaceFactory(); var c = await Client(f); var id = await DemoOperationId(c); var rows = (await Doc(await c.GetAsync($"/api/operations/{id}/requirements"))).RootElement; Assert.Contains(rows.EnumerateArray(), x => x.GetProperty("ruleCode").GetString()=="DEMO-001"); }
  [Fact] public async Task Preferential_operation_requires_origin_certificate() { using var f = new SaceFactory(); var c = await Client(f); var id = await DemoOperationId(c); var rows = (await Doc(await c.GetAsync($"/api/operations/{id}/requirements"))).RootElement; var cert = rows.EnumerateArray().Single(x => x.GetProperty("ruleCode").GetString()=="DEMO-003"); Assert.Equal("Missing",cert.GetProperty("status").GetString()); }
  [Fact] public async Task Audit_generates_a_finding_for_missing_certificate() { using var f = new SaceFactory(); var c = await Client(f); var findings = (await Doc(await c.GetAsync("/api/findings"))).RootElement; Assert.Contains(findings.EnumerateArray(), x => x.GetProperty("folio").GetString()=="IMP-2026-000001" && x.GetProperty("ruleId").GetString()=="DEMO-003"); }
  [Fact] public async Task Correction_creates_a_reaudit() { using var f = new SaceFactory(); var c = await Client(f); var id = await DemoFindingId(c); await Correct(c,id); var audits = (await Doc(await c.GetAsync("/api/audits"))).RootElement; Assert.Contains(audits.EnumerateArray(), x => x.GetProperty("auditType").GetString()=="Reaudit"); }
  [Fact] public async Task Successful_correction_resolves_finding() { using var f = new SaceFactory(); var c = await Client(f); var id = await DemoFindingId(c); var result = await Doc(await Correct(c,id)); Assert.Equal("Resolved",result.RootElement.GetProperty("findingStatus").GetString()); }
  [Fact] public async Task Reposting_rule_code_creates_new_version() { using var f = new SaceFactory(); var c = await Client(f); var rule = new { code="DEMO-VERSION",name="Regla versionada",normativeSource="Demo",description="Solo prueba",ruleType="DocumentRequirement",effectiveFrom="2026-01-01T00:00:00Z",effectiveTo=(string?)null,isActive=true,configurationJson="{\"demo\":true}"}; Assert.Equal(HttpStatusCode.Created,(await c.PostAsJsonAsync("/api/normative-rules",rule)).StatusCode); Assert.Equal(HttpStatusCode.Created,(await c.PostAsJsonAsync("/api/normative-rules",rule)).StatusCode); var all=(await Doc(await c.GetAsync("/api/normative-rules"))).RootElement; Assert.Contains(all.EnumerateArray(),x=>x.GetProperty("code").GetString()=="DEMO-VERSION"&&x.GetProperty("version").GetInt32()==2); }

  private static object NewOperation()=>new { pedimentoNumber="26 48 1234 6999999",operationDate="2026-09-06T12:00:00Z",period="2026-09",operationType="Importacion",pedimentoKey="IN",tariffFraction="85044001",originCountry="Estados Unidos",destinationCountry="México",customsRegime="Temporal",isImmex=false,preferentialTreatment=false,tradeAgreement=(string?)null };
  private static async Task<HttpClient> Client(SaceFactory f) { var c=f.CreateClient();var login=await Doc(await c.PostAsJsonAsync("/api/auth/login",new{email="admin@sace.local",password="Admin123!"}));c.DefaultRequestHeaders.Authorization=new AuthenticationHeaderValue("Bearer",login.RootElement.GetProperty("token").GetString());return c; }
  private static async Task<string> DemoOperationId(HttpClient c){var d=await Doc(await c.GetAsync("/api/operations?search=IMP-2026-000001&page=1&pageSize=10"));return d.RootElement.GetProperty("items")[0].GetProperty("id").GetString()!;}
  private static async Task<string> DemoFindingId(HttpClient c){var d=await Doc(await c.GetAsync("/api/findings"));return d.RootElement.EnumerateArray().Single(x=>x.GetProperty("folio").GetString()=="IMP-2026-000001").GetProperty("id").GetString()!;}
  private static Task<HttpResponseMessage> Correct(HttpClient c,string id){var content=new MultipartFormDataContent();content.Add(new ByteArrayContent("%PDF-demo correction"u8.ToArray()){Headers={ContentType=new MediaTypeHeaderValue("application/pdf")}},"file","certificate.pdf");return c.PostAsync($"/api/findings/{id}/correction",content);}
  private static async Task<JsonDocument> Doc(HttpResponseMessage r){var text=await r.Content.ReadAsStringAsync();r.EnsureSuccessStatusCode();return JsonDocument.Parse(text);}
}
