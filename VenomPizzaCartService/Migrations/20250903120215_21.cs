using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace VenomPizzaCartService.Migrations
{
    /// <inheritdoc />
    public partial class _21 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_products_carts_CartId",
                table: "products");

            migrationBuilder.RenameColumn(
                name: "Quantity",
                table: "products",
                newName: "quantity");

            migrationBuilder.RenameColumn(
                name: "ProductId",
                table: "products",
                newName: "product_id");

            migrationBuilder.RenameColumn(
                name: "CartId",
                table: "products",
                newName: "cart_id");

            migrationBuilder.RenameColumn(
                name: "TotalPrice",
                table: "carts",
                newName: "total_price");

            migrationBuilder.RenameColumn(
                name: "UserId",
                table: "carts",
                newName: "user_id");

            migrationBuilder.AddForeignKey(
                name: "FK_products_carts_cart_id",
                table: "products",
                column: "cart_id",
                principalTable: "carts",
                principalColumn: "user_id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_products_carts_cart_id",
                table: "products");

            migrationBuilder.RenameColumn(
                name: "quantity",
                table: "products",
                newName: "Quantity");

            migrationBuilder.RenameColumn(
                name: "product_id",
                table: "products",
                newName: "ProductId");

            migrationBuilder.RenameColumn(
                name: "cart_id",
                table: "products",
                newName: "CartId");

            migrationBuilder.RenameColumn(
                name: "total_price",
                table: "carts",
                newName: "TotalPrice");

            migrationBuilder.RenameColumn(
                name: "user_id",
                table: "carts",
                newName: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_products_carts_CartId",
                table: "products",
                column: "CartId",
                principalTable: "carts",
                principalColumn: "UserId",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
