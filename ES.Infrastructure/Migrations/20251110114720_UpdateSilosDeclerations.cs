using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ES.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class UpdateSilosDeclerations : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "DescriptionAr",
                table: "SilosDeclerations",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "NotesAr",
                table: "SilosDeclerations",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "SubjectAr",
                table: "SilosDeclerations",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DescriptionAr",
                table: "SilosDeclerations");

            migrationBuilder.DropColumn(
                name: "NotesAr",
                table: "SilosDeclerations");

            migrationBuilder.DropColumn(
                name: "SubjectAr",
                table: "SilosDeclerations");
        }
    }
}
