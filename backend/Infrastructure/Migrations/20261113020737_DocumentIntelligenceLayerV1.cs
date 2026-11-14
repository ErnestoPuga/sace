using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Sace.Api.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class DocumentIntelligenceLayerV1 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ClassificationConfidenceLevel",
                table: "OperationDocuments",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ClassificationEvidenceJson",
                table: "OperationDocuments",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "DetectedLanguage",
                table: "OperationDocuments",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "IntelligenceStatus",
                table: "OperationDocuments",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "IntelligenceVersion",
                table: "OperationDocuments",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<DateTime>(
                name: "LastAnalyzedAt",
                table: "OperationDocuments",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "LayoutFingerprint",
                table: "OperationDocuments",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "OcrUsed",
                table: "OperationDocuments",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<decimal>(
                name: "Cartons",
                table: "DocumentLineItems",
                type: "TEXT",
                precision: 18,
                scale: 6,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ConfidenceLevel",
                table: "DocumentLineItems",
                type: "TEXT",
                nullable: false,
                defaultValue: "MEDIUM");

            migrationBuilder.AddColumn<string>(
                name: "DimensionUnit",
                table: "DocumentLineItems",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "Height",
                table: "DocumentLineItems",
                type: "TEXT",
                precision: 18,
                scale: 6,
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "Length",
                table: "DocumentLineItems",
                type: "TEXT",
                precision: 18,
                scale: 6,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "LotNumber",
                table: "DocumentLineItems",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Model",
                table: "DocumentLineItems",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "PageNumber",
                table: "DocumentLineItems",
                type: "INTEGER",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "Pallets",
                table: "DocumentLineItems",
                type: "TEXT",
                precision: 18,
                scale: 6,
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "PiecesPerCarton",
                table: "DocumentLineItems",
                type: "TEXT",
                precision: 18,
                scale: 6,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PoNumber",
                table: "DocumentLineItems",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "RecognitionMethod",
                table: "DocumentLineItems",
                type: "TEXT",
                nullable: false,
                defaultValue: "PDF_TABLE_HEADER");

            migrationBuilder.AddColumn<string>(
                name: "RegionJson",
                table: "DocumentLineItems",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "SerialNumber",
                table: "DocumentLineItems",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "SoNumber",
                table: "DocumentLineItems",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "SourceKind",
                table: "DocumentLineItems",
                type: "TEXT",
                nullable: false,
                defaultValue: "EXTRACTED");

            migrationBuilder.AddColumn<string>(
                name: "Submodel",
                table: "DocumentLineItems",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "VolumeCbm",
                table: "DocumentLineItems",
                type: "TEXT",
                precision: 18,
                scale: 6,
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "Width",
                table: "DocumentLineItems",
                type: "TEXT",
                precision: 18,
                scale: 6,
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "AnalysisVersion",
                table: "DocumentFields",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "CanonicalFieldCode",
                table: "DocumentFields",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "ColumnIndex",
                table: "DocumentFields",
                type: "INTEGER",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ConfidenceLevel",
                table: "DocumentFields",
                type: "TEXT",
                nullable: false,
                defaultValue: "MEDIUM");

            migrationBuilder.AddColumn<string>(
                name: "Formula",
                table: "DocumentFields",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "OriginalLabel",
                table: "DocumentFields",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "OriginalLanguage",
                table: "DocumentFields",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "OriginalText",
                table: "DocumentFields",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "PageNumber",
                table: "DocumentFields",
                type: "INTEGER",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "ProviderProfileId",
                table: "DocumentFields",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "RecognitionMethod",
                table: "DocumentFields",
                type: "TEXT",
                nullable: false,
                defaultValue: "PDF_TEXT_LABEL");

            migrationBuilder.AddColumn<string>(
                name: "RecognitionReason",
                table: "DocumentFields",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "RegionJson",
                table: "DocumentFields",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "RequiresReview",
                table: "DocumentFields",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<int>(
                name: "RowIndex",
                table: "DocumentFields",
                type: "INTEGER",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Section",
                table: "DocumentFields",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "SourceKind",
                table: "DocumentFields",
                type: "TEXT",
                nullable: false,
                defaultValue: "EXTRACTED");

            migrationBuilder.AddColumn<int>(
                name: "TableIndex",
                table: "DocumentFields",
                type: "INTEGER",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "CanonicalFieldDefinitions",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    Code = table.Column<string>(type: "TEXT", nullable: false),
                    Name = table.Column<string>(type: "TEXT", nullable: false),
                    DataType = table.Column<string>(type: "TEXT", nullable: false),
                    Category = table.Column<string>(type: "TEXT", nullable: false),
                    Description = table.Column<string>(type: "TEXT", nullable: false),
                    IsRepeatable = table.Column<bool>(type: "INTEGER", nullable: false),
                    AllowedDocumentTypesJson = table.Column<string>(type: "TEXT", nullable: false),
                    CanonicalUnitType = table.Column<string>(type: "TEXT", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CanonicalFieldDefinitions", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "DocumentFieldCorrections",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    DocumentFieldId = table.Column<Guid>(type: "TEXT", nullable: false),
                    OriginalCanonicalFieldCode = table.Column<string>(type: "TEXT", nullable: false),
                    OriginalRawValue = table.Column<string>(type: "TEXT", nullable: true),
                    OriginalNormalizedValue = table.Column<string>(type: "TEXT", nullable: true),
                    CorrectedCanonicalFieldCode = table.Column<string>(type: "TEXT", nullable: false),
                    CorrectedRawValue = table.Column<string>(type: "TEXT", nullable: true),
                    CorrectedNormalizedValue = table.Column<string>(type: "TEXT", nullable: true),
                    CorrectedUnit = table.Column<string>(type: "TEXT", nullable: true),
                    Reason = table.Column<string>(type: "TEXT", nullable: false),
                    Evidence = table.Column<string>(type: "TEXT", nullable: true),
                    CorrectedBy = table.Column<Guid>(type: "TEXT", nullable: false),
                    CorrectedAt = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DocumentFieldCorrections", x => x.Id);
                    table.ForeignKey(
                        name: "FK_DocumentFieldCorrections_DocumentFields_DocumentFieldId",
                        column: x => x.DocumentFieldId,
                        principalTable: "DocumentFields",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "DocumentIntelligenceAnalyses",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    OperationDocumentId = table.Column<Guid>(type: "TEXT", nullable: false),
                    Version = table.Column<int>(type: "INTEGER", nullable: false),
                    DetectedDocumentTypeCode = table.Column<string>(type: "TEXT", nullable: false),
                    ClassificationConfidence = table.Column<string>(type: "TEXT", nullable: false),
                    ClassificationEvidenceJson = table.Column<string>(type: "TEXT", nullable: false),
                    DetectedLanguage = table.Column<string>(type: "TEXT", nullable: true),
                    ReadingMethod = table.Column<string>(type: "TEXT", nullable: false),
                    OcrUsed = table.Column<bool>(type: "INTEGER", nullable: false),
                    OcrPagesJson = table.Column<string>(type: "TEXT", nullable: false),
                    Status = table.Column<string>(type: "TEXT", nullable: false),
                    NativeTextLength = table.Column<int>(type: "INTEGER", nullable: false),
                    FieldCount = table.Column<int>(type: "INTEGER", nullable: false),
                    LineItemCount = table.Column<int>(type: "INTEGER", nullable: false),
                    LayoutFingerprint = table.Column<string>(type: "TEXT", nullable: false),
                    SnapshotJson = table.Column<string>(type: "TEXT", nullable: false),
                    ErrorCode = table.Column<string>(type: "TEXT", nullable: true),
                    Explanation = table.Column<string>(type: "TEXT", nullable: true),
                    CreatedBy = table.Column<Guid>(type: "TEXT", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DocumentIntelligenceAnalyses", x => x.Id);
                    table.ForeignKey(
                        name: "FK_DocumentIntelligenceAnalyses_OperationDocuments_OperationDocumentId",
                        column: x => x.OperationDocumentId,
                        principalTable: "OperationDocuments",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "DocumentParticipants",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    OperationDocumentId = table.Column<Guid>(type: "TEXT", nullable: false),
                    Role = table.Column<string>(type: "TEXT", nullable: false),
                    Name = table.Column<string>(type: "TEXT", nullable: false),
                    TaxId = table.Column<string>(type: "TEXT", nullable: true),
                    RawAddress = table.Column<string>(type: "TEXT", nullable: true),
                    Street = table.Column<string>(type: "TEXT", nullable: true),
                    ExteriorNumber = table.Column<string>(type: "TEXT", nullable: true),
                    PostalCode = table.Column<string>(type: "TEXT", nullable: true),
                    City = table.Column<string>(type: "TEXT", nullable: true),
                    State = table.Column<string>(type: "TEXT", nullable: true),
                    Country = table.Column<string>(type: "TEXT", nullable: true),
                    SourceKind = table.Column<string>(type: "TEXT", nullable: false),
                    ConfidenceLevel = table.Column<string>(type: "TEXT", nullable: false),
                    SourceLocator = table.Column<string>(type: "TEXT", nullable: true),
                    PageNumber = table.Column<int>(type: "INTEGER", nullable: true),
                    RegionJson = table.Column<string>(type: "TEXT", nullable: true),
                    AnalysisVersion = table.Column<int>(type: "INTEGER", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DocumentParticipants", x => x.Id);
                    table.ForeignKey(
                        name: "FK_DocumentParticipants_OperationDocuments_OperationDocumentId",
                        column: x => x.OperationDocumentId,
                        principalTable: "OperationDocuments",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "DocumentRelationshipCandidates",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    SourceDocumentId = table.Column<Guid>(type: "TEXT", nullable: false),
                    TargetDocumentId = table.Column<Guid>(type: "TEXT", nullable: false),
                    RelationshipType = table.Column<string>(type: "TEXT", nullable: false),
                    EvidenceJson = table.Column<string>(type: "TEXT", nullable: false),
                    ConfidenceLevel = table.Column<string>(type: "TEXT", nullable: false),
                    ReviewStatus = table.Column<string>(type: "TEXT", nullable: false),
                    Explanation = table.Column<string>(type: "TEXT", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DocumentRelationshipCandidates", x => x.Id);
                    table.ForeignKey(
                        name: "FK_DocumentRelationshipCandidates_OperationDocuments_SourceDocumentId",
                        column: x => x.SourceDocumentId,
                        principalTable: "OperationDocuments",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_DocumentRelationshipCandidates_OperationDocuments_TargetDocumentId",
                        column: x => x.TargetDocumentId,
                        principalTable: "OperationDocuments",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ProviderDocumentProfiles",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    ProviderName = table.Column<string>(type: "TEXT", nullable: false),
                    ProviderKey = table.Column<string>(type: "TEXT", nullable: false),
                    DocumentTypeCode = table.Column<string>(type: "TEXT", nullable: false),
                    Version = table.Column<int>(type: "INTEGER", nullable: false),
                    Status = table.Column<string>(type: "TEXT", nullable: false),
                    LayoutFingerprint = table.Column<string>(type: "TEXT", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProviderDocumentProfiles", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "UnitAliases",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    Alias = table.Column<string>(type: "TEXT", nullable: false),
                    CanonicalUnit = table.Column<string>(type: "TEXT", nullable: false),
                    Language = table.Column<string>(type: "TEXT", nullable: false),
                    ConversionFactor = table.Column<decimal>(type: "TEXT", precision: 18, scale: 9, nullable: true),
                    ConversionBaseUnit = table.Column<string>(type: "TEXT", nullable: true),
                    RequiresVerifiedFactor = table.Column<bool>(type: "INTEGER", nullable: false),
                    IsActive = table.Column<bool>(type: "INTEGER", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UnitAliases", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "FieldAliases",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    CanonicalFieldDefinitionId = table.Column<Guid>(type: "TEXT", nullable: false),
                    CanonicalFieldCode = table.Column<string>(type: "TEXT", nullable: false),
                    Alias = table.Column<string>(type: "TEXT", nullable: false),
                    Language = table.Column<string>(type: "TEXT", nullable: false),
                    DocumentTypeCode = table.Column<string>(type: "TEXT", nullable: true),
                    ProviderKey = table.Column<string>(type: "TEXT", nullable: true),
                    Priority = table.Column<int>(type: "INTEGER", nullable: false),
                    IsActive = table.Column<bool>(type: "INTEGER", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FieldAliases", x => x.Id);
                    table.ForeignKey(
                        name: "FK_FieldAliases_CanonicalFieldDefinitions_CanonicalFieldDefinitionId",
                        column: x => x.CanonicalFieldDefinitionId,
                        principalTable: "CanonicalFieldDefinitions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ApprovedFieldMappings",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    ProviderProfileId = table.Column<Guid>(type: "TEXT", nullable: false),
                    CanonicalFieldCode = table.Column<string>(type: "TEXT", nullable: false),
                    OriginalLabel = table.Column<string>(type: "TEXT", nullable: false),
                    ContextFingerprint = table.Column<string>(type: "TEXT", nullable: false),
                    Section = table.Column<string>(type: "TEXT", nullable: true),
                    DocumentTypeCode = table.Column<string>(type: "TEXT", nullable: false),
                    ApprovedBy = table.Column<Guid>(type: "TEXT", nullable: false),
                    ApprovedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    Evidence = table.Column<string>(type: "TEXT", nullable: false),
                    Notes = table.Column<string>(type: "TEXT", nullable: true),
                    IsActive = table.Column<bool>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ApprovedFieldMappings", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ApprovedFieldMappings_ProviderDocumentProfiles_ProviderProfileId",
                        column: x => x.ProviderProfileId,
                        principalTable: "ProviderDocumentProfiles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_OperationDocuments_TradeOperationId_IntelligenceStatus",
                table: "OperationDocuments",
                columns: new[] { "TradeOperationId", "IntelligenceStatus" });

            migrationBuilder.CreateIndex(
                name: "IX_DocumentFields_OperationDocumentId_CanonicalFieldCode_AnalysisVersion",
                table: "DocumentFields",
                columns: new[] { "OperationDocumentId", "CanonicalFieldCode", "AnalysisVersion" });

            migrationBuilder.CreateIndex(
                name: "IX_DocumentFields_ProviderProfileId",
                table: "DocumentFields",
                column: "ProviderProfileId");

            migrationBuilder.CreateIndex(
                name: "IX_ApprovedFieldMappings_ProviderProfileId_OriginalLabel_ContextFingerprint_CanonicalFieldCode",
                table: "ApprovedFieldMappings",
                columns: new[] { "ProviderProfileId", "OriginalLabel", "ContextFingerprint", "CanonicalFieldCode" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_CanonicalFieldDefinitions_Code",
                table: "CanonicalFieldDefinitions",
                column: "Code",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_DocumentFieldCorrections_DocumentFieldId_CorrectedAt",
                table: "DocumentFieldCorrections",
                columns: new[] { "DocumentFieldId", "CorrectedAt" });

            migrationBuilder.CreateIndex(
                name: "IX_DocumentIntelligenceAnalyses_OperationDocumentId_Version",
                table: "DocumentIntelligenceAnalyses",
                columns: new[] { "OperationDocumentId", "Version" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_DocumentParticipants_OperationDocumentId_Role_AnalysisVersion",
                table: "DocumentParticipants",
                columns: new[] { "OperationDocumentId", "Role", "AnalysisVersion" });

            migrationBuilder.CreateIndex(
                name: "IX_DocumentRelationshipCandidates_SourceDocumentId_TargetDocumentId_RelationshipType",
                table: "DocumentRelationshipCandidates",
                columns: new[] { "SourceDocumentId", "TargetDocumentId", "RelationshipType" });

            migrationBuilder.CreateIndex(
                name: "IX_DocumentRelationshipCandidates_TargetDocumentId",
                table: "DocumentRelationshipCandidates",
                column: "TargetDocumentId");

            migrationBuilder.CreateIndex(
                name: "IX_FieldAliases_CanonicalFieldCode_Alias_Language_DocumentTypeCode_ProviderKey",
                table: "FieldAliases",
                columns: new[] { "CanonicalFieldCode", "Alias", "Language", "DocumentTypeCode", "ProviderKey" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_FieldAliases_CanonicalFieldDefinitionId",
                table: "FieldAliases",
                column: "CanonicalFieldDefinitionId");

            migrationBuilder.CreateIndex(
                name: "IX_ProviderDocumentProfiles_ProviderKey_DocumentTypeCode_Version",
                table: "ProviderDocumentProfiles",
                columns: new[] { "ProviderKey", "DocumentTypeCode", "Version" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_UnitAliases_Alias_Language",
                table: "UnitAliases",
                columns: new[] { "Alias", "Language" },
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_DocumentFields_ProviderDocumentProfiles_ProviderProfileId",
                table: "DocumentFields",
                column: "ProviderProfileId",
                principalTable: "ProviderDocumentProfiles",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_DocumentFields_ProviderDocumentProfiles_ProviderProfileId",
                table: "DocumentFields");

            migrationBuilder.DropTable(
                name: "ApprovedFieldMappings");

            migrationBuilder.DropTable(
                name: "DocumentFieldCorrections");

            migrationBuilder.DropTable(
                name: "DocumentIntelligenceAnalyses");

            migrationBuilder.DropTable(
                name: "DocumentParticipants");

            migrationBuilder.DropTable(
                name: "DocumentRelationshipCandidates");

            migrationBuilder.DropTable(
                name: "FieldAliases");

            migrationBuilder.DropTable(
                name: "UnitAliases");

            migrationBuilder.DropTable(
                name: "ProviderDocumentProfiles");

            migrationBuilder.DropTable(
                name: "CanonicalFieldDefinitions");

            migrationBuilder.DropIndex(
                name: "IX_OperationDocuments_TradeOperationId_IntelligenceStatus",
                table: "OperationDocuments");

            migrationBuilder.DropIndex(
                name: "IX_DocumentFields_OperationDocumentId_CanonicalFieldCode_AnalysisVersion",
                table: "DocumentFields");

            migrationBuilder.DropIndex(
                name: "IX_DocumentFields_ProviderProfileId",
                table: "DocumentFields");

            migrationBuilder.DropColumn(
                name: "ClassificationConfidenceLevel",
                table: "OperationDocuments");

            migrationBuilder.DropColumn(
                name: "ClassificationEvidenceJson",
                table: "OperationDocuments");

            migrationBuilder.DropColumn(
                name: "DetectedLanguage",
                table: "OperationDocuments");

            migrationBuilder.DropColumn(
                name: "IntelligenceStatus",
                table: "OperationDocuments");

            migrationBuilder.DropColumn(
                name: "IntelligenceVersion",
                table: "OperationDocuments");

            migrationBuilder.DropColumn(
                name: "LastAnalyzedAt",
                table: "OperationDocuments");

            migrationBuilder.DropColumn(
                name: "LayoutFingerprint",
                table: "OperationDocuments");

            migrationBuilder.DropColumn(
                name: "OcrUsed",
                table: "OperationDocuments");

            migrationBuilder.DropColumn(
                name: "Cartons",
                table: "DocumentLineItems");

            migrationBuilder.DropColumn(
                name: "ConfidenceLevel",
                table: "DocumentLineItems");

            migrationBuilder.DropColumn(
                name: "DimensionUnit",
                table: "DocumentLineItems");

            migrationBuilder.DropColumn(
                name: "Height",
                table: "DocumentLineItems");

            migrationBuilder.DropColumn(
                name: "Length",
                table: "DocumentLineItems");

            migrationBuilder.DropColumn(
                name: "LotNumber",
                table: "DocumentLineItems");

            migrationBuilder.DropColumn(
                name: "Model",
                table: "DocumentLineItems");

            migrationBuilder.DropColumn(
                name: "PageNumber",
                table: "DocumentLineItems");

            migrationBuilder.DropColumn(
                name: "Pallets",
                table: "DocumentLineItems");

            migrationBuilder.DropColumn(
                name: "PiecesPerCarton",
                table: "DocumentLineItems");

            migrationBuilder.DropColumn(
                name: "PoNumber",
                table: "DocumentLineItems");

            migrationBuilder.DropColumn(
                name: "RecognitionMethod",
                table: "DocumentLineItems");

            migrationBuilder.DropColumn(
                name: "RegionJson",
                table: "DocumentLineItems");

            migrationBuilder.DropColumn(
                name: "SerialNumber",
                table: "DocumentLineItems");

            migrationBuilder.DropColumn(
                name: "SoNumber",
                table: "DocumentLineItems");

            migrationBuilder.DropColumn(
                name: "SourceKind",
                table: "DocumentLineItems");

            migrationBuilder.DropColumn(
                name: "Submodel",
                table: "DocumentLineItems");

            migrationBuilder.DropColumn(
                name: "VolumeCbm",
                table: "DocumentLineItems");

            migrationBuilder.DropColumn(
                name: "Width",
                table: "DocumentLineItems");

            migrationBuilder.DropColumn(
                name: "AnalysisVersion",
                table: "DocumentFields");

            migrationBuilder.DropColumn(
                name: "CanonicalFieldCode",
                table: "DocumentFields");

            migrationBuilder.DropColumn(
                name: "ColumnIndex",
                table: "DocumentFields");

            migrationBuilder.DropColumn(
                name: "ConfidenceLevel",
                table: "DocumentFields");

            migrationBuilder.DropColumn(
                name: "Formula",
                table: "DocumentFields");

            migrationBuilder.DropColumn(
                name: "OriginalLabel",
                table: "DocumentFields");

            migrationBuilder.DropColumn(
                name: "OriginalLanguage",
                table: "DocumentFields");

            migrationBuilder.DropColumn(
                name: "OriginalText",
                table: "DocumentFields");

            migrationBuilder.DropColumn(
                name: "PageNumber",
                table: "DocumentFields");

            migrationBuilder.DropColumn(
                name: "ProviderProfileId",
                table: "DocumentFields");

            migrationBuilder.DropColumn(
                name: "RecognitionMethod",
                table: "DocumentFields");

            migrationBuilder.DropColumn(
                name: "RecognitionReason",
                table: "DocumentFields");

            migrationBuilder.DropColumn(
                name: "RegionJson",
                table: "DocumentFields");

            migrationBuilder.DropColumn(
                name: "RequiresReview",
                table: "DocumentFields");

            migrationBuilder.DropColumn(
                name: "RowIndex",
                table: "DocumentFields");

            migrationBuilder.DropColumn(
                name: "Section",
                table: "DocumentFields");

            migrationBuilder.DropColumn(
                name: "SourceKind",
                table: "DocumentFields");

            migrationBuilder.DropColumn(
                name: "TableIndex",
                table: "DocumentFields");
        }
    }
}
