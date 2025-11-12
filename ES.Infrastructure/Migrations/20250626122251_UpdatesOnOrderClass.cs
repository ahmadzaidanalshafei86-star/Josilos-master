#nullable disable

namespace ES.Infrastructure.Migrations
{
    public partial class UpdatesOnOrderClass : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_OrderItemAttributes_OrderItems_ExcludedOrderItemId",
                table: "OrderItemAttributes");

            migrationBuilder.DropIndex(
                name: "IX_OrderItemAttributes_ExcludedOrderItemId",
                table: "OrderItemAttributes");

            migrationBuilder.DropColumn(
                name: "Latitude",
                table: "Orders");

            migrationBuilder.DropColumn(
                name: "Longitude",
                table: "Orders");

            migrationBuilder.DropColumn(
                name: "ExcludedOrderItemId",
                table: "OrderItemAttributes");

            migrationBuilder.RenameColumn(
                name: "Name",
                table: "Orders",
                newName: "StreetAddress");

            migrationBuilder.RenameColumn(
                name: "Address",
                table: "Orders",
                newName: "CustomerFullName");

            migrationBuilder.AddColumn<string>(
                name: "City",
                table: "Orders",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Country",
                table: "Orders",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Email",
                table: "Orders",
                type: "nvarchar(max)",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "City",
                table: "Orders");

            migrationBuilder.DropColumn(
                name: "Country",
                table: "Orders");

            migrationBuilder.DropColumn(
                name: "Email",
                table: "Orders");

            migrationBuilder.RenameColumn(
                name: "StreetAddress",
                table: "Orders",
                newName: "Name");

            migrationBuilder.RenameColumn(
                name: "CustomerFullName",
                table: "Orders",
                newName: "Address");

            migrationBuilder.AddColumn<double>(
                name: "Latitude",
                table: "Orders",
                type: "float",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<double>(
                name: "Longitude",
                table: "Orders",
                type: "float",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<int>(
                name: "ExcludedOrderItemId",
                table: "OrderItemAttributes",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_OrderItemAttributes_ExcludedOrderItemId",
                table: "OrderItemAttributes",
                column: "ExcludedOrderItemId");

            migrationBuilder.AddForeignKey(
                name: "FK_OrderItemAttributes_OrderItems_ExcludedOrderItemId",
                table: "OrderItemAttributes",
                column: "ExcludedOrderItemId",
                principalTable: "OrderItems",
                principalColumn: "Id");
        }
    }
}
