using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ES.Infrastructure.Migrations
{
    public partial class AddLinkUrlToPages : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "LinkToUrl",
                table: "Pages",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "LinkToUrl",
                table: "Pages");
        }
    }
}
