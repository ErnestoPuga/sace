namespace Sace.Api.Domain;

public enum OperationType { Importacion, Exportacion }
public enum OperationStatus { Draft, PendingAudit, Audited, WithFindings, Completed }
public enum CorrectionResponsible { Company, CustomsBroker }
public enum ValidationStatus { Pending, Valid, Invalid }
public enum AuditType { Daily, Monthly, Reaudit, Legal }
public enum AuditStatus { Running, Completed, Failed }
public enum ResultStatus { Valid, Warning, Invalid, Missing, NotApplicable }
public enum Severity { Low, Medium, High, Critical }
public enum FindingStatus { Pending, InCorrection, Resolved, OPEN, UNDER_REVIEW, CONFIRMED, CORRECTED, DISMISSED, CLOSED }
public enum NormativeSource { Appendix22, IMMEX, Annex24, TradeAgreement, Demo, Technical, CustomsLaw, RGCE }
public enum RuleCategory { LEGAL, TECHNICAL, DEMO }
public enum AutomationLevel { AUTOMATIC, SEMIAUTOMATIC, MANUAL }
public enum LegalOutcome { COMPLIANT, NON_COMPLIANT, NOT_APPLICABLE, INSUFFICIENT_EVIDENCE, MANUAL_REVIEW }
public enum EvaluationDateBasis { OPERATION_DATE, ENTRY_DATE, PAYMENT_DATE, PRESENTATION_DATE }
public enum AdjustmentType { INCREMENTABLE, NON_INCREMENTABLE, UNKNOWN }
public enum LegalQualificationStatus { CONFIRMED, PENDING_REVIEW }
public enum DocumentFieldDataType { TEXT, IDENTIFIER, DECIMAL, CURRENCY, QUANTITY, WEIGHT, DATE }
public enum ExtractionMethod { XML, PDF_TEXT, FILENAME, EXISTING_METADATA, MANUAL }
public enum CrossComparisonType { EXACT_TEXT, NORMALIZED_TEXT, NORMALIZED_IDENTIFIER, EXACT_NUMERIC, NUMERIC_TOLERANCE, PERCENT_TOLERANCE, CURRENCY_AMOUNT, DATE, PRESENCE }
public enum MissingEvidenceBehavior { INSUFFICIENT_EVIDENCE, MANUAL_REVIEW, NOT_APPLICABLE }
public enum CrossValidationOutcome { MATCH, MISMATCH, INSUFFICIENT_EVIDENCE, MANUAL_REVIEW, NOT_APPLICABLE }
public enum ValuationOutcome { MATCH, MISMATCH, INSUFFICIENT_EVIDENCE, MANUAL_REVIEW, NOT_APPLICABLE }
public enum ValuationComponentType { TRANSACTION_VALUE, FREIGHT, INSURANCE, PACKAGING, OTHER_INCREMENTABLE, UNCLASSIFIED_INCREMENTABLE }
public enum ValuationTreatment { ADD, SUBTRACT, INFORMATIONAL }
public enum EvidenceRole { PRIMARY, SUPPORTING, PAYMENT, DECLARED_VALUE }
public enum ContributionOutcome { CALCULATED, RECONCILED, MISMATCH, INSUFFICIENT_EVIDENCE, MANUAL_REVIEW, NOT_APPLICABLE }
public enum RegulatoryRequirementStatus { DRAFT, VERIFIED, RETIRED }
public enum RequirementCategory { NOM, PERMISO_PREVIO, AVISO_AUTOMATICO, COFEPRIS, SEMARNAT, SEDENA, FITOSANITARIO, OTHER }
public enum RegulatoryAuthorityCode { ECONOMIA, SALUD_COFEPRIS, SEMARNAT, SEDENA, AGRICULTURA_SENASICA, SENER, CONAGUA, IFT, OTHER }
public enum RegulatoryOperationScope { IMPORT, EXPORT, BOTH }
public enum RegulatoryConditionOperator { EQUALS, NOT_EQUALS, IN, NOT_IN, PREFIX, NUMERIC_RANGE, PRESENT, NOT_PRESENT, CONTAINS_NORMALIZED }
public enum RegulatoryApplicabilityOutcome { APPLIES, DOES_NOT_APPLY, INSUFFICIENT_DATA, MANUAL_REVIEW }
public enum RegulatoryEvidenceStatus { COMPLETE, PARTIAL, MISSING_EVIDENCE, INSUFFICIENT_DATA, MANUAL_REVIEW, NOT_REQUIRED }
public enum RegulatoryEvidenceRole { PRIMARY, ALTERNATIVE, SUPPORTING }
public enum RegulatoryManualOutcome { CONFIRMED_APPLIES, CONFIRMED_DOES_NOT_APPLY, EVIDENCE_ACCEPTED, EVIDENCE_REJECTED, NEEDS_MORE_INFORMATION }
public enum OfficialSourceType { DOF_PUBLICATION, OFFICIAL_AGREEMENT, OFFICIAL_ANNEX, RGCE, NOM, OFFICIAL_COMPILATION, OTHER_OFFICIAL }
public enum SourceVerificationStatus { STAGED, REVIEWED, VERIFIED, SUPERSEDED, REJECTED }
public enum OfficialCitationType { ARTICLE, RULE, ANNEX, SECTION, NUMERAL, FRACTION, SUBSECTION, TRANSITORY, OTHER }
public enum QualificationType { NONE, ONLY_IF, EXCEPT_IF, MANUAL_SCOPE, OTHER }
public enum RegulatoryVerificationLevel { OFFICIAL_SOURCE, OFFICIAL_COMPILATION, MANUAL_VERIFICATION }
public enum RegulatoryCatalogImportStatus { IMPORTED, FAILED, ALREADY_IMPORTED }
public enum RegulatoryDiffKind { ADDED, REMOVED, MODIFIED, UNCHANGED }
public enum BatchAuditStatus { DRAFT, QUEUED, RUNNING, COMPLETED, COMPLETED_WITH_ERRORS, FAILED, CANCELLED }
public enum BatchOperationStatus { PENDING, RUNNING, COMPLETED, FAILED, CANCELLED }
public enum RiskProfileStatus { DRAFT, ACTIVE, RETIRED }
public enum RiskCategory { DOCUMENTATION, CROSS_VALIDATION, VALUATION, CONTRIBUTIONS, RRNA, LEGAL, DATA_QUALITY, MANUAL_REVIEW, IMMEX }
public enum RiskSourceType { DOCUMENTATION, CROSS_VALIDATION, VALUATION, CONTRIBUTIONS, RRNA, LEGAL, IMMEX }
public enum RiskSeverity { INFO, LOW, MEDIUM, HIGH, CRITICAL }
public enum RiskLevel { LOW, MEDIUM, HIGH, CRITICAL }
public enum RiskCorrelationMode { SUM, MAX }
public enum ReviewQueueStatus { OPEN, ASSIGNED, IN_REVIEW, WAITING_INFORMATION, RESOLVED, DISMISSED }
public enum ReviewResolution { CORRECTED, ACCEPTED, FALSE_POSITIVE, NO_ACTION_REQUIRED, ESCALATED }
public enum Annex24CoverageStatus { SUPPORTED, PARTIAL, MANUAL, NOT_IMPLEMENTED }
public enum ImmexProgramStatus { ACTIVE, SUSPENDED, CANCELLED, EXPIRED, UNKNOWN }
public enum ImmexSourceType { MANUAL, OPERATION_IMPORT, OFFICIAL_SOURCE }
public enum ImmexInventoryClassification { MATERIAL, RAW_MATERIAL, PART, COMPONENT, PACKAGING, LABEL, CONTAINER, FIXED_ASSET, TOOL, MACHINERY, EQUIPMENT, OTHER }
public enum ImmexInventoryEntryStatus { OPEN, PARTIALLY_DISCHARGED, DISCHARGED, CLOSED_BY_REGIME_CHANGE, CLOSED_BY_RETURN, CLOSED_BY_TRANSFER, CLOSED_BY_DESTRUCTION, CLOSED_BY_DONATION, PAST_DUE, MANUAL_REVIEW }
public enum ImmexExitType { RETURN, TRANSFER, REGIME_CHANGE, DESTRUCTION, DONATION, WASTE_RETURN, REPAIR_RETURN, OTHER }
public enum ImmexDischargeMethod { FIFO_QUANTITY }
public enum ImmexDischargeOutcome { ALLOCATED, PARTIALLY_ALLOCATED, INSUFFICIENT_INVENTORY, UNIT_CONVERSION_REQUIRED, MANUAL_REVIEW }
public enum ImmexFixedAssetStatus { OPEN, RETURNED, TRANSFERRED, REGIME_CHANGED, DESTROYED, DONATED, MANUAL_REVIEW }
public enum ImmexDurationType { FIXED_PERIOD, PROGRAM_VALIDITY, MANUAL, NOT_APPLICABLE }
public enum ImmexDurationUnit { DAY, MONTH, YEAR }
public enum ImmexTemporalityOutcome { ACTIVE, DUE_SOON, DUE_CRITICAL, PAST_DUE, RESOLVED, INSUFFICIENT_DATA, MANUAL_REVIEW, NOT_APPLICABLE }
public enum ImmexIssueSeverity { INFO, LOW, MEDIUM, HIGH, CRITICAL }
public enum ImmexIssueStatus { OPEN, IN_REVIEW, RESOLVED, DISMISSED }
public enum LegalRuleStatus { DRAFT, REVIEW_REQUIRED, VERIFIED, RETIRED }
public enum LegalConditionOperator { EQUALS, NOT_EQUALS, IN, NOT_IN, PRESENT, NOT_PRESENT, GREATER_THAN, GREATER_OR_EQUAL, LESS_THAN, LESS_OR_EQUAL, BETWEEN, DATE_BEFORE, DATE_AFTER, BOOLEAN_TRUE, BOOLEAN_FALSE }
public enum LegalValueType { TEXT, DECIMAL, DATE, BOOLEAN }
public enum LegalFailureBehavior { NON_COMPLIANT, NOT_APPLICABLE, INSUFFICIENT_EVIDENCE, MANUAL_REVIEW }
public enum LegalEvidenceType { DOCUMENT, DOCUMENT_FIELD, TRADE_OPERATION, CROSS_VALIDATION, VALUATION, CONTRIBUTION, RRNA, IMMEX, MANUAL, OTHER }
public enum LegalDecisionMode { AUTOMATIC, MANUAL, AUTOMATIC_REVIEWED }
public enum LegalReviewDecisionType { CONFIRM, OVERRIDE, REQUEST_INFORMATION }
public enum LegalCorrectionStatus { PENDING, IN_PROGRESS, CORRECTED, VERIFIED, REJECTED }

