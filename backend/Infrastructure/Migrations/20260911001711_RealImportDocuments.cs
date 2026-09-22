using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Sace.Api.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class RealImportDocuments : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_OperationDocuments_TradeOperationId",
                table: "OperationDocuments");

            migrationBuilder.AddColumn<decimal>(
                name: "CommercialValue",
                table: "TradeOperations",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CoveNumber",
                table: "TradeOperations",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Currency",
                table: "TradeOperations",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CustomsOffice",
                table: "TradeOperations",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "GrossWeight",
                table: "TradeOperations",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "InvoiceNumber",
                table: "TradeOperations",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "Quantity",
                table: "TradeOperations",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Unit",
                table: "TradeOperations",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "DocumentDate",
                table: "OperationDocuments",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "DocumentNumber",
                table: "OperationDocuments",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ExtractedMetadataJson",
                table: "OperationDocuments",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "FileHash",
                table: "OperationDocuments",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Notes",
                table: "OperationDocuments",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "SourceSystem",
                table: "OperationDocuments",
                type: "TEXT",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "DocumentReferences",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    OperationDocumentId = table.Column<Guid>(type: "TEXT", nullable: false),
                    ReferenceType = table.Column<string>(type: "TEXT", nullable: false),
                    Value = table.Column<string>(type: "TEXT", nullable: false),
                    Source = table.Column<string>(type: "TEXT", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DocumentReferences", x => x.Id);
                    table.ForeignKey(
                        name: "FK_DocumentReferences_OperationDocuments_OperationDocumentId",
                        column: x => x.OperationDocumentId,
                        principalTable: "OperationDocuments",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_OperationDocuments_TradeOperationId_FileHash",
                table: "OperationDocuments",
                columns: new[] { "TradeOperationId", "FileHash" });

            migrationBuilder.CreateIndex(
                name: "IX_DocumentReferences_OperationDocumentId",
                table: "DocumentReferences",
                column: "OperationDocumentId");

            migrationBuilder.CreateIndex(
                name: "IX_DocumentReferences_ReferenceType_Value",
                table: "DocumentReferences",
                columns: new[] { "ReferenceType", "Value" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "DocumentReferences");

            migrationBuilder.DropIndex(
                name: "IX_OperationDocuments_TradeOperationId_FileHash",
                table: "OperationDocuments");

            migrationBuilder.DropColumn(
                name: "CommercialValue",
                table: "TradeOperations");

            migrationBuilder.DropColumn(
                name: "CoveNumber",
                table: "TradeOperations");

            migrationBuilder.DropColumn(
                name: "Currency",
                table: "TradeOperations");

            migrationBuilder.DropColumn(
                name: "CustomsOffice",
                table: "TradeOperations");

            migrationBuilder.DropColumn(
                name: "GrossWeight",
                table: "TradeOperations");

            migrationBuilder.DropColumn(
                name: "InvoiceNumber",
                table: "TradeOperations");

            migrationBuilder.DropColumn(
                name: "Quantity",
                table: "TradeOperations");

            migrationBuilder.DropColumn(
                name: "Unit",
                table: "TradeOperations");

            migrationBuilder.DropColumn(
                name: "DocumentDate",
                table: "OperationDocuments");

            migrationBuilder.DropColumn(
                name: "DocumentNumber",
                table: "OperationDocuments");

            migrationBuilder.DropColumn(
                name: "ExtractedMetadataJson",
                table: "OperationDocuments");

            migrationBuilder.DropColumn(
                name: "FileHash",
                table: "OperationDocuments");

            migrationBuilder.DropColumn(
                name: "Notes",
                table: "OperationDocuments");

            migrationBuilder.DropColumn(
                name: "SourceSystem",
                table: "OperationDocuments");

            migrationBuilder.CreateIndex(
                name: "IX_OperationDocuments_TradeOperationId",
                table: "OperationDocuments",
                column: "TradeOperationId");
        }
    }
}
