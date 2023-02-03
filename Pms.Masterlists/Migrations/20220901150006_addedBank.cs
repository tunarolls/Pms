using Microsoft.EntityFrameworkCore.Migrations;

namespace Pms.Masterlists.Migrations
{
    public partial class addedBank : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "BankName",
                table: "masterlist");

            migrationBuilder.AddColumn<byte>(
                name: "Bank",
                table: "masterlist",
                type: "TINYINT",
                nullable: false,
                defaultValue: (byte)0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Bank",
                table: "masterlist");

            migrationBuilder.AddColumn<string>(
                name: "BankName",
                table: "masterlist",
                type: "VARCHAR(30)",
                nullable: true);
        }
    }
}
