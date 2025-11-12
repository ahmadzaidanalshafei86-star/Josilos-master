#nullable disable

namespace ES.Infrastructure.Migrations
{
    public partial class AddOrderCommentToOrdercs : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "OrderComment",
                table: "Orders",
                type: "nvarchar(max)",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "OrderComment",
                table: "Orders");
        }
    }
}