public static class DocumentFieldCodes {
  public const string PedimentoNumber="PEDIMENTO_NUMBER", OperationType="OPERATION_TYPE", EntryDate="ENTRY_DATE", Date="DATE", DocumentNumber="DOCUMENT_NUMBER", CustomsOffice="CUSTOMS_OFFICE", InvoiceNumber="INVOICE_NUMBER", CoveNumber="COVE_NUMBER", TariffFraction="TARIFF_FRACTION", Nico="NICO", Description="DESCRIPTION", Quantity="QUANTITY", Unit="UNIT", UnitPrice="UNIT_PRICE", CommercialValue="COMMERCIAL_VALUE", TotalAmount="TOTAL_AMOUNT", Currency="CURRENCY", Incoterm="INCOTERM", NetWeight="NET_WEIGHT", GrossWeight="GROSS_WEIGHT", PackageCount="PACKAGE_COUNT", OriginCountry="ORIGIN_COUNTRY", DestinationCountry="DESTINATION_COUNTRY", SupplierName="SUPPLIER_NAME", BuyerName="BUYER_NAME", TransportReference="TRANSPORT_REFERENCE", TransportMode="TRANSPORT_MODE", Origin="ORIGIN", Destination="DESTINATION", Carrier="CARRIER", ExchangeRate="EXCHANGE_RATE", DeclaredCustomsValue="DECLARED_CUSTOMS_VALUE";
  public const string TransactionValue="TRANSACTION_VALUE", PricePaid="PRICE_PAID", PricePayable="PRICE_PAYABLE", FreightAmount="FREIGHT_AMOUNT", InsuranceAmount="INSURANCE_AMOUNT", PackagingAmount="PACKAGING_AMOUNT", OtherIncrementableAmount="OTHER_INCREMENTABLE_AMOUNT", IncrementableTotal="INCREMENTABLE_TOTAL", DeclaredFreight="DECLARED_FREIGHT", DeclaredInsurance="DECLARED_INSURANCE", DeclaredPackaging="DECLARED_PACKAGING", DeclaredOtherIncrementables="DECLARED_OTHER_INCREMENTABLES", CalculatedCustomsValue="CALCULATED_CUSTOMS_VALUE", PaymentAmount="PAYMENT_AMOUNT", PaymentDate="PAYMENT_DATE", PaymentReference="PAYMENT_REFERENCE", PaymentBeneficiary="PAYMENT_BENEFICIARY", ServiceProvider="SERVICE_PROVIDER", ServiceConcept="SERVICE_CONCEPT", OriginalCurrency="ORIGINAL_CURRENCY", ConvertedAmount="CONVERTED_AMOUNT", ContributionCode="CONTRIBUTION_CODE", ContributionBase="CONTRIBUTION_BASE", ContributionRate="CONTRIBUTION_RATE", ContributionDeclaredAmount="CONTRIBUTION_DECLARED_AMOUNT", ContributionCalculatedAmount="CONTRIBUTION_CALCULATED_AMOUNT", ContributionPaidAmount="CONTRIBUTION_PAID_AMOUNT", PolicyNumber="POLICY_NUMBER", ValidFrom="VALID_FROM", ValidTo="VALID_TO", InsuredName="INSURED_NAME", InsuredGoods="INSURED_GOODS", ValuationMethod="VALUATION_METHOD", Provider="PROVIDER";
}

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
  public Guid? ImmexProgramId { get; set; } public ImmexProgram? ImmexProgram { get; set; } public bool? IsTemporaryImport { get; set; } public string? CustomsRegimeCode { get; set; }
  public DateTime CreatedAt { get; set; } = DateTime.UtcNow; public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
  public List<OperationDocument> Documents { get; set; } = []; public List<Audit> Audits { get; set; } = []; public List<Finding> Findings { get; set; } = []; public List<CustomsValueAdjustment> Adjustments { get; set; } = []; public List<CrossValidationResult> CrossValidationResults { get; set; } = []; public List<CustomsValuationCalculation> ValuationCalculations { get; set; } = []; public List<ContributionCalculation> ContributionCalculations { get; set; } = []; public List<RegulatoryRequirementEvaluation> RegulatoryEvaluations { get; set; } = []; public List<BatchAuditOperation> BatchAuditOperations { get; set; } = []; public List<OperationRiskScore> RiskScores { get; set; } = []; public List<ReviewQueueItem> ReviewQueueItems { get; set; } = []; public List<ImmexInventoryEntry> ImmexInventoryEntries { get; set; } = []; public List<ImmexInventoryExit> ImmexInventoryExits { get; set; } = []; public List<ImmexFixedAsset> ImmexFixedAssets { get; set; } = [];
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
  public List<DocumentReference> References { get; set; } = []; public List<DocumentField> Fields { get; set; } = [];
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
  public string? RuleCode { get; set; } public int? RuleVersion { get; set; } public string? RuleTitle { get; set; } public Guid? SourceVersionId { get; set; } public OfficialLegalSourceVersion? SourceVersion { get; set; }
  public string? SourceCitationSnapshot { get; set; } public DateTime? RuleEffectiveFrom { get; set; } public DateTime? RuleEffectiveTo { get; set; } public string? ApplicabilityExplanation { get; set; } public string? EvidenceExplanation { get; set; } public string? DecisionExplanation { get; set; } public LegalDecisionMode? DecisionMode { get; set; }
  public List<LegalEvaluationEvidence> LegalEvidence { get; set; } = []; public List<LegalReviewDecision> LegalReviews { get; set; } = [];
}
public class Finding { public Guid Id { get; set; } = Guid.NewGuid(); public string Number { get; set; } = ""; public Guid AuditId { get; set; } public Audit Audit { get; set; } = null!; public Guid TradeOperationId { get; set; } public TradeOperation TradeOperation { get; set; } = null!; public Guid DocumentTypeId { get; set; } public DocumentType DocumentType { get; set; } = null!; public string RuleId { get; set; } = ""; public Severity Severity { get; set; } public string Description { get; set; } = ""; public CorrectionResponsible CorrectionResponsible { get; set; } public FindingStatus Status { get; set; } = FindingStatus.Pending; public DateTime CreatedAt { get; set; } = DateTime.UtcNow; public DateTime UpdatedAt { get; set; } = DateTime.UtcNow; public DateTime? ResolvedAt { get; set; } public Guid? OriginalAuditResultId { get; set; } public AuditResult? OriginalAuditResult { get; set; } public Guid? LatestAuditResultId { get; set; } public AuditResult? LatestAuditResult { get; set; } public Guid? ConfirmedBy { get; set; } public User? Confirmer { get; set; } public DateTime? ConfirmedAt { get; set; } public Guid? DismissedBy { get; set; } public User? Dismisser { get; set; } public DateTime? DismissedAt { get; set; } public string? DismissalReason { get; set; } public LegalCorrectionStatus CorrectionStatus { get; set; } = LegalCorrectionStatus.PENDING; public string? CorrectionNotes { get; set; } public List<FindingHistory> History { get; set; } = []; public List<LegalReviewDecision> LegalReviews { get; set; } = []; }
public class FindingHistory { public Guid Id { get; set; } = Guid.NewGuid(); public Guid FindingId { get; set; } public Finding Finding { get; set; } = null!; public FindingStatus? PreviousStatus { get; set; } public FindingStatus NewStatus { get; set; } public string Comment { get; set; } = ""; public Guid ChangedBy { get; set; } public DateTime ChangedAt { get; set; } = DateTime.UtcNow; }
public class NormativeRule {
  public Guid Id { get; set; } = Guid.NewGuid(); public string Code { get; set; } = ""; public string Name { get; set; } = ""; public NormativeSource NormativeSource { get; set; } = NormativeSource.Demo; public RuleCategory RuleCategory { get; set; } = RuleCategory.DEMO;
  public string Description { get; set; } = ""; public string RuleType { get; set; } = "DocumentRequirement"; public string? SourceDocument { get; set; } public string? Article { get; set; } public string? Section { get; set; } public string? Subsection { get; set; } public string? RuleNumber { get; set; } public string? Annex { get; set; } public string? Appendix { get; set; } public string? OfficialUrl { get; set; } public DateTime? PublicationDate { get; set; }
  public DateTime EffectiveFrom { get; set; } public DateTime? EffectiveTo { get; set; } public int Version { get; set; } = 1; public bool IsActive { get; set; } = true; public AutomationLevel AutomationLevel { get; set; } = AutomationLevel.AUTOMATIC; public bool RequiresManualReview { get; set; } public bool IsLegalRule { get; set; } public bool IsIncludedInLegalCompliance { get; set; }
  public string ConfigurationJson { get; set; } = "{}"; public DateTime CreatedAt { get; set; } = DateTime.UtcNow; public LegalRuleStatus LegalStatus { get; set; } = LegalRuleStatus.REVIEW_REQUIRED; public string? Limitations { get; set; }
  public List<LegalRuleCondition> LegalConditions { get; set; } = []; public List<LegalEvidenceRequirement> LegalEvidenceRequirements { get; set; } = []; public List<LegalEvidenceMapping> LegalEvidenceMappings { get; set; } = []; public List<LegalRuleSourceCitation> LegalSourceCitations { get; set; } = [];
}

