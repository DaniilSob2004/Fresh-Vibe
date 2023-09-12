using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace StoreExam.Migrations
{
    /// <inheritdoc />
    public partial class NavProp_ProductCategory : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_Products_IdCat",
                table: "Products",
                column: "IdCat");

            migrationBuilder.AddForeignKey(
                name: "FK_Products_Categories_IdCat",
                table: "Products",
                column: "IdCat",
                principalTable: "Categories",
                principalColumn: "Id",
                onDelete: ReferentialAction.NoAction);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Products_Categories_IdCat",
                table: "Products");

            migrationBuilder.DropIndex(
                name: "IX_Products_IdCat",
                table: "Products");
        }
    }
}
