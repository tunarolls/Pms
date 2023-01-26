using Microsoft.EntityFrameworkCore.Migrations;

namespace Pms.Masterlists.Migrations
{
    public partial class payroll_code_table_name : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            //migrationBuilder.DropPrimaryKey(
            //    name: "PK_PayrollCodes",
            //    table: "PayrollCodes");

            //migrationBuilder.RenameTable(
            //    name: "PayrollCodes",
            //    newName: "payrollcodes");

            //migrationBuilder.AddPrimaryKey(
            //    name: "PK_payrollcodes",
            //    table: "payrollcodes",
            //    column: "PayrollCodeId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            //migrationBuilder.DropPrimaryKey(
            //    name: "PK_payrollcodes",
            //    table: "payrollcodes");

            //migrationBuilder.RenameTable(
            //    name: "payrollcodes",
            //    newName: "PayrollCodes");

            //migrationBuilder.AddPrimaryKey(
            //    name: "PK_PayrollCodes",
            //    table: "PayrollCodes",
            //    column: "PayrollCodeId");
        }
    }
}
