using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ECommerceProductsAPI.Migrations
{
    /// <inheritdoc />
    public partial class GroupedProductItemColumns : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_GroupedProductItems_Products_ProductId",
                table: "GroupedProductItems");

            migrationBuilder.RenameColumn(
                name: "ProductId",
                table: "GroupedProductItems",
                newName: "ProductItemId");

            migrationBuilder.RenameIndex(
                name: "IX_GroupedProductItems_ProductId",
                table: "GroupedProductItems",
                newName: "IX_GroupedProductItems_ProductItemId");

            migrationBuilder.AddColumn<int>(
                name: "GroupedProductId",
                table: "GroupedProductItems",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_GroupedProductItems_GroupedProductId",
                table: "GroupedProductItems",
                column: "GroupedProductId");

            migrationBuilder.AddForeignKey(
                name: "FK_GroupedProductItems_Products_GroupedProductId",
                table: "GroupedProductItems",
                column: "GroupedProductId",
                principalTable: "Products",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_GroupedProductItems_Products_ProductItemId",
                table: "GroupedProductItems",
                column: "ProductItemId",
                principalTable: "Products",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_GroupedProductItems_Products_GroupedProductId",
                table: "GroupedProductItems");

            migrationBuilder.DropForeignKey(
                name: "FK_GroupedProductItems_Products_ProductItemId",
                table: "GroupedProductItems");

            migrationBuilder.DropIndex(
                name: "IX_GroupedProductItems_GroupedProductId",
                table: "GroupedProductItems");

            migrationBuilder.DropColumn(
                name: "GroupedProductId",
                table: "GroupedProductItems");

            migrationBuilder.RenameColumn(
                name: "ProductItemId",
                table: "GroupedProductItems",
                newName: "ProductId");

            migrationBuilder.RenameIndex(
                name: "IX_GroupedProductItems_ProductItemId",
                table: "GroupedProductItems",
                newName: "IX_GroupedProductItems_ProductId");

            migrationBuilder.AddForeignKey(
                name: "FK_GroupedProductItems_Products_ProductId",
                table: "GroupedProductItems",
                column: "ProductId",
                principalTable: "Products",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