public class LegalRuleCondition { public Guid Id { get; set; } = Guid.NewGuid(); public Guid NormativeRuleId { get; set; } public NormativeRule NormativeRule { get; set; } = null!; public string GroupCode { get; set; } = "A"; public int Sequence { get; set; } = 1; public string FieldCode { get; set; } = ""; public LegalConditionOperator Operator { get; set; } public string? ExpectedValue { get; set; } public string? ExpectedValue2 { get; set; } public LegalValueType ValueType { get; set; } = LegalValueType.TEXT; public bool Required { get; set; } = true; public LegalFailureBehavior FailureBehavior { get; set; } = LegalFailureBehavior.NON_COMPLIANT; public DateTime CreatedAt { get; set; } = DateTime.UtcNow; }
public class LegalEvidenceRequirement { public Guid Id { get; set; } = Guid.NewGuid(); public Guid NormativeRuleId { get; set; } public NormativeRule NormativeRule { get; set; } = null!; public string Code { get; set; } = ""; public string Name { get; set; } = ""; public LegalEvidenceType EvidenceType { get; set; } public bool Required { get; set; } = true; public string? AlternativeGroup { get; set; } public double? MinimumConfidence { get; set; } public bool ManualVerificationRequired { get; set; } public string Description { get; set; } = ""; public Guid? OfficialSourceCitationId { get; set; } public OfficialSourceCitation? OfficialSourceCitation { get; set; } public DateTime CreatedAt { get; set; } = DateTime.UtcNow; }
public class LegalEvidenceMapping { public Guid Id { get; set; } = Guid.NewGuid(); public Guid NormativeRuleId { get; set; } public NormativeRule NormativeRule { get; set; } = null!; public LegalEvidenceType SourceType { get; set; } public string? SourceOutcome { get; set; } public string TargetEvidenceCode { get; set; } = ""; public string? SourceFieldCode { get; set; } public DateTime CreatedAt { get; set; } = DateTime.UtcNow; }
public class LegalRuleSourceCitation { public Guid Id { get; set; } = Guid.NewGuid(); public Guid NormativeRuleId { get; set; } public NormativeRule NormativeRule { get; set; } = null!; public Guid OfficialSourceCitationId { get; set; } public OfficialSourceCitation OfficialSourceCitation { get; set; } = null!; public bool IsPrimary { get; set; } = true; }
public class LegalEvaluationEvidence { public Guid Id { get; set; } = Guid.NewGuid(); public Guid AuditResultId { get; set; } public AuditResult AuditResult { get; set; } = null!; public LegalEvidenceType EvidenceType { get; set; } public string SourceEntityType { get; set; } = ""; public Guid? SourceEntityId { get; set; } public string? FieldCode { get; set; } public string? RawValue { get; set; } public string? NormalizedValue { get; set; } public string? Outcome { get; set; } public double? Confidence { get; set; } public Guid? DocumentId { get; set; } public OperationDocument? Document { get; set; } public Guid? DocumentFieldId { get; set; } public DocumentField? DocumentField { get; set; } public string Explanation { get; set; } = ""; public DateTime CapturedAt { get; set; } = DateTime.UtcNow; }
public class LegalReviewDecision { public Guid Id { get; set; } = Guid.NewGuid(); public Guid AuditResultId { get; set; } public AuditResult AuditResult { get; set; } = null!; public Guid? FindingId { get; set; } public Finding? Finding { get; set; } public Guid ReviewerUserId { get; set; } public User ReviewerUser { get; set; } = null!; public LegalOutcome PreviousOutcome { get; set; } public LegalOutcome? ReviewedOutcome { get; set; } public LegalReviewDecisionType Decision { get; set; } public string Reason { get; set; } = ""; public DateTime ReviewedAt { get; set; } = DateTime.UtcNow; }
public class CustomsValueAdjustment {
  public Guid Id { get; set; } = Guid.NewGuid(); public Guid TradeOperationId { get; set; } public TradeOperation TradeOperation { get; set; } = null!; public string Concept { get; set; } = ""; public string? Description { get; set; } public decimal Amount { get; set; } public string Currency { get; set; } = "USD"; public AdjustmentType AdjustmentType { get; set; } = AdjustmentType.UNKNOWN; public LegalQualificationStatus LegalQualificationStatus { get; set; } = LegalQualificationStatus.PENDING_REVIEW; public Guid? SourceDocumentId { get; set; } public OperationDocument? SourceDocument { get; set; } public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}
