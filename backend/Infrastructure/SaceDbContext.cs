using Microsoft.EntityFrameworkCore;
using Sace.Api.Domain;

namespace Sace.Api.Infrastructure;

public class SaceDbContext(DbContextOptions<SaceDbContext> options) : DbContext(options) {
  public DbSet<User> Users => Set<User>(); public DbSet<TradeOperation> TradeOperations => Set<TradeOperation>(); public DbSet<DocumentType> DocumentTypes => Set<DocumentType>();
  public DbSet<OperationDocument> OperationDocuments => Set<OperationDocument>(); public DbSet<Audit> Audits => Set<Audit>(); public DbSet<AuditResult> AuditResults => Set<AuditResult>();
  public DbSet<DocumentReference> DocumentReferences => Set<DocumentReference>();
  public DbSet<CustomsValueAdjustment> CustomsValueAdjustments => Set<CustomsValueAdjustment>();
  public DbSet<Finding> Findings => Set<Finding>(); public DbSet<FindingHistory> FindingHistories => Set<FindingHistory>(); public DbSet<NormativeRule> NormativeRules => Set<NormativeRule>(); public DbSet<SystemAuditLog> SystemAuditLogs => Set<SystemAuditLog>();
  protected override void OnModelCreating(ModelBuilder b) {
    b.Entity<User>().HasIndex(x => x.Email).IsUnique(); b.Entity<TradeOperation>().HasIndex(x => x.Folio).IsUnique(); b.Entity<DocumentType>().HasIndex(x => x.Code).IsUnique(); b.Entity<NormativeRule>().HasIndex(x => new { x.Code, x.Version }).IsUnique();
    b.Entity<OperationDocument>().HasIndex(x => new { x.TradeOperationId, x.FileHash });
    b.Entity<DocumentReference>().HasIndex(x => new { x.ReferenceType, x.Value });
    b.Entity<TradeOperation>().Property(x => x.OperationType).HasConversion<string>(); b.Entity<TradeOperation>().Property(x => x.Status).HasConversion<string>(); b.Entity<DocumentType>().Property(x => x.CorrectionResponsible).HasConversion<string>(); b.Entity<OperationDocument>().Property(x => x.ValidationStatus).HasConversion<string>(); b.Entity<Audit>().Property(x => x.AuditType).HasConversion<string>(); b.Entity<Audit>().Property(x => x.Status).HasConversion<string>(); b.Entity<Audit>().Property(x => x.EvaluationDateBasis).HasConversion<string>(); b.Entity<AuditResult>().Property(x => x.Status).HasConversion<string>(); b.Entity<AuditResult>().Property(x => x.Outcome).HasConversion<string>(); b.Entity<AuditResult>().Property(x => x.ManualOutcome).HasConversion<string>(); b.Entity<Finding>().Property(x => x.Severity).HasConversion<string>(); b.Entity<Finding>().Property(x => x.CorrectionResponsible).HasConversion<string>(); b.Entity<Finding>().Property(x => x.Status).HasConversion<string>(); b.Entity<FindingHistory>().Property(x => x.PreviousStatus).HasConversion<string>(); b.Entity<FindingHistory>().Property(x => x.NewStatus).HasConversion<string>(); b.Entity<NormativeRule>().Property(x => x.NormativeSource).HasConversion<string>(); b.Entity<NormativeRule>().Property(x => x.RuleCategory).HasConversion<string>(); b.Entity<NormativeRule>().Property(x => x.AutomationLevel).HasConversion<string>(); b.Entity<CustomsValueAdjustment>().Property(x => x.AdjustmentType).HasConversion<string>(); b.Entity<CustomsValueAdjustment>().Property(x => x.LegalQualificationStatus).HasConversion<string>();
    b.Entity<Audit>().HasOne(x => x.TradeOperation).WithMany(x => x.Audits).HasForeignKey(x => x.TradeOperationId).OnDelete(DeleteBehavior.Cascade);
    b.Entity<Finding>().HasOne(x => x.Audit).WithMany().HasForeignKey(x => x.AuditId).OnDelete(DeleteBehavior.Restrict);
    b.Entity<AuditResult>().HasOne(x => x.NormativeRule).WithMany().HasForeignKey(x => x.NormativeRuleId).OnDelete(DeleteBehavior.Restrict);
    b.Entity<AuditResult>().HasOne(x => x.ReviewerUser).WithMany().HasForeignKey(x => x.ReviewerUserId).OnDelete(DeleteBehavior.Restrict);
    b.Entity<CustomsValueAdjustment>().HasOne(x => x.SourceDocument).WithMany().HasForeignKey(x => x.SourceDocumentId).OnDelete(DeleteBehavior.SetNull);
  }
}
