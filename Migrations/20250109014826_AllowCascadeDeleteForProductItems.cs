using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ECommerceProductsAPI.Migrations
{
    /// <inheritdoc />
    public partial class AllowCascadeDeleteForProductItems : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_GroupedProductItems_Products_ProductItemId",
                table: "GroupedProductItems");

            migrationBuilder.AddForeignKey(
                name: "FK_GroupedProductItems_Products_ProductItemId",
                table: "GroupedProductItems",
                column: "ProductItemId",
                principalTable: "Products",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_GroupedProductItems_Products_ProductItemId",
                table: "GroupedProductItems");

            migrationBuilder.AddForeignKey(
                name: "FK_GroupedProductItems_Products_ProductItemId",
                table: "GroupedProductItems",
                column: "ProductItemId",
                principalTable: "Products",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
