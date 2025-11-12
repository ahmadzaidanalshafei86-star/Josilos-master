#nullable disable

namespace ES.Infrastructure.Migrations
{
    public partial class AddLabelTabel : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "ProductLabelId",
                table: "Products",
                type: "int",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "ProductLabels",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    LanguageId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProductLabels", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ProductLabels_Languages_LanguageId",
                        column: x => x.LanguageId,
                        principalTable: "Languages",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "ProductLabelTranslations",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ProductLabelId = table.Column<int>(type: "int", nullable: false),
                    LanguageId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProductLabelTranslations", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ProductLabelTranslations_Languages_LanguageId",
                        column: x => x.LanguageId,
                        principalTable: "Languages",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_ProductLabelTranslations_ProductLabels_ProductLabelId",
                        column: x => x.ProductLabelId,
                        principalTable: "ProductLabels",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Products_ProductLabelId",
                table: "Products",
                column: "ProductLabelId");

            migrationBuilder.CreateIndex(
                name: "IX_ProductLabels_LanguageId",
                table: "ProductLabels",
                column: "LanguageId");

            migrationBuilder.CreateIndex(
                name: "IX_ProductLabelTranslations_LanguageId",
                table: "ProductLabelTranslations",
                column: "LanguageId");

            migrationBuilder.CreateIndex(
                name: "IX_ProductLabelTranslations_ProductLabelId",
                table: "ProductLabelTranslations",
                column: "ProductLabelId");

            migrationBuilder.AddForeignKey(
                name: "FK_Products_ProductLabels_ProductLabelId",
                table: "Products",
                column: "ProductLabelId",
                principalTable: "ProductLabels",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Products_ProductLabels_ProductLabelId",
                table: "Products");

            migrationBuilder.DropTable(
                name: "ProductLabelTranslations");

            migrationBuilder.DropTable(
                name: "ProductLabels");

            migrationBuilder.DropIndex(
                name: "IX_Products_ProductLabelId",
                table: "Products");

            migrationBuilder.DropColumn(
                name: "ProductLabelId",
                table: "Products");
        }
    }
}
