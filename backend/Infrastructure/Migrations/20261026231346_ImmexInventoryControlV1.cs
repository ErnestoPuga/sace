using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Sace.Api.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class ImmexInventoryControlV1 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "CustomsRegimeCode",
                table: "TradeOperations",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "ImmexProgramId",
                table: "TradeOperations",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsTemporaryImport",
                table: "TradeOperations",
                type: "INTEGER",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "Annex24CoverageItems",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    Code = table.Column<string>(type: "TEXT", nullable: false),
                    Section = table.Column<string>(type: "TEXT", nullable: false),
                    Description = table.Column<string>(type: "TEXT", nullable: false),
                    CoverageStatus = table.Column<string>(type: "TEXT", nullable: false),
                    ImplementationReference = table.Column<string>(type: "TEXT", nullable: true),
                    Notes = table.Column<string>(type: "TEXT", nullable: true),
                    OfficialSourceCitationId = table.Column<Guid>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Annex24CoverageItems", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Annex24CoverageItems_OfficialSourceCitations_OfficialSourceCitationId",
                        column: x => x.OfficialSourceCitationId,
                        principalTable: "OfficialSourceCitations",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateTable(
                name: "ImmexPrograms",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    ProgramNumber = table.Column<string>(type: "TEXT", nullable: false),
                    CompanyName = table.Column<string>(type: "TEXT", nullable: false),
                    Rfc = table.Column<string>(type: "TEXT", nullable: false),
                    Status = table.Column<string>(type: "TEXT", nullable: false),
                    ValidFrom = table.Column<DateTime>(type: "TEXT", nullable: true),
                    ValidTo = table.Column<DateTime>(type: "TEXT", nullable: true),
                    ProgramModality = table.Column<string>(type: "TEXT", nullable: true),
                    CertifiedCompanyStatus = table.Column<string>(type: "TEXT", nullable: true),
                    FiscalAddress = table.Column<string>(type: "TEXT", nullable: true),
                    IndustrialPlantAddressesJson = table.Column<string>(type: "TEXT", nullable: true),
                    WarehouseAddressesJson = table.Column<string>(type: "TEXT", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    SourceType = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ImmexPrograms", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ImmexTemporalityRules",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    Code = table.Column<string>(type: "TEXT", nullable: false),
                    Version = table.Column<int>(type: "INTEGER", nullable: false),
                    Name = table.Column<string>(type: "TEXT", nullable: false),
                    InventoryClassification = table.Column<string>(type: "TEXT", nullable: false),
                    DurationType = table.Column<string>(type: "TEXT", nullable: false),
                    DurationValue = table.Column<int>(type: "INTEGER", nullable: true),
                    DurationUnit = table.Column<string>(type: "TEXT", nullable: true),
                    EffectiveFrom = table.Column<DateTime>(type: "TEXT", nullable: false),
                    EffectiveTo = table.Column<DateTime>(type: "TEXT", nullable: true),
                    Status = table.Column<string>(type: "TEXT", nullable: false),
                    OfficialSourceCitationId = table.Column<Guid>(type: "TEXT", nullable: true),
                    ConditionsJson = table.Column<string>(type: "TEXT", nullable: false),
                    RequiresManualQualification = table.Column<bool>(type: "INTEGER", nullable: false),
                    IsOverride = table.Column<bool>(type: "INTEGER", nullable: false),
                    Priority = table.Column<int>(type: "INTEGER", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ImmexTemporalityRules", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ImmexTemporalityRules_OfficialSourceCitations_OfficialSourceCitationId",
                        column: x => x.OfficialSourceCitationId,
                        principalTable: "OfficialSourceCitations",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "ImmexFixedAssets",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    ImmexProgramId = table.Column<Guid>(type: "TEXT", nullable: false),
                    TradeOperationId = table.Column<Guid>(type: "TEXT", nullable: true),
                    Description = table.Column<string>(type: "TEXT", nullable: false),
                    Brand = table.Column<string>(type: "TEXT", nullable: true),
                    Model = table.Column<string>(type: "TEXT", nullable: true),
                    SerialNumber = table.Column<string>(type: "TEXT", nullable: true),
                    TariffFraction = table.Column<string>(type: "TEXT", nullable: true),
                    Nico = table.Column<string>(type: "TEXT", nullable: true),
                    PedimentoNumber = table.Column<string>(type: "TEXT", nullable: false),
                    PedimentoKey = table.Column<string>(type: "TEXT", nullable: false),
                    EntryDate = table.Column<DateTime>(type: "TEXT", nullable: false),
                    Status = table.Column<string>(type: "TEXT", nullable: false),
                    ExitType = table.Column<string>(type: "TEXT", nullable: true),
                    ExitDate = table.Column<DateTime>(type: "TEXT", nullable: true),
                    ExitPedimentoNumber = table.Column<string>(type: "TEXT", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ImmexFixedAssets", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ImmexFixedAssets_ImmexPrograms_ImmexProgramId",
                        column: x => x.ImmexProgramId,
                        principalTable: "ImmexPrograms",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ImmexFixedAssets_TradeOperations_TradeOperationId",
                        column: x => x.TradeOperationId,
                        principalTable: "TradeOperations",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "ImmexMaterials",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    ImmexProgramId = table.Column<Guid>(type: "TEXT", nullable: false),
                    MaterialCode = table.Column<string>(type: "TEXT", nullable: false),
                    Description = table.Column<string>(type: "TEXT", nullable: false),
                    TariffFraction = table.Column<string>(type: "TEXT", nullable: false),
                    Nico = table.Column<string>(type: "TEXT", nullable: true),
                    TigieUnit = table.Column<string>(type: "TEXT", nullable: false),
                    CommercialUnit = table.Column<string>(type: "TEXT", nullable: true),
                    IsActive = table.Column<bool>(type: "INTEGER", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ImmexMaterials", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ImmexMaterials_ImmexPrograms_ImmexProgramId",
                        column: x => x.ImmexProgramId,
                        principalTable: "ImmexPrograms",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ImmexProducts",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    ImmexProgramId = table.Column<Guid>(type: "TEXT", nullable: false),
                    ProductCode = table.Column<string>(type: "TEXT", nullable: false),
                    Description = table.Column<string>(type: "TEXT", nullable: false),
                    TariffFraction = table.Column<string>(type: "TEXT", nullable: false),
                    Nico = table.Column<string>(type: "TEXT", nullable: true),
                    TigieUnit = table.Column<string>(type: "TEXT", nullable: false),
                    CommercialUnit = table.Column<string>(type: "TEXT", nullable: true),
                    IsActive = table.Column<bool>(type: "INTEGER", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ImmexProducts", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ImmexProducts_ImmexPrograms_ImmexProgramId",
                        column: x => x.ImmexProgramId,
                        principalTable: "ImmexPrograms",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ImmexInventoryEntries",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    ImmexProgramId = table.Column<Guid>(type: "TEXT", nullable: false),
                    TradeOperationId = table.Column<Guid>(type: "TEXT", nullable: true),
                    OperationDocumentId = table.Column<Guid>(type: "TEXT", nullable: true),
                    MaterialId = table.Column<Guid>(type: "TEXT", nullable: true),
                    PedimentoNumber = table.Column<string>(type: "TEXT", nullable: false),
                    PedimentoKey = table.Column<string>(type: "TEXT", nullable: false),
                    PedimentoItemNumber = table.Column<int>(type: "INTEGER", nullable: true),
                    Sequence = table.Column<int>(type: "INTEGER", nullable: false),
                    EntryDate = table.Column<DateTime>(type: "TEXT", nullable: false),
                    CustomsOffice = table.Column<string>(type: "TEXT", nullable: true),
                    TariffFraction = table.Column<string>(type: "TEXT", nullable: false),
                    Nico = table.Column<string>(type: "TEXT", nullable: true),
                    PartNumber = table.Column<string>(type: "TEXT", nullable: true),
                    Description = table.Column<string>(type: "TEXT", nullable: false),
                    OriginalQuantity = table.Column<decimal>(type: "TEXT", precision: 18, scale: 6, nullable: false),
                    AvailableQuantity = table.Column<decimal>(type: "TEXT", precision: 18, scale: 6, nullable: false),
                    TigieUnit = table.Column<string>(type: "TEXT", nullable: false),
                    CommercialUnit = table.Column<string>(type: "TEXT", nullable: true),
                    CommercialValue = table.Column<decimal>(type: "TEXT", precision: 18, scale: 6, nullable: true),
                    Currency = table.Column<string>(type: "TEXT", nullable: true),
                    InventoryClassification = table.Column<string>(type: "TEXT", nullable: false),
                    TemporalRuleId = table.Column<Guid>(type: "TEXT", nullable: true),
                    DueDate = table.Column<DateTime>(type: "TEXT", nullable: true),
                    TemporalityOutcome = table.Column<string>(type: "TEXT", nullable: false),
                    TemporalityRuleCode = table.Column<string>(type: "TEXT", nullable: true),
                    TemporalityRuleVersion = table.Column<int>(type: "INTEGER", nullable: true),
                    TemporalityCalculatedAt = table.Column<DateTime>(type: "TEXT", nullable: true),
                    TemporalityExplanation = table.Column<string>(type: "TEXT", nullable: true),
                    Status = table.Column<string>(type: "TEXT", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ImmexInventoryEntries", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ImmexInventoryEntries_ImmexMaterials_MaterialId",
                        column: x => x.MaterialId,
                        principalTable: "ImmexMaterials",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ImmexInventoryEntries_ImmexPrograms_ImmexProgramId",
                        column: x => x.ImmexProgramId,
                        principalTable: "ImmexPrograms",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ImmexInventoryEntries_ImmexTemporalityRules_TemporalRuleId",
                        column: x => x.TemporalRuleId,
                        principalTable: "ImmexTemporalityRules",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_ImmexInventoryEntries_OperationDocuments_OperationDocumentId",
                        column: x => x.OperationDocumentId,
                        principalTable: "OperationDocuments",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_ImmexInventoryEntries_TradeOperations_TradeOperationId",
                        column: x => x.TradeOperationId,
                        principalTable: "TradeOperations",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "ImmexUnitConversions",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    MaterialId = table.Column<Guid>(type: "TEXT", nullable: false),
                    FromUnit = table.Column<string>(type: "TEXT", nullable: false),
                    ToUnit = table.Column<string>(type: "TEXT", nullable: false),
                    Factor = table.Column<decimal>(type: "TEXT", precision: 18, scale: 9, nullable: false),
                    EffectiveFrom = table.Column<DateTime>(type: "TEXT", nullable: false),
                    EffectiveTo = table.Column<DateTime>(type: "TEXT", nullable: true),
                    Source = table.Column<string>(type: "TEXT", nullable: false),
                    IsVerified = table.Column<bool>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ImmexUnitConversions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ImmexUnitConversions_ImmexMaterials_MaterialId",
                        column: x => x.MaterialId,
                        principalTable: "ImmexMaterials",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ImmexInventoryExits",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    ImmexProgramId = table.Column<Guid>(type: "TEXT", nullable: false),
                    TradeOperationId = table.Column<Guid>(type: "TEXT", nullable: true),
                    ExitType = table.Column<string>(type: "TEXT", nullable: false),
                    PedimentoNumber = table.Column<string>(type: "TEXT", nullable: true),
                    PedimentoKey = table.Column<string>(type: "TEXT", nullable: true),
                    ExitDate = table.Column<DateTime>(type: "TEXT", nullable: false),
                    ProductId = table.Column<Guid>(type: "TEXT", nullable: true),
                    MaterialId = table.Column<Guid>(type: "TEXT", nullable: true),
                    Quantity = table.Column<decimal>(type: "TEXT", precision: 18, scale: 6, nullable: false),
                    Unit = table.Column<string>(type: "TEXT", nullable: false),
                    TariffFraction = table.Column<string>(type: "TEXT", nullable: true),
                    Nico = table.Column<string>(type: "TEXT", nullable: true),
                    Reference = table.Column<string>(type: "TEXT", nullable: true),
                    AllocatedQuantity = table.Column<decimal>(type: "TEXT", precision: 18, scale: 6, nullable: false),
                    UnallocatedQuantity = table.Column<decimal>(type: "TEXT", precision: 18, scale: 6, nullable: false),
                    DischargeOutcome = table.Column<string>(type: "TEXT", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ImmexInventoryExits", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ImmexInventoryExits_ImmexMaterials_MaterialId",
                        column: x => x.MaterialId,
                        principalTable: "ImmexMaterials",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ImmexInventoryExits_ImmexProducts_ProductId",
                        column: x => x.ProductId,
                        principalTable: "ImmexProducts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_ImmexInventoryExits_ImmexPrograms_ImmexProgramId",
                        column: x => x.ImmexProgramId,
                        principalTable: "ImmexPrograms",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ImmexInventoryExits_TradeOperations_TradeOperationId",
                        column: x => x.TradeOperationId,
                        principalTable: "TradeOperations",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "ImmexMaterialUsages",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    ImmexProgramId = table.Column<Guid>(type: "TEXT", nullable: false),
                    ProductId = table.Column<Guid>(type: "TEXT", nullable: true),
                    MaterialId = table.Column<Guid>(type: "TEXT", nullable: false),
                    PeriodStart = table.Column<DateTime>(type: "TEXT", nullable: false),
                    PeriodEnd = table.Column<DateTime>(type: "TEXT", nullable: false),
                    QuantityUsed = table.Column<decimal>(type: "TEXT", precision: 18, scale: 6, nullable: false),
                    LossQuantity = table.Column<decimal>(type: "TEXT", precision: 18, scale: 6, nullable: false),
                    WasteQuantity = table.Column<decimal>(type: "TEXT", precision: 18, scale: 6, nullable: false),
                    Unit = table.Column<string>(type: "TEXT", nullable: false),
                    SourceType = table.Column<string>(type: "TEXT", nullable: false),
                    SourceDocumentId = table.Column<Guid>(type: "TEXT", nullable: true),
                    SourceFieldId = table.Column<Guid>(type: "TEXT", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ImmexMaterialUsages", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ImmexMaterialUsages_DocumentFields_SourceFieldId",
                        column: x => x.SourceFieldId,
                        principalTable: "DocumentFields",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_ImmexMaterialUsages_ImmexMaterials_MaterialId",
                        column: x => x.MaterialId,
                        principalTable: "ImmexMaterials",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ImmexMaterialUsages_ImmexProducts_ProductId",
                        column: x => x.ProductId,
                        principalTable: "ImmexProducts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_ImmexMaterialUsages_ImmexPrograms_ImmexProgramId",
                        column: x => x.ImmexProgramId,
                        principalTable: "ImmexPrograms",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ImmexMaterialUsages_OperationDocuments_SourceDocumentId",
                        column: x => x.SourceDocumentId,
                        principalTable: "OperationDocuments",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateTable(
                name: "ImmexInventoryDischarges",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    InventoryEntryId = table.Column<Guid>(type: "TEXT", nullable: false),
                    InventoryExitId = table.Column<Guid>(type: "TEXT", nullable: false),
                    MaterialId = table.Column<Guid>(type: "TEXT", nullable: false),
                    Sequence = table.Column<int>(type: "INTEGER", nullable: false),
                    QuantityDischarged = table.Column<decimal>(type: "TEXT", precision: 18, scale: 6, nullable: false),
                    Unit = table.Column<string>(type: "TEXT", nullable: false),
                    DischargeMethod = table.Column<string>(type: "TEXT", nullable: false),
                    EntryBalanceBefore = table.Column<decimal>(type: "TEXT", precision: 18, scale: 6, nullable: false),
                    EntryBalanceAfter = table.Column<decimal>(type: "TEXT", precision: 18, scale: 6, nullable: false),
                    DischargedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    Automatic = table.Column<bool>(type: "INTEGER", nullable: false),
                    Explanation = table.Column<string>(type: "TEXT", nullable: false),
                    SourceCitationId = table.Column<Guid>(type: "TEXT", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ImmexInventoryDischarges", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ImmexInventoryDischarges_ImmexInventoryEntries_InventoryEntryId",
                        column: x => x.InventoryEntryId,
                        principalTable: "ImmexInventoryEntries",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ImmexInventoryDischarges_ImmexInventoryExits_InventoryExitId",
                        column: x => x.InventoryExitId,
                        principalTable: "ImmexInventoryExits",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ImmexInventoryDischarges_ImmexMaterials_MaterialId",
                        column: x => x.MaterialId,
                        principalTable: "ImmexMaterials",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ImmexInventoryDischarges_OfficialSourceCitations_SourceCitationId",
                        column: x => x.SourceCitationId,
                        principalTable: "OfficialSourceCitations",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateTable(
                name: "ImmexInventoryIssues",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    ImmexProgramId = table.Column<Guid>(type: "TEXT", nullable: false),
                    InventoryEntryId = table.Column<Guid>(type: "TEXT", nullable: true),
                    InventoryExitId = table.Column<Guid>(type: "TEXT", nullable: true),
                    IssueCode = table.Column<string>(type: "TEXT", nullable: false),
                    Severity = table.Column<string>(type: "TEXT", nullable: false),
                    Status = table.Column<string>(type: "TEXT", nullable: false),
                    Description = table.Column<string>(type: "TEXT", nullable: false),
                    DetectedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    ResolvedAt = table.Column<DateTime>(type: "TEXT", nullable: true),
                    ResolvedBy = table.Column<Guid>(type: "TEXT", nullable: true),
                    Resolution = table.Column<string>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ImmexInventoryIssues", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ImmexInventoryIssues_ImmexInventoryEntries_InventoryEntryId",
                        column: x => x.InventoryEntryId,
                        principalTable: "ImmexInventoryEntries",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_ImmexInventoryIssues_ImmexInventoryExits_InventoryExitId",
                        column: x => x.InventoryExitId,
                        principalTable: "ImmexInventoryExits",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_ImmexInventoryIssues_ImmexPrograms_ImmexProgramId",
                        column: x => x.ImmexProgramId,
                        principalTable: "ImmexPrograms",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ImmexInventoryIssues_Users_ResolvedBy",
                        column: x => x.ResolvedBy,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "ImmexDischargeReversals",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    OriginalDischargeId = table.Column<Guid>(type: "TEXT", nullable: false),
                    Reason = table.Column<string>(type: "TEXT", nullable: false),
                    ReversedBy = table.Column<Guid>(type: "TEXT", nullable: false),
                    ReversedAt = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ImmexDischargeReversals", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ImmexDischargeReversals_ImmexInventoryDischarges_OriginalDischargeId",
                        column: x => x.OriginalDischargeId,
                        principalTable: "ImmexInventoryDischarges",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ImmexDischargeReversals_Users_ReversedBy",
                        column: x => x.ReversedBy,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_TradeOperations_ImmexProgramId",
                table: "TradeOperations",
                column: "ImmexProgramId");

            migrationBuilder.CreateIndex(
                name: "IX_Annex24CoverageItems_Code",
                table: "Annex24CoverageItems",
                column: "Code",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Annex24CoverageItems_OfficialSourceCitationId",
                table: "Annex24CoverageItems",
                column: "OfficialSourceCitationId");

            migrationBuilder.CreateIndex(
                name: "IX_ImmexDischargeReversals_OriginalDischargeId",
                table: "ImmexDischargeReversals",
                column: "OriginalDischargeId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ImmexDischargeReversals_ReversedBy",
                table: "ImmexDischargeReversals",
                column: "ReversedBy");

            migrationBuilder.CreateIndex(
                name: "IX_ImmexFixedAssets_ImmexProgramId_Status",
                table: "ImmexFixedAssets",
                columns: new[] { "ImmexProgramId", "Status" });

            migrationBuilder.CreateIndex(
                name: "IX_ImmexFixedAssets_TradeOperationId",
                table: "ImmexFixedAssets",
                column: "TradeOperationId");

            migrationBuilder.CreateIndex(
                name: "IX_ImmexInventoryDischarges_InventoryEntryId_InventoryExitId",
                table: "ImmexInventoryDischarges",
                columns: new[] { "InventoryEntryId", "InventoryExitId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ImmexInventoryDischarges_InventoryExitId_Sequence",
                table: "ImmexInventoryDischarges",
                columns: new[] { "InventoryExitId", "Sequence" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ImmexInventoryDischarges_MaterialId",
                table: "ImmexInventoryDischarges",
                column: "MaterialId");

            migrationBuilder.CreateIndex(
                name: "IX_ImmexInventoryDischarges_SourceCitationId",
                table: "ImmexInventoryDischarges",
                column: "SourceCitationId");

            migrationBuilder.CreateIndex(
                name: "IX_ImmexInventoryEntries_EntryDate",
                table: "ImmexInventoryEntries",
                column: "EntryDate");

            migrationBuilder.CreateIndex(
                name: "IX_ImmexInventoryEntries_ImmexProgramId_DueDate",
                table: "ImmexInventoryEntries",
                columns: new[] { "ImmexProgramId", "DueDate" });

            migrationBuilder.CreateIndex(
                name: "IX_ImmexInventoryEntries_ImmexProgramId_MaterialId_Status",
                table: "ImmexInventoryEntries",
                columns: new[] { "ImmexProgramId", "MaterialId", "Status" });

            migrationBuilder.CreateIndex(
                name: "IX_ImmexInventoryEntries_MaterialId",
                table: "ImmexInventoryEntries",
                column: "MaterialId");

            migrationBuilder.CreateIndex(
                name: "IX_ImmexInventoryEntries_OperationDocumentId",
                table: "ImmexInventoryEntries",
                column: "OperationDocumentId");

            migrationBuilder.CreateIndex(
                name: "IX_ImmexInventoryEntries_PedimentoNumber",
                table: "ImmexInventoryEntries",
                column: "PedimentoNumber");

            migrationBuilder.CreateIndex(
                name: "IX_ImmexInventoryEntries_TariffFraction",
                table: "ImmexInventoryEntries",
                column: "TariffFraction");

            migrationBuilder.CreateIndex(
                name: "IX_ImmexInventoryEntries_TemporalRuleId",
                table: "ImmexInventoryEntries",
                column: "TemporalRuleId");

            migrationBuilder.CreateIndex(
                name: "IX_ImmexInventoryEntries_TradeOperationId_Sequence",
                table: "ImmexInventoryEntries",
                columns: new[] { "TradeOperationId", "Sequence" },
                unique: true,
                filter: "\"TradeOperationId\" IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_ImmexInventoryExits_ImmexProgramId_ExitDate",
                table: "ImmexInventoryExits",
                columns: new[] { "ImmexProgramId", "ExitDate" });

            migrationBuilder.CreateIndex(
                name: "IX_ImmexInventoryExits_MaterialId",
                table: "ImmexInventoryExits",
                column: "MaterialId");

            migrationBuilder.CreateIndex(
                name: "IX_ImmexInventoryExits_PedimentoNumber",
                table: "ImmexInventoryExits",
                column: "PedimentoNumber");

            migrationBuilder.CreateIndex(
                name: "IX_ImmexInventoryExits_ProductId",
                table: "ImmexInventoryExits",
                column: "ProductId");

            migrationBuilder.CreateIndex(
                name: "IX_ImmexInventoryExits_TradeOperationId",
                table: "ImmexInventoryExits",
                column: "TradeOperationId");

            migrationBuilder.CreateIndex(
                name: "IX_ImmexInventoryIssues_ImmexProgramId_Status_IssueCode",
                table: "ImmexInventoryIssues",
                columns: new[] { "ImmexProgramId", "Status", "IssueCode" });

            migrationBuilder.CreateIndex(
                name: "IX_ImmexInventoryIssues_InventoryEntryId",
                table: "ImmexInventoryIssues",
                column: "InventoryEntryId");

            migrationBuilder.CreateIndex(
                name: "IX_ImmexInventoryIssues_InventoryExitId",
                table: "ImmexInventoryIssues",
                column: "InventoryExitId");

            migrationBuilder.CreateIndex(
                name: "IX_ImmexInventoryIssues_ResolvedBy",
                table: "ImmexInventoryIssues",
                column: "ResolvedBy");

            migrationBuilder.CreateIndex(
                name: "IX_ImmexMaterials_ImmexProgramId_MaterialCode",
                table: "ImmexMaterials",
                columns: new[] { "ImmexProgramId", "MaterialCode" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ImmexMaterialUsages_ImmexProgramId",
                table: "ImmexMaterialUsages",
                column: "ImmexProgramId");

            migrationBuilder.CreateIndex(
                name: "IX_ImmexMaterialUsages_MaterialId",
                table: "ImmexMaterialUsages",
                column: "MaterialId");

            migrationBuilder.CreateIndex(
                name: "IX_ImmexMaterialUsages_ProductId",
                table: "ImmexMaterialUsages",
                column: "ProductId");

            migrationBuilder.CreateIndex(
                name: "IX_ImmexMaterialUsages_SourceDocumentId",
                table: "ImmexMaterialUsages",
                column: "SourceDocumentId");

            migrationBuilder.CreateIndex(
                name: "IX_ImmexMaterialUsages_SourceFieldId",
                table: "ImmexMaterialUsages",
                column: "SourceFieldId");

            migrationBuilder.CreateIndex(
                name: "IX_ImmexProducts_ImmexProgramId_ProductCode",
                table: "ImmexProducts",
                columns: new[] { "ImmexProgramId", "ProductCode" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ImmexPrograms_ProgramNumber",
                table: "ImmexPrograms",
                column: "ProgramNumber",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ImmexPrograms_Rfc",
                table: "ImmexPrograms",
                column: "Rfc");

            migrationBuilder.CreateIndex(
                name: "IX_ImmexTemporalityRules_Code_Version",
                table: "ImmexTemporalityRules",
                columns: new[] { "Code", "Version" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ImmexTemporalityRules_OfficialSourceCitationId",
                table: "ImmexTemporalityRules",
                column: "OfficialSourceCitationId");

            migrationBuilder.CreateIndex(
                name: "IX_ImmexUnitConversions_MaterialId_FromUnit_ToUnit_EffectiveFrom",
                table: "ImmexUnitConversions",
                columns: new[] { "MaterialId", "FromUnit", "ToUnit", "EffectiveFrom" },
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_TradeOperations_ImmexPrograms_ImmexProgramId",
                table: "TradeOperations",
                column: "ImmexProgramId",
                principalTable: "ImmexPrograms",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_TradeOperations_ImmexPrograms_ImmexProgramId",
                table: "TradeOperations");

            migrationBuilder.DropTable(
                name: "Annex24CoverageItems");

            migrationBuilder.DropTable(
                name: "ImmexDischargeReversals");

            migrationBuilder.DropTable(
                name: "ImmexFixedAssets");

            migrationBuilder.DropTable(
                name: "ImmexInventoryIssues");

            migrationBuilder.DropTable(
                name: "ImmexMaterialUsages");

            migrationBuilder.DropTable(
                name: "ImmexUnitConversions");

            migrationBuilder.DropTable(
                name: "ImmexInventoryDischarges");

            migrationBuilder.DropTable(
                name: "ImmexInventoryEntries");

            migrationBuilder.DropTable(
                name: "ImmexInventoryExits");

            migrationBuilder.DropTable(
                name: "ImmexTemporalityRules");

            migrationBuilder.DropTable(
                name: "ImmexMaterials");

            migrationBuilder.DropTable(
                name: "ImmexProducts");

            migrationBuilder.DropTable(
                name: "ImmexPrograms");

            migrationBuilder.DropIndex(
                name: "IX_TradeOperations_ImmexProgramId",
                table: "TradeOperations");

            migrationBuilder.DropColumn(
                name: "CustomsRegimeCode",
                table: "TradeOperations");

            migrationBuilder.DropColumn(
                name: "ImmexProgramId",
                table: "TradeOperations");

            migrationBuilder.DropColumn(
                name: "IsTemporaryImport",
                table: "TradeOperations");
        }
    }
}