public class DocumentField {
  public Guid Id { get; set; } = Guid.NewGuid(); public Guid OperationDocumentId { get; set; } public OperationDocument OperationDocument { get; set; } = null!;
  public string FieldCode { get; set; } = ""; public string? RawValue { get; set; } public string? NormalizedValue { get; set; }
  public DocumentFieldDataType DataType { get; set; } = DocumentFieldDataType.TEXT; public string? Unit { get; set; } public string? Currency { get; set; }
  public double? Confidence { get; set; } public ExtractionMethod ExtractionMethod { get; set; } public string? SourceLocator { get; set; }
  public string? GroupKey { get; set; } public int? Sequence { get; set; } public bool IsManuallyConfirmed { get; set; }
  public DateTime CreatedAt { get; set; } = DateTime.UtcNow; public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
}
public class CrossValidationRule {
  public Guid Id { get; set; } = Guid.NewGuid(); public string Code { get; set; } = ""; public int Version { get; set; } = 1; public string Name { get; set; } = ""; public string Description { get; set; } = ""; public string Category { get; set; } = "Otros";
  public string SourceDocumentTypeCode { get; set; } = ""; public string SourceFieldCode { get; set; } = ""; public string TargetDocumentTypeCode { get; set; } = ""; public string TargetFieldCode { get; set; } = "";
  public CrossComparisonType ComparisonType { get; set; } public decimal? AbsoluteTolerance { get; set; } public decimal? PercentTolerance { get; set; } public Severity Severity { get; set; } = Severity.Medium;
  public MissingEvidenceBehavior MissingEvidenceBehavior { get; set; } = MissingEvidenceBehavior.INSUFFICIENT_EVIDENCE; public bool IsActive { get; set; } = true; public DateTime? EffectiveFrom { get; set; } public DateTime? EffectiveTo { get; set; }
  public DateTime CreatedAt { get; set; } = DateTime.UtcNow; public DateTime UpdatedAt { get; set; } = DateTime.UtcNow; public List<CrossValidationResult> Results { get; set; } = [];
}
public class CrossValidationResult {
  public Guid Id { get; set; } = Guid.NewGuid(); public Guid TradeOperationId { get; set; } public TradeOperation TradeOperation { get; set; } = null!; public Guid? AuditId { get; set; } public Audit? Audit { get; set; }
  public Guid CrossValidationRuleId { get; set; } public CrossValidationRule CrossValidationRule { get; set; } = null!; public string RuleCode { get; set; } = ""; public int RuleVersion { get; set; }
  public Guid? SourceDocumentId { get; set; } public OperationDocument? SourceDocument { get; set; } public Guid? TargetDocumentId { get; set; } public OperationDocument? TargetDocument { get; set; }
  public Guid? SourceFieldId { get; set; } public DocumentField? SourceField { get; set; } public Guid? TargetFieldId { get; set; } public DocumentField? TargetField { get; set; }
  public string? SourceRawValue { get; set; } public string? TargetRawValue { get; set; } public string? SourceNormalizedValue { get; set; } public string? TargetNormalizedValue { get; set; }
  public CrossValidationOutcome Outcome { get; set; } public decimal? NumericDifference { get; set; } public decimal? PercentDifference { get; set; } public double? Confidence { get; set; }
  public string Explanation { get; set; } = ""; public DateTime EvaluatedAt { get; set; } = DateTime.UtcNow;
  [System.ComponentModel.DataAnnotations.Schema.NotMapped] public ExtractionMethod? SourceExtractionMethod => SourceField?.ExtractionMethod;
  [System.ComponentModel.DataAnnotations.Schema.NotMapped] public ExtractionMethod? TargetExtractionMethod => TargetField?.ExtractionMethod ?? (TargetFieldId is null && TargetRawValue is not null ? ExtractionMethod.EXISTING_METADATA : null);
}
public class CustomsValuationCalculation {
  public Guid Id { get; set; } = Guid.NewGuid(); public Guid TradeOperationId { get; set; } public TradeOperation TradeOperation { get; set; } = null!; public Guid? AuditId { get; set; } public Audit? Audit { get; set; }
  public int CalculationVersion { get; set; } = 1; public decimal? TransactionValue { get; set; } public string? Currency { get; set; } public decimal? ExchangeRate { get; set; } public string? ExchangeRateSource { get; set; } public decimal? TransactionValueConverted { get; set; }
  public decimal FreightTotal { get; set; } public decimal InsuranceTotal { get; set; } public decimal PackagingTotal { get; set; } public decimal OtherIncrementablesTotal { get; set; } public decimal IncrementablesTotal { get; set; }
  public decimal? DeclaredCustomsValue { get; set; } public decimal? CalculatedCustomsValue { get; set; } public decimal? DifferenceAmount { get; set; } public decimal? DifferencePercent { get; set; }
  public decimal AbsoluteToleranceUsed { get; set; } public decimal PercentToleranceUsed { get; set; } public ValuationOutcome Outcome { get; set; } public string Formula { get; set; } = ""; public string Explanation { get; set; } = ""; public string SnapshotJson { get; set; } = "{}"; public DateTime CalculatedAt { get; set; } = DateTime.UtcNow; public bool IsManualReviewRequired { get; set; }
  public List<CustomsValuationComponent> Components { get; set; } = [];
}
public class CustomsValuationComponent {
  public Guid Id { get; set; } = Guid.NewGuid(); public Guid CustomsValuationCalculationId { get; set; } public CustomsValuationCalculation CustomsValuationCalculation { get; set; } = null!;
  public ValuationComponentType ComponentType { get; set; } public string EconomicKey { get; set; } = ""; public Guid? PrimaryDocumentId { get; set; } public OperationDocument? PrimaryDocument { get; set; } public Guid? PrimaryDocumentFieldId { get; set; } public DocumentField? PrimaryDocumentField { get; set; }
  public string? RawAmount { get; set; } public decimal? NormalizedAmount { get; set; } public string? Currency { get; set; } public decimal? ExchangeRate { get; set; } public string? ExchangeRateSource { get; set; } public decimal? ConvertedAmount { get; set; }
  public ValuationTreatment Treatment { get; set; } = ValuationTreatment.ADD; public double? Confidence { get; set; } public string EvidenceDescription { get; set; } = ""; public DateTime CreatedAt { get; set; } = DateTime.UtcNow; public List<ValuationComponentEvidence> Evidence { get; set; } = [];
}
public class ValuationComponentEvidence {
  public Guid Id { get; set; } = Guid.NewGuid(); public Guid ValuationComponentId { get; set; } public CustomsValuationComponent ValuationComponent { get; set; } = null!; public Guid OperationDocumentId { get; set; } public OperationDocument OperationDocument { get; set; } = null!; public Guid? DocumentFieldId { get; set; } public DocumentField? DocumentField { get; set; } public EvidenceRole EvidenceRole { get; set; } public double? Confidence { get; set; } public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}
