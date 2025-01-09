using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ECommerceProductsAPI.Migrations
{
    /// <inheritdoc />
    public partial class RemoveGroupedProductsTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_GroupedProductItems_GroupedProducts_GroupedProductId",
                table: "GroupedProductItems");

            migrationBuilder.DropTable(
                name: "GroupedProducts");

            migrationBuilder.DropIndex(
                name: "IX_GroupedProductItems_GroupedProductId",
                table: "GroupedProductItems");

            migrationBuilder.DropColumn(
                name: "GroupedProductId",
                table: "GroupedProductItems");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "GroupedProductId",
                table: "GroupedProductItems",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "GroupedProducts",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Price = table.Column<decimal>(type: "decimal(18,2)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GroupedProducts", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_GroupedProductItems_GroupedProductId",
                table: "GroupedProductItems",
                column: "GroupedProductId");

            migrationBuilder.AddForeignKey(
                name: "FK_GroupedProductItems_GroupedProducts_GroupedProductId",
                table: "GroupedProductItems",
                column: "GroupedProductId",
                principalTable: "GroupedProducts",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
