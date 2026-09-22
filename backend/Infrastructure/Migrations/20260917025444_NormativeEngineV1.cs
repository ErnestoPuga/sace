using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Sace.Api.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class NormativeEngineV1 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<decimal>(
                name: "DeclaredCustomsValueMxn",
                table: "TradeOperations",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "ExchangeRate",
                table: "TradeOperations",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "GoodsDescription",
                table: "TradeOperations",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Incoterm",
                table: "TradeOperations",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Nico",
                table: "TradeOperations",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Annex",
                table: "NormativeRules",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Appendix",
                table: "NormativeRules",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Article",
                table: "NormativeRules",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "AutomationLevel",
                table: "NormativeRules",
                type: "TEXT",
                nullable: false,
                defaultValue: "AUTOMATIC");

            migrationBuilder.AddColumn<bool>(
                name: "IsIncludedInLegalCompliance",
                table: "NormativeRules",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsLegalRule",
                table: "NormativeRules",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "OfficialUrl",
                table: "NormativeRules",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "PublicationDate",
                table: "NormativeRules",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "RequiresManualReview",
                table: "NormativeRules",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "RuleCategory",
                table: "NormativeRules",
                type: "TEXT",
                nullable: false,
                defaultValue: "DEMO");

            migrationBuilder.AddColumn<string>(
                name: "RuleNumber",
                table: "NormativeRules",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Section",
                table: "NormativeRules",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "SourceDocument",
                table: "NormativeRules",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Subsection",
                table: "NormativeRules",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "EvaluationDate",
                table: "Audits",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "EvaluationDateBasis",
                table: "Audits",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "NormativeVersionLabel",
                table: "Audits",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CalculationJson",
                table: "AuditResults",
                type: "TEXT",
                nullable: false,
                defaultValue: "{}");

            migrationBuilder.AddColumn<string>(
                name: "ComparedValuesJson",
                table: "AuditResults",
                type: "TEXT",
                nullable: false,
                defaultValue: "{}");

            migrationBuilder.AddColumn<DateTime>(
                name: "EvaluatedAt",
                table: "AuditResults",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "EvidenceJson",
                table: "AuditResults",
                type: "TEXT",
                nullable: false,
                defaultValue: "{}");

            migrationBuilder.Sql("UPDATE NormativeRules SET RuleCategory = 'TECHNICAL' WHERE Code LIKE 'TECH-%';");

            migrationBuilder.AddColumn<string>(
                name: "Explanation",
                table: "AuditResults",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsIncludedInLegalCompliance",
                table: "AuditResults",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "ManualOutcome",
                table: "AuditResults",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "NormativeRuleId",
                table: "AuditResults",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "NormativeRuleVersion",
                table: "AuditResults",
                type: "INTEGER",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Outcome",
                table: "AuditResults",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "RequiresManualReview",
                table: "AuditResults",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "ReviewComment",
                table: "AuditResults",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "ReviewedAt",
                table: "AuditResults",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "ReviewerUserId",
                table: "AuditResults",
                type: "TEXT",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "CustomsValueAdjustments",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    TradeOperationId = table.Column<Guid>(type: "TEXT", nullable: false),
                    Concept = table.Column<string>(type: "TEXT", nullable: false),
                    Description = table.Column<string>(type: "TEXT", nullable: true),
                    Amount = table.Column<decimal>(type: "TEXT", nullable: false),
                    Currency = table.Column<string>(type: "TEXT", nullable: false),
                    AdjustmentType = table.Column<string>(type: "TEXT", nullable: false),
                    LegalQualificationStatus = table.Column<string>(type: "TEXT", nullable: false),
                    SourceDocumentId = table.Column<Guid>(type: "TEXT", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CustomsValueAdjustments", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CustomsValueAdjustments_OperationDocuments_SourceDocumentId",
                        column: x => x.SourceDocumentId,
                        principalTable: "OperationDocuments",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_CustomsValueAdjustments_TradeOperations_TradeOperationId",
                        column: x => x.TradeOperationId,
                        principalTable: "TradeOperations",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AuditResults_NormativeRuleId",
                table: "AuditResults",
                column: "NormativeRuleId");

            migrationBuilder.CreateIndex(
                name: "IX_AuditResults_ReviewerUserId",
                table: "AuditResults",
                column: "ReviewerUserId");

            migrationBuilder.CreateIndex(
                name: "IX_CustomsValueAdjustments_SourceDocumentId",
                table: "CustomsValueAdjustments",
                column: "SourceDocumentId");

            migrationBuilder.CreateIndex(
                name: "IX_CustomsValueAdjustments_TradeOperationId",
                table: "CustomsValueAdjustments",
                column: "TradeOperationId");

            migrationBuilder.AddForeignKey(
                name: "FK_AuditResults_NormativeRules_NormativeRuleId",
                table: "AuditResults",
                column: "NormativeRuleId",
                principalTable: "NormativeRules",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_AuditResults_Users_ReviewerUserId",
                table: "AuditResults",
                column: "ReviewerUserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AuditResults_NormativeRules_NormativeRuleId",
                table: "AuditResults");

            migrationBuilder.DropForeignKey(
                name: "FK_AuditResults_Users_ReviewerUserId",
                table: "AuditResults");

            migrationBuilder.DropTable(
                name: "CustomsValueAdjustments");

            migrationBuilder.DropIndex(
                name: "IX_AuditResults_NormativeRuleId",
                table: "AuditResults");

            migrationBuilder.DropIndex(
                name: "IX_AuditResults_ReviewerUserId",
                table: "AuditResults");

            migrationBuilder.DropColumn(
                name: "DeclaredCustomsValueMxn",
                table: "TradeOperations");

            migrationBuilder.DropColumn(
                name: "ExchangeRate",
                table: "TradeOperations");

            migrationBuilder.DropColumn(
                name: "GoodsDescription",
                table: "TradeOperations");

            migrationBuilder.DropColumn(
                name: "Incoterm",
                table: "TradeOperations");

            migrationBuilder.DropColumn(
                name: "Nico",
                table: "TradeOperations");

            migrationBuilder.DropColumn(
                name: "Annex",
                table: "NormativeRules");

            migrationBuilder.DropColumn(
                name: "Appendix",
                table: "NormativeRules");

            migrationBuilder.DropColumn(
                name: "Article",
                table: "NormativeRules");

            migrationBuilder.DropColumn(
                name: "AutomationLevel",
                table: "NormativeRules");

            migrationBuilder.DropColumn(
                name: "IsIncludedInLegalCompliance",
                table: "NormativeRules");

            migrationBuilder.DropColumn(
                name: "IsLegalRule",
                table: "NormativeRules");

            migrationBuilder.DropColumn(
                name: "OfficialUrl",
                table: "NormativeRules");

            migrationBuilder.DropColumn(
                name: "PublicationDate",
                table: "NormativeRules");

            migrationBuilder.DropColumn(
                name: "RequiresManualReview",
                table: "NormativeRules");

            migrationBuilder.DropColumn(
                name: "RuleCategory",
                table: "NormativeRules");

            migrationBuilder.DropColumn(
                name: "RuleNumber",
                table: "NormativeRules");

            migrationBuilder.DropColumn(
                name: "Section",
                table: "NormativeRules");

            migrationBuilder.DropColumn(
                name: "SourceDocument",
                table: "NormativeRules");

            migrationBuilder.DropColumn(
                name: "Subsection",
                table: "NormativeRules");

            migrationBuilder.DropColumn(
                name: "EvaluationDate",
                table: "Audits");

            migrationBuilder.DropColumn(
                name: "EvaluationDateBasis",
                table: "Audits");

            migrationBuilder.DropColumn(
                name: "NormativeVersionLabel",
                table: "Audits");

            migrationBuilder.DropColumn(
                name: "CalculationJson",
                table: "AuditResults");

            migrationBuilder.DropColumn(
                name: "ComparedValuesJson",
                table: "AuditResults");

            migrationBuilder.DropColumn(
                name: "EvaluatedAt",
                table: "AuditResults");

            migrationBuilder.DropColumn(
                name: "EvidenceJson",
                table: "AuditResults");

            migrationBuilder.DropColumn(
                name: "Explanation",
                table: "AuditResults");

            migrationBuilder.DropColumn(
                name: "IsIncludedInLegalCompliance",
                table: "AuditResults");

            migrationBuilder.DropColumn(
                name: "ManualOutcome",
                table: "AuditResults");

            migrationBuilder.DropColumn(
                name: "NormativeRuleId",
                table: "AuditResults");

            migrationBuilder.DropColumn(
                name: "NormativeRuleVersion",
                table: "AuditResults");

            migrationBuilder.DropColumn(
                name: "Outcome",
                table: "AuditResults");

            migrationBuilder.DropColumn(
                name: "RequiresManualReview",
                table: "AuditResults");

            migrationBuilder.DropColumn(
                name: "ReviewComment",
                table: "AuditResults");

            migrationBuilder.DropColumn(
                name: "ReviewedAt",
                table: "AuditResults");

            migrationBuilder.DropColumn(
                name: "ReviewerUserId",
                table: "AuditResults");
        }
    }
}
