namespace Sace.Api.Domain;

public enum OperationType { Importacion, Exportacion }
public enum OperationStatus { Draft, PendingAudit, Audited, WithFindings, Completed }
public enum CorrectionResponsible { Company, CustomsBroker }
public enum ValidationStatus { Pending, Valid, Invalid }
public enum AuditType { Daily, Monthly, Reaudit, Legal }
public enum AuditStatus { Running, Completed, Failed }
public enum ResultStatus { Valid, Warning, Invalid, Missing, NotApplicable }
public enum Severity { Low, Medium, High, Critical }
public enum FindingStatus { Pending, InCorrection, Resolved }
public enum NormativeSource { Appendix22, IMMEX, Annex24, TradeAgreement, Demo, Technical, CustomsLaw, RGCE }
public enum RuleCategory { LEGAL, TECHNICAL, DEMO }
public enum AutomationLevel { AUTOMATIC, SEMIAUTOMATIC, MANUAL }
public enum LegalOutcome { COMPLIANT, NON_COMPLIANT, NOT_APPLICABLE, INSUFFICIENT_EVIDENCE, MANUAL_REVIEW }
public enum EvaluationDateBasis { OPERATION_DATE, ENTRY_DATE, PAYMENT_DATE, PRESENTATION_DATE }
public enum AdjustmentType { INCREMENTABLE, NON_INCREMENTABLE, UNKNOWN }
public enum LegalQualificationStatus { CONFIRMED, PENDING_REVIEW }

public class User { public Guid Id { get; set; } = Guid.NewGuid(); public string Name { get; set; } = ""; public string Email { get; set; } = ""; public string PasswordHash { get; set; } = ""; public string Role { get; set; } = "Usuario"; public bool IsActive { get; set; } = true; public DateTime CreatedAt { get; set; } = DateTime.UtcNow; }
public class TradeOperation {
  public Guid Id { get; set; } = Guid.NewGuid(); public string Folio { get; set; } = ""; public string PedimentoNumber { get; set; } = "";
  public OperationType OperationType { get; set; } public string Period { get; set; } = ""; public DateTime OperationDate { get; set; }
  public string PedimentoKey { get; set; } = ""; public string TariffFraction { get; set; } = ""; public string OriginCountry { get; set; } = "";
  public string DestinationCountry { get; set; } = ""; public string CustomsRegime { get; set; } = ""; public bool IsImmex { get; set; }
  public bool PreferentialTreatment { get; set; } public string? TradeAgreement { get; set; } public OperationStatus Status { get; set; } = OperationStatus.PendingAudit;
  public string? CustomsOffice { get; set; } public string? InvoiceNumber { get; set; } public decimal? CommercialValue { get; set; }
  public string? Currency { get; set; } public decimal? Quantity { get; set; } public string? Unit { get; set; }
  public decimal? GrossWeight { get; set; } public string? CoveNumber { get; set; }
  public decimal? ExchangeRate { get; set; } public decimal? DeclaredCustomsValueMxn { get; set; } public string? Incoterm { get; set; }
  public string? Nico { get; set; } public string? GoodsDescription { get; set; }
  public DateTime CreatedAt { get; set; } = DateTime.UtcNow; public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
  public List<OperationDocument> Documents { get; set; } = []; public List<Audit> Audits { get; set; } = []; public List<Finding> Findings { get; set; } = []; public List<CustomsValueAdjustment> Adjustments { get; set; } = [];
}
public class DocumentType { public Guid Id { get; set; } = Guid.NewGuid(); public string Code { get; set; } = ""; public string Name { get; set; } = ""; public string Description { get; set; } = ""; public CorrectionResponsible CorrectionResponsible { get; set; } public bool IsActive { get; set; } = true; }
public class OperationDocument {
  public Guid Id { get; set; } = Guid.NewGuid(); public Guid TradeOperationId { get; set; } public TradeOperation TradeOperation { get; set; } = null!;
  public Guid DocumentTypeId { get; set; } public DocumentType DocumentType { get; set; } = null!;
  public string OriginalFileName { get; set; } = ""; public string StoredFileName { get; set; } = ""; public string MimeType { get; set; } = "";
  public string Extension { get; set; } = ""; public long FileSize { get; set; } public string FilePath { get; set; } = "";
  public int Version { get; set; } = 1; public ValidationStatus ValidationStatus { get; set; } = ValidationStatus.Pending;
  public DateTime UploadedAt { get; set; } = DateTime.UtcNow; public Guid UploadedBy { get; set; }
  public string? DocumentNumber { get; set; } public DateTime? DocumentDate { get; set; } public string? SourceSystem { get; set; }
  public string? Notes { get; set; } public string? FileHash { get; set; } public string? ExtractedMetadataJson { get; set; }
  public List<DocumentReference> References { get; set; } = [];
}
public class DocumentReference {
  public Guid Id { get; set; } = Guid.NewGuid(); public Guid OperationDocumentId { get; set; }
  public OperationDocument OperationDocument { get; set; } = null!; public string ReferenceType { get; set; } = "OTHER";
  public string Value { get; set; } = ""; public string? Source { get; set; } public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}
