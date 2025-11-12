#nullable disable

namespace ES.Infrastructure.Migrations
{
    public partial class EditsOnSocialMediaLinks : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "LogoURl",
                table: "SocialMediaLinks");

            migrationBuilder.AddColumn<string>(
                name: "IconClass",
                table: "SocialMediaLinks",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "IconColor",
                table: "SocialMediaLinks",
                type: "nvarchar(max)",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IconClass",
                table: "SocialMediaLinks");

            migrationBuilder.DropColumn(
                name: "IconColor",
                table: "SocialMediaLinks");

            migrationBuilder.AddColumn<string>(
                name: "LogoURl",
                table: "SocialMediaLinks",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }
    }
}
