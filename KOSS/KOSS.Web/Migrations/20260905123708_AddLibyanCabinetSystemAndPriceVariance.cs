using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace KOSS.Web.Migrations
{
    /// <inheritdoc />
    public partial class AddLibyanCabinetSystemAndPriceVariance : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AttachmentsPath",
                table: "SiteVisits");

            migrationBuilder.AlterColumn<string>(
                name: "Notes",
                table: "SiteVisits",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(1000)",
                oldMaxLength: 1000,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "ApprovedBy",
                table: "SiteVisits",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ApplianceOutletsNotes",
                table: "SiteVisits",
                type: "nvarchar(300)",
                maxLength: 300,
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "CornerAngleDegrees",
                table: "SiteVisits",
                type: "decimal(18,3)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<bool>(
                name: "HasSquareCorners",
                table: "SiteVisits",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<decimal>(
                name: "WindowSillHeightCm",
                table: "SiteVisits",
                type: "decimal(18,3)",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Method",
                table: "Quotations",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "PriceVarianceNotes",
                table: "Quotations",
                type: "nvarchar(1000)",
                maxLength: 1000,
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Category",
                table: "KitchenRequests",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "CabinetUnits",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    KitchenRequestId = table.Column<int>(type: "int", nullable: false),
                    BoxCode = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false),
                    Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Category = table.Column<int>(type: "int", nullable: false),
                    WidthCm = table.Column<decimal>(type: "decimal(18,3)", nullable: false),
                    HeightCm = table.Column<decimal>(type: "decimal(18,3)", nullable: false),
                    DepthCm = table.Column<decimal>(type: "decimal(18,3)", nullable: false),
                    Carcass = table.Column<int>(type: "int", nullable: false),
                    DoorType = table.Column<int>(type: "int", nullable: false),
                    Mechanism = table.Column<int>(type: "int", nullable: false),
                    HasLedLighting = table.Column<bool>(type: "bit", nullable: false),
                    HasGolaProfile = table.Column<bool>(type: "bit", nullable: false),
                    ManufacturingCost = table.Column<decimal>(type: "decimal(18,3)", nullable: false),
                    SellingPrice = table.Column<decimal>(type: "decimal(18,3)", nullable: false),
                    Notes = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CabinetUnits", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CabinetUnits_KitchenRequests_KitchenRequestId",
                        column: x => x.KitchenRequestId,
                        principalTable: "KitchenRequests",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_CabinetUnits_KitchenRequestId",
                table: "CabinetUnits",
                column: "KitchenRequestId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CabinetUnits");

            migrationBuilder.DropColumn(
                name: "ApplianceOutletsNotes",
                table: "SiteVisits");

            migrationBuilder.DropColumn(
                name: "CornerAngleDegrees",
                table: "SiteVisits");

            migrationBuilder.DropColumn(
                name: "HasSquareCorners",
                table: "SiteVisits");

            migrationBuilder.DropColumn(
                name: "WindowSillHeightCm",
                table: "SiteVisits");

            migrationBuilder.DropColumn(
                name: "Method",
                table: "Quotations");

            migrationBuilder.DropColumn(
                name: "PriceVarianceNotes",
                table: "Quotations");

            migrationBuilder.DropColumn(
                name: "Category",
                table: "KitchenRequests");

            migrationBuilder.AlterColumn<string>(
                name: "Notes",
                table: "SiteVisits",
                type: "nvarchar(1000)",
                maxLength: 1000,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(500)",
                oldMaxLength: 500,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "ApprovedBy",
                table: "SiteVisits",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(100)",
                oldMaxLength: 100,
                oldNullable: true);

            migrationBuilder.AddColumn<string>(
                name: "AttachmentsPath",
                table: "SiteVisits",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: true);
        }
    }
}