public class ContributionCalculation {
  public Guid Id { get; set; } = Guid.NewGuid(); public Guid TradeOperationId { get; set; } public TradeOperation TradeOperation { get; set; } = null!; public Guid? AuditId { get; set; } public Audit? Audit { get; set; } public int Version { get; set; } = 1; public ContributionOutcome Status { get; set; } public DateTime CalculatedAt { get; set; } = DateTime.UtcNow; public string Explanation { get; set; } = ""; public string SnapshotJson { get; set; } = "{}"; public List<ContributionLine> Lines { get; set; } = [];
}
public class ContributionLine {
  public Guid Id { get; set; } = Guid.NewGuid(); public Guid ContributionCalculationId { get; set; } public ContributionCalculation ContributionCalculation { get; set; } = null!;
  public string RawContributionCode { get; set; } = ""; public string? RawDescription { get; set; } public string? RawRate { get; set; } public string? RawAmount { get; set; } public string ContributionCode { get; set; } = ""; public string Description { get; set; } = "";
  public decimal? TaxBase { get; set; } public decimal? Rate { get; set; } public decimal? DeclaredAmount { get; set; } public decimal? CalculatedAmount { get; set; } public decimal? PaidAmount { get; set; } public decimal? DifferenceDeclaredVsCalculated { get; set; } public decimal? DifferenceDeclaredVsPaid { get; set; } public string Currency { get; set; } = "MXN";
  public ContributionOutcome Outcome { get; set; } public string Explanation { get; set; } = ""; public Guid? SourceDocumentId { get; set; } public OperationDocument? SourceDocument { get; set; } public Guid? SourceFieldId { get; set; } public DocumentField? SourceField { get; set; } public Guid? PaymentDocumentId { get; set; } public OperationDocument? PaymentDocument { get; set; }
}
public class RegulatoryRequirement {
  public Guid Id { get; set; } = Guid.NewGuid(); public string Code { get; set; } = ""; public int Version { get; set; } = 1; public string Name { get; set; } = ""; public string? Description { get; set; }
  public RequirementCategory RequirementCategory { get; set; } public RegulatoryAuthorityCode? AuthorityCode { get; set; } public string? AuthorityName { get; set; } public RegulatoryOperationScope OperationScope { get; set; } = RegulatoryOperationScope.BOTH;
  public string? LegalBasis { get; set; } public string? OfficialSourceUrl { get; set; } public DateTime? PublicationDate { get; set; } public DateTime? EffectiveFrom { get; set; } public DateTime? EffectiveTo { get; set; }
  public RegulatoryRequirementStatus Status { get; set; } = RegulatoryRequirementStatus.DRAFT; public RegulatoryVerificationLevel? VerificationLevel { get; set; } public Guid? VerifiedBy { get; set; } public User? Verifier { get; set; } public DateTime? VerifiedAt { get; set; } public bool IsAutomatable { get; set; } = true; public bool RequiresManualReview { get; set; }
  public DateTime CreatedAt { get; set; } = DateTime.UtcNow; public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
  public List<RegulatoryRequirementCondition> Conditions { get; set; } = []; public List<RegulatoryEvidenceRequirement> EvidenceRequirements { get; set; } = []; public List<RegulatoryRequirementSourceCitation> SourceCitations { get; set; } = []; public List<RegulatoryTariffEntry> TariffEntries { get; set; } = []; public List<RegulatoryException> Exceptions { get; set; } = []; public List<RegulatoryRequirementEvaluation> Evaluations { get; set; } = [];
}
public class RegulatoryRequirementCondition {
  public Guid Id { get; set; } = Guid.NewGuid(); public Guid RegulatoryRequirementId { get; set; } public RegulatoryRequirement RegulatoryRequirement { get; set; } = null!;
  public string FieldCode { get; set; } = ""; public RegulatoryConditionOperator Operator { get; set; } public string? ExpectedValue { get; set; } public string? ExpectedValue2 { get; set; }
  public string ConditionGroup { get; set; } = "DEFAULT"; public int Sequence { get; set; } = 1;
}
public class RegulatoryEvidenceRequirement {
  public Guid Id { get; set; } = Guid.NewGuid(); public Guid RegulatoryRequirementId { get; set; } public RegulatoryRequirement RegulatoryRequirement { get; set; } = null!;
  public string DocumentTypeCode { get; set; } = ""; public RegulatoryEvidenceRole EvidenceRole { get; set; } = RegulatoryEvidenceRole.PRIMARY; public bool IsRequired { get; set; } = true; public string? Description { get; set; } public string? AlternativeGroup { get; set; }
}
public class RegulatoryRequirementEvaluation {
  public Guid Id { get; set; } = Guid.NewGuid(); public Guid TradeOperationId { get; set; } public TradeOperation TradeOperation { get; set; } = null!; public Guid? AuditId { get; set; } public Audit? Audit { get; set; }
  public Guid RegulatoryRequirementId { get; set; } public RegulatoryRequirement RegulatoryRequirement { get; set; } = null!; public string RequirementCode { get; set; } = ""; public int RequirementVersion { get; set; }
  public DateTime EvaluationDate { get; set; } public RegulatoryApplicabilityOutcome AutomatedOutcome { get; set; } public RegulatoryEvidenceStatus EvidenceStatus { get; set; } public string Explanation { get; set; } = ""; public double? Confidence { get; set; }
  public bool ManualReviewRequired { get; set; } public DateTime EvaluatedAt { get; set; } = DateTime.UtcNow; public string SnapshotJson { get; set; } = "{}";
  public RegulatoryManualOutcome? ManualOutcome { get; set; } public string? ManualNote { get; set; } public Guid? ReviewedBy { get; set; } public User? Reviewer { get; set; } public DateTime? ReviewedAt { get; set; }
  public List<RegulatoryRequirementEvaluationEvidence> Evidence { get; set; } = [];
}
public class RegulatoryRequirementEvaluationEvidence {
  public Guid Id { get; set; } = Guid.NewGuid(); public Guid RegulatoryRequirementEvaluationId { get; set; } public RegulatoryRequirementEvaluation RegulatoryRequirementEvaluation { get; set; } = null!;
  public Guid? RegulatoryEvidenceRequirementId { get; set; } public RegulatoryEvidenceRequirement? RegulatoryEvidenceRequirement { get; set; }
  public Guid? OperationDocumentId { get; set; } public OperationDocument? OperationDocument { get; set; } public string DocumentTypeCode { get; set; } = ""; public RegulatoryEvidenceRole EvidenceRole { get; set; }
  public bool IsRequired { get; set; } public bool IsPresent { get; set; } public string? AlternativeGroup { get; set; } public string? Notes { get; set; }
}
public class OfficialLegalSource {
  public Guid Id { get; set; } = Guid.NewGuid(); public string Code { get; set; } = ""; public string Title { get; set; } = ""; public OfficialSourceType SourceType { get; set; }
  public string Authority { get; set; } = ""; public string OfficialUrl { get; set; } = ""; public SourceVerificationStatus Status { get; set; } = SourceVerificationStatus.STAGED;
  public DateTime CreatedAt { get; set; } = DateTime.UtcNow; public DateTime UpdatedAt { get; set; } = DateTime.UtcNow; public List<OfficialLegalSourceVersion> Versions { get; set; } = [];
}
public class OfficialLegalSourceVersion {
  public Guid Id { get; set; } = Guid.NewGuid(); public Guid OfficialLegalSourceId { get; set; } public OfficialLegalSource OfficialLegalSource { get; set; } = null!; public int Version { get; set; } = 1;
  public string VersionLabel { get; set; } = ""; public string? DofPublicationCode { get; set; } public DateTime PublicationDate { get; set; } public DateTime EffectiveFrom { get; set; } public DateTime? EffectiveTo { get; set; }
  public string OfficialUrl { get; set; } = ""; public string? Sha256 { get; set; } public SourceVerificationStatus VerificationStatus { get; set; } = SourceVerificationStatus.STAGED; public Guid? VerifiedBy { get; set; } public User? Verifier { get; set; } public DateTime? VerifiedAt { get; set; } public string? VerificationNotes { get; set; }
  public DateTime CreatedAt { get; set; } = DateTime.UtcNow; public List<OfficialSourceCitation> Citations { get; set; } = [];
}
public class OfficialSourceCitation {
  public Guid Id { get; set; } = Guid.NewGuid(); public Guid OfficialLegalSourceVersionId { get; set; } public OfficialLegalSourceVersion OfficialLegalSourceVersion { get; set; } = null!;
  public OfficialCitationType CitationType { get; set; } public string CitationLabel { get; set; } = ""; public string? Article { get; set; } public string? RuleNumber { get; set; } public string? Annex { get; set; } public string? Section { get; set; } public string? Subsection { get; set; } public string? Numeral { get; set; }
  public string? ExactReferenceText { get; set; } public string? OfficialUrl { get; set; } public DateTime CreatedAt { get; set; } = DateTime.UtcNow; public List<RegulatoryRequirementSourceCitation> Requirements { get; set; } = []; public List<RegulatoryTariffEntry> TariffEntries { get; set; } = [];
}
public class RegulatoryRequirementSourceCitation {
  public Guid Id { get; set; } = Guid.NewGuid(); public Guid RegulatoryRequirementId { get; set; } public RegulatoryRequirement RegulatoryRequirement { get; set; } = null!; public Guid OfficialSourceCitationId { get; set; } public OfficialSourceCitation OfficialSourceCitation { get; set; } = null!; public bool IsPrimary { get; set; } = true;
}
public class RegulatoryTariffEntry {
  public Guid Id { get; set; } = Guid.NewGuid(); public Guid RegulatoryRequirementId { get; set; } public RegulatoryRequirement RegulatoryRequirement { get; set; } = null!; public string RawTariffFraction { get; set; } = ""; public string NormalizedTariffFraction { get; set; } = ""; public string? Nico { get; set; }
  public RegulatoryOperationScope OperationScope { get; set; } = RegulatoryOperationScope.BOTH; public string? CustomsRegime { get; set; } public string? GoodsDescription { get; set; } public QualificationType QualificationType { get; set; } public string? QualificationText { get; set; } public string? OriginCountryCodes { get; set; } public string? DestinationCountryCodes { get; set; }
  public DateTime EffectiveFrom { get; set; } public DateTime? EffectiveTo { get; set; } public Guid OfficialSourceCitationId { get; set; } public OfficialSourceCitation OfficialSourceCitation { get; set; } = null!; public SourceVerificationStatus Status { get; set; } = SourceVerificationStatus.STAGED; public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}
