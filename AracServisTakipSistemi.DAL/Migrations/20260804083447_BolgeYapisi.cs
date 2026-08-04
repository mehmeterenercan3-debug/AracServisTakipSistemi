using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AracServisTakipSistemi.DAL.Migrations
{
    /// <inheritdoc />
    public partial class BolgeYapisi : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_RotaDuraklari_Personeller_PersonelId",
                table: "RotaDuraklari");

            migrationBuilder.DropTable(
                name: "AracAtamalari");

            migrationBuilder.RenameColumn(
                name: "AdSoyad",
                table: "Personeller",
                newName: "Soyad");

            migrationBuilder.AddColumn<int>(
                name: "Durum",
                table: "Rotalar",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "Ad",
                table: "Personeller",
                type: "longtext",
                nullable: false)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<int>(
                name: "BolgeId",
                table: "Personeller",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "ServisDurumu",
                table: "Personeller",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "Bolgeler",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    BolgeKodu = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    BolgeAdi = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    MerkezEnlem = table.Column<double>(type: "double", nullable: true),
                    MerkezBoylam = table.Column<double>(type: "double", nullable: true),
                    MinPersonelEsigi = table.Column<int>(type: "int", nullable: false),
                    KapasiteTamponu = table.Column<int>(type: "int", nullable: false),
                    Aciklama = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    AktifMi = table.Column<bool>(type: "tinyint(1)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Bolgeler", x => x.Id);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "RotaBolgeleri",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    RotaId = table.Column<int>(type: "int", nullable: false),
                    BolgeId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RotaBolgeleri", x => x.Id);
                    table.ForeignKey(
                        name: "FK_RotaBolgeleri_Bolgeler_BolgeId",
                        column: x => x.BolgeId,
                        principalTable: "Bolgeler",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_RotaBolgeleri_Rotalar_RotaId",
                        column: x => x.RotaId,
                        principalTable: "Rotalar",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateIndex(
                name: "IX_Personeller_BolgeId",
                table: "Personeller",
                column: "BolgeId");

            migrationBuilder.CreateIndex(
                name: "IX_RotaBolgeleri_BolgeId",
                table: "RotaBolgeleri",
                column: "BolgeId");

            migrationBuilder.CreateIndex(
                name: "IX_RotaBolgeleri_RotaId",
                table: "RotaBolgeleri",
                column: "RotaId");

            migrationBuilder.AddForeignKey(
                name: "FK_Personeller_Bolgeler_BolgeId",
                table: "Personeller",
                column: "BolgeId",
                principalTable: "Bolgeler",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "FK_RotaDuraklari_Personeller_PersonelId",
                table: "RotaDuraklari",
                column: "PersonelId",
                principalTable: "Personeller",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Personeller_Bolgeler_BolgeId",
                table: "Personeller");

            migrationBuilder.DropForeignKey(
                name: "FK_RotaDuraklari_Personeller_PersonelId",
                table: "RotaDuraklari");

            migrationBuilder.DropTable(
                name: "RotaBolgeleri");

            migrationBuilder.DropTable(
                name: "Bolgeler");

            migrationBuilder.DropIndex(
                name: "IX_Personeller_BolgeId",
                table: "Personeller");

            migrationBuilder.DropColumn(
                name: "Durum",
                table: "Rotalar");

            migrationBuilder.DropColumn(
                name: "Ad",
                table: "Personeller");

            migrationBuilder.DropColumn(
                name: "BolgeId",
                table: "Personeller");

            migrationBuilder.DropColumn(
                name: "ServisDurumu",
                table: "Personeller");

            migrationBuilder.RenameColumn(
                name: "Soyad",
                table: "Personeller",
                newName: "AdSoyad");

            migrationBuilder.CreateTable(
                name: "AracAtamalari",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    AracId = table.Column<int>(type: "int", nullable: false),
                    PersonelId = table.Column<int>(type: "int", nullable: false),
                    AktifMi = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    AtamaBitisTarihi = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    AtamaTarihi = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    ErpKayitId = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    ErpyeAktarildiMi = table.Column<bool>(type: "tinyint(1)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AracAtamalari", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AracAtamalari_Araclar_AracId",
                        column: x => x.AracId,
                        principalTable: "Araclar",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_AracAtamalari_Personeller_PersonelId",
                        column: x => x.PersonelId,
                        principalTable: "Personeller",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateIndex(
                name: "IX_AracAtamalari_AracId",
                table: "AracAtamalari",
                column: "AracId");

            migrationBuilder.CreateIndex(
                name: "IX_AracAtamalari_PersonelId",
                table: "AracAtamalari",
                column: "PersonelId");

            migrationBuilder.AddForeignKey(
                name: "FK_RotaDuraklari_Personeller_PersonelId",
                table: "RotaDuraklari",
                column: "PersonelId",
                principalTable: "Personeller",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
