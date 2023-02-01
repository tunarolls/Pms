using Microsoft.EntityFrameworkCore.Migrations;

namespace Pms.Adjustments.Migrations
{
    public partial class addedPayrollCodeAndBankCat : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "BankCategory",
                table: "adjustment_billing",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PayrollCode",
                table: "adjustment_billing",
                type: "text",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "BankCategory",
                table: "adjustment_billing");

            migrationBuilder.DropColumn(
                name: "PayrollCode",
                table: "adjustment_billing");
        }
    }
}
