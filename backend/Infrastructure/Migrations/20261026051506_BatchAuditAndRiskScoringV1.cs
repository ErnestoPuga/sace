using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Sace.Api.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class BatchAuditAndRiskScoringV1 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "BatchAudits",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    Name = table.Column<string>(type: "TEXT", nullable: false),
                    Description = table.Column<string>(type: "TEXT", nullable: true),
                    PeriodStart = table.Column<DateTime>(type: "TEXT", nullable: true),
                    PeriodEnd = table.Column<DateTime>(type: "TEXT", nullable: true),
                    CreatedBy = table.Column<Guid>(type: "TEXT", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    StartedAt = table.Column<DateTime>(type: "TEXT", nullable: true),
                    CompletedAt = table.Column<DateTime>(type: "TEXT", nullable: true),
                    Status = table.Column<string>(type: "TEXT", nullable: false),
                    TotalOperations = table.Column<int>(type: "INTEGER", nullable: false),
                    ProcessedOperations = table.Column<int>(type: "INTEGER", nullable: false),
                    SuccessfulOperations = table.Column<int>(type: "INTEGER", nullable: false),
                    FailedOperations = table.Column<int>(type: "INTEGER", nullable: false),
                    RiskModelVersion = table.Column<string>(type: "TEXT", nullable: true),
                    ParametersJson = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BatchAudits", x => x.Id);
                    table.ForeignKey(
                        name: "FK_BatchAudits_Users_CreatedBy",
                        column: x => x.CreatedBy,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "RiskProfiles",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    Code = table.Column<string>(type: "TEXT", nullable: false),
                    Version = table.Column<int>(type: "INTEGER", nullable: false),
                    Name = table.Column<string>(type: "TEXT", nullable: false),
                    Description = table.Column<string>(type: "TEXT", nullable: false),
                    Status = table.Column<string>(type: "TEXT", nullable: false),
                    EffectiveFrom = table.Column<DateTime>(type: "TEXT", nullable: true),
                    EffectiveTo = table.Column<DateTime>(type: "TEXT", nullable: true),
                    LowMax = table.Column<decimal>(type: "TEXT", precision: 7, scale: 2, nullable: false),
                    MediumMax = table.Column<decimal>(type: "TEXT", precision: 7, scale: 2, nullable: false),
                    HighMax = table.Column<decimal>(type: "TEXT", precision: 7, scale: 2, nullable: false),
                    CategoryCapsJson = table.Column<string>(type: "TEXT", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RiskProfiles", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "OperationRiskScores",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    TradeOperationId = table.Column<Guid>(type: "TEXT", nullable: false),
                    AuditId = table.Column<Guid>(type: "TEXT", nullable: true),
                    RiskProfileId = table.Column<Guid>(type: "TEXT", nullable: false),
                    RiskProfileCode = table.Column<string>(type: "TEXT", nullable: false),
                    RiskProfileVersion = table.Column<int>(type: "INTEGER", nullable: false),
                    Version = table.Column<int>(type: "INTEGER", nullable: false),
                    RawScore = table.Column<decimal>(type: "TEXT", precision: 7, scale: 2, nullable: false),
                    NormalizedScore = table.Column<decimal>(type: "TEXT", precision: 7, scale: 2, nullable: false),
                    RiskLevel = table.Column<string>(type: "TEXT", nullable: false),
                    FactorCount = table.Column<int>(type: "INTEGER", nullable: false),
                    CriticalFactorCount = table.Column<int>(type: "INTEGER", nullable: false),
                    HighFactorCount = table.Column<int>(type: "INTEGER", nullable: false),
                    Explanation = table.Column<string>(type: "TEXT", nullable: false),
                    CalculatedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    SnapshotJson = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OperationRiskScores", x => x.Id);
                    table.ForeignKey(
                        name: "FK_OperationRiskScores_Audits_AuditId",
                        column: x => x.AuditId,
                        principalTable: "Audits",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_OperationRiskScores_RiskProfiles_RiskProfileId",
                        column: x => x.RiskProfileId,
                        principalTable: "RiskProfiles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_OperationRiskScores_TradeOperations_TradeOperationId",
                        column: x => x.TradeOperationId,
                        principalTable: "TradeOperations",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "RiskFactorDefinitions",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    RiskProfileId = table.Column<Guid>(type: "TEXT", nullable: false),
                    Code = table.Column<string>(type: "TEXT", nullable: false),
                    Name = table.Column<string>(type: "TEXT", nullable: false),
                    Category = table.Column<string>(type: "TEXT", nullable: false),
                    SourceType = table.Column<string>(type: "TEXT", nullable: false),
                    SourceOutcome = table.Column<string>(type: "TEXT", nullable: true),
                    Weight = table.Column<decimal>(type: "TEXT", precision: 7, scale: 2, nullable: false),
                    MaxPoints = table.Column<decimal>(type: "TEXT", precision: 7, scale: 2, nullable: false),
                    Severity = table.Column<string>(type: "TEXT", nullable: false),
                    IsActive = table.Column<bool>(type: "INTEGER", nullable: false),
                    ExplanationTemplate = table.Column<string>(type: "TEXT", nullable: true),
                    CorrelationKey = table.Column<string>(type: "TEXT", nullable: true),
                    CorrelationMode = table.Column<string>(type: "TEXT", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RiskFactorDefinitions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_RiskFactorDefinitions_RiskProfiles_RiskProfileId",
                        column: x => x.RiskProfileId,
                        principalTable: "RiskProfiles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "BatchAuditOperations",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    BatchAuditId = table.Column<Guid>(type: "TEXT", nullable: false),
                    TradeOperationId = table.Column<Guid>(type: "TEXT", nullable: false),
                    Status = table.Column<string>(type: "TEXT", nullable: false),
                    StartedAt = table.Column<DateTime>(type: "TEXT", nullable: true),
                    CompletedAt = table.Column<DateTime>(type: "TEXT", nullable: true),
                    AuditId = table.Column<Guid>(type: "TEXT", nullable: true),
                    RiskScoreId = table.Column<Guid>(type: "TEXT", nullable: true),
                    ErrorCode = table.Column<string>(type: "TEXT", nullable: true),
                    ErrorMessage = table.Column<string>(type: "TEXT", nullable: true),
                    RetryCount = table.Column<int>(type: "INTEGER", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BatchAuditOperations", x => x.Id);
                    table.ForeignKey(
                        name: "FK_BatchAuditOperations_Audits_AuditId",
                        column: x => x.AuditId,
                        principalTable: "Audits",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_BatchAuditOperations_BatchAudits_BatchAuditId",
                        column: x => x.BatchAuditId,
                        principalTable: "BatchAudits",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_BatchAuditOperations_OperationRiskScores_RiskScoreId",
                        column: x => x.RiskScoreId,
                        principalTable: "OperationRiskScores",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_BatchAuditOperations_TradeOperations_TradeOperationId",
                        column: x => x.TradeOperationId,
                        principalTable: "TradeOperations",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "OperationRiskFactors",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    OperationRiskScoreId = table.Column<Guid>(type: "TEXT", nullable: false),
                    FactorCode = table.Column<string>(type: "TEXT", nullable: false),
                    Category = table.Column<string>(type: "TEXT", nullable: false),
                    SourceEntityType = table.Column<string>(type: "TEXT", nullable: false),
                    SourceEntityId = table.Column<Guid>(type: "TEXT", nullable: true),
                    SourceOutcome = table.Column<string>(type: "TEXT", nullable: true),
                    Points = table.Column<decimal>(type: "TEXT", precision: 7, scale: 2, nullable: false),
                    Weight = table.Column<decimal>(type: "TEXT", precision: 7, scale: 2, nullable: false),
                    Severity = table.Column<string>(type: "TEXT", nullable: false),
                    Title = table.Column<string>(type: "TEXT", nullable: false),
                    Explanation = table.Column<string>(type: "TEXT", nullable: false),
                    CorrelationKey = table.Column<string>(type: "TEXT", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OperationRiskFactors", x => x.Id);
                    table.ForeignKey(
                        name: "FK_OperationRiskFactors_OperationRiskScores_OperationRiskScoreId",
                        column: x => x.OperationRiskScoreId,
                        principalTable: "OperationRiskScores",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ReviewQueueItems",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    TradeOperationId = table.Column<Guid>(type: "TEXT", nullable: false),
                    RiskScoreId = table.Column<Guid>(type: "TEXT", nullable: false),
                    OriginalPriority = table.Column<string>(type: "TEXT", nullable: false),
                    CurrentPriority = table.Column<string>(type: "TEXT", nullable: false),
                    Status = table.Column<string>(type: "TEXT", nullable: false),
                    AssignedTo = table.Column<Guid>(type: "TEXT", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    AssignedAt = table.Column<DateTime>(type: "TEXT", nullable: true),
                    StartedAt = table.Column<DateTime>(type: "TEXT", nullable: true),
                    CompletedAt = table.Column<DateTime>(type: "TEXT", nullable: true),
                    DueDate = table.Column<DateTime>(type: "TEXT", nullable: true),
                    Resolution = table.Column<string>(type: "TEXT", nullable: true),
                    Notes = table.Column<string>(type: "TEXT", nullable: true),
                    PriorityChangedBy = table.Column<Guid>(type: "TEXT", nullable: true),
                    PriorityChangedAt = table.Column<DateTime>(type: "TEXT", nullable: true),
                    PriorityChangeReason = table.Column<string>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ReviewQueueItems", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ReviewQueueItems_OperationRiskScores_RiskScoreId",
                        column: x => x.RiskScoreId,
                        principalTable: "OperationRiskScores",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ReviewQueueItems_TradeOperations_TradeOperationId",
                        column: x => x.TradeOperationId,
                        principalTable: "TradeOperations",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ReviewQueueItems_Users_AssignedTo",
                        column: x => x.AssignedTo,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ReviewQueueItems_Users_PriorityChangedBy",
                        column: x => x.PriorityChangedBy,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "ReviewQueueHistory",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    ReviewQueueItemId = table.Column<Guid>(type: "TEXT", nullable: false),
                    Action = table.Column<string>(type: "TEXT", nullable: false),
                    PreviousStatus = table.Column<string>(type: "TEXT", nullable: true),
                    NewStatus = table.Column<string>(type: "TEXT", nullable: true),
                    UserId = table.Column<Guid>(type: "TEXT", nullable: false),
                    Notes = table.Column<string>(type: "TEXT", nullable: true),
                    Timestamp = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ReviewQueueHistory", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ReviewQueueHistory_ReviewQueueItems_ReviewQueueItemId",
                        column: x => x.ReviewQueueItemId,
                        principalTable: "ReviewQueueItems",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ReviewQueueHistory_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_BatchAuditOperations_AuditId",
                table: "BatchAuditOperations",
                column: "AuditId");

            migrationBuilder.CreateIndex(
                name: "IX_BatchAuditOperations_BatchAuditId_TradeOperationId",
                table: "BatchAuditOperations",
                columns: new[] { "BatchAuditId", "TradeOperationId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_BatchAuditOperations_RiskScoreId",
                table: "BatchAuditOperations",
                column: "RiskScoreId");

            migrationBuilder.CreateIndex(
                name: "IX_BatchAuditOperations_Status",
                table: "BatchAuditOperations",
                column: "Status");

            migrationBuilder.CreateIndex(
                name: "IX_BatchAuditOperations_TradeOperationId",
                table: "BatchAuditOperations",
                column: "TradeOperationId");

            migrationBuilder.CreateIndex(
                name: "IX_BatchAudits_CreatedBy",
                table: "BatchAudits",
                column: "CreatedBy");

            migrationBuilder.CreateIndex(
                name: "IX_BatchAudits_Status_CreatedAt",
                table: "BatchAudits",
                columns: new[] { "Status", "CreatedAt" });

            migrationBuilder.CreateIndex(
                name: "IX_OperationRiskFactors_FactorCode_Category",
                table: "OperationRiskFactors",
                columns: new[] { "FactorCode", "Category" });

            migrationBuilder.CreateIndex(
                name: "IX_OperationRiskFactors_OperationRiskScoreId",
                table: "OperationRiskFactors",
                column: "OperationRiskScoreId");

            migrationBuilder.CreateIndex(
                name: "IX_OperationRiskScores_AuditId",
                table: "OperationRiskScores",
                column: "AuditId");

            migrationBuilder.CreateIndex(
                name: "IX_OperationRiskScores_RiskLevel_CalculatedAt",
                table: "OperationRiskScores",
                columns: new[] { "RiskLevel", "CalculatedAt" });

            migrationBuilder.CreateIndex(
                name: "IX_OperationRiskScores_RiskProfileId",
                table: "OperationRiskScores",
                column: "RiskProfileId");

            migrationBuilder.CreateIndex(
                name: "IX_OperationRiskScores_TradeOperationId_Version",
                table: "OperationRiskScores",
                columns: new[] { "TradeOperationId", "Version" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ReviewQueueHistory_ReviewQueueItemId_Timestamp",
                table: "ReviewQueueHistory",
                columns: new[] { "ReviewQueueItemId", "Timestamp" });

            migrationBuilder.CreateIndex(
                name: "IX_ReviewQueueHistory_UserId",
                table: "ReviewQueueHistory",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_ReviewQueueItems_AssignedTo",
                table: "ReviewQueueItems",
                column: "AssignedTo");

            migrationBuilder.CreateIndex(
                name: "IX_ReviewQueueItems_PriorityChangedBy",
                table: "ReviewQueueItems",
                column: "PriorityChangedBy");

            migrationBuilder.CreateIndex(
                name: "IX_ReviewQueueItems_RiskScoreId",
                table: "ReviewQueueItems",
                column: "RiskScoreId");

            migrationBuilder.CreateIndex(
                name: "IX_ReviewQueueItems_Status_CurrentPriority_CreatedAt",
                table: "ReviewQueueItems",
                columns: new[] { "Status", "CurrentPriority", "CreatedAt" });

            migrationBuilder.CreateIndex(
                name: "IX_ReviewQueueItems_TradeOperationId",
                table: "ReviewQueueItems",
                column: "TradeOperationId",
                unique: true,
                filter: "\"Status\" NOT IN ('RESOLVED', 'DISMISSED')");

            migrationBuilder.CreateIndex(
                name: "IX_RiskFactorDefinitions_RiskProfileId_Code",
                table: "RiskFactorDefinitions",
                columns: new[] { "RiskProfileId", "Code" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_RiskProfiles_Code_Version",
                table: "RiskProfiles",
                columns: new[] { "Code", "Version" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "BatchAuditOperations");

            migrationBuilder.DropTable(
                name: "OperationRiskFactors");

            migrationBuilder.DropTable(
                name: "ReviewQueueHistory");

            migrationBuilder.DropTable(
                name: "RiskFactorDefinitions");

            migrationBuilder.DropTable(
                name: "BatchAudits");

            migrationBuilder.DropTable(
                name: "ReviewQueueItems");

            migrationBuilder.DropTable(
                name: "OperationRiskScores");

            migrationBuilder.DropTable(
                name: "RiskProfiles");
        }
    }
}
