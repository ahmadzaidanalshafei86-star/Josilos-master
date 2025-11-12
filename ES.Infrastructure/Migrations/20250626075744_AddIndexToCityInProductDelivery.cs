#nullable disable

namespace ES.Infrastructure.Migrations
{
    public partial class AddIndexToCityInProductDelivery : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_ProductDeliveries_City",
                table: "ProductDeliveries",
                column: "City");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_ProductDeliveries_City",
                table: "ProductDeliveries");
        }
    }
}
