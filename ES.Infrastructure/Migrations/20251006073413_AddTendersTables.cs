using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ES.Infrastructure.Migrations
{
    public partial class AddTendersTables : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Tenders",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Title = table.Column<string>(type: "nvarchar(300)", maxLength: 300, nullable: false),
                    Slug = table.Column<string>(type: "nvarchar(300)", maxLength: 300, nullable: false),
                    Code = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    CopyPrice = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    StartDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    EndDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    EnvelopeOpeningDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    LastCopyPurchaseDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Details = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PricesOffered = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    MetaDescription = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    MetaKeywords = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    TenderImageUrl = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PricesOfferedAttachmentUrl = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    InitialAwardFileUrl = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    FinalAwardFileUrl = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Publish = table.Column<bool>(type: "bit", nullable: false),
                    PublishPricesOffered = table.Column<bool>(type: "bit", nullable: false),
                    SpecialOfferBlink = table.Column<bool>(type: "bit", nullable: false),
                    MoveToArchive = table.Column<bool>(type: "bit", nullable: false),
                    BlinkStartDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    BlinkEndDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    LanguageId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Tenders", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Tenders_Languages_LanguageId",
                        column: x => x.LanguageId,
                        principalTable: "Languages",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "TenderMaterials",
                columns: table => new
                {
                    TenderId = table.Column<int>(type: "int", nullable: false),
                    MaterialId = table.Column<int>(type: "int", nullable: false),
                    TenderId1 = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TenderMaterials", x => new { x.TenderId, x.MaterialId });
                    table.ForeignKey(
                        name: "FK_TenderMaterials_Materials_MaterialId",
                        column: x => x.MaterialId,
                        principalTable: "Materials",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_TenderMaterials_Tenders_TenderId",
                        column: x => x.TenderId,
                        principalTable: "Tenders",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_TenderMaterials_Tenders_TenderId1",
                        column: x => x.TenderId1,
                        principalTable: "Tenders",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "TenderOtherAttachments",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TenderId = table.Column<int>(type: "int", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    FileUrl = table.Column<string>(type: "nvarchar(300)", maxLength: 300, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TenderOtherAttachments", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TenderOtherAttachments_Tenders_TenderId",
                        column: x => x.TenderId,
                        principalTable: "Tenders",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "TendersFiles",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TenderId = table.Column<int>(type: "int", nullable: false),
                    FileUrl = table.Column<string>(type: "nvarchar(300)", maxLength: 300, nullable: false),
                    FileName = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TendersFiles", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TendersFiles_Tenders_TenderId",
                        column: x => x.TenderId,
                        principalTable: "Tenders",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "TenderTranslates",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Title = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Details = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PricesOffered = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    MetaDescription = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    MetaKeywords = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    TenderId = table.Column<int>(type: "int", nullable: false),
                    LanguageId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TenderTranslates", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TenderTranslates_Languages_LanguageId",
                        column: x => x.LanguageId,
                        principalTable: "Languages",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_TenderTranslates_Tenders_TenderId",
                        column: x => x.TenderId,
                        principalTable: "Tenders",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_TenderMaterials_MaterialId",
                table: "TenderMaterials",
                column: "MaterialId");

            migrationBuilder.CreateIndex(
                name: "IX_TenderMaterials_TenderId1",
                table: "TenderMaterials",
                column: "TenderId1");

            migrationBuilder.CreateIndex(
                name: "IX_TenderOtherAttachments_TenderId",
                table: "TenderOtherAttachments",
                column: "TenderId");

            migrationBuilder.CreateIndex(
                name: "IX_Tenders_Code",
                table: "Tenders",
                column: "Code",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Tenders_LanguageId",
                table: "Tenders",
                column: "LanguageId");

            migrationBuilder.CreateIndex(
                name: "IX_TendersFiles_TenderId",
                table: "TendersFiles",
                column: "TenderId");

            migrationBuilder.CreateIndex(
                name: "IX_TenderTranslates_LanguageId",
                table: "TenderTranslates",
                column: "LanguageId");

            migrationBuilder.CreateIndex(
                name: "IX_TenderTranslates_TenderId",
                table: "TenderTranslates",
                column: "TenderId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "TenderMaterials");

            migrationBuilder.DropTable(
                name: "TenderOtherAttachments");

            migrationBuilder.DropTable(
                name: "TendersFiles");

            migrationBuilder.DropTable(
                name: "TenderTranslates");

            migrationBuilder.DropTable(
                name: "Tenders");
        }
    }
}
