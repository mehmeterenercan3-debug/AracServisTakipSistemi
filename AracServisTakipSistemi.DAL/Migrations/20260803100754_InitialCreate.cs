using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AracServisTakipSistemi.DAL.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterDatabase()
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "SemtReferanslari",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    SemtAdi = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    MerkezEnlem = table.Column<double>(type: "double", nullable: false),
                    MerkezBoylam = table.Column<double>(type: "double", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SemtReferanslari", x => x.Id);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "Vardiyalar",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    VardiyaAdi = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    BaslangicSaati = table.Column<TimeSpan>(type: "time(6)", nullable: false),
                    BitisSaati = table.Column<TimeSpan>(type: "time(6)", nullable: false),
                    AktifMi = table.Column<bool>(type: "tinyint(1)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Vardiyalar", x => x.Id);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "Personeller",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    AdSoyad = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    SicilNo = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    PersonelTuru = table.Column<int>(type: "int", nullable: false),
                    Cinsiyet = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    DogumTarihi = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    AnneAdi = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    BabaAdi = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    KanGrubu = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Telefon = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Eposta = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    AktifMi = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    IseGirisTarihi = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    IstenCikisTarihi = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    ErpPersonelKodu = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    ErpyeAktarildiMi = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    SubeId = table.Column<int>(type: "int", nullable: true),
                    VardiyaId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Personeller", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Personeller_Vardiyalar_VardiyaId",
                        column: x => x.VardiyaId,
                        principalTable: "Vardiyalar",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "Araclar",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    Plaka = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Marka = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Model = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    AracTipi = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    ModelYili = table.Column<int>(type: "int", nullable: false),
                    GuncelKm = table.Column<double>(type: "double", nullable: false),
                    SatinAlmaTarihi = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    KapasiteSayisi = table.Column<int>(type: "int", nullable: false),
                    AktifMi = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    BakimdaMi = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    ErpAracKartNo = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    ErpyeAktarildiMi = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    SasiNo = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    MotorNo = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    MuayeneTarihi = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    SigortaBitisTarihi = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    SubeId = table.Column<int>(type: "int", nullable: true),
                    SoforPersonelId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Araclar", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Araclar_Personeller_SoforPersonelId",
                        column: x => x.SoforPersonelId,
                        principalTable: "Personeller",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "PersonelAdresleri",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    PersonelId = table.Column<int>(type: "int", nullable: false),
                    AdresTuru = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    BaslangicTarihi = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    BitisTarihi = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    AktifMi = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    Mahalle = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Semt = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    IlceAdi = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Sehir = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Sokak = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    ApartmanNo = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    DisKapiNo = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Enlem = table.Column<double>(type: "double", nullable: true),
                    Boylam = table.Column<double>(type: "double", nullable: true),
                    GeocodeTarihi = table.Column<DateTime>(type: "datetime(6)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PersonelAdresleri", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PersonelAdresleri_Personeller_PersonelId",
                        column: x => x.PersonelId,
                        principalTable: "Personeller",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "AracArizaKayitlari",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    AracId = table.Column<int>(type: "int", nullable: false),
                    ArizaTarihi = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    ArizaTuru = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Aciklama = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    OnarimMaliyeti = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    ArizaAnindakiKm = table.Column<double>(type: "double", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AracArizaKayitlari", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AracArizaKayitlari_Araclar_AracId",
                        column: x => x.AracId,
                        principalTable: "Araclar",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "AracAtamalari",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    AracId = table.Column<int>(type: "int", nullable: false),
                    PersonelId = table.Column<int>(type: "int", nullable: false),
                    AtamaTarihi = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    AtamaBitisTarihi = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    AktifMi = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    ErpyeAktarildiMi = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    ErpKayitId = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4")
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

            migrationBuilder.CreateTable(
                name: "BakimKayitlari",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    AracId = table.Column<int>(type: "int", nullable: false),
                    BakimTarihi = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    BakimTuru = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    SonrakiBakimKm = table.Column<int>(type: "int", nullable: true),
                    SonrakiBakimTarihi = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    Maliyet = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    Aciklama = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BakimKayitlari", x => x.Id);
                    table.ForeignKey(
                        name: "FK_BakimKayitlari_Araclar_AracId",
                        column: x => x.AracId,
                        principalTable: "Araclar",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "RiskSkorlari",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    AracId = table.Column<int>(type: "int", nullable: false),
                    SkorDegeri = table.Column<decimal>(type: "decimal(5,2)", precision: 5, scale: 2, nullable: false),
                    RiskSeviyesi = table.Column<int>(type: "int", nullable: false),
                    HesaplamaTarihi = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    OnerilenAksiyon = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RiskSkorlari", x => x.Id);
                    table.ForeignKey(
                        name: "FK_RiskSkorlari_Araclar_AracId",
                        column: x => x.AracId,
                        principalTable: "Araclar",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "Rotalar",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    AracId = table.Column<int>(type: "int", nullable: false),
                    RotaTarihi = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    ToplamMesafeKm = table.Column<double>(type: "double", nullable: false),
                    TahminiSureDk = table.Column<int>(type: "int", nullable: false),
                    AktifMi = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    ErpyeAktarildiMi = table.Column<bool>(type: "tinyint(1)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Rotalar", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Rotalar_Araclar_AracId",
                        column: x => x.AracId,
                        principalTable: "Araclar",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "RotaDuraklari",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    RotaId = table.Column<int>(type: "int", nullable: false),
                    PersonelId = table.Column<int>(type: "int", nullable: false),
                    SiraNo = table.Column<int>(type: "int", nullable: false),
                    TahminiVarisSaati = table.Column<TimeSpan>(type: "time(6)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RotaDuraklari", x => x.Id);
                    table.ForeignKey(
                        name: "FK_RotaDuraklari_Personeller_PersonelId",
                        column: x => x.PersonelId,
                        principalTable: "Personeller",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_RotaDuraklari_Rotalar_RotaId",
                        column: x => x.RotaId,
                        principalTable: "Rotalar",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.InsertData(
                table: "Vardiyalar",
                columns: new[] { "Id", "AktifMi", "BaslangicSaati", "BitisSaati", "VardiyaAdi" },
                values: new object[] { 1, true, new TimeSpan(0, 8, 30, 0, 0), new TimeSpan(0, 18, 0, 0, 0), "Sabah" });

            migrationBuilder.CreateIndex(
                name: "IX_AracArizaKayitlari_AracId",
                table: "AracArizaKayitlari",
                column: "AracId");

            migrationBuilder.CreateIndex(
                name: "IX_AracAtamalari_AracId",
                table: "AracAtamalari",
                column: "AracId");

            migrationBuilder.CreateIndex(
                name: "IX_AracAtamalari_PersonelId",
                table: "AracAtamalari",
                column: "PersonelId");

            migrationBuilder.CreateIndex(
                name: "IX_Araclar_SoforPersonelId",
                table: "Araclar",
                column: "SoforPersonelId");

            migrationBuilder.CreateIndex(
                name: "IX_BakimKayitlari_AracId",
                table: "BakimKayitlari",
                column: "AracId");

            migrationBuilder.CreateIndex(
                name: "IX_PersonelAdresleri_PersonelId",
                table: "PersonelAdresleri",
                column: "PersonelId");

            migrationBuilder.CreateIndex(
                name: "IX_Personeller_VardiyaId",
                table: "Personeller",
                column: "VardiyaId");

            migrationBuilder.CreateIndex(
                name: "IX_RiskSkorlari_AracId",
                table: "RiskSkorlari",
                column: "AracId");

            migrationBuilder.CreateIndex(
                name: "IX_RotaDuraklari_PersonelId",
                table: "RotaDuraklari",
                column: "PersonelId");

            migrationBuilder.CreateIndex(
                name: "IX_RotaDuraklari_RotaId",
                table: "RotaDuraklari",
                column: "RotaId");

            migrationBuilder.CreateIndex(
                name: "IX_Rotalar_AracId",
                table: "Rotalar",
                column: "AracId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AracArizaKayitlari");

            migrationBuilder.DropTable(
                name: "AracAtamalari");

            migrationBuilder.DropTable(
                name: "BakimKayitlari");

            migrationBuilder.DropTable(
                name: "PersonelAdresleri");

            migrationBuilder.DropTable(
                name: "RiskSkorlari");

            migrationBuilder.DropTable(
                name: "RotaDuraklari");

            migrationBuilder.DropTable(
                name: "SemtReferanslari");

            migrationBuilder.DropTable(
                name: "Rotalar");

            migrationBuilder.DropTable(
                name: "Araclar");

            migrationBuilder.DropTable(
                name: "Personeller");

            migrationBuilder.DropTable(
                name: "Vardiyalar");
        }
    }
}
