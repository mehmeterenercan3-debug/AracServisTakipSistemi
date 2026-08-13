using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AracServisTakipSistemi.DAL.Migrations
{
    /// <inheritdoc />
    public partial class SirketAyarTablosuEklendi : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "SirketAyarlari",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    Ad = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Enlem = table.Column<double>(type: "double", nullable: false),
                    Boylam = table.Column<double>(type: "double", nullable: false),
                    GidisVarisTamponDk = table.Column<int>(type: "int", nullable: false),
                    DonusKalkisTamponDk = table.Column<int>(type: "int", nullable: false),
                    MaksimumBolgeMesafesiKm = table.Column<double>(type: "double", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SirketAyarlari", x => x.Id);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.InsertData(
                table: "SirketAyarlari",
                columns: new[] { "Id", "Ad", "Boylam", "DonusKalkisTamponDk", "Enlem", "GidisVarisTamponDk", "MaksimumBolgeMesafesiKm" },
                values: new object[] { 1, "Merkez Ofis", 28.889399539926625, 15, 41.020184446800286, 15, 100.0 });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "SirketAyarlari");
        }
    }
}
