using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace StoreExam.Migrations
{
    /// <inheritdoc />
    public partial class NavProp_User_BasketProducts : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_BasketProducts_UserId",
                table: "BasketProducts",
                column: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_BasketProducts_Users_UserId",
                table: "BasketProducts",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_BasketProducts_Users_UserId",
                table: "BasketProducts");

            migrationBuilder.DropIndex(
                name: "IX_BasketProducts_UserId",
                table: "BasketProducts");
        }
    }
}