public class RegulatoryException {
  public Guid Id { get; set; } = Guid.NewGuid(); public Guid RegulatoryRequirementId { get; set; } public RegulatoryRequirement RegulatoryRequirement { get; set; } = null!; public string ExceptionType { get; set; } = "OTHER"; public string Description { get; set; } = ""; public string? CountryCodes { get; set; } public string? CustomsRegime { get; set; } public DateTime? EffectiveFrom { get; set; } public DateTime? EffectiveTo { get; set; } public Guid? OfficialSourceCitationId { get; set; } public OfficialSourceCitation? OfficialSourceCitation { get; set; }
}
public class RegulatoryCatalogImport {
  public Guid Id { get; set; } = Guid.NewGuid(); public string PackageId { get; set; } = ""; public int PackageVersion { get; set; } public string PackageHash { get; set; } = ""; public string SourceCode { get; set; } = ""; public string SourceVersionLabel { get; set; } = ""; public DateTime PublicationDate { get; set; } public DateTime EffectiveFrom { get; set; }
  public RegulatoryCatalogImportStatus Status { get; set; } public string SummaryJson { get; set; } = "{}"; public string PayloadJson { get; set; } = "{}"; public Guid? ImportedBy { get; set; } public User? Importer { get; set; } public DateTime ImportedAt { get; set; } = DateTime.UtcNow;
}
public class BatchAudit {
  public Guid Id { get; set; } = Guid.NewGuid(); public string Name { get; set; } = ""; public string? Description { get; set; } public DateTime? PeriodStart { get; set; } public DateTime? PeriodEnd { get; set; }
  public Guid CreatedBy { get; set; } public User Creator { get; set; } = null!; public DateTime CreatedAt { get; set; } = DateTime.UtcNow; public DateTime? StartedAt { get; set; } public DateTime? CompletedAt { get; set; }
  public BatchAuditStatus Status { get; set; } = BatchAuditStatus.DRAFT; public int TotalOperations { get; set; } public int ProcessedOperations { get; set; } public int SuccessfulOperations { get; set; } public int FailedOperations { get; set; }
  public string? RiskModelVersion { get; set; } public string ParametersJson { get; set; } = "{}"; public List<BatchAuditOperation> Operations { get; set; } = [];
}
public class BatchAuditOperation {
  public Guid Id { get; set; } = Guid.NewGuid(); public Guid BatchAuditId { get; set; } public BatchAudit BatchAudit { get; set; } = null!; public Guid TradeOperationId { get; set; } public TradeOperation TradeOperation { get; set; } = null!;
  public BatchOperationStatus Status { get; set; } = BatchOperationStatus.PENDING; public DateTime? StartedAt { get; set; } public DateTime? CompletedAt { get; set; } public Guid? AuditId { get; set; } public Audit? Audit { get; set; } public Guid? RiskScoreId { get; set; } public OperationRiskScore? RiskScore { get; set; }
  public string? ErrorCode { get; set; } public string? ErrorMessage { get; set; } public int RetryCount { get; set; } public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}
public class RiskProfile {
  public Guid Id { get; set; } = Guid.NewGuid(); public string Code { get; set; } = ""; public int Version { get; set; } = 1; public string Name { get; set; } = ""; public string Description { get; set; } = ""; public RiskProfileStatus Status { get; set; } = RiskProfileStatus.DRAFT;
  public DateTime? EffectiveFrom { get; set; } public DateTime? EffectiveTo { get; set; } public decimal LowMax { get; set; } = 19; public decimal MediumMax { get; set; } = 39; public decimal HighMax { get; set; } = 69; public string CategoryCapsJson { get; set; } = "{}";
  public DateTime CreatedAt { get; set; } = DateTime.UtcNow; public DateTime UpdatedAt { get; set; } = DateTime.UtcNow; public List<RiskFactorDefinition> Factors { get; set; } = []; public List<OperationRiskScore> Scores { get; set; } = [];
}
public class RiskFactorDefinition {
  public Guid Id { get; set; } = Guid.NewGuid(); public Guid RiskProfileId { get; set; } public RiskProfile RiskProfile { get; set; } = null!; public string Code { get; set; } = ""; public string Name { get; set; } = ""; public RiskCategory Category { get; set; } public RiskSourceType SourceType { get; set; } public string? SourceOutcome { get; set; }
  public decimal Weight { get; set; } public decimal MaxPoints { get; set; } public RiskSeverity Severity { get; set; } public bool IsActive { get; set; } = true; public string? ExplanationTemplate { get; set; } public string? CorrelationKey { get; set; } public RiskCorrelationMode CorrelationMode { get; set; } = RiskCorrelationMode.SUM; public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}
