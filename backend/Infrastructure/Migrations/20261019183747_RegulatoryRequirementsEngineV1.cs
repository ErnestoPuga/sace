using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Sace.Api.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class RegulatoryRequirementsEngineV1 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "RegulatoryRequirements",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    Code = table.Column<string>(type: "TEXT", nullable: false),
                    Version = table.Column<int>(type: "INTEGER", nullable: false),
                    Name = table.Column<string>(type: "TEXT", nullable: false),
                    Description = table.Column<string>(type: "TEXT", nullable: true),
                    RequirementCategory = table.Column<string>(type: "TEXT", nullable: false),
                    AuthorityCode = table.Column<string>(type: "TEXT", nullable: true),
                    AuthorityName = table.Column<string>(type: "TEXT", nullable: true),
                    OperationScope = table.Column<string>(type: "TEXT", nullable: false),
                    LegalBasis = table.Column<string>(type: "TEXT", nullable: true),
                    OfficialSourceUrl = table.Column<string>(type: "TEXT", nullable: true),
                    PublicationDate = table.Column<DateTime>(type: "TEXT", nullable: true),
                    EffectiveFrom = table.Column<DateTime>(type: "TEXT", nullable: true),
                    EffectiveTo = table.Column<DateTime>(type: "TEXT", nullable: true),
                    Status = table.Column<string>(type: "TEXT", nullable: false),
                    IsAutomatable = table.Column<bool>(type: "INTEGER", nullable: false),
                    RequiresManualReview = table.Column<bool>(type: "INTEGER", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RegulatoryRequirements", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "RegulatoryEvidenceRequirements",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    RegulatoryRequirementId = table.Column<Guid>(type: "TEXT", nullable: false),
                    DocumentTypeCode = table.Column<string>(type: "TEXT", nullable: false),
                    EvidenceRole = table.Column<string>(type: "TEXT", nullable: false),
                    IsRequired = table.Column<bool>(type: "INTEGER", nullable: false),
                    Description = table.Column<string>(type: "TEXT", nullable: true),
                    AlternativeGroup = table.Column<string>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RegulatoryEvidenceRequirements", x => x.Id);
                    table.ForeignKey(
                        name: "FK_RegulatoryEvidenceRequirements_RegulatoryRequirements_RegulatoryRequirementId",
                        column: x => x.RegulatoryRequirementId,
                        principalTable: "RegulatoryRequirements",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "RegulatoryRequirementConditions",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    RegulatoryRequirementId = table.Column<Guid>(type: "TEXT", nullable: false),
                    FieldCode = table.Column<string>(type: "TEXT", nullable: false),
                    Operator = table.Column<string>(type: "TEXT", nullable: false),
                    ExpectedValue = table.Column<string>(type: "TEXT", nullable: true),
                    ExpectedValue2 = table.Column<string>(type: "TEXT", nullable: true),
                    ConditionGroup = table.Column<string>(type: "TEXT", nullable: false),
                    Sequence = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RegulatoryRequirementConditions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_RegulatoryRequirementConditions_RegulatoryRequirements_RegulatoryRequirementId",
                        column: x => x.RegulatoryRequirementId,
                        principalTable: "RegulatoryRequirements",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "RegulatoryRequirementEvaluations",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    TradeOperationId = table.Column<Guid>(type: "TEXT", nullable: false),
                    AuditId = table.Column<Guid>(type: "TEXT", nullable: true),
                    RegulatoryRequirementId = table.Column<Guid>(type: "TEXT", nullable: false),
                    RequirementCode = table.Column<string>(type: "TEXT", nullable: false),
                    RequirementVersion = table.Column<int>(type: "INTEGER", nullable: false),
                    EvaluationDate = table.Column<DateTime>(type: "TEXT", nullable: false),
                    AutomatedOutcome = table.Column<string>(type: "TEXT", nullable: false),
                    EvidenceStatus = table.Column<string>(type: "TEXT", nullable: false),
                    Explanation = table.Column<string>(type: "TEXT", nullable: false),
                    Confidence = table.Column<double>(type: "REAL", nullable: true),
                    ManualReviewRequired = table.Column<bool>(type: "INTEGER", nullable: false),
                    EvaluatedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    SnapshotJson = table.Column<string>(type: "TEXT", nullable: false),
                    ManualOutcome = table.Column<string>(type: "TEXT", nullable: true),
                    ManualNote = table.Column<string>(type: "TEXT", nullable: true),
                    ReviewedBy = table.Column<Guid>(type: "TEXT", nullable: true),
                    ReviewedAt = table.Column<DateTime>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RegulatoryRequirementEvaluations", x => x.Id);
                    table.ForeignKey(
                        name: "FK_RegulatoryRequirementEvaluations_Audits_AuditId",
                        column: x => x.AuditId,
                        principalTable: "Audits",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_RegulatoryRequirementEvaluations_RegulatoryRequirements_RegulatoryRequirementId",
                        column: x => x.RegulatoryRequirementId,
                        principalTable: "RegulatoryRequirements",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_RegulatoryRequirementEvaluations_TradeOperations_TradeOperationId",
                        column: x => x.TradeOperationId,
                        principalTable: "TradeOperations",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_RegulatoryRequirementEvaluations_Users_ReviewedBy",
                        column: x => x.ReviewedBy,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "RegulatoryRequirementEvaluationEvidence",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    RegulatoryRequirementEvaluationId = table.Column<Guid>(type: "TEXT", nullable: false),
                    RegulatoryEvidenceRequirementId = table.Column<Guid>(type: "TEXT", nullable: true),
                    OperationDocumentId = table.Column<Guid>(type: "TEXT", nullable: true),
                    DocumentTypeCode = table.Column<string>(type: "TEXT", nullable: false),
                    EvidenceRole = table.Column<string>(type: "TEXT", nullable: false),
                    IsRequired = table.Column<bool>(type: "INTEGER", nullable: false),
                    IsPresent = table.Column<bool>(type: "INTEGER", nullable: false),
                    AlternativeGroup = table.Column<string>(type: "TEXT", nullable: true),
                    Notes = table.Column<string>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RegulatoryRequirementEvaluationEvidence", x => x.Id);
                    table.ForeignKey(
                        name: "FK_RegulatoryRequirementEvaluationEvidence_OperationDocuments_OperationDocumentId",
                        column: x => x.OperationDocumentId,
                        principalTable: "OperationDocuments",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_RegulatoryRequirementEvaluationEvidence_RegulatoryEvidenceRequirements_RegulatoryEvidenceRequirementId",
                        column: x => x.RegulatoryEvidenceRequirementId,
                        principalTable: "RegulatoryEvidenceRequirements",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_RegulatoryRequirementEvaluationEvidence_RegulatoryRequirementEvaluations_RegulatoryRequirementEvaluationId",
                        column: x => x.RegulatoryRequirementEvaluationId,
                        principalTable: "RegulatoryRequirementEvaluations",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_RegulatoryEvidenceRequirements_RegulatoryRequirementId",
                table: "RegulatoryEvidenceRequirements",
                column: "RegulatoryRequirementId");

            migrationBuilder.CreateIndex(
                name: "IX_RegulatoryRequirementConditions_RegulatoryRequirementId_ConditionGroup_Sequence",
                table: "RegulatoryRequirementConditions",
                columns: new[] { "RegulatoryRequirementId", "ConditionGroup", "Sequence" });

            migrationBuilder.CreateIndex(
                name: "IX_RegulatoryRequirementEvaluationEvidence_OperationDocumentId",
                table: "RegulatoryRequirementEvaluationEvidence",
                column: "OperationDocumentId");

            migrationBuilder.CreateIndex(
                name: "IX_RegulatoryRequirementEvaluationEvidence_RegulatoryEvidenceRequirementId",
                table: "RegulatoryRequirementEvaluationEvidence",
                column: "RegulatoryEvidenceRequirementId");

            migrationBuilder.CreateIndex(
                name: "IX_RegulatoryRequirementEvaluationEvidence_RegulatoryRequirementEvaluationId",
                table: "RegulatoryRequirementEvaluationEvidence",
                column: "RegulatoryRequirementEvaluationId");

            migrationBuilder.CreateIndex(
                name: "IX_RegulatoryRequirementEvaluations_AuditId",
                table: "RegulatoryRequirementEvaluations",
                column: "AuditId");

            migrationBuilder.CreateIndex(
                name: "IX_RegulatoryRequirementEvaluations_RegulatoryRequirementId",
                table: "RegulatoryRequirementEvaluations",
                column: "RegulatoryRequirementId");

            migrationBuilder.CreateIndex(
                name: "IX_RegulatoryRequirementEvaluations_ReviewedBy",
                table: "RegulatoryRequirementEvaluations",
                column: "ReviewedBy");

            migrationBuilder.CreateIndex(
                name: "IX_RegulatoryRequirementEvaluations_TradeOperationId",
                table: "RegulatoryRequirementEvaluations",
                column: "TradeOperationId");

            migrationBuilder.CreateIndex(
                name: "IX_RegulatoryRequirementEvaluations_TradeOperationId_RequirementCode_EvaluatedAt",
                table: "RegulatoryRequirementEvaluations",
                columns: new[] { "TradeOperationId", "RequirementCode", "EvaluatedAt" });

            migrationBuilder.CreateIndex(
                name: "IX_RegulatoryRequirements_Code_Version",
                table: "RegulatoryRequirements",
                columns: new[] { "Code", "Version" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_RegulatoryRequirements_Status_EffectiveFrom_EffectiveTo",
                table: "RegulatoryRequirements",
                columns: new[] { "Status", "EffectiveFrom", "EffectiveTo" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "RegulatoryRequirementConditions");

            migrationBuilder.DropTable(
                name: "RegulatoryRequirementEvaluationEvidence");

            migrationBuilder.DropTable(
                name: "RegulatoryEvidenceRequirements");

            migrationBuilder.DropTable(
                name: "RegulatoryRequirementEvaluations");

            migrationBuilder.DropTable(
                name: "RegulatoryRequirements");
        }
    }
}
