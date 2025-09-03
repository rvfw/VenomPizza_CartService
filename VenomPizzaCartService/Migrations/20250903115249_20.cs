using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace VenomPizzaCartService.Migrations
{
    /// <inheritdoc />
    public partial class _20 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CartProducts_Carts_CartId",
                table: "CartProducts");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Carts",
                table: "Carts");

            migrationBuilder.DropPrimaryKey(
                name: "PK_CartProducts",
                table: "CartProducts");

            migrationBuilder.RenameTable(
                name: "Carts",
                newName: "carts");

            migrationBuilder.RenameTable(
                name: "CartProducts",
                newName: "products");

            migrationBuilder.AddPrimaryKey(
                name: "PK_carts",
                table: "carts",
                column: "UserId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_products",
                table: "products",
                columns: new[] { "CartId", "ProductId" });

            migrationBuilder.AddForeignKey(
                name: "FK_products_carts_CartId",
                table: "products",
                column: "CartId",
                principalTable: "carts",
                principalColumn: "UserId",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_products_carts_CartId",
                table: "products");

            migrationBuilder.DropPrimaryKey(
                name: "PK_carts",
                table: "carts");

            migrationBuilder.DropPrimaryKey(
                name: "PK_products",
                table: "products");

            migrationBuilder.RenameTable(
                name: "carts",
                newName: "Carts");

            migrationBuilder.RenameTable(
                name: "products",
                newName: "CartProducts");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Carts",
                table: "Carts",
                column: "UserId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_CartProducts",
                table: "CartProducts",
                columns: new[] { "CartId", "ProductId" });

            migrationBuilder.AddForeignKey(
                name: "FK_CartProducts_Carts_CartId",
                table: "CartProducts",
                column: "CartId",
                principalTable: "Carts",
                principalColumn: "UserId",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
