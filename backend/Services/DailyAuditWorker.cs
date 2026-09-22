using Microsoft.EntityFrameworkCore;
using Sace.Api.Domain;
using Sace.Api.Infrastructure;

namespace Sace.Api.Services;

public sealed class DailyAuditWorker(IServiceScopeFactory scopes, IConfiguration configuration, ILogger<DailyAuditWorker> logger) : BackgroundService {
  protected override async Task ExecuteAsync(CancellationToken stoppingToken) {
    if (!configuration.GetValue("DailyAudit:Enabled", true)) return;
    while (!stoppingToken.IsCancellationRequested) {
      var now = DateTime.UtcNow; var hour = configuration.GetValue("DailyAudit:RunAtHourUtc", 6); var next = now.Date.AddHours(hour); if (next <= now) next = next.AddDays(1);
      try { await Task.Delay(next - now, stoppingToken); await RunPendingAsync(stoppingToken); }
      catch (OperationCanceledException) when (stoppingToken.IsCancellationRequested) { break; }
      catch (Exception ex) { logger.LogError(ex, "Falló la ejecución programada de auditoría diaria."); await Task.Delay(TimeSpan.FromMinutes(5), stoppingToken); }
    }
  }
  private async Task RunPendingAsync(CancellationToken ct) {
    using var scope = scopes.CreateScope(); var db = scope.ServiceProvider.GetRequiredService<SaceDbContext>(); var engine = scope.ServiceProvider.GetRequiredService<IAuditEngine>(); var user = await db.Users.Where(x => x.IsActive).Select(x => x.Id).FirstOrDefaultAsync(ct); var operations = await db.TradeOperations.Where(x => x.Status == OperationStatus.PendingAudit).Select(x => x.Id).ToListAsync(ct);
    foreach (var operation in operations) await engine.ExecuteAsync(operation, AuditType.Daily, user, ct); logger.LogInformation("Auditoría diaria programada completada para {Count} operaciones.", operations.Count);
  }
}
