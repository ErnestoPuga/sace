using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Sace.Api.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class CrossValidationEngineV1 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "CrossValidationRules",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    Code = table.Column<string>(type: "TEXT", nullable: false),
                    Version = table.Column<int>(type: "INTEGER", nullable: false),
                    Name = table.Column<string>(type: "TEXT", nullable: false),
                    Description = table.Column<string>(type: "TEXT", nullable: false),
                    Category = table.Column<string>(type: "TEXT", nullable: false),
                    SourceDocumentTypeCode = table.Column<string>(type: "TEXT", nullable: false),
                    SourceFieldCode = table.Column<string>(type: "TEXT", nullable: false),
                    TargetDocumentTypeCode = table.Column<string>(type: "TEXT", nullable: false),
                    TargetFieldCode = table.Column<string>(type: "TEXT", nullable: false),
                    ComparisonType = table.Column<string>(type: "TEXT", nullable: false),
                    AbsoluteTolerance = table.Column<decimal>(type: "TEXT", precision: 18, scale: 6, nullable: true),
                    PercentTolerance = table.Column<decimal>(type: "TEXT", precision: 18, scale: 6, nullable: true),
                    Severity = table.Column<string>(type: "TEXT", nullable: false),
                    MissingEvidenceBehavior = table.Column<string>(type: "TEXT", nullable: false),
                    IsActive = table.Column<bool>(type: "INTEGER", nullable: false),
                    EffectiveFrom = table.Column<DateTime>(type: "TEXT", nullable: true),
                    EffectiveTo = table.Column<DateTime>(type: "TEXT", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CrossValidationRules", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "DocumentFields",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    OperationDocumentId = table.Column<Guid>(type: "TEXT", nullable: false),
                    FieldCode = table.Column<string>(type: "TEXT", nullable: false),
                    RawValue = table.Column<string>(type: "TEXT", nullable: true),
                    NormalizedValue = table.Column<string>(type: "TEXT", nullable: true),
                    DataType = table.Column<string>(type: "TEXT", nullable: false),
                    Unit = table.Column<string>(type: "TEXT", nullable: true),
                    Currency = table.Column<string>(type: "TEXT", nullable: true),
                    Confidence = table.Column<double>(type: "REAL", nullable: true),
                    ExtractionMethod = table.Column<string>(type: "TEXT", nullable: false),
                    SourceLocator = table.Column<string>(type: "TEXT", nullable: true),
                    GroupKey = table.Column<string>(type: "TEXT", nullable: true),
                    Sequence = table.Column<int>(type: "INTEGER", nullable: true),
                    IsManuallyConfirmed = table.Column<bool>(type: "INTEGER", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DocumentFields", x => x.Id);
                    table.ForeignKey(
                        name: "FK_DocumentFields_OperationDocuments_OperationDocumentId",
                        column: x => x.OperationDocumentId,
                        principalTable: "OperationDocuments",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "CrossValidationResults",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    TradeOperationId = table.Column<Guid>(type: "TEXT", nullable: false),
                    AuditId = table.Column<Guid>(type: "TEXT", nullable: true),
                    CrossValidationRuleId = table.Column<Guid>(type: "TEXT", nullable: false),
                    RuleCode = table.Column<string>(type: "TEXT", nullable: false),
                    RuleVersion = table.Column<int>(type: "INTEGER", nullable: false),
                    SourceDocumentId = table.Column<Guid>(type: "TEXT", nullable: true),
                    TargetDocumentId = table.Column<Guid>(type: "TEXT", nullable: true),
                    SourceFieldId = table.Column<Guid>(type: "TEXT", nullable: true),
                    TargetFieldId = table.Column<Guid>(type: "TEXT", nullable: true),
                    SourceRawValue = table.Column<string>(type: "TEXT", nullable: true),
                    TargetRawValue = table.Column<string>(type: "TEXT", nullable: true),
                    SourceNormalizedValue = table.Column<string>(type: "TEXT", nullable: true),
                    TargetNormalizedValue = table.Column<string>(type: "TEXT", nullable: true),
                    Outcome = table.Column<string>(type: "TEXT", nullable: false),
                    NumericDifference = table.Column<decimal>(type: "TEXT", precision: 18, scale: 6, nullable: true),
                    PercentDifference = table.Column<decimal>(type: "TEXT", precision: 18, scale: 6, nullable: true),
                    Confidence = table.Column<double>(type: "REAL", nullable: true),
                    Explanation = table.Column<string>(type: "TEXT", nullable: false),
                    EvaluatedAt = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CrossValidationResults", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CrossValidationResults_Audits_AuditId",
                        column: x => x.AuditId,
                        principalTable: "Audits",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_CrossValidationResults_CrossValidationRules_CrossValidationRuleId",
                        column: x => x.CrossValidationRuleId,
                        principalTable: "CrossValidationRules",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_CrossValidationResults_DocumentFields_SourceFieldId",
                        column: x => x.SourceFieldId,
                        principalTable: "DocumentFields",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_CrossValidationResults_DocumentFields_TargetFieldId",
                        column: x => x.TargetFieldId,
                        principalTable: "DocumentFields",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_CrossValidationResults_OperationDocuments_SourceDocumentId",
                        column: x => x.SourceDocumentId,
                        principalTable: "OperationDocuments",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_CrossValidationResults_OperationDocuments_TargetDocumentId",
                        column: x => x.TargetDocumentId,
                        principalTable: "OperationDocuments",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_CrossValidationResults_TradeOperations_TradeOperationId",
                        column: x => x.TradeOperationId,
                        principalTable: "TradeOperations",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_CrossValidationResults_AuditId",
                table: "CrossValidationResults",
                column: "AuditId");

            migrationBuilder.CreateIndex(
                name: "IX_CrossValidationResults_CrossValidationRuleId",
                table: "CrossValidationResults",
                column: "CrossValidationRuleId");

            migrationBuilder.CreateIndex(
                name: "IX_CrossValidationResults_RuleCode",
                table: "CrossValidationResults",
                column: "RuleCode");

            migrationBuilder.CreateIndex(
                name: "IX_CrossValidationResults_SourceDocumentId",
                table: "CrossValidationResults",
                column: "SourceDocumentId");

            migrationBuilder.CreateIndex(
                name: "IX_CrossValidationResults_SourceFieldId",
                table: "CrossValidationResults",
                column: "SourceFieldId");

            migrationBuilder.CreateIndex(
                name: "IX_CrossValidationResults_TargetDocumentId",
                table: "CrossValidationResults",
                column: "TargetDocumentId");

            migrationBuilder.CreateIndex(
                name: "IX_CrossValidationResults_TargetFieldId",
                table: "CrossValidationResults",
                column: "TargetFieldId");

            migrationBuilder.CreateIndex(
                name: "IX_CrossValidationResults_TradeOperationId",
                table: "CrossValidationResults",
                column: "TradeOperationId");

            migrationBuilder.CreateIndex(
                name: "IX_CrossValidationResults_TradeOperationId_RuleCode_EvaluatedAt",
                table: "CrossValidationResults",
                columns: new[] { "TradeOperationId", "RuleCode", "EvaluatedAt" });

            migrationBuilder.CreateIndex(
                name: "IX_CrossValidationRules_Code_Version",
                table: "CrossValidationRules",
                columns: new[] { "Code", "Version" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_DocumentFields_OperationDocumentId_FieldCode",
                table: "DocumentFields",
                columns: new[] { "OperationDocumentId", "FieldCode" });

            migrationBuilder.CreateIndex(
                name: "IX_DocumentFields_OperationDocumentId_FieldCode_Sequence",
                table: "DocumentFields",
                columns: new[] { "OperationDocumentId", "FieldCode", "Sequence" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CrossValidationResults");

            migrationBuilder.DropTable(
                name: "CrossValidationRules");

            migrationBuilder.DropTable(
                name: "DocumentFields");
        }
    }
}
