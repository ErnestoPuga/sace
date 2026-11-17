using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Sace.Api.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class InvoiceCoveReconciliationV2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "CalculationJson",
                table: "DocumentReconciliationResults",
                type: "TEXT",
                nullable: false,
                defaultValue: "[]");

            migrationBuilder.AddColumn<string>(
                name: "PairingExplanation",
                table: "DocumentReconciliationResults",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PairingMethod",
                table: "DocumentReconciliationResults",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PairingReasonCode",
                table: "DocumentReconciliationResults",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PairingStatus",
                table: "DocumentReconciliationResults",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "DeferredValidationTarget",
                table: "DocumentReconciliationProfileFields",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "DisplayName",
                table: "DocumentReconciliationProfileFields",
                type: "TEXT",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<bool>(
                name: "ExpectedInLeft",
                table: "DocumentReconciliationProfileFields",
                type: "INTEGER",
                nullable: false,
                defaultValue: true);

            migrationBuilder.AddColumn<bool>(
                name: "ExpectedInRight",
                table: "DocumentReconciliationProfileFields",
                type: "INTEGER",
                nullable: false,
                defaultValue: true);

            migrationBuilder.AddColumn<string>(
                name: "Treatment",
                table: "DocumentReconciliationProfileFields",
                type: "TEXT",
                nullable: false,
                defaultValue: "DIRECT_COMPARE");

            migrationBuilder.AddColumn<string>(
                name: "DeferredValidationTarget",
                table: "DocumentReconciliationFieldResults",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "DisplayName",
                table: "DocumentReconciliationFieldResults",
                type: "TEXT",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "FunctionalOutcome",
                table: "DocumentReconciliationFieldResults",
                type: "TEXT",
                nullable: false,
                defaultValue: "NO_VERIFICABLE");

            migrationBuilder.AddColumn<string>(
                name: "LeftLocation",
                table: "DocumentReconciliationFieldResults",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ReasonCode",
                table: "DocumentReconciliationFieldResults",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "RightLocation",
                table: "DocumentReconciliationFieldResults",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Treatment",
                table: "DocumentReconciliationFieldResults",
                type: "TEXT",
                nullable: false,
                defaultValue: "DIRECT_COMPARE");

            migrationBuilder.AddColumn<string>(
                name: "Brand",
                table: "DocumentLineItems",
                type: "TEXT",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CalculationJson",
                table: "DocumentReconciliationResults");

            migrationBuilder.DropColumn(
                name: "PairingExplanation",
                table: "DocumentReconciliationResults");

            migrationBuilder.DropColumn(
                name: "PairingMethod",
                table: "DocumentReconciliationResults");

            migrationBuilder.DropColumn(
                name: "PairingReasonCode",
                table: "DocumentReconciliationResults");

            migrationBuilder.DropColumn(
                name: "PairingStatus",
                table: "DocumentReconciliationResults");

            migrationBuilder.DropColumn(
                name: "DeferredValidationTarget",
                table: "DocumentReconciliationProfileFields");

            migrationBuilder.DropColumn(
                name: "DisplayName",
                table: "DocumentReconciliationProfileFields");

            migrationBuilder.DropColumn(
                name: "ExpectedInLeft",
                table: "DocumentReconciliationProfileFields");

            migrationBuilder.DropColumn(
                name: "ExpectedInRight",
                table: "DocumentReconciliationProfileFields");

            migrationBuilder.DropColumn(
                name: "Treatment",
                table: "DocumentReconciliationProfileFields");

            migrationBuilder.DropColumn(
                name: "DeferredValidationTarget",
                table: "DocumentReconciliationFieldResults");

            migrationBuilder.DropColumn(
                name: "DisplayName",
                table: "DocumentReconciliationFieldResults");

            migrationBuilder.DropColumn(
                name: "FunctionalOutcome",
                table: "DocumentReconciliationFieldResults");

            migrationBuilder.DropColumn(
                name: "LeftLocation",
                table: "DocumentReconciliationFieldResults");

            migrationBuilder.DropColumn(
                name: "ReasonCode",
                table: "DocumentReconciliationFieldResults");

            migrationBuilder.DropColumn(
                name: "RightLocation",
                table: "DocumentReconciliationFieldResults");

            migrationBuilder.DropColumn(
                name: "Treatment",
                table: "DocumentReconciliationFieldResults");

            migrationBuilder.DropColumn(
                name: "Brand",
                table: "DocumentLineItems");
        }
    }
}
