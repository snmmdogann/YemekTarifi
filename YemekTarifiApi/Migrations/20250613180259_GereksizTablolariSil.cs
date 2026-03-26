using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace YemekTarifiApi.Migrations
{
    /// <inheritdoc />
    public partial class GereksizTablolariSil : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Malzemeler");

            migrationBuilder.DropTable(
                name: "TarifMalzemeler");

            migrationBuilder.DropTable(
                name: "YemekPlaniler");

            migrationBuilder.DropTable(
                name: "YemekPlanlari");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Malzemeler",
                columns: table => new
                {
                    MalzemeId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Ad = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    OlcuBirimi = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Malzemeler", x => x.MalzemeId);
                });

            migrationBuilder.CreateTable(
                name: "TarifMalzemeler",
                columns: table => new
                {
                    TarifMalzemeId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    MalzemeId = table.Column<int>(type: "int", nullable: false),
                    Miktar = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    TarifId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TarifMalzemeler", x => x.TarifMalzemeId);
                });

            migrationBuilder.CreateTable(
                name: "YemekPlaniler",
                columns: table => new
                {
                    PlanId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    AksamTarifiId = table.Column<int>(type: "int", nullable: true),
                    Gun = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    KullaniciId = table.Column<int>(type: "int", nullable: false),
                    OgleTarifiId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_YemekPlaniler", x => x.PlanId);
                });

            migrationBuilder.CreateTable(
                name: "YemekPlanlari",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    KullaniciId = table.Column<int>(type: "int", nullable: false),
                    TarifId = table.Column<int>(type: "int", nullable: false),
                    Gun = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Ogun = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_YemekPlanlari", x => x.Id);
                    table.ForeignKey(
                        name: "FK_YemekPlanlari_Kullanicilar_KullaniciId",
                        column: x => x.KullaniciId,
                        principalTable: "Kullanicilar",
                        principalColumn: "KullaniciId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_YemekPlanlari_Tarifler_TarifId",
                        column: x => x.TarifId,
                        principalTable: "Tarifler",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_YemekPlanlari_KullaniciId",
                table: "YemekPlanlari",
                column: "KullaniciId");

            migrationBuilder.CreateIndex(
                name: "IX_YemekPlanlari_TarifId",
                table: "YemekPlanlari",
                column: "TarifId");
        }
    }
}
