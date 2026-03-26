using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace YemekTarifiApi.Migrations
{
    /// <inheritdoc />
    public partial class HaftalikPlanGunOgun : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Gunler",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Ad = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Gunler", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Ogunler",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Ad = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Ogunler", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "HaftalikPlanlar",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    KullaniciId = table.Column<int>(type: "int", nullable: false),
                    GunId = table.Column<int>(type: "int", nullable: false),
                    OgunId = table.Column<int>(type: "int", nullable: false),
                    TarifId = table.Column<int>(type: "int", nullable: false),
                    OlusturmaTarihi = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_HaftalikPlanlar", x => x.Id);
                    table.ForeignKey(
                        name: "FK_HaftalikPlanlar_Gunler_GunId",
                        column: x => x.GunId,
                        principalTable: "Gunler",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_HaftalikPlanlar_Kullanicilar_KullaniciId",
                        column: x => x.KullaniciId,
                        principalTable: "Kullanicilar",
                        principalColumn: "KullaniciId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_HaftalikPlanlar_Ogunler_OgunId",
                        column: x => x.OgunId,
                        principalTable: "Ogunler",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_HaftalikPlanlar_Tarifler_TarifId",
                        column: x => x.TarifId,
                        principalTable: "Tarifler",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_HaftalikPlanlar_GunId",
                table: "HaftalikPlanlar",
                column: "GunId");

            migrationBuilder.CreateIndex(
                name: "IX_HaftalikPlanlar_KullaniciId",
                table: "HaftalikPlanlar",
                column: "KullaniciId");

            migrationBuilder.CreateIndex(
                name: "IX_HaftalikPlanlar_OgunId",
                table: "HaftalikPlanlar",
                column: "OgunId");

            migrationBuilder.CreateIndex(
                name: "IX_HaftalikPlanlar_TarifId",
                table: "HaftalikPlanlar",
                column: "TarifId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "HaftalikPlanlar");

            migrationBuilder.DropTable(
                name: "Gunler");

            migrationBuilder.DropTable(
                name: "Ogunler");
        }
    }
}
