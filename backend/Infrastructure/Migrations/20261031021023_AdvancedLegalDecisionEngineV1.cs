using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Sace.Api.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AdvancedLegalDecisionEngineV1 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "LegalStatus",
                table: "NormativeRules",
                type: "TEXT",
                nullable: false,
                defaultValue: "REVIEW_REQUIRED");

            migrationBuilder.AddColumn<string>(
                name: "Limitations",
                table: "NormativeRules",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "ConfirmedAt",
                table: "Findings",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "ConfirmedBy",
                table: "Findings",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CorrectionNotes",
                table: "Findings",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CorrectionStatus",
                table: "Findings",
                type: "TEXT",
                nullable: false,
                defaultValue: "PENDING");

            migrationBuilder.AddColumn<string>(
                name: "DismissalReason",
                table: "Findings",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "DismissedAt",
                table: "Findings",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "DismissedBy",
                table: "Findings",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "LatestAuditResultId",
                table: "Findings",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "OriginalAuditResultId",
                table: "Findings",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ApplicabilityExplanation",
                table: "AuditResults",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "DecisionExplanation",
                table: "AuditResults",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "DecisionMode",
                table: "AuditResults",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "EvidenceExplanation",
                table: "AuditResults",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "RuleCode",
                table: "AuditResults",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "RuleEffectiveFrom",
                table: "AuditResults",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "RuleEffectiveTo",
                table: "AuditResults",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "RuleTitle",
                table: "AuditResults",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "RuleVersion",
                table: "AuditResults",
                type: "INTEGER",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "SourceCitationSnapshot",
                table: "AuditResults",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "SourceVersionId",
                table: "AuditResults",
                type: "TEXT",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "LegalEvaluationEvidence",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    AuditResultId = table.Column<Guid>(type: "TEXT", nullable: false),
                    EvidenceType = table.Column<string>(type: "TEXT", nullable: false),
                    SourceEntityType = table.Column<string>(type: "TEXT", nullable: false),
                    SourceEntityId = table.Column<Guid>(type: "TEXT", nullable: true),
                    FieldCode = table.Column<string>(type: "TEXT", nullable: true),
                    RawValue = table.Column<string>(type: "TEXT", nullable: true),
                    NormalizedValue = table.Column<string>(type: "TEXT", nullable: true),
                    Outcome = table.Column<string>(type: "TEXT", nullable: true),
                    Confidence = table.Column<double>(type: "REAL", nullable: true),
                    DocumentId = table.Column<Guid>(type: "TEXT", nullable: true),
                    DocumentFieldId = table.Column<Guid>(type: "TEXT", nullable: true),
                    Explanation = table.Column<string>(type: "TEXT", nullable: false),
                    CapturedAt = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LegalEvaluationEvidence", x => x.Id);
                    table.ForeignKey(
                        name: "FK_LegalEvaluationEvidence_AuditResults_AuditResultId",
                        column: x => x.AuditResultId,
                        principalTable: "AuditResults",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_LegalEvaluationEvidence_DocumentFields_DocumentFieldId",
                        column: x => x.DocumentFieldId,
                        principalTable: "DocumentFields",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_LegalEvaluationEvidence_OperationDocuments_DocumentId",
                        column: x => x.DocumentId,
                        principalTable: "OperationDocuments",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateTable(
                name: "LegalEvidenceMappings",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    NormativeRuleId = table.Column<Guid>(type: "TEXT", nullable: false),
                    SourceType = table.Column<string>(type: "TEXT", nullable: false),
                    SourceOutcome = table.Column<string>(type: "TEXT", nullable: true),
                    TargetEvidenceCode = table.Column<string>(type: "TEXT", nullable: false),
                    SourceFieldCode = table.Column<string>(type: "TEXT", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LegalEvidenceMappings", x => x.Id);
                    table.ForeignKey(
                        name: "FK_LegalEvidenceMappings_NormativeRules_NormativeRuleId",
                        column: x => x.NormativeRuleId,
                        principalTable: "NormativeRules",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "LegalEvidenceRequirements",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    NormativeRuleId = table.Column<Guid>(type: "TEXT", nullable: false),
                    Code = table.Column<string>(type: "TEXT", nullable: false),
                    Name = table.Column<string>(type: "TEXT", nullable: false),
                    EvidenceType = table.Column<string>(type: "TEXT", nullable: false),
                    Required = table.Column<bool>(type: "INTEGER", nullable: false),
                    AlternativeGroup = table.Column<string>(type: "TEXT", nullable: true),
                    MinimumConfidence = table.Column<double>(type: "REAL", nullable: true),
                    ManualVerificationRequired = table.Column<bool>(type: "INTEGER", nullable: false),
                    Description = table.Column<string>(type: "TEXT", nullable: false),
                    OfficialSourceCitationId = table.Column<Guid>(type: "TEXT", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LegalEvidenceRequirements", x => x.Id);
                    table.ForeignKey(
                        name: "FK_LegalEvidenceRequirements_NormativeRules_NormativeRuleId",
                        column: x => x.NormativeRuleId,
                        principalTable: "NormativeRules",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_LegalEvidenceRequirements_OfficialSourceCitations_OfficialSourceCitationId",
                        column: x => x.OfficialSourceCitationId,
                        principalTable: "OfficialSourceCitations",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "LegalReviewDecisions",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    AuditResultId = table.Column<Guid>(type: "TEXT", nullable: false),
                    FindingId = table.Column<Guid>(type: "TEXT", nullable: true),
                    ReviewerUserId = table.Column<Guid>(type: "TEXT", nullable: false),
                    PreviousOutcome = table.Column<string>(type: "TEXT", nullable: false),
                    ReviewedOutcome = table.Column<string>(type: "TEXT", nullable: true),
                    Decision = table.Column<string>(type: "TEXT", nullable: false),
                    Reason = table.Column<string>(type: "TEXT", nullable: false),
                    ReviewedAt = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LegalReviewDecisions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_LegalReviewDecisions_AuditResults_AuditResultId",
                        column: x => x.AuditResultId,
                        principalTable: "AuditResults",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_LegalReviewDecisions_Findings_FindingId",
                        column: x => x.FindingId,
                        principalTable: "Findings",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_LegalReviewDecisions_Users_ReviewerUserId",
                        column: x => x.ReviewerUserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "LegalRuleConditions",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    NormativeRuleId = table.Column<Guid>(type: "TEXT", nullable: false),
                    GroupCode = table.Column<string>(type: "TEXT", nullable: false),
                    Sequence = table.Column<int>(type: "INTEGER", nullable: false),
                    FieldCode = table.Column<string>(type: "TEXT", nullable: false),
                    Operator = table.Column<string>(type: "TEXT", nullable: false),
                    ExpectedValue = table.Column<string>(type: "TEXT", nullable: true),
                    ExpectedValue2 = table.Column<string>(type: "TEXT", nullable: true),
                    ValueType = table.Column<string>(type: "TEXT", nullable: false),
                    Required = table.Column<bool>(type: "INTEGER", nullable: false),
                    FailureBehavior = table.Column<string>(type: "TEXT", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LegalRuleConditions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_LegalRuleConditions_NormativeRules_NormativeRuleId",
                        column: x => x.NormativeRuleId,
                        principalTable: "NormativeRules",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "LegalRuleSourceCitations",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    NormativeRuleId = table.Column<Guid>(type: "TEXT", nullable: false),
                    OfficialSourceCitationId = table.Column<Guid>(type: "TEXT", nullable: false),
                    IsPrimary = table.Column<bool>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LegalRuleSourceCitations", x => x.Id);
                    table.ForeignKey(
                        name: "FK_LegalRuleSourceCitations_NormativeRules_NormativeRuleId",
                        column: x => x.NormativeRuleId,
                        principalTable: "NormativeRules",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_LegalRuleSourceCitations_OfficialSourceCitations_OfficialSourceCitationId",
                        column: x => x.OfficialSourceCitationId,
                        principalTable: "OfficialSourceCitations",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Findings_ConfirmedBy",
                table: "Findings",
                column: "ConfirmedBy");

            migrationBuilder.CreateIndex(
                name: "IX_Findings_DismissedBy",
                table: "Findings",
                column: "DismissedBy");

            migrationBuilder.CreateIndex(
                name: "IX_Findings_LatestAuditResultId",
                table: "Findings",
                column: "LatestAuditResultId");

            migrationBuilder.CreateIndex(
                name: "IX_Findings_OriginalAuditResultId",
                table: "Findings",
                column: "OriginalAuditResultId");

            migrationBuilder.CreateIndex(
                name: "IX_AuditResults_SourceVersionId",
                table: "AuditResults",
                column: "SourceVersionId");

            migrationBuilder.CreateIndex(
                name: "IX_LegalEvaluationEvidence_AuditResultId",
                table: "LegalEvaluationEvidence",
                column: "AuditResultId");

            migrationBuilder.CreateIndex(
                name: "IX_LegalEvaluationEvidence_DocumentFieldId",
                table: "LegalEvaluationEvidence",
                column: "DocumentFieldId");

            migrationBuilder.CreateIndex(
                name: "IX_LegalEvaluationEvidence_DocumentId",
                table: "LegalEvaluationEvidence",
                column: "DocumentId");

            migrationBuilder.CreateIndex(
                name: "IX_LegalEvaluationEvidence_SourceEntityType_SourceEntityId",
                table: "LegalEvaluationEvidence",
                columns: new[] { "SourceEntityType", "SourceEntityId" });

            migrationBuilder.CreateIndex(
                name: "IX_LegalEvidenceMappings_NormativeRuleId_SourceType_SourceOutcome",
                table: "LegalEvidenceMappings",
                columns: new[] { "NormativeRuleId", "SourceType", "SourceOutcome" });

            migrationBuilder.CreateIndex(
                name: "IX_LegalEvidenceRequirements_NormativeRuleId_Code",
                table: "LegalEvidenceRequirements",
                columns: new[] { "NormativeRuleId", "Code" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_LegalEvidenceRequirements_OfficialSourceCitationId",
                table: "LegalEvidenceRequirements",
                column: "OfficialSourceCitationId");

            migrationBuilder.CreateIndex(
                name: "IX_LegalReviewDecisions_AuditResultId_ReviewedAt",
                table: "LegalReviewDecisions",
                columns: new[] { "AuditResultId", "ReviewedAt" });

            migrationBuilder.CreateIndex(
                name: "IX_LegalReviewDecisions_FindingId",
                table: "LegalReviewDecisions",
                column: "FindingId");

            migrationBuilder.CreateIndex(
                name: "IX_LegalReviewDecisions_ReviewerUserId",
                table: "LegalReviewDecisions",
                column: "ReviewerUserId");

            migrationBuilder.CreateIndex(
                name: "IX_LegalRuleConditions_NormativeRuleId_GroupCode_Sequence",
                table: "LegalRuleConditions",
                columns: new[] { "NormativeRuleId", "GroupCode", "Sequence" });

            migrationBuilder.CreateIndex(
                name: "IX_LegalRuleSourceCitations_NormativeRuleId_OfficialSourceCitationId",
                table: "LegalRuleSourceCitations",
                columns: new[] { "NormativeRuleId", "OfficialSourceCitationId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_LegalRuleSourceCitations_OfficialSourceCitationId",
                table: "LegalRuleSourceCitations",
                column: "OfficialSourceCitationId");

            migrationBuilder.AddForeignKey(
                name: "FK_AuditResults_OfficialLegalSourceVersions_SourceVersionId",
                table: "AuditResults",
                column: "SourceVersionId",
                principalTable: "OfficialLegalSourceVersions",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Findings_AuditResults_LatestAuditResultId",
                table: "Findings",
                column: "LatestAuditResultId",
                principalTable: "AuditResults",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Findings_AuditResults_OriginalAuditResultId",
                table: "Findings",
                column: "OriginalAuditResultId",
                principalTable: "AuditResults",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Findings_Users_ConfirmedBy",
                table: "Findings",
                column: "ConfirmedBy",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Findings_Users_DismissedBy",
                table: "Findings",
                column: "DismissedBy",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AuditResults_OfficialLegalSourceVersions_SourceVersionId",
                table: "AuditResults");

            migrationBuilder.DropForeignKey(
                name: "FK_Findings_AuditResults_LatestAuditResultId",
                table: "Findings");

            migrationBuilder.DropForeignKey(
                name: "FK_Findings_AuditResults_OriginalAuditResultId",
                table: "Findings");

            migrationBuilder.DropForeignKey(
                name: "FK_Findings_Users_ConfirmedBy",
                table: "Findings");

            migrationBuilder.DropForeignKey(
                name: "FK_Findings_Users_DismissedBy",
                table: "Findings");

            migrationBuilder.DropTable(
                name: "LegalEvaluationEvidence");

            migrationBuilder.DropTable(
                name: "LegalEvidenceMappings");

            migrationBuilder.DropTable(
                name: "LegalEvidenceRequirements");

            migrationBuilder.DropTable(
                name: "LegalReviewDecisions");

            migrationBuilder.DropTable(
                name: "LegalRuleConditions");

            migrationBuilder.DropTable(
                name: "LegalRuleSourceCitations");

            migrationBuilder.DropIndex(
                name: "IX_Findings_ConfirmedBy",
                table: "Findings");

            migrationBuilder.DropIndex(
                name: "IX_Findings_DismissedBy",
                table: "Findings");

            migrationBuilder.DropIndex(
                name: "IX_Findings_LatestAuditResultId",
                table: "Findings");

            migrationBuilder.DropIndex(
                name: "IX_Findings_OriginalAuditResultId",
                table: "Findings");

            migrationBuilder.DropIndex(
                name: "IX_AuditResults_SourceVersionId",
                table: "AuditResults");

            migrationBuilder.DropColumn(
                name: "LegalStatus",
                table: "NormativeRules");

            migrationBuilder.DropColumn(
                name: "Limitations",
                table: "NormativeRules");

            migrationBuilder.DropColumn(
                name: "ConfirmedAt",
                table: "Findings");

            migrationBuilder.DropColumn(
                name: "ConfirmedBy",
                table: "Findings");

            migrationBuilder.DropColumn(
                name: "CorrectionNotes",
                table: "Findings");

            migrationBuilder.DropColumn(
                name: "CorrectionStatus",
                table: "Findings");

            migrationBuilder.DropColumn(
                name: "DismissalReason",
                table: "Findings");

            migrationBuilder.DropColumn(
                name: "DismissedAt",
                table: "Findings");

            migrationBuilder.DropColumn(
                name: "DismissedBy",
                table: "Findings");

            migrationBuilder.DropColumn(
                name: "LatestAuditResultId",
                table: "Findings");

            migrationBuilder.DropColumn(
                name: "OriginalAuditResultId",
                table: "Findings");

            migrationBuilder.DropColumn(
                name: "ApplicabilityExplanation",
                table: "AuditResults");

            migrationBuilder.DropColumn(
                name: "DecisionExplanation",
                table: "AuditResults");

            migrationBuilder.DropColumn(
                name: "DecisionMode",
                table: "AuditResults");

            migrationBuilder.DropColumn(
                name: "EvidenceExplanation",
                table: "AuditResults");

            migrationBuilder.DropColumn(
                name: "RuleCode",
                table: "AuditResults");

            migrationBuilder.DropColumn(
                name: "RuleEffectiveFrom",
                table: "AuditResults");

            migrationBuilder.DropColumn(
                name: "RuleEffectiveTo",
                table: "AuditResults");

            migrationBuilder.DropColumn(
                name: "RuleTitle",
                table: "AuditResults");

            migrationBuilder.DropColumn(
                name: "RuleVersion",
                table: "AuditResults");

            migrationBuilder.DropColumn(
                name: "SourceCitationSnapshot",
                table: "AuditResults");

            migrationBuilder.DropColumn(
                name: "SourceVersionId",
                table: "AuditResults");
        }
    }
}
