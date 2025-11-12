#nullable disable

namespace ES.Infrastructure.Migrations
{
    public partial class addformsToPage : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "FormId",
                table: "Pages",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Pages_FormId",
                table: "Pages",
                column: "FormId");

            migrationBuilder.AddForeignKey(
                name: "FK_Pages_Forms_FormId",
                table: "Pages",
                column: "FormId",
                principalTable: "Forms",
                principalColumn: "Id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Pages_Forms_FormId",
                table: "Pages");

            migrationBuilder.DropIndex(
                name: "IX_Pages_FormId",
                table: "Pages");

            migrationBuilder.DropColumn(
                name: "FormId",
                table: "Pages");
        }
    }
}
