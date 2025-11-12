using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ES.Infrastructure.Migrations
{
    public partial class FixMtarialRelation : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_TenderMaterials_Tenders_TenderId1",
                table: "TenderMaterials");

            migrationBuilder.DropIndex(
                name: "IX_TenderMaterials_TenderId1",
                table: "TenderMaterials");

            migrationBuilder.DropColumn(
                name: "TenderId1",
                table: "TenderMaterials");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "TenderId1",
                table: "TenderMaterials",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_TenderMaterials_TenderId1",
                table: "TenderMaterials",
                column: "TenderId1");

            migrationBuilder.AddForeignKey(
                name: "FK_TenderMaterials_Tenders_TenderId1",
                table: "TenderMaterials",
                column: "TenderId1",
                principalTable: "Tenders",
                principalColumn: "Id");
        }
    }
}
