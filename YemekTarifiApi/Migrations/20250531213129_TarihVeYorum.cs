using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace YemekTarifiApi.Migrations
{
    /// <inheritdoc />
    public partial class TarihVeYorum : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ResimYolu",
                table: "Tarifler",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<DateTime>(
                name: "EklenmeTarihi",
                table: "Puanlar",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "EklenmeTarihi",
                table: "Favoriler",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.CreateIndex(
                name: "IX_Puanlar_KullaniciId",
                table: "Puanlar",
                column: "KullaniciId");

            migrationBuilder.CreateIndex(
                name: "IX_Puanlar_TarifId",
                table: "Puanlar",
                column: "TarifId");

            migrationBuilder.CreateIndex(
                name: "IX_Favoriler_KullaniciId",
                table: "Favoriler",
                column: "KullaniciId");

            migrationBuilder.CreateIndex(
                name: "IX_Favoriler_TarifId",
                table: "Favoriler",
                column: "TarifId");

            migrationBuilder.AddForeignKey(
                name: "FK_Favoriler_Kullanicilar_KullaniciId",
                table: "Favoriler",
                column: "KullaniciId",
                principalTable: "Kullanicilar",
                principalColumn: "KullaniciId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Favoriler_Tarifler_TarifId",
                table: "Favoriler",
                column: "TarifId",
                principalTable: "Tarifler",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Puanlar_Kullanicilar_KullaniciId",
                table: "Puanlar",
                column: "KullaniciId",
                principalTable: "Kullanicilar",
                principalColumn: "KullaniciId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Puanlar_Tarifler_TarifId",
                table: "Puanlar",
                column: "TarifId",
                principalTable: "Tarifler",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Favoriler_Kullanicilar_KullaniciId",
                table: "Favoriler");

            migrationBuilder.DropForeignKey(
                name: "FK_Favoriler_Tarifler_TarifId",
                table: "Favoriler");

            migrationBuilder.DropForeignKey(
                name: "FK_Puanlar_Kullanicilar_KullaniciId",
                table: "Puanlar");

            migrationBuilder.DropForeignKey(
                name: "FK_Puanlar_Tarifler_TarifId",
                table: "Puanlar");

            migrationBuilder.DropIndex(
                name: "IX_Puanlar_KullaniciId",
                table: "Puanlar");

            migrationBuilder.DropIndex(
                name: "IX_Puanlar_TarifId",
                table: "Puanlar");

            migrationBuilder.DropIndex(
                name: "IX_Favoriler_KullaniciId",
                table: "Favoriler");

            migrationBuilder.DropIndex(
                name: "IX_Favoriler_TarifId",
                table: "Favoriler");

            migrationBuilder.DropColumn(
                name: "ResimYolu",
                table: "Tarifler");

            migrationBuilder.DropColumn(
                name: "EklenmeTarihi",
                table: "Puanlar");

            migrationBuilder.DropColumn(
                name: "EklenmeTarihi",
                table: "Favoriler");
        }
    }
}
