using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace StoreExam.Migrations
{
    /// <inheritdoc />
    public partial class NavProp_BasketProduct_Product : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_BasketProducts_ProductId",
                table: "BasketProducts",
                column: "ProductId");

            migrationBuilder.AddForeignKey(
                name: "FK_BasketProducts_Products_ProductId",
                table: "BasketProducts",
                column: "ProductId",
                principalTable: "Products",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_BasketProducts_Products_ProductId",
                table: "BasketProducts");

            migrationBuilder.DropIndex(
                name: "IX_BasketProducts_ProductId",
                table: "BasketProducts");
        }
    }
}
