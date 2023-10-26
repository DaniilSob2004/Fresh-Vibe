using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace StoreExam.Migrations
{
    /// <inheritdoc />
    public partial class User_ConfirmCode : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ConfirmCode",
                table: "Users",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ConfirmCode",
                table: "Users");
        }
    }
}
