using Microsoft.EntityFrameworkCore.Migrations;

namespace Pms.Masterlists.Migrations
{
    public partial class removedBankCategoryField : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "BankCategory",
                table: "masterlist");

            migrationBuilder.AlterColumn<string>(
                name: "NameExtension",
                table: "masterlist",
                type: "VARCHAR(6)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true);

            migrationBuilder.AddColumn<string>(
                name: "JobCode",
                table: "masterlist",
                type: "text",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "JobCode",
                table: "masterlist");

            migrationBuilder.AlterColumn<string>(
                name: "NameExtension",
                table: "masterlist",
                type: "text",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "VARCHAR(6)",
                oldNullable: true);

            migrationBuilder.AddColumn<string>(
                name: "BankCategory",
                table: "masterlist",
                type: "VARCHAR(10)",
                nullable: true);
        }
    }
}
