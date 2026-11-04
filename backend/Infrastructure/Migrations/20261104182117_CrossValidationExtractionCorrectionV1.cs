using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Sace.Api.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class CrossValidationExtractionCorrectionV1 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "TextExtractedAt",
                table: "OperationDocuments",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "TextExtractionStatus",
                table: "OperationDocuments",
                type: "TEXT",
                maxLength: 32,
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "TextExtractedAt",
                table: "OperationDocuments");

            migrationBuilder.DropColumn(
                name: "TextExtractionStatus",
                table: "OperationDocuments");
        }
    }
}