public class OperationRiskScore {
  public Guid Id { get; set; } = Guid.NewGuid(); public Guid TradeOperationId { get; set; } public TradeOperation TradeOperation { get; set; } = null!; public Guid? AuditId { get; set; } public Audit? Audit { get; set; } public Guid RiskProfileId { get; set; } public RiskProfile RiskProfile { get; set; } = null!;
  public string RiskProfileCode { get; set; } = ""; public int RiskProfileVersion { get; set; } public int Version { get; set; } = 1; public decimal RawScore { get; set; } public decimal NormalizedScore { get; set; } public RiskLevel RiskLevel { get; set; }
  public int FactorCount { get; set; } public int CriticalFactorCount { get; set; } public int HighFactorCount { get; set; } public string Explanation { get; set; } = ""; public DateTime CalculatedAt { get; set; } = DateTime.UtcNow; public string SnapshotJson { get; set; } = "{}"; public List<OperationRiskFactor> Factors { get; set; } = [];
}
public class OperationRiskFactor {
  public Guid Id { get; set; } = Guid.NewGuid(); public Guid OperationRiskScoreId { get; set; } public OperationRiskScore OperationRiskScore { get; set; } = null!; public string FactorCode { get; set; } = ""; public RiskCategory Category { get; set; }
  public string SourceEntityType { get; set; } = ""; public Guid? SourceEntityId { get; set; } public string? SourceOutcome { get; set; } public decimal Points { get; set; } public decimal Weight { get; set; } public RiskSeverity Severity { get; set; }
  public string Title { get; set; } = ""; public string Explanation { get; set; } = ""; public string? CorrelationKey { get; set; } public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}
public class ReviewQueueItem {
  public Guid Id { get; set; } = Guid.NewGuid(); public Guid TradeOperationId { get; set; } public TradeOperation TradeOperation { get; set; } = null!; public Guid RiskScoreId { get; set; } public OperationRiskScore RiskScore { get; set; } = null!;
  public RiskLevel OriginalPriority { get; set; } public RiskLevel CurrentPriority { get; set; } public ReviewQueueStatus Status { get; set; } = ReviewQueueStatus.OPEN; public Guid? AssignedTo { get; set; } public User? Assignee { get; set; }
  public DateTime CreatedAt { get; set; } = DateTime.UtcNow; public DateTime? AssignedAt { get; set; } public DateTime? StartedAt { get; set; } public DateTime? CompletedAt { get; set; } public DateTime? DueDate { get; set; }
  public ReviewResolution? Resolution { get; set; } public string? Notes { get; set; } public Guid? PriorityChangedBy { get; set; } public User? PriorityChanger { get; set; } public DateTime? PriorityChangedAt { get; set; } public string? PriorityChangeReason { get; set; } public List<ReviewQueueHistory> History { get; set; } = [];
}
public class ReviewQueueHistory {
  public Guid Id { get; set; } = Guid.NewGuid(); public Guid ReviewQueueItemId { get; set; } public ReviewQueueItem ReviewQueueItem { get; set; } = null!; public string Action { get; set; } = ""; public ReviewQueueStatus? PreviousStatus { get; set; } public ReviewQueueStatus? NewStatus { get; set; } public Guid UserId { get; set; } public User User { get; set; } = null!; public string? Notes { get; set; } public DateTime Timestamp { get; set; } = DateTime.UtcNow;
}
public class SystemAuditLog { public Guid Id { get; set; } = Guid.NewGuid(); public Guid? UserId { get; set; } public User? User { get; set; } public string Action { get; set; } = ""; public string EntityType { get; set; } = ""; public string EntityId { get; set; } = ""; public string Details { get; set; } = ""; public DateTime Timestamp { get; set; } = DateTime.UtcNow; }

public class Annex24CoverageItem {
  public Guid Id { get; set; } = Guid.NewGuid(); public string Code { get; set; } = ""; public string Section { get; set; } = ""; public string Description { get; set; } = ""; public Annex24CoverageStatus CoverageStatus { get; set; }
  public string? ImplementationReference { get; set; } public string? Notes { get; set; } public Guid? OfficialSourceCitationId { get; set; } public OfficialSourceCitation? OfficialSourceCitation { get; set; }
}
public class ImmexProgram {
  public Guid Id { get; set; } = Guid.NewGuid(); public string ProgramNumber { get; set; } = ""; public string CompanyName { get; set; } = ""; public string Rfc { get; set; } = ""; public ImmexProgramStatus Status { get; set; } = ImmexProgramStatus.UNKNOWN;
  public DateTime? ValidFrom { get; set; } public DateTime? ValidTo { get; set; } public string? ProgramModality { get; set; } public string? CertifiedCompanyStatus { get; set; } public string? FiscalAddress { get; set; } public string? IndustrialPlantAddressesJson { get; set; } public string? WarehouseAddressesJson { get; set; }
  public DateTime CreatedAt { get; set; } = DateTime.UtcNow; public DateTime UpdatedAt { get; set; } = DateTime.UtcNow; public ImmexSourceType SourceType { get; set; } = ImmexSourceType.MANUAL; public List<ImmexMaterial> Materials { get; set; } = []; public List<ImmexProduct> Products { get; set; } = [];
}
public class ImmexMaterial {
  public Guid Id { get; set; } = Guid.NewGuid(); public Guid ImmexProgramId { get; set; } public ImmexProgram ImmexProgram { get; set; } = null!; public string MaterialCode { get; set; } = ""; public string Description { get; set; } = ""; public string TariffFraction { get; set; } = ""; public string? Nico { get; set; } public string TigieUnit { get; set; } = ""; public string? CommercialUnit { get; set; } public bool IsActive { get; set; } = true; public DateTime CreatedAt { get; set; } = DateTime.UtcNow; public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
}
public class ImmexProduct {
  public Guid Id { get; set; } = Guid.NewGuid(); public Guid ImmexProgramId { get; set; } public ImmexProgram ImmexProgram { get; set; } = null!; public string ProductCode { get; set; } = ""; public string Description { get; set; } = ""; public string TariffFraction { get; set; } = ""; public string? Nico { get; set; } public string TigieUnit { get; set; } = ""; public string? CommercialUnit { get; set; } public bool IsActive { get; set; } = true; public DateTime CreatedAt { get; set; } = DateTime.UtcNow; public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
}
public class ImmexInventoryEntry {
  public Guid Id { get; set; } = Guid.NewGuid(); public Guid ImmexProgramId { get; set; } public ImmexProgram ImmexProgram { get; set; } = null!; public Guid? TradeOperationId { get; set; } public TradeOperation? TradeOperation { get; set; } public Guid? OperationDocumentId { get; set; } public OperationDocument? OperationDocument { get; set; } public Guid? MaterialId { get; set; } public ImmexMaterial? Material { get; set; }
  public string PedimentoNumber { get; set; } = ""; public string PedimentoKey { get; set; } = ""; public int? PedimentoItemNumber { get; set; } public int Sequence { get; set; } = 1; public DateTime EntryDate { get; set; } public string? CustomsOffice { get; set; } public string TariffFraction { get; set; } = ""; public string? Nico { get; set; } public string? PartNumber { get; set; } public string Description { get; set; } = "";
  public decimal OriginalQuantity { get; set; } public decimal AvailableQuantity { get; set; } public string TigieUnit { get; set; } = ""; public string? CommercialUnit { get; set; } public decimal? CommercialValue { get; set; } public string? Currency { get; set; } public ImmexInventoryClassification InventoryClassification { get; set; } = ImmexInventoryClassification.OTHER;
  public Guid? TemporalRuleId { get; set; } public ImmexTemporalityRule? TemporalRule { get; set; } public DateTime? DueDate { get; set; } public ImmexTemporalityOutcome TemporalityOutcome { get; set; } = ImmexTemporalityOutcome.INSUFFICIENT_DATA; public string? TemporalityRuleCode { get; set; } public int? TemporalityRuleVersion { get; set; } public DateTime? TemporalityCalculatedAt { get; set; } public string? TemporalityExplanation { get; set; }
  public ImmexInventoryEntryStatus Status { get; set; } = ImmexInventoryEntryStatus.OPEN; public DateTime CreatedAt { get; set; } = DateTime.UtcNow; public DateTime UpdatedAt { get; set; } = DateTime.UtcNow; public List<ImmexInventoryDischarge> Discharges { get; set; } = [];
}
public class ImmexMaterialUsage {
  public Guid Id { get; set; } = Guid.NewGuid(); public Guid ImmexProgramId { get; set; } public ImmexProgram ImmexProgram { get; set; } = null!; public Guid? ProductId { get; set; } public ImmexProduct? Product { get; set; } public Guid MaterialId { get; set; } public ImmexMaterial Material { get; set; } = null!; public DateTime PeriodStart { get; set; } public DateTime PeriodEnd { get; set; } public decimal QuantityUsed { get; set; } public decimal LossQuantity { get; set; } public decimal WasteQuantity { get; set; } public string Unit { get; set; } = ""; public ImmexSourceType SourceType { get; set; } = ImmexSourceType.MANUAL; public Guid? SourceDocumentId { get; set; } public OperationDocument? SourceDocument { get; set; } public Guid? SourceFieldId { get; set; } public DocumentField? SourceField { get; set; } public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}
