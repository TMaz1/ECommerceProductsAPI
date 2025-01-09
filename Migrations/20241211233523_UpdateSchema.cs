using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ECommerceProductsAPI.Migrations
{
    /// <inheritdoc />
    public partial class UpdateSchema : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ProductVariationAttributes_ProductAttributes_AttributeId",
                table: "ProductVariationAttributes");

            // migrationBuilder.AlterColumn<byte[]>(
            //     name: "PasswordSalt",
            //     table: "Users",
            //     type: "varbinary(max)",
            //     nullable: false,
            //     oldClrType: typeof(string),
            //     oldType: "nvarchar(max)");

            // migrationBuilder.AlterColumn<byte[]>(
            //     name: "PasswordHash",
            //     table: "Users",
            //     type: "varbinary(max)",
            //     nullable: false,
            //     oldClrType: typeof(string),
            //     oldType: "nvarchar(max)");

            // Step 1: Add a temporary column with the new data type
            migrationBuilder.AddColumn<byte[]>(
                name: "PasswordSaltTemp",
                table: "Users",
                type: "varbinary(max)",
                nullable: true);

            // Step 2: Migrate data from the old column to the new column
            migrationBuilder.Sql(
                @"
                UPDATE Users
                SET PasswordSaltTemp = CAST(PasswordSalt AS varbinary(max))
                WHERE PasswordSalt IS NOT NULL");

            // Step 3: Drop the old column
            migrationBuilder.DropColumn(
                name: "PasswordSalt",
                table: "Users");

            // Step 4: Rename the temporary column to the original column name
            migrationBuilder.RenameColumn(
                name: "PasswordSaltTemp",
                table: "Users",
                newName: "PasswordSalt");

            // Repeated the same steps for PasswordHash
            migrationBuilder.AddColumn<byte[]>(
                name: "PasswordHashTemp",
                table: "Users",
                type: "varbinary(max)",
                nullable: true);

            migrationBuilder.Sql(
                @"
                UPDATE Users
                SET PasswordHashTemp = CAST(PasswordHash AS varbinary(max))
                WHERE PasswordHash IS NOT NULL");

            migrationBuilder.DropColumn(
                name: "PasswordHash",
                table: "Users");

            migrationBuilder.RenameColumn(
                name: "PasswordHashTemp",
                table: "Users",
                newName: "PasswordHash");

            migrationBuilder.AddForeignKey(
                name: "FK_ProductVariationAttributes_ProductAttributes_AttributeId",
                table: "ProductVariationAttributes",
                column: "AttributeId",
                principalTable: "ProductAttributes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // Down method allows to restore to the previous state

            migrationBuilder.DropForeignKey(
                name: "FK_ProductVariationAttributes_ProductAttributes_AttributeId",
                table: "ProductVariationAttributes");

            // migrationBuilder.AlterColumn<string>(
            //     name: "PasswordSalt",
            //     table: "Users",
            //     type: "nvarchar(max)",
            //     nullable: false,
            //     oldClrType: typeof(byte[]),
            //     oldType: "varbinary(max)");

            // migrationBuilder.AlterColumn<string>(
            //     name: "PasswordHash",
            //     table: "Users",
            //     type: "nvarchar(max)",
            //     nullable: false,
            //     oldClrType: typeof(byte[]),
            //     oldType: "varbinary(max)");

            // Step 1: Add a temporary column with the old data type
            migrationBuilder.AddColumn<string>(
                name: "PasswordSaltTemp",
                table: "Users",
                type: "nvarchar(max)",
                nullable: true);

            // Step 2: Migrate data back to the old column
            migrationBuilder.Sql(
                @"
                UPDATE Users
                SET PasswordSaltTemp = CAST(PasswordSalt AS nvarchar(max))
                WHERE PasswordSalt IS NOT NULL");

            // Step 3: Drop the new column
            migrationBuilder.DropColumn(
                name: "PasswordSalt",
                table: "Users");

            // Step 4: Rename the temporary column back to the original name
            migrationBuilder.RenameColumn(
                name: "PasswordSaltTemp",
                table: "Users",
                newName: "PasswordSalt");

            // Repeated the same steps for PasswordHash
            migrationBuilder.AddColumn<string>(
                name: "PasswordHashTemp",
                table: "Users",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.Sql(
                @"
                UPDATE Users
                SET PasswordHashTemp = CAST(PasswordHash AS nvarchar(max))
                WHERE PasswordHash IS NOT NULL");

            migrationBuilder.DropColumn(
                name: "PasswordHash",
                table: "Users");

            migrationBuilder.RenameColumn(
                name: "PasswordHashTemp",
                table: "Users",
                newName: "PasswordHash");

            migrationBuilder.AddForeignKey(
                name: "FK_ProductVariationAttributes_ProductAttributes_AttributeId",
                table: "ProductVariationAttributes",
                column: "AttributeId",
                principalTable: "ProductAttributes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
