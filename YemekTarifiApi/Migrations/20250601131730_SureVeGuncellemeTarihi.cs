using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace YemekTarifiApi.Migrations
{
    /// <inheritdoc />
    public partial class SureVeGuncellemeTarihi : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "GuncellenmeTarihi",
                table: "Tarifler",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<int>(
                name: "PisirmeSuresi",
                table: "Tarifler",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<DateTime>(
                name: "GuncellenmeTarihi",
                table: "Puanlar",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "GuncellenmeTarihi",
                table: "Tarifler");

            migrationBuilder.DropColumn(
                name: "PisirmeSuresi",
                table: "Tarifler");

            migrationBuilder.DropColumn(
                name: "GuncellenmeTarihi",
                table: "Puanlar");
        }
    }
}
