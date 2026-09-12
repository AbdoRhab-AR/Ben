using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace KOSS.Web.Migrations
{
    /// <inheritdoc />
    public partial class AddPricingSettingsTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "PricingSettings",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    MeterRateMelamine = table.Column<decimal>(type: "decimal(18,3)", nullable: false),
                    MeterRateAcrylic = table.Column<decimal>(type: "decimal(18,3)", nullable: false),
                    MeterRatePolylac = table.Column<decimal>(type: "decimal(18,3)", nullable: false),
                    MeterRateAlumGlass = table.Column<decimal>(type: "decimal(18,3)", nullable: false),
                    MeterRateOpenDressing = table.Column<decimal>(type: "decimal(18,3)", nullable: false),
                    CarcassRateWhiteMdf = table.Column<decimal>(type: "decimal(18,3)", nullable: false),
                    CarcassRateGreenHmr = table.Column<decimal>(type: "decimal(18,3)", nullable: false),
                    CarcassRatePlywood = table.Column<decimal>(type: "decimal(18,3)", nullable: false),
                    CarcassRateChipboard = table.Column<decimal>(type: "decimal(18,3)", nullable: false),
                    DoorRateMelamine = table.Column<decimal>(type: "decimal(18,3)", nullable: false),
                    DoorRateAcrylic = table.Column<decimal>(type: "decimal(18,3)", nullable: false),
                    DoorRatePolylac = table.Column<decimal>(type: "decimal(18,3)", nullable: false),
                    DoorRateAlumGlass = table.Column<decimal>(type: "decimal(18,3)", nullable: false),
                    PriceStandardHinges = table.Column<decimal>(type: "decimal(18,3)", nullable: false),
                    PriceBlumSoftClose = table.Column<decimal>(type: "decimal(18,3)", nullable: false),
                    PriceBlumAventos = table.Column<decimal>(type: "decimal(18,3)", nullable: false),
                    PriceMagicCorner = table.Column<decimal>(type: "decimal(18,3)", nullable: false),
                    PriceTandemBox = table.Column<decimal>(type: "decimal(18,3)", nullable: false),
                    PriceJewelryOrganizer = table.Column<decimal>(type: "decimal(18,3)", nullable: false),
                    PriceTrouserRack = table.Column<decimal>(type: "decimal(18,3)", nullable: false),
                    PriceSpiceRack = table.Column<decimal>(type: "decimal(18,3)", nullable: false),
                    PriceCountertopArtificial = table.Column<decimal>(type: "decimal(18,3)", nullable: false),
                    PriceCountertopQuartz = table.Column<decimal>(type: "decimal(18,3)", nullable: false),
                    PriceCountertopDekton = table.Column<decimal>(type: "decimal(18,3)", nullable: false),
                    PriceCountertopGranite = table.Column<decimal>(type: "decimal(18,3)", nullable: false),
                    PriceLedAddon = table.Column<decimal>(type: "decimal(18,3)", nullable: false),
                    PriceGolaAddon = table.Column<decimal>(type: "decimal(18,3)", nullable: false),
                    ProfitMarginPercentage = table.Column<decimal>(type: "decimal(18,3)", nullable: false),
                    LastUpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedBy = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PricingSettings", x => x.Id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "PricingSettings");
        }
    }
}
