using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Sace.Api.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class OfficialRegulatoryCatalogV1 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "VerificationLevel",
                table: "RegulatoryRequirements",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "VerifiedAt",
                table: "RegulatoryRequirements",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "VerifiedBy",
                table: "RegulatoryRequirements",
                type: "TEXT",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "OfficialLegalSources",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    Code = table.Column<string>(type: "TEXT", nullable: false),
                    Title = table.Column<string>(type: "TEXT", nullable: false),
                    SourceType = table.Column<string>(type: "TEXT", nullable: false),
                    Authority = table.Column<string>(type: "TEXT", nullable: false),
                    OfficialUrl = table.Column<string>(type: "TEXT", nullable: false),
                    Status = table.Column<string>(type: "TEXT", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OfficialLegalSources", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "RegulatoryCatalogImports",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    PackageId = table.Column<string>(type: "TEXT", nullable: false),
                    PackageVersion = table.Column<int>(type: "INTEGER", nullable: false),
                    PackageHash = table.Column<string>(type: "TEXT", nullable: false),
                    SourceCode = table.Column<string>(type: "TEXT", nullable: false),
                    SourceVersionLabel = table.Column<string>(type: "TEXT", nullable: false),
                    PublicationDate = table.Column<DateTime>(type: "TEXT", nullable: false),
                    EffectiveFrom = table.Column<DateTime>(type: "TEXT", nullable: false),
                    Status = table.Column<string>(type: "TEXT", nullable: false),
                    SummaryJson = table.Column<string>(type: "TEXT", nullable: false),
                    PayloadJson = table.Column<string>(type: "TEXT", nullable: false),
                    ImportedBy = table.Column<Guid>(type: "TEXT", nullable: true),
                    ImportedAt = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RegulatoryCatalogImports", x => x.Id);
                    table.ForeignKey(
                        name: "FK_RegulatoryCatalogImports_Users_ImportedBy",
                        column: x => x.ImportedBy,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "OfficialLegalSourceVersions",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    OfficialLegalSourceId = table.Column<Guid>(type: "TEXT", nullable: false),
                    Version = table.Column<int>(type: "INTEGER", nullable: false),
                    VersionLabel = table.Column<string>(type: "TEXT", nullable: false),
                    DofPublicationCode = table.Column<string>(type: "TEXT", nullable: true),
                    PublicationDate = table.Column<DateTime>(type: "TEXT", nullable: false),
                    EffectiveFrom = table.Column<DateTime>(type: "TEXT", nullable: false),
                    EffectiveTo = table.Column<DateTime>(type: "TEXT", nullable: true),
                    OfficialUrl = table.Column<string>(type: "TEXT", nullable: false),
                    Sha256 = table.Column<string>(type: "TEXT", nullable: true),
                    VerificationStatus = table.Column<string>(type: "TEXT", nullable: false),
                    VerifiedBy = table.Column<Guid>(type: "TEXT", nullable: true),
                    VerifiedAt = table.Column<DateTime>(type: "TEXT", nullable: true),
                    VerificationNotes = table.Column<string>(type: "TEXT", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OfficialLegalSourceVersions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_OfficialLegalSourceVersions_OfficialLegalSources_OfficialLegalSourceId",
                        column: x => x.OfficialLegalSourceId,
                        principalTable: "OfficialLegalSources",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_OfficialLegalSourceVersions_Users_VerifiedBy",
                        column: x => x.VerifiedBy,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "OfficialSourceCitations",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    OfficialLegalSourceVersionId = table.Column<Guid>(type: "TEXT", nullable: false),
                    CitationType = table.Column<string>(type: "TEXT", nullable: false),
                    CitationLabel = table.Column<string>(type: "TEXT", nullable: false),
                    Article = table.Column<string>(type: "TEXT", nullable: true),
                    RuleNumber = table.Column<string>(type: "TEXT", nullable: true),
                    Annex = table.Column<string>(type: "TEXT", nullable: true),
                    Section = table.Column<string>(type: "TEXT", nullable: true),
                    Subsection = table.Column<string>(type: "TEXT", nullable: true),
                    Numeral = table.Column<string>(type: "TEXT", nullable: true),
                    ExactReferenceText = table.Column<string>(type: "TEXT", nullable: true),
                    OfficialUrl = table.Column<string>(type: "TEXT", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OfficialSourceCitations", x => x.Id);
                    table.ForeignKey(
                        name: "FK_OfficialSourceCitations_OfficialLegalSourceVersions_OfficialLegalSourceVersionId",
                        column: x => x.OfficialLegalSourceVersionId,
                        principalTable: "OfficialLegalSourceVersions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "RegulatoryExceptions",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    RegulatoryRequirementId = table.Column<Guid>(type: "TEXT", nullable: false),
                    ExceptionType = table.Column<string>(type: "TEXT", nullable: false),
                    Description = table.Column<string>(type: "TEXT", nullable: false),
                    CountryCodes = table.Column<string>(type: "TEXT", nullable: true),
                    CustomsRegime = table.Column<string>(type: "TEXT", nullable: true),
                    EffectiveFrom = table.Column<DateTime>(type: "TEXT", nullable: true),
                    EffectiveTo = table.Column<DateTime>(type: "TEXT", nullable: true),
                    OfficialSourceCitationId = table.Column<Guid>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RegulatoryExceptions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_RegulatoryExceptions_OfficialSourceCitations_OfficialSourceCitationId",
                        column: x => x.OfficialSourceCitationId,
                        principalTable: "OfficialSourceCitations",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_RegulatoryExceptions_RegulatoryRequirements_RegulatoryRequirementId",
                        column: x => x.RegulatoryRequirementId,
                        principalTable: "RegulatoryRequirements",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "RegulatoryRequirementSourceCitations",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    RegulatoryRequirementId = table.Column<Guid>(type: "TEXT", nullable: false),
                    OfficialSourceCitationId = table.Column<Guid>(type: "TEXT", nullable: false),
                    IsPrimary = table.Column<bool>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RegulatoryRequirementSourceCitations", x => x.Id);
                    table.ForeignKey(
                        name: "FK_RegulatoryRequirementSourceCitations_OfficialSourceCitations_OfficialSourceCitationId",
                        column: x => x.OfficialSourceCitationId,
                        principalTable: "OfficialSourceCitations",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_RegulatoryRequirementSourceCitations_RegulatoryRequirements_RegulatoryRequirementId",
                        column: x => x.RegulatoryRequirementId,
                        principalTable: "RegulatoryRequirements",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "RegulatoryTariffEntries",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    RegulatoryRequirementId = table.Column<Guid>(type: "TEXT", nullable: false),
                    RawTariffFraction = table.Column<string>(type: "TEXT", nullable: false),
                    NormalizedTariffFraction = table.Column<string>(type: "TEXT", nullable: false),
                    Nico = table.Column<string>(type: "TEXT", nullable: true),
                    OperationScope = table.Column<string>(type: "TEXT", nullable: false),
                    CustomsRegime = table.Column<string>(type: "TEXT", nullable: true),
                    GoodsDescription = table.Column<string>(type: "TEXT", nullable: true),
                    QualificationType = table.Column<string>(type: "TEXT", nullable: false),
                    QualificationText = table.Column<string>(type: "TEXT", nullable: true),
                    OriginCountryCodes = table.Column<string>(type: "TEXT", nullable: true),
                    DestinationCountryCodes = table.Column<string>(type: "TEXT", nullable: true),
                    EffectiveFrom = table.Column<DateTime>(type: "TEXT", nullable: false),
                    EffectiveTo = table.Column<DateTime>(type: "TEXT", nullable: true),
                    OfficialSourceCitationId = table.Column<Guid>(type: "TEXT", nullable: false),
                    Status = table.Column<string>(type: "TEXT", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RegulatoryTariffEntries", x => x.Id);
                    table.ForeignKey(
                        name: "FK_RegulatoryTariffEntries_OfficialSourceCitations_OfficialSourceCitationId",
                        column: x => x.OfficialSourceCitationId,
                        principalTable: "OfficialSourceCitations",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_RegulatoryTariffEntries_RegulatoryRequirements_RegulatoryRequirementId",
                        column: x => x.RegulatoryRequirementId,
                        principalTable: "RegulatoryRequirements",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_RegulatoryRequirements_VerifiedBy",
                table: "RegulatoryRequirements",
                column: "VerifiedBy");

            migrationBuilder.CreateIndex(
                name: "IX_OfficialLegalSources_Code",
                table: "OfficialLegalSources",
                column: "Code",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_OfficialLegalSourceVersions_OfficialLegalSourceId_Version",
                table: "OfficialLegalSourceVersions",
                columns: new[] { "OfficialLegalSourceId", "Version" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_OfficialLegalSourceVersions_Sha256",
                table: "OfficialLegalSourceVersions",
                column: "Sha256");

            migrationBuilder.CreateIndex(
                name: "IX_OfficialLegalSourceVersions_VerifiedBy",
                table: "OfficialLegalSourceVersions",
                column: "VerifiedBy");

            migrationBuilder.CreateIndex(
                name: "IX_OfficialSourceCitations_OfficialLegalSourceVersionId",
                table: "OfficialSourceCitations",
                column: "OfficialLegalSourceVersionId");

            migrationBuilder.CreateIndex(
                name: "IX_RegulatoryCatalogImports_ImportedBy",
                table: "RegulatoryCatalogImports",
                column: "ImportedBy");

            migrationBuilder.CreateIndex(
                name: "IX_RegulatoryCatalogImports_PackageHash",
                table: "RegulatoryCatalogImports",
                column: "PackageHash",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_RegulatoryCatalogImports_PackageId",
                table: "RegulatoryCatalogImports",
                column: "PackageId");

            migrationBuilder.CreateIndex(
                name: "IX_RegulatoryExceptions_OfficialSourceCitationId",
                table: "RegulatoryExceptions",
                column: "OfficialSourceCitationId");

            migrationBuilder.CreateIndex(
                name: "IX_RegulatoryExceptions_RegulatoryRequirementId",
                table: "RegulatoryExceptions",
                column: "RegulatoryRequirementId");

            migrationBuilder.CreateIndex(
                name: "IX_RegulatoryRequirementSourceCitations_OfficialSourceCitationId",
                table: "RegulatoryRequirementSourceCitations",
                column: "OfficialSourceCitationId");

            migrationBuilder.CreateIndex(
                name: "IX_RegulatoryRequirementSourceCitations_RegulatoryRequirementId_OfficialSourceCitationId",
                table: "RegulatoryRequirementSourceCitations",
                columns: new[] { "RegulatoryRequirementId", "OfficialSourceCitationId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_RegulatoryTariffEntries_NormalizedTariffFraction_Nico_EffectiveFrom",
                table: "RegulatoryTariffEntries",
                columns: new[] { "NormalizedTariffFraction", "Nico", "EffectiveFrom" });

            migrationBuilder.CreateIndex(
                name: "IX_RegulatoryTariffEntries_OfficialSourceCitationId",
                table: "RegulatoryTariffEntries",
                column: "OfficialSourceCitationId");

            migrationBuilder.CreateIndex(
                name: "IX_RegulatoryTariffEntries_RegulatoryRequirementId",
                table: "RegulatoryTariffEntries",
                column: "RegulatoryRequirementId");

            migrationBuilder.AddForeignKey(
                name: "FK_RegulatoryRequirements_Users_VerifiedBy",
                table: "RegulatoryRequirements",
                column: "VerifiedBy",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_RegulatoryRequirements_Users_VerifiedBy",
                table: "RegulatoryRequirements");

            migrationBuilder.DropTable(
                name: "RegulatoryCatalogImports");

            migrationBuilder.DropTable(
                name: "RegulatoryExceptions");

            migrationBuilder.DropTable(
                name: "RegulatoryRequirementSourceCitations");

            migrationBuilder.DropTable(
                name: "RegulatoryTariffEntries");

            migrationBuilder.DropTable(
                name: "OfficialSourceCitations");

            migrationBuilder.DropTable(
                name: "OfficialLegalSourceVersions");

            migrationBuilder.DropTable(
                name: "OfficialLegalSources");

            migrationBuilder.DropIndex(
                name: "IX_RegulatoryRequirements_VerifiedBy",
                table: "RegulatoryRequirements");

            migrationBuilder.DropColumn(
                name: "VerificationLevel",
                table: "RegulatoryRequirements");

            migrationBuilder.DropColumn(
                name: "VerifiedAt",
                table: "RegulatoryRequirements");

            migrationBuilder.DropColumn(
                name: "VerifiedBy",
                table: "RegulatoryRequirements");
        }
    }
}