public class ImmexInventoryExit {
  public Guid Id { get; set; } = Guid.NewGuid(); public Guid ImmexProgramId { get; set; } public ImmexProgram ImmexProgram { get; set; } = null!; public Guid? TradeOperationId { get; set; } public TradeOperation? TradeOperation { get; set; } public ImmexExitType ExitType { get; set; } public string? PedimentoNumber { get; set; } public string? PedimentoKey { get; set; } public DateTime ExitDate { get; set; } public Guid? ProductId { get; set; } public ImmexProduct? Product { get; set; } public Guid? MaterialId { get; set; } public ImmexMaterial? Material { get; set; } public decimal Quantity { get; set; } public string Unit { get; set; } = ""; public string? TariffFraction { get; set; } public string? Nico { get; set; } public string? Reference { get; set; } public decimal AllocatedQuantity { get; set; } public decimal UnallocatedQuantity { get; set; } public ImmexDischargeOutcome DischargeOutcome { get; set; } = ImmexDischargeOutcome.MANUAL_REVIEW; public DateTime CreatedAt { get; set; } = DateTime.UtcNow; public List<ImmexInventoryDischarge> Discharges { get; set; } = [];
}
public class ImmexInventoryDischarge {
  public Guid Id { get; set; } = Guid.NewGuid(); public Guid InventoryEntryId { get; set; } public ImmexInventoryEntry InventoryEntry { get; set; } = null!; public Guid InventoryExitId { get; set; } public ImmexInventoryExit InventoryExit { get; set; } = null!; public Guid MaterialId { get; set; } public ImmexMaterial Material { get; set; } = null!; public int Sequence { get; set; } public decimal QuantityDischarged { get; set; } public string Unit { get; set; } = ""; public ImmexDischargeMethod DischargeMethod { get; set; } = ImmexDischargeMethod.FIFO_QUANTITY; public decimal EntryBalanceBefore { get; set; } public decimal EntryBalanceAfter { get; set; } public DateTime DischargedAt { get; set; } = DateTime.UtcNow; public bool Automatic { get; set; } = true; public string Explanation { get; set; } = ""; public Guid? SourceCitationId { get; set; } public OfficialSourceCitation? SourceCitation { get; set; } public DateTime CreatedAt { get; set; } = DateTime.UtcNow; public ImmexDischargeReversal? Reversal { get; set; }
}
public class ImmexDischargeReversal {
  public Guid Id { get; set; } = Guid.NewGuid(); public Guid OriginalDischargeId { get; set; } public ImmexInventoryDischarge OriginalDischarge { get; set; } = null!; public string Reason { get; set; } = ""; public Guid ReversedBy { get; set; } public User User { get; set; } = null!; public DateTime ReversedAt { get; set; } = DateTime.UtcNow;
}
public class ImmexFixedAsset {
  public Guid Id { get; set; } = Guid.NewGuid(); public Guid ImmexProgramId { get; set; } public ImmexProgram ImmexProgram { get; set; } = null!; public Guid? TradeOperationId { get; set; } public TradeOperation? TradeOperation { get; set; } public string Description { get; set; } = ""; public string? Brand { get; set; } public string? Model { get; set; } public string? SerialNumber { get; set; } public string? TariffFraction { get; set; } public string? Nico { get; set; } public string PedimentoNumber { get; set; } = ""; public string PedimentoKey { get; set; } = ""; public DateTime EntryDate { get; set; } public ImmexFixedAssetStatus Status { get; set; } = ImmexFixedAssetStatus.OPEN; public ImmexExitType? ExitType { get; set; } public DateTime? ExitDate { get; set; } public string? ExitPedimentoNumber { get; set; } public DateTime CreatedAt { get; set; } = DateTime.UtcNow; public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
}
public class ImmexTemporalityRule {
  public Guid Id { get; set; } = Guid.NewGuid(); public string Code { get; set; } = ""; public int Version { get; set; } = 1; public string Name { get; set; } = ""; public ImmexInventoryClassification InventoryClassification { get; set; } public ImmexDurationType DurationType { get; set; } public int? DurationValue { get; set; } public ImmexDurationUnit? DurationUnit { get; set; } public DateTime EffectiveFrom { get; set; } public DateTime? EffectiveTo { get; set; } public SourceVerificationStatus Status { get; set; } = SourceVerificationStatus.STAGED; public Guid? OfficialSourceCitationId { get; set; } public OfficialSourceCitation? OfficialSourceCitation { get; set; } public string ConditionsJson { get; set; } = "{}"; public bool RequiresManualQualification { get; set; } public bool IsOverride { get; set; } public int Priority { get; set; } public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}
public class ImmexUnitConversion {
  public Guid Id { get; set; } = Guid.NewGuid(); public Guid MaterialId { get; set; } public ImmexMaterial Material { get; set; } = null!; public string FromUnit { get; set; } = ""; public string ToUnit { get; set; } = ""; public decimal Factor { get; set; } public DateTime EffectiveFrom { get; set; } public DateTime? EffectiveTo { get; set; } public string Source { get; set; } = ""; public bool IsVerified { get; set; }
}
public class ImmexInventoryIssue {
  public Guid Id { get; set; } = Guid.NewGuid(); public Guid ImmexProgramId { get; set; } public ImmexProgram ImmexProgram { get; set; } = null!; public Guid? InventoryEntryId { get; set; } public ImmexInventoryEntry? InventoryEntry { get; set; } public Guid? InventoryExitId { get; set; } public ImmexInventoryExit? InventoryExit { get; set; } public string IssueCode { get; set; } = ""; public ImmexIssueSeverity Severity { get; set; } public ImmexIssueStatus Status { get; set; } = ImmexIssueStatus.OPEN; public string Description { get; set; } = ""; public DateTime DetectedAt { get; set; } = DateTime.UtcNow; public DateTime? ResolvedAt { get; set; } public Guid? ResolvedBy { get; set; } public User? Resolver { get; set; } public string? Resolution { get; set; }
}
