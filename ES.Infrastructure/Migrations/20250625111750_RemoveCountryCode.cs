#nullable disable

namespace ES.Infrastructure.Migrations
{
    public partial class RemoveCountryCode : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CountryCode",
                table: "ProductDeliveries");

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedAt",
                table: "ProductDeliveries",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CreatedAt",
                table: "ProductDeliveries");

            migrationBuilder.AddColumn<string>(
                name: "CountryCode",
                table: "ProductDeliveries",
                type: "nvarchar(2)",
                maxLength: 2,
                nullable: false,
                defaultValue: "");
        }
    }
}
