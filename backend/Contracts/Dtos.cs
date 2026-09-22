using Sace.Api.Domain;
namespace Sace.Api.Contracts;

public record LoginRequest(string Email, string Password);
public record CreateOperationRequest(string PedimentoNumber, DateTime OperationDate, string Period, OperationType OperationType, string PedimentoKey, string TariffFraction, string OriginCountry, string DestinationCountry, string CustomsRegime, bool IsImmex, bool PreferentialTreatment, string? TradeAgreement, string? CustomsOffice = null, string? InvoiceNumber = null, decimal? CommercialValue = null, string? Currency = null, decimal? Quantity = null, string? Unit = null, decimal? GrossWeight = null, string? CoveNumber = null, decimal? ExchangeRate = null, decimal? DeclaredCustomsValueMxn = null, string? Incoterm = null, string? Nico = null, string? GoodsDescription = null);
public record RequirementDto(Guid DocumentTypeId, string DocumentType, string Code, bool Required, ResultStatus Status, string RuleCode, string Message, int? CurrentVersion, Guid? DocumentId);
public record ChangeFindingStatusRequest(FindingStatus Status, string? Comment);
public record RuleRequest(string Code, string Name, NormativeSource NormativeSource, string Description, string RuleType, DateTime EffectiveFrom, DateTime? EffectiveTo, bool IsActive, string ConfigurationJson, RuleCategory RuleCategory = RuleCategory.DEMO, string? SourceDocument = null, string? Article = null, string? Section = null, string? Subsection = null, string? RuleNumber = null, string? Annex = null, string? Appendix = null, string? OfficialUrl = null, DateTime? PublicationDate = null, AutomationLevel AutomationLevel = AutomationLevel.AUTOMATIC, bool RequiresManualReview = false, bool IsLegalRule = false, bool IsIncludedInLegalCompliance = false);
public record ApiError(int Status, string Code, string Message, object Errors);
public record DocumentClassificationDto(string DocumentTypeCode, double Confidence, string Reason, bool RequiresManualSelection);
public record DocumentMetadataRequest(string? DocumentNumber, DateTime? DocumentDate, string? SourceSystem, string? Notes);
public record DocumentReferenceRequest(string ReferenceType, string Value, string? Source);
public record UpdateDocumentRequest(string? DocumentNumber, DateTime? DocumentDate, string? SourceSystem, string? Notes, IReadOnlyList<DocumentReferenceRequest>? References);
public record ManualReviewRequest(LegalOutcome ManualOutcome, string Comment);
public record CustomsValueAdjustmentRequest(string Concept, string? Description, decimal Amount, string Currency, AdjustmentType AdjustmentType, LegalQualificationStatus LegalQualificationStatus, Guid? SourceDocumentId);
