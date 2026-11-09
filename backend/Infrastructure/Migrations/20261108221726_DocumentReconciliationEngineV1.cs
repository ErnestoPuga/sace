using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Sace.Api.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class DocumentReconciliationEngineV1 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "DocumentLineItems",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    OperationDocumentId = table.Column<Guid>(type: "TEXT", nullable: false),
                    Sequence = table.Column<int>(type: "INTEGER", nullable: false),
                    ItemNumber = table.Column<string>(type: "TEXT", nullable: true),
                    PartNumber = table.Column<string>(type: "TEXT", nullable: true),
                    Sku = table.Column<string>(type: "TEXT", nullable: true),
                    Description = table.Column<string>(type: "TEXT", nullable: true),
                    Quantity = table.Column<decimal>(type: "TEXT", precision: 18, scale: 6, nullable: true),
                    Unit = table.Column<string>(type: "TEXT", nullable: true),
                    UnitValue = table.Column<decimal>(type: "TEXT", precision: 18, scale: 6, nullable: true),
                    TotalValue = table.Column<decimal>(type: "TEXT", precision: 18, scale: 6, nullable: true),
                    Currency = table.Column<string>(type: "TEXT", nullable: true),
                    TariffFraction = table.Column<string>(type: "TEXT", nullable: true),
                    Nico = table.Column<string>(type: "TEXT", nullable: true),
                    GrossWeight = table.Column<decimal>(type: "TEXT", precision: 18, scale: 6, nullable: true),
                    NetWeight = table.Column<decimal>(type: "TEXT", precision: 18, scale: 6, nullable: true),
                    CountryOfOrigin = table.Column<string>(type: "TEXT", nullable: true),
                    RawLocator = table.Column<string>(type: "TEXT", nullable: true),
                    ExtractionMethod = table.Column<string>(type: "TEXT", nullable: false),
                    Confidence = table.Column<double>(type: "REAL", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DocumentLineItems", x => x.Id);
                    table.ForeignKey(
                        name: "FK_DocumentLineItems_OperationDocuments_OperationDocumentId",
                        column: x => x.OperationDocumentId,
                        principalTable: "OperationDocuments",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "DocumentReconciliationProfiles",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    Code = table.Column<string>(type: "TEXT", nullable: false),
                    Version = table.Column<int>(type: "INTEGER", nullable: false),
                    Name = table.Column<string>(type: "TEXT", nullable: false),
                    LeftDocumentTypeCode = table.Column<string>(type: "TEXT", nullable: false),
                    RightDocumentTypeCode = table.Column<string>(type: "TEXT", nullable: false),
                    Status = table.Column<string>(type: "TEXT", nullable: false),
                    EffectiveFrom = table.Column<DateTime>(type: "TEXT", nullable: true),
                    EffectiveTo = table.Column<DateTime>(type: "TEXT", nullable: true),
                    Description = table.Column<string>(type: "TEXT", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DocumentReconciliationProfiles", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "DocumentReconciliationProfileFields",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    ProfileId = table.Column<Guid>(type: "TEXT", nullable: false),
                    Section = table.Column<string>(type: "TEXT", nullable: false),
                    LeftFieldCode = table.Column<string>(type: "TEXT", nullable: false),
                    RightFieldCode = table.Column<string>(type: "TEXT", nullable: false),
                    ComparisonType = table.Column<string>(type: "TEXT", nullable: false),
                    RequiredForCoverage = table.Column<bool>(type: "INTEGER", nullable: false),
                    Sequence = table.Column<int>(type: "INTEGER", nullable: false),
                    Weight = table.Column<decimal>(type: "TEXT", precision: 18, scale: 6, nullable: true),
                    ToleranceType = table.Column<string>(type: "TEXT", nullable: true),
                    ToleranceValue = table.Column<decimal>(type: "TEXT", precision: 18, scale: 6, nullable: true),
                    LineItemScope = table.Column<string>(type: "TEXT", nullable: false),
                    IsActive = table.Column<bool>(type: "INTEGER", nullable: false),
                    CrossValidationRuleId = table.Column<Guid>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DocumentReconciliationProfileFields", x => x.Id);
                    table.ForeignKey(
                        name: "FK_DocumentReconciliationProfileFields_CrossValidationRules_CrossValidationRuleId",
                        column: x => x.CrossValidationRuleId,
                        principalTable: "CrossValidationRules",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_DocumentReconciliationProfileFields_DocumentReconciliationProfiles_ProfileId",
                        column: x => x.ProfileId,
                        principalTable: "DocumentReconciliationProfiles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "DocumentReconciliationResults",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    TradeOperationId = table.Column<Guid>(type: "TEXT", nullable: false),
                    ProfileId = table.Column<Guid>(type: "TEXT", nullable: false),
                    ProfileCode = table.Column<string>(type: "TEXT", nullable: false),
                    ProfileVersion = table.Column<int>(type: "INTEGER", nullable: false),
                    LeftDocumentId = table.Column<Guid>(type: "TEXT", nullable: true),
                    RightDocumentId = table.Column<Guid>(type: "TEXT", nullable: true),
                    Outcome = table.Column<string>(type: "TEXT", nullable: false),
                    ComparableFieldCount = table.Column<int>(type: "INTEGER", nullable: false),
                    MatchCount = table.Column<int>(type: "INTEGER", nullable: false),
                    MismatchCount = table.Column<int>(type: "INTEGER", nullable: false),
                    InsufficientEvidenceCount = table.Column<int>(type: "INTEGER", nullable: false),
                    ManualReviewCount = table.Column<int>(type: "INTEGER", nullable: false),
                    NotApplicableCount = table.Column<int>(type: "INTEGER", nullable: false),
                    LineItemsLeft = table.Column<int>(type: "INTEGER", nullable: false),
                    LineItemsRight = table.Column<int>(type: "INTEGER", nullable: false),
                    LineItemsMatched = table.Column<int>(type: "INTEGER", nullable: false),
                    LineItemsUnmatched = table.Column<int>(type: "INTEGER", nullable: false),
                    CoveragePercent = table.Column<decimal>(type: "TEXT", precision: 8, scale: 4, nullable: true),
                    EvaluatedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    Explanation = table.Column<string>(type: "TEXT", nullable: false),
                    SnapshotJson = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DocumentReconciliationResults", x => x.Id);
                    table.ForeignKey(
                        name: "FK_DocumentReconciliationResults_DocumentReconciliationProfiles_ProfileId",
                        column: x => x.ProfileId,
                        principalTable: "DocumentReconciliationProfiles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_DocumentReconciliationResults_OperationDocuments_LeftDocumentId",
                        column: x => x.LeftDocumentId,
                        principalTable: "OperationDocuments",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_DocumentReconciliationResults_OperationDocuments_RightDocumentId",
                        column: x => x.RightDocumentId,
                        principalTable: "OperationDocuments",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_DocumentReconciliationResults_TradeOperations_TradeOperationId",
                        column: x => x.TradeOperationId,
                        principalTable: "TradeOperations",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "DocumentLineItemMatches",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    ReconciliationResultId = table.Column<Guid>(type: "TEXT", nullable: false),
                    LeftLineItemId = table.Column<Guid>(type: "TEXT", nullable: true),
                    RightLineItemId = table.Column<Guid>(type: "TEXT", nullable: true),
                    MatchMethod = table.Column<string>(type: "TEXT", nullable: false),
                    MatchKey = table.Column<string>(type: "TEXT", nullable: true),
                    Status = table.Column<string>(type: "TEXT", nullable: false),
                    Explanation = table.Column<string>(type: "TEXT", nullable: false),
                    LeftItemSnapshot = table.Column<string>(type: "TEXT", nullable: true),
                    RightItemSnapshot = table.Column<string>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DocumentLineItemMatches", x => x.Id);
                    table.ForeignKey(
                        name: "FK_DocumentLineItemMatches_DocumentLineItems_LeftLineItemId",
                        column: x => x.LeftLineItemId,
                        principalTable: "DocumentLineItems",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_DocumentLineItemMatches_DocumentLineItems_RightLineItemId",
                        column: x => x.RightLineItemId,
                        principalTable: "DocumentLineItems",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_DocumentLineItemMatches_DocumentReconciliationResults_ReconciliationResultId",
                        column: x => x.ReconciliationResultId,
                        principalTable: "DocumentReconciliationResults",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "DocumentReconciliationFieldResults",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    ReconciliationResultId = table.Column<Guid>(type: "TEXT", nullable: false),
                    ProfileFieldId = table.Column<Guid>(type: "TEXT", nullable: false),
                    CrossValidationResultId = table.Column<Guid>(type: "TEXT", nullable: true),
                    LeftLineItemId = table.Column<Guid>(type: "TEXT", nullable: true),
                    RightLineItemId = table.Column<Guid>(type: "TEXT", nullable: true),
                    Section = table.Column<string>(type: "TEXT", nullable: false),
                    LeftFieldCode = table.Column<string>(type: "TEXT", nullable: false),
                    RightFieldCode = table.Column<string>(type: "TEXT", nullable: false),
                    LeftRawValue = table.Column<string>(type: "TEXT", nullable: true),
                    RightRawValue = table.Column<string>(type: "TEXT", nullable: true),
                    LeftNormalizedValue = table.Column<string>(type: "TEXT", nullable: true),
                    RightNormalizedValue = table.Column<string>(type: "TEXT", nullable: true),
                    LeftExtractionMethod = table.Column<string>(type: "TEXT", nullable: true),
                    RightExtractionMethod = table.Column<string>(type: "TEXT", nullable: true),
                    Outcome = table.Column<string>(type: "TEXT", nullable: false),
                    NumericDifference = table.Column<decimal>(type: "TEXT", precision: 18, scale: 6, nullable: true),
                    PercentDifference = table.Column<decimal>(type: "TEXT", precision: 18, scale: 6, nullable: true),
                    Tolerance = table.Column<string>(type: "TEXT", nullable: true),
                    RuleCode = table.Column<string>(type: "TEXT", nullable: true),
                    EvidenceSource = table.Column<string>(type: "TEXT", nullable: false),
                    Explanation = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DocumentReconciliationFieldResults", x => x.Id);
                    table.ForeignKey(
                        name: "FK_DocumentReconciliationFieldResults_CrossValidationResults_CrossValidationResultId",
                        column: x => x.CrossValidationResultId,
                        principalTable: "CrossValidationResults",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_DocumentReconciliationFieldResults_DocumentLineItems_LeftLineItemId",
                        column: x => x.LeftLineItemId,
                        principalTable: "DocumentLineItems",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_DocumentReconciliationFieldResults_DocumentLineItems_RightLineItemId",
                        column: x => x.RightLineItemId,
                        principalTable: "DocumentLineItems",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_DocumentReconciliationFieldResults_DocumentReconciliationProfileFields_ProfileFieldId",
                        column: x => x.ProfileFieldId,
                        principalTable: "DocumentReconciliationProfileFields",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_DocumentReconciliationFieldResults_DocumentReconciliationResults_ReconciliationResultId",
                        column: x => x.ReconciliationResultId,
                        principalTable: "DocumentReconciliationResults",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_DocumentLineItemMatches_LeftLineItemId",
                table: "DocumentLineItemMatches",
                column: "LeftLineItemId");

            migrationBuilder.CreateIndex(
                name: "IX_DocumentLineItemMatches_ReconciliationResultId",
                table: "DocumentLineItemMatches",
                column: "ReconciliationResultId");

            migrationBuilder.CreateIndex(
                name: "IX_DocumentLineItemMatches_RightLineItemId",
                table: "DocumentLineItemMatches",
                column: "RightLineItemId");

            migrationBuilder.CreateIndex(
                name: "IX_DocumentLineItems_OperationDocumentId_PartNumber",
                table: "DocumentLineItems",
                columns: new[] { "OperationDocumentId", "PartNumber" });

            migrationBuilder.CreateIndex(
                name: "IX_DocumentLineItems_OperationDocumentId_Sequence",
                table: "DocumentLineItems",
                columns: new[] { "OperationDocumentId", "Sequence" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_DocumentReconciliationFieldResults_CrossValidationResultId",
                table: "DocumentReconciliationFieldResults",
                column: "CrossValidationResultId");

            migrationBuilder.CreateIndex(
                name: "IX_DocumentReconciliationFieldResults_LeftLineItemId",
                table: "DocumentReconciliationFieldResults",
                column: "LeftLineItemId");

            migrationBuilder.CreateIndex(
                name: "IX_DocumentReconciliationFieldResults_ProfileFieldId",
                table: "DocumentReconciliationFieldResults",
                column: "ProfileFieldId");

            migrationBuilder.CreateIndex(
                name: "IX_DocumentReconciliationFieldResults_ReconciliationResultId_Section",
                table: "DocumentReconciliationFieldResults",
                columns: new[] { "ReconciliationResultId", "Section" });

            migrationBuilder.CreateIndex(
                name: "IX_DocumentReconciliationFieldResults_RightLineItemId",
                table: "DocumentReconciliationFieldResults",
                column: "RightLineItemId");

            migrationBuilder.CreateIndex(
                name: "IX_DocumentReconciliationProfileFields_CrossValidationRuleId",
                table: "DocumentReconciliationProfileFields",
                column: "CrossValidationRuleId");

            migrationBuilder.CreateIndex(
                name: "IX_DocumentReconciliationProfileFields_ProfileId_LeftFieldCode_RightFieldCode_LineItemScope",
                table: "DocumentReconciliationProfileFields",
                columns: new[] { "ProfileId", "LeftFieldCode", "RightFieldCode", "LineItemScope" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_DocumentReconciliationProfileFields_ProfileId_Sequence",
                table: "DocumentReconciliationProfileFields",
                columns: new[] { "ProfileId", "Sequence" });

            migrationBuilder.CreateIndex(
                name: "IX_DocumentReconciliationProfiles_Code_Version",
                table: "DocumentReconciliationProfiles",
                columns: new[] { "Code", "Version" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_DocumentReconciliationProfiles_Status_EffectiveFrom_EffectiveTo",
                table: "DocumentReconciliationProfiles",
                columns: new[] { "Status", "EffectiveFrom", "EffectiveTo" });

            migrationBuilder.CreateIndex(
                name: "IX_DocumentReconciliationResults_LeftDocumentId",
                table: "DocumentReconciliationResults",
                column: "LeftDocumentId");

            migrationBuilder.CreateIndex(
                name: "IX_DocumentReconciliationResults_ProfileCode_ProfileVersion",
                table: "DocumentReconciliationResults",
                columns: new[] { "ProfileCode", "ProfileVersion" });

            migrationBuilder.CreateIndex(
                name: "IX_DocumentReconciliationResults_ProfileId",
                table: "DocumentReconciliationResults",
                column: "ProfileId");

            migrationBuilder.CreateIndex(
                name: "IX_DocumentReconciliationResults_RightDocumentId",
                table: "DocumentReconciliationResults",
                column: "RightDocumentId");

            migrationBuilder.CreateIndex(
                name: "IX_DocumentReconciliationResults_TradeOperationId_EvaluatedAt",
                table: "DocumentReconciliationResults",
                columns: new[] { "TradeOperationId", "EvaluatedAt" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "DocumentLineItemMatches");

            migrationBuilder.DropTable(
                name: "DocumentReconciliationFieldResults");

            migrationBuilder.DropTable(
                name: "DocumentLineItems");

            migrationBuilder.DropTable(
                name: "DocumentReconciliationProfileFields");

            migrationBuilder.DropTable(
                name: "DocumentReconciliationResults");

            migrationBuilder.DropTable(
                name: "DocumentReconciliationProfiles");
        }
    }
}
