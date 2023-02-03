using Microsoft.EntityFrameworkCore.Migrations;

namespace Pms.Timesheets.Migrations
{
    public partial class removedBankCategoryField : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "BankCategory",
                table: "timesheet");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "BankCategory",
                table: "timesheet",
                type: "VARCHAR(6)",
                nullable: true);
        }
    }
}
