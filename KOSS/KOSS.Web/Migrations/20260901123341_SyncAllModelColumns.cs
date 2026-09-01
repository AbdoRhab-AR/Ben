using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace KOSS.Web.Migrations
{
    /// <inheritdoc />
    public partial class SyncAllModelColumns : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_StockTransactions_KitchenRequests_KitchenRequestId",
                table: "StockTransactions");

            migrationBuilder.DropIndex(
                name: "IX_StockTransactions_KitchenRequestId",
                table: "StockTransactions");

            migrationBuilder.DropColumn(
                name: "PlannedStartDate",
                table: "WorkOrders");

            migrationBuilder.DropColumn(
                name: "Priority",
                table: "WorkOrders");

            migrationBuilder.DropColumn(
                name: "InQuantity",
                table: "StockTransactions");

            migrationBuilder.DropColumn(
                name: "KitchenRequestId",
                table: "StockTransactions");

            migrationBuilder.DropColumn(
                name: "AcceptedAt",
                table: "Quotations");

            migrationBuilder.DropColumn(
                name: "SentToCustomerAt",
                table: "Quotations");

            migrationBuilder.DropColumn(
                name: "Description",
                table: "QuotationItems");

            migrationBuilder.DropColumn(
                name: "CustomerReceiptId",
                table: "PaymentSchedules");

            migrationBuilder.DropColumn(
                name: "ItemCode",
                table: "MaterialRequirements");

            migrationBuilder.DropColumn(
                name: "RenderImagesPaths",
                table: "DesignVersions");

            migrationBuilder.DropColumn(
                name: "Email",
                table: "Customers");

            migrationBuilder.DropColumn(
                name: "LeadSource",
                table: "Customers");

            migrationBuilder.DropColumn(
                name: "NationalOrTaxId",
                table: "Customers");

            migrationBuilder.DropColumn(
                name: "Phone2",
                table: "Customers");

            migrationBuilder.DropColumn(
                name: "LostReason",
                table: "CustomerInquiries");

            migrationBuilder.DropColumn(
                name: "PreferredContactTime",
                table: "CustomerInquiries");

            migrationBuilder.DropColumn(
                name: "PenaltyPerDay",
                table: "Contracts");

            migrationBuilder.DropColumn(
                name: "SignedContractFilePath",
                table: "Contracts");

            migrationBuilder.RenameColumn(
                name: "Notes",
                table: "WorkOrders",
                newName: "ManufacturingNotes");

            migrationBuilder.RenameColumn(
                name: "OutQuantity",
                table: "StockTransactions",
                newName: "Quantity");

            migrationBuilder.RenameColumn(
                name: "QuantityReserved",
                table: "MaterialRequirements",
                newName: "QuantityInStock");

            migrationBuilder.RenameColumn(
                name: "DesignFilePath",
                table: "DesignVersions",
                newName: "RenderFilesPath");

            migrationBuilder.RenameColumn(
                name: "Notes",
                table: "Contracts",
                newName: "SpecialTerms");

            migrationBuilder.AlterColumn<string>(
                name: "OrderNumber",
                table: "WorkOrders",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(50)",
                oldMaxLength: 50,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "ReferenceNumber",
                table: "StockTransactions",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(100)",
                oldMaxLength: 100,
                oldNullable: true);

            migrationBuilder.AlterColumn<decimal>(
                name: "WallLength3Cm",
                table: "SiteVisits",
                type: "decimal(18,3)",
                nullable: false,
                defaultValue: 0m,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,3)",
                oldNullable: true);

            migrationBuilder.AlterColumn<decimal>(
                name: "WallLength2Cm",
                table: "SiteVisits",
                type: "decimal(18,3)",
                nullable: false,
                defaultValue: 0m,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,3)",
                oldNullable: true);

            migrationBuilder.AlterColumn<decimal>(
                name: "WallLength1Cm",
                table: "SiteVisits",
                type: "decimal(18,3)",
                nullable: false,
                defaultValue: 0m,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,3)",
                oldNullable: true);

            migrationBuilder.AlterColumn<DateTime>(
                name: "ScheduledDate",
                table: "SiteVisits",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified),
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldNullable: true);

            migrationBuilder.AlterColumn<decimal>(
                name: "EstimatedAreaM2",
                table: "SiteVisits",
                type: "decimal(18,3)",
                nullable: false,
                defaultValue: 0m,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,3)",
                oldNullable: true);

            migrationBuilder.AlterColumn<decimal>(
                name: "CeilingHeightCm",
                table: "SiteVisits",
                type: "decimal(18,3)",
                nullable: false,
                defaultValue: 0m,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,3)",
                oldNullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CreatedBy",
                table: "SiteVisits",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Notes",
                table: "SiteVisits",
                type: "nvarchar(1000)",
                maxLength: 1000,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "StructuralObstacles",
                table: "SiteVisits",
                type: "nvarchar(300)",
                maxLength: 300,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Notes",
                table: "QuotationItems",
                type: "nvarchar(250)",
                maxLength: 250,
                nullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Unit",
                table: "MaterialRequirements",
                type: "nvarchar(30)",
                maxLength: 30,
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(30)",
                oldMaxLength: 30,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Category",
                table: "MaterialRequirements",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(100)",
                oldMaxLength: 100,
                oldNullable: true);

            migrationBuilder.AlterColumn<decimal>(
                name: "EstimatedLinearMeters",
                table: "DesignVersions",
                type: "decimal(18,3)",
                nullable: false,
                defaultValue: 0m,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,3)",
                oldNullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "ApprovedAt",
                table: "DesignVersions",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "VersionCode",
                table: "DesignVersions",
                type: "nvarchar(20)",
                maxLength: 20,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AlterColumn<string>(
                name: "Address",
                table: "Customers",
                type: "nvarchar(250)",
                maxLength: 250,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(300)",
                oldMaxLength: 300,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Notes",
                table: "CustomerInquiries",
                type: "nvarchar(1000)",
                maxLength: 1000,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(500)",
                oldMaxLength: 500,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Location",
                table: "CustomerInquiries",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(200)",
                oldMaxLength: 200,
                oldNullable: true);

            migrationBuilder.AlterColumn<DateTime>(
                name: "SignedDate",
                table: "Contracts",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified),
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "ContractNumber",
                table: "Contracts",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(50)",
                oldMaxLength: 50,
                oldNullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CreatedBy",
                table: "SiteVisits");

            migrationBuilder.DropColumn(
                name: "Notes",
                table: "SiteVisits");

            migrationBuilder.DropColumn(
                name: "StructuralObstacles",
                table: "SiteVisits");

            migrationBuilder.DropColumn(
                name: "Notes",
                table: "QuotationItems");

            migrationBuilder.DropColumn(
                name: "ApprovedAt",
                table: "DesignVersions");

            migrationBuilder.DropColumn(
                name: "VersionCode",
                table: "DesignVersions");

            migrationBuilder.RenameColumn(
                name: "ManufacturingNotes",
                table: "WorkOrders",
                newName: "Notes");

            migrationBuilder.RenameColumn(
                name: "Quantity",
                table: "StockTransactions",
                newName: "OutQuantity");

            migrationBuilder.RenameColumn(
                name: "QuantityInStock",
                table: "MaterialRequirements",
                newName: "QuantityReserved");

            migrationBuilder.RenameColumn(
                name: "RenderFilesPath",
                table: "DesignVersions",
                newName: "DesignFilePath");

            migrationBuilder.RenameColumn(
                name: "SpecialTerms",
                table: "Contracts",
                newName: "Notes");

            migrationBuilder.AlterColumn<string>(
                name: "OrderNumber",
                table: "WorkOrders",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(50)",
                oldMaxLength: 50);

            migrationBuilder.AddColumn<DateTime>(
                name: "PlannedStartDate",
                table: "WorkOrders",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Priority",
                table: "WorkOrders",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AlterColumn<string>(
                name: "ReferenceNumber",
                table: "StockTransactions",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(50)",
                oldMaxLength: 50,
                oldNullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "InQuantity",
                table: "StockTransactions",
                type: "decimal(18,3)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<int>(
                name: "KitchenRequestId",
                table: "StockTransactions",
                type: "int",
                nullable: true);

            migrationBuilder.AlterColumn<decimal>(
                name: "WallLength3Cm",
                table: "SiteVisits",
                type: "decimal(18,3)",
                nullable: true,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,3)");

            migrationBuilder.AlterColumn<decimal>(
                name: "WallLength2Cm",
                table: "SiteVisits",
                type: "decimal(18,3)",
                nullable: true,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,3)");

            migrationBuilder.AlterColumn<decimal>(
                name: "WallLength1Cm",
                table: "SiteVisits",
                type: "decimal(18,3)",
                nullable: true,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,3)");

            migrationBuilder.AlterColumn<DateTime>(
                name: "ScheduledDate",
                table: "SiteVisits",
                type: "datetime2",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "datetime2");

            migrationBuilder.AlterColumn<decimal>(
                name: "EstimatedAreaM2",
                table: "SiteVisits",
                type: "decimal(18,3)",
                nullable: true,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,3)");

            migrationBuilder.AlterColumn<decimal>(
                name: "CeilingHeightCm",
                table: "SiteVisits",
                type: "decimal(18,3)",
                nullable: true,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,3)");

            migrationBuilder.AddColumn<DateTime>(
                name: "AcceptedAt",
                table: "Quotations",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "SentToCustomerAt",
                table: "Quotations",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Description",
                table: "QuotationItems",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "CustomerReceiptId",
                table: "PaymentSchedules",
                type: "int",
                nullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Unit",
                table: "MaterialRequirements",
                type: "nvarchar(30)",
                maxLength: 30,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(30)",
                oldMaxLength: 30);

            migrationBuilder.AlterColumn<string>(
                name: "Category",
                table: "MaterialRequirements",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(100)",
                oldMaxLength: 100);

            migrationBuilder.AddColumn<string>(
                name: "ItemCode",
                table: "MaterialRequirements",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: true);

            migrationBuilder.AlterColumn<decimal>(
                name: "EstimatedLinearMeters",
                table: "DesignVersions",
                type: "decimal(18,3)",
                nullable: true,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,3)");

            migrationBuilder.AddColumn<string>(
                name: "RenderImagesPaths",
                table: "DesignVersions",
                type: "nvarchar(1000)",
                maxLength: 1000,
                nullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Address",
                table: "Customers",
                type: "nvarchar(300)",
                maxLength: 300,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(250)",
                oldMaxLength: 250,
                oldNullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Email",
                table: "Customers",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "LeadSource",
                table: "Customers",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "NationalOrTaxId",
                table: "Customers",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Phone2",
                table: "Customers",
                type: "nvarchar(30)",
                maxLength: 30,
                nullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Notes",
                table: "CustomerInquiries",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(1000)",
                oldMaxLength: 1000,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Location",
                table: "CustomerInquiries",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(100)",
                oldMaxLength: 100,
                oldNullable: true);

            migrationBuilder.AddColumn<string>(
                name: "LostReason",
                table: "CustomerInquiries",
                type: "nvarchar(300)",
                maxLength: 300,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PreferredContactTime",
                table: "CustomerInquiries",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: true);

            migrationBuilder.AlterColumn<DateTime>(
                name: "SignedDate",
                table: "Contracts",
                type: "datetime2",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "datetime2");

            migrationBuilder.AlterColumn<string>(
                name: "ContractNumber",
                table: "Contracts",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(50)",
                oldMaxLength: 50);

            migrationBuilder.AddColumn<decimal>(
                name: "PenaltyPerDay",
                table: "Contracts",
                type: "decimal(18,3)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<string>(
                name: "SignedContractFilePath",
                table: "Contracts",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_StockTransactions_KitchenRequestId",
                table: "StockTransactions",
                column: "KitchenRequestId");

            migrationBuilder.AddForeignKey(
                name: "FK_StockTransactions_KitchenRequests_KitchenRequestId",
                table: "StockTransactions",
                column: "KitchenRequestId",
                principalTable: "KitchenRequests",
                principalColumn: "Id");
        }
    }
}