public class Audit { public Guid Id { get; set; } = Guid.NewGuid(); public string Number { get; set; } = ""; public Guid? TradeOperationId { get; set; } public TradeOperation? TradeOperation { get; set; } public string? Period { get; set; } public AuditType AuditType { get; set; } public AuditStatus Status { get; set; } = AuditStatus.Running; public DateTime StartedAt { get; set; } = DateTime.UtcNow; public DateTime? CompletedAt { get; set; } public Guid CreatedBy { get; set; } public DateTime? EvaluationDate { get; set; } public EvaluationDateBasis? EvaluationDateBasis { get; set; } public string? NormativeVersionLabel { get; set; } public List<AuditResult> Results { get; set; } = []; }
public class AuditResult {
  public Guid Id { get; set; } = Guid.NewGuid(); public Guid AuditId { get; set; } public Audit Audit { get; set; } = null!; public Guid? DocumentTypeId { get; set; } public DocumentType? DocumentType { get; set; }
  public ResultStatus Status { get; set; } public string Message { get; set; } = ""; public string RuleId { get; set; } = ""; public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
  public Guid? NormativeRuleId { get; set; } public NormativeRule? NormativeRule { get; set; } public int? NormativeRuleVersion { get; set; }
  public LegalOutcome? Outcome { get; set; } public string? Explanation { get; set; } public string EvidenceJson { get; set; } = "{}"; public string ComparedValuesJson { get; set; } = "{}"; public string CalculationJson { get; set; } = "{}";
  public bool RequiresManualReview { get; set; } public DateTime? EvaluatedAt { get; set; } public Guid? ReviewerUserId { get; set; } public User? ReviewerUser { get; set; } public DateTime? ReviewedAt { get; set; } public LegalOutcome? ManualOutcome { get; set; } public string? ReviewComment { get; set; } public bool IsIncludedInLegalCompliance { get; set; }
}
public class Finding { public Guid Id { get; set; } = Guid.NewGuid(); public string Number { get; set; } = ""; public Guid AuditId { get; set; } public Audit Audit { get; set; } = null!; public Guid TradeOperationId { get; set; } public TradeOperation TradeOperation { get; set; } = null!; public Guid DocumentTypeId { get; set; } public DocumentType DocumentType { get; set; } = null!; public string RuleId { get; set; } = ""; public Severity Severity { get; set; } public string Description { get; set; } = ""; public CorrectionResponsible CorrectionResponsible { get; set; } public FindingStatus Status { get; set; } = FindingStatus.Pending; public DateTime CreatedAt { get; set; } = DateTime.UtcNow; public DateTime UpdatedAt { get; set; } = DateTime.UtcNow; public DateTime? ResolvedAt { get; set; } public List<FindingHistory> History { get; set; } = []; }
public class FindingHistory { public Guid Id { get; set; } = Guid.NewGuid(); public Guid FindingId { get; set; } public Finding Finding { get; set; } = null!; public FindingStatus? PreviousStatus { get; set; } public FindingStatus NewStatus { get; set; } public string Comment { get; set; } = ""; public Guid ChangedBy { get; set; } public DateTime ChangedAt { get; set; } = DateTime.UtcNow; }
public class NormativeRule {
  public Guid Id { get; set; } = Guid.NewGuid(); public string Code { get; set; } = ""; public string Name { get; set; } = ""; public NormativeSource NormativeSource { get; set; } = NormativeSource.Demo; public RuleCategory RuleCategory { get; set; } = RuleCategory.DEMO;
  public string Description { get; set; } = ""; public string RuleType { get; set; } = "DocumentRequirement"; public string? SourceDocument { get; set; } public string? Article { get; set; } public string? Section { get; set; } public string? Subsection { get; set; } public string? RuleNumber { get; set; } public string? Annex { get; set; } public string? Appendix { get; set; } public string? OfficialUrl { get; set; } public DateTime? PublicationDate { get; set; }
  public DateTime EffectiveFrom { get; set; } public DateTime? EffectiveTo { get; set; } public int Version { get; set; } = 1; public bool IsActive { get; set; } = true; public AutomationLevel AutomationLevel { get; set; } = AutomationLevel.AUTOMATIC; public bool RequiresManualReview { get; set; } public bool IsLegalRule { get; set; } public bool IsIncludedInLegalCompliance { get; set; }
  public string ConfigurationJson { get; set; } = "{}"; public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}
public class CustomsValueAdjustment {
  public Guid Id { get; set; } = Guid.NewGuid(); public Guid TradeOperationId { get; set; } public TradeOperation TradeOperation { get; set; } = null!; public string Concept { get; set; } = ""; public string? Description { get; set; } public decimal Amount { get; set; } public string Currency { get; set; } = "USD"; public AdjustmentType AdjustmentType { get; set; } = AdjustmentType.UNKNOWN; public LegalQualificationStatus LegalQualificationStatus { get; set; } = LegalQualificationStatus.PENDING_REVIEW; public Guid? SourceDocumentId { get; set; } public OperationDocument? SourceDocument { get; set; } public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}
public class SystemAuditLog { public Guid Id { get; set; } = Guid.NewGuid(); public Guid? UserId { get; set; } public User? User { get; set; } public string Action { get; set; } = ""; public string EntityType { get; set; } = ""; public string EntityId { get; set; } = ""; public string Details { get; set; } = ""; public DateTime Timestamp { get; set; } = DateTime.UtcNow; }
