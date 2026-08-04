using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AracServisTakipSistemi.DAL.Migrations
{
    /// <inheritdoc />
    public partial class GeocodeVeRiskDuzeltmeleri : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "GeocodeBasariliMi",
                table: "PersonelAdresleri",
                type: "tinyint(1)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "GeocodeKaynagi",
                table: "PersonelAdresleri",
                type: "longtext",
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "GeocodeBasariliMi",
                table: "PersonelAdresleri");

            migrationBuilder.DropColumn(
                name: "GeocodeKaynagi",
                table: "PersonelAdresleri");
        }
    }
}
