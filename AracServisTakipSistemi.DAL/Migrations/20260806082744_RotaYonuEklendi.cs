using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AracServisTakipSistemi.DAL.Migrations
{
    /// <inheritdoc />
    public partial class RotaYonuEklendi : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "Yon",
                table: "Rotalar",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Yon",
                table: "Rotalar");
        }
    }
}
