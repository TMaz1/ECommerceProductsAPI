using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ECommerceProductsAPI.Migrations
{
    /// <inheritdoc />
    public partial class UpdateProductVariations : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ProductAttributes_Products_ProductId",
                table: "ProductAttributes");

            migrationBuilder.DropIndex(
                name: "IX_ProductAttributes_ProductId",
                table: "ProductAttributes");

            migrationBuilder.DropColumn(
                name: "Name",
                table: "ProductAttributes");

            migrationBuilder.DropColumn(
                name: "ProductId",
                table: "ProductAttributes");

            migrationBuilder.AlterColumn<string>(
                name: "Value",
                table: "ProductAttributes",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Type",
                table: "ProductAttributes",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateTable(
                name: "ProductVariationAttributes",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    VariationId = table.Column<int>(type: "int", nullable: false),
                    AttributeId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProductVariationAttributes", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ProductVariationAttributes_ProductAttributes_AttributeId",
                        column: x => x.AttributeId,
                        principalTable: "ProductAttributes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ProductVariationAttributes_ProductVariations_VariationId",
                        column: x => x.VariationId,
                        principalTable: "ProductVariations",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ProductVariationAttributes_AttributeId",
                table: "ProductVariationAttributes",
                column: "AttributeId");

            migrationBuilder.CreateIndex(
                name: "IX_ProductVariationAttributes_VariationId",
                table: "ProductVariationAttributes",
                column: "VariationId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ProductVariationAttributes");

            migrationBuilder.DropColumn(
                name: "Type",
                table: "ProductAttributes");

            migrationBuilder.AlterColumn<string>(
                name: "Value",
                table: "ProductAttributes",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AddColumn<string>(
                name: "Name",
                table: "ProductAttributes",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "ProductId",
                table: "ProductAttributes",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_ProductAttributes_ProductId",
                table: "ProductAttributes",
                column: "ProductId");

            migrationBuilder.AddForeignKey(
                name: "FK_ProductAttributes_Products_ProductId",
                table: "ProductAttributes",
                column: "ProductId",
                principalTable: "Products",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
