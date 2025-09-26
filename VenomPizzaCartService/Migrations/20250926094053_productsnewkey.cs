using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace VenomPizzaCartService.Migrations
{
    /// <inheritdoc />
    public partial class productsnewkey : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_products",
                table: "products");

            migrationBuilder.AddPrimaryKey(
                name: "PK_products",
                table: "products",
                columns: new[] { "cart_id", "product_id", "price_id" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_products",
                table: "products");

            migrationBuilder.AddPrimaryKey(
                name: "PK_products",
                table: "products",
                columns: new[] { "cart_id", "product_id" });
        }
    }
}
