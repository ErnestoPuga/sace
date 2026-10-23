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
  public DateTime CreatedAt { get; set; } = DateTime.UtcNow; public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
  public List<OperationDocument> Documents { get; set; } = []; public List<Audit> Audits { get; set; } = []; public List<Finding> Findings { get; set; } = []; public List<CustomsValueAdjustment> Adjustments { get; set; } = []; public List<CrossValidationResult> CrossValidationResults { get; set; } = []; public List<CustomsValuationCalculation> ValuationCalculations { get; set; } = []; public List<ContributionCalculation> ContributionCalculations { get; set; } = []; public List<RegulatoryRequirementEvaluation> RegulatoryEvaluations { get; set; } = [];
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
public class SystemAuditLog { public Guid Id { get; set; } = Guid.NewGuid(); public Guid? UserId { get; set; } public User? User { get; set; } public string Action { get; set; } = ""; public string EntityType { get; set; } = ""; public string EntityId { get; set; } = ""; public string Details { get; set; } = ""; public DateTime Timestamp { get; set; } = DateTime.UtcNow; }
