using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Sace.Api.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class CustomsValuationEngineV1 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ContributionCalculations",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    TradeOperationId = table.Column<Guid>(type: "TEXT", nullable: false),
                    AuditId = table.Column<Guid>(type: "TEXT", nullable: true),
                    Version = table.Column<int>(type: "INTEGER", nullable: false),
                    Status = table.Column<string>(type: "TEXT", nullable: false),
                    CalculatedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    Explanation = table.Column<string>(type: "TEXT", nullable: false),
                    SnapshotJson = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ContributionCalculations", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ContributionCalculations_Audits_AuditId",
                        column: x => x.AuditId,
                        principalTable: "Audits",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_ContributionCalculations_TradeOperations_TradeOperationId",
                        column: x => x.TradeOperationId,
                        principalTable: "TradeOperations",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "CustomsValuationCalculations",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    TradeOperationId = table.Column<Guid>(type: "TEXT", nullable: false),
                    AuditId = table.Column<Guid>(type: "TEXT", nullable: true),
                    CalculationVersion = table.Column<int>(type: "INTEGER", nullable: false),
                    TransactionValue = table.Column<decimal>(type: "TEXT", precision: 18, scale: 6, nullable: true),
                    Currency = table.Column<string>(type: "TEXT", nullable: true),
                    ExchangeRate = table.Column<decimal>(type: "TEXT", precision: 18, scale: 6, nullable: true),
                    ExchangeRateSource = table.Column<string>(type: "TEXT", nullable: true),
                    TransactionValueConverted = table.Column<decimal>(type: "TEXT", precision: 18, scale: 6, nullable: true),
                    FreightTotal = table.Column<decimal>(type: "TEXT", precision: 18, scale: 6, nullable: false),
                    InsuranceTotal = table.Column<decimal>(type: "TEXT", precision: 18, scale: 6, nullable: false),
                    PackagingTotal = table.Column<decimal>(type: "TEXT", precision: 18, scale: 6, nullable: false),
                    OtherIncrementablesTotal = table.Column<decimal>(type: "TEXT", precision: 18, scale: 6, nullable: false),
                    IncrementablesTotal = table.Column<decimal>(type: "TEXT", precision: 18, scale: 6, nullable: false),
                    DeclaredCustomsValue = table.Column<decimal>(type: "TEXT", precision: 18, scale: 6, nullable: true),
                    CalculatedCustomsValue = table.Column<decimal>(type: "TEXT", precision: 18, scale: 6, nullable: true),
                    DifferenceAmount = table.Column<decimal>(type: "TEXT", precision: 18, scale: 6, nullable: true),
                    DifferencePercent = table.Column<decimal>(type: "TEXT", precision: 18, scale: 6, nullable: true),
                    AbsoluteToleranceUsed = table.Column<decimal>(type: "TEXT", precision: 18, scale: 6, nullable: false),
                    PercentToleranceUsed = table.Column<decimal>(type: "TEXT", precision: 18, scale: 6, nullable: false),
                    Outcome = table.Column<string>(type: "TEXT", nullable: false),
                    Formula = table.Column<string>(type: "TEXT", nullable: false),
                    Explanation = table.Column<string>(type: "TEXT", nullable: false),
                    SnapshotJson = table.Column<string>(type: "TEXT", nullable: false),
                    CalculatedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    IsManualReviewRequired = table.Column<bool>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CustomsValuationCalculations", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CustomsValuationCalculations_Audits_AuditId",
                        column: x => x.AuditId,
                        principalTable: "Audits",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_CustomsValuationCalculations_TradeOperations_TradeOperationId",
                        column: x => x.TradeOperationId,
                        principalTable: "TradeOperations",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ContributionLines",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    ContributionCalculationId = table.Column<Guid>(type: "TEXT", nullable: false),
                    RawContributionCode = table.Column<string>(type: "TEXT", nullable: false),
                    RawDescription = table.Column<string>(type: "TEXT", nullable: true),
                    RawRate = table.Column<string>(type: "TEXT", nullable: true),
                    RawAmount = table.Column<string>(type: "TEXT", nullable: true),
                    ContributionCode = table.Column<string>(type: "TEXT", nullable: false),
                    Description = table.Column<string>(type: "TEXT", nullable: false),
                    TaxBase = table.Column<decimal>(type: "TEXT", precision: 18, scale: 6, nullable: true),
                    Rate = table.Column<decimal>(type: "TEXT", precision: 18, scale: 6, nullable: true),
                    DeclaredAmount = table.Column<decimal>(type: "TEXT", precision: 18, scale: 6, nullable: true),
                    CalculatedAmount = table.Column<decimal>(type: "TEXT", precision: 18, scale: 6, nullable: true),
                    PaidAmount = table.Column<decimal>(type: "TEXT", precision: 18, scale: 6, nullable: true),
                    DifferenceDeclaredVsCalculated = table.Column<decimal>(type: "TEXT", precision: 18, scale: 6, nullable: true),
                    DifferenceDeclaredVsPaid = table.Column<decimal>(type: "TEXT", precision: 18, scale: 6, nullable: true),
                    Currency = table.Column<string>(type: "TEXT", nullable: false),
                    Outcome = table.Column<string>(type: "TEXT", nullable: false),
                    Explanation = table.Column<string>(type: "TEXT", nullable: false),
                    SourceDocumentId = table.Column<Guid>(type: "TEXT", nullable: true),
                    SourceFieldId = table.Column<Guid>(type: "TEXT", nullable: true),
                    PaymentDocumentId = table.Column<Guid>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ContributionLines", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ContributionLines_ContributionCalculations_ContributionCalculationId",
                        column: x => x.ContributionCalculationId,
                        principalTable: "ContributionCalculations",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ContributionLines_DocumentFields_SourceFieldId",
                        column: x => x.SourceFieldId,
                        principalTable: "DocumentFields",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_ContributionLines_OperationDocuments_PaymentDocumentId",
                        column: x => x.PaymentDocumentId,
                        principalTable: "OperationDocuments",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_ContributionLines_OperationDocuments_SourceDocumentId",
                        column: x => x.SourceDocumentId,
                        principalTable: "OperationDocuments",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateTable(
                name: "CustomsValuationComponents",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    CustomsValuationCalculationId = table.Column<Guid>(type: "TEXT", nullable: false),
                    ComponentType = table.Column<string>(type: "TEXT", nullable: false),
                    EconomicKey = table.Column<string>(type: "TEXT", nullable: false),
                    PrimaryDocumentId = table.Column<Guid>(type: "TEXT", nullable: true),
                    PrimaryDocumentFieldId = table.Column<Guid>(type: "TEXT", nullable: true),
                    RawAmount = table.Column<string>(type: "TEXT", nullable: true),
                    NormalizedAmount = table.Column<decimal>(type: "TEXT", precision: 18, scale: 6, nullable: true),
                    Currency = table.Column<string>(type: "TEXT", nullable: true),
                    ExchangeRate = table.Column<decimal>(type: "TEXT", precision: 18, scale: 6, nullable: true),
                    ExchangeRateSource = table.Column<string>(type: "TEXT", nullable: true),
                    ConvertedAmount = table.Column<decimal>(type: "TEXT", precision: 18, scale: 6, nullable: true),
                    Treatment = table.Column<string>(type: "TEXT", nullable: false),
                    Confidence = table.Column<double>(type: "REAL", nullable: true),
                    EvidenceDescription = table.Column<string>(type: "TEXT", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CustomsValuationComponents", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CustomsValuationComponents_CustomsValuationCalculations_CustomsValuationCalculationId",
                        column: x => x.CustomsValuationCalculationId,
                        principalTable: "CustomsValuationCalculations",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CustomsValuationComponents_DocumentFields_PrimaryDocumentFieldId",
                        column: x => x.PrimaryDocumentFieldId,
                        principalTable: "DocumentFields",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_CustomsValuationComponents_OperationDocuments_PrimaryDocumentId",
                        column: x => x.PrimaryDocumentId,
                        principalTable: "OperationDocuments",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateTable(
                name: "ValuationComponentEvidence",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    ValuationComponentId = table.Column<Guid>(type: "TEXT", nullable: false),
                    OperationDocumentId = table.Column<Guid>(type: "TEXT", nullable: false),
                    DocumentFieldId = table.Column<Guid>(type: "TEXT", nullable: true),
                    EvidenceRole = table.Column<string>(type: "TEXT", nullable: false),
                    Confidence = table.Column<double>(type: "REAL", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ValuationComponentEvidence", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ValuationComponentEvidence_CustomsValuationComponents_ValuationComponentId",
                        column: x => x.ValuationComponentId,
                        principalTable: "CustomsValuationComponents",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ValuationComponentEvidence_DocumentFields_DocumentFieldId",
                        column: x => x.DocumentFieldId,
                        principalTable: "DocumentFields",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_ValuationComponentEvidence_OperationDocuments_OperationDocumentId",
                        column: x => x.OperationDocumentId,
                        principalTable: "OperationDocuments",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ContributionCalculations_AuditId",
                table: "ContributionCalculations",
                column: "AuditId");

            migrationBuilder.CreateIndex(
                name: "IX_ContributionCalculations_TradeOperationId",
                table: "ContributionCalculations",
                column: "TradeOperationId");

            migrationBuilder.CreateIndex(
                name: "IX_ContributionCalculations_TradeOperationId_Version",
                table: "ContributionCalculations",
                columns: new[] { "TradeOperationId", "Version" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ContributionLines_ContributionCalculationId",
                table: "ContributionLines",
                column: "ContributionCalculationId");

            migrationBuilder.CreateIndex(
                name: "IX_ContributionLines_ContributionCode",
                table: "ContributionLines",
                column: "ContributionCode");

            migrationBuilder.CreateIndex(
                name: "IX_ContributionLines_PaymentDocumentId",
                table: "ContributionLines",
                column: "PaymentDocumentId");

            migrationBuilder.CreateIndex(
                name: "IX_ContributionLines_SourceDocumentId",
                table: "ContributionLines",
                column: "SourceDocumentId");

            migrationBuilder.CreateIndex(
                name: "IX_ContributionLines_SourceFieldId",
                table: "ContributionLines",
                column: "SourceFieldId");

            migrationBuilder.CreateIndex(
                name: "IX_CustomsValuationCalculations_AuditId",
                table: "CustomsValuationCalculations",
                column: "AuditId");

            migrationBuilder.CreateIndex(
                name: "IX_CustomsValuationCalculations_TradeOperationId",
                table: "CustomsValuationCalculations",
                column: "TradeOperationId");

            migrationBuilder.CreateIndex(
                name: "IX_CustomsValuationCalculations_TradeOperationId_CalculationVersion",
                table: "CustomsValuationCalculations",
                columns: new[] { "TradeOperationId", "CalculationVersion" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_CustomsValuationComponents_ComponentType",
                table: "CustomsValuationComponents",
                column: "ComponentType");

            migrationBuilder.CreateIndex(
                name: "IX_CustomsValuationComponents_CustomsValuationCalculationId",
                table: "CustomsValuationComponents",
                column: "CustomsValuationCalculationId");

            migrationBuilder.CreateIndex(
                name: "IX_CustomsValuationComponents_CustomsValuationCalculationId_EconomicKey",
                table: "CustomsValuationComponents",
                columns: new[] { "CustomsValuationCalculationId", "EconomicKey" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_CustomsValuationComponents_PrimaryDocumentFieldId",
                table: "CustomsValuationComponents",
                column: "PrimaryDocumentFieldId");

            migrationBuilder.CreateIndex(
                name: "IX_CustomsValuationComponents_PrimaryDocumentId",
                table: "CustomsValuationComponents",
                column: "PrimaryDocumentId");

            migrationBuilder.CreateIndex(
                name: "IX_ValuationComponentEvidence_DocumentFieldId",
                table: "ValuationComponentEvidence",
                column: "DocumentFieldId");

            migrationBuilder.CreateIndex(
                name: "IX_ValuationComponentEvidence_OperationDocumentId",
                table: "ValuationComponentEvidence",
                column: "OperationDocumentId");

            migrationBuilder.CreateIndex(
                name: "IX_ValuationComponentEvidence_ValuationComponentId",
                table: "ValuationComponentEvidence",
                column: "ValuationComponentId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ContributionLines");

            migrationBuilder.DropTable(
                name: "ValuationComponentEvidence");

            migrationBuilder.DropTable(
                name: "ContributionCalculations");

            migrationBuilder.DropTable(
                name: "CustomsValuationComponents");

            migrationBuilder.DropTable(
                name: "CustomsValuationCalculations");
        }
    }
}
