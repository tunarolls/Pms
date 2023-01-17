using Microsoft.EntityFrameworkCore.Migrations;

namespace Pms.Payrolls.Migrations
{
    public partial class init : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "payroll",
                columns: table => new
                {
                    PayrollId = table.Column<string>(type: "VARCHAR(35)", nullable: false),
                    AbsTar = table.Column<double>(type: "DOUBLE(6,2)", nullable: false),
                    Adjust1Total = table.Column<double>(type: "DOUBLE(9,2)", nullable: false),
                    Adjust2Total = table.Column<double>(type: "DOUBLE(9,2)", nullable: false),
                    CompanyId = table.Column<string>(type: "VARCHAR(35)", nullable: false),
                    CutoffId = table.Column<string>(type: "VARCHAR(20)", nullable: false),
                    EEId = table.Column<string>(type: "VARCHAR(8)", nullable: false),
                    EmployeePagibig = table.Column<double>(type: "DOUBLE(9,2)", nullable: false),
                    EmployeePhilHealth = table.Column<double>(type: "DOUBLE(9,2)", nullable: false),
                    EmployeeSSS = table.Column<double>(type: "DOUBLE(9,2)", nullable: false),
                    GovernmentTotal = table.Column<double>(type: "DOUBLE(9,2)", nullable: false),
                    GrossPay = table.Column<double>(type: "DOUBLE(9,2)", nullable: false),
                    HolidayOvertime = table.Column<double>(type: "DOUBLE(6,2)", nullable: false),
                    NetPay = table.Column<double>(type: "DOUBLE(9,2)", nullable: false),
                    NightDifferential = table.Column<double>(type: "DOUBLE(6,2)", nullable: false),
                    Overtime = table.Column<double>(type: "DOUBLE(6,2)", nullable: false),
                    PayrollCode = table.Column<string>(type: "VARCHAR(6)", nullable: false),
                    RegHours = table.Column<double>(type: "DOUBLE(6,2)", nullable: false),
                    RegularPay = table.Column<double>(type: "DOUBLE(9,2)", nullable: false),
                    RestDayOvertime = table.Column<double>(type: "DOUBLE(6,2)", nullable: false),
                    WithholdingTax = table.Column<double>(type: "DOUBLE(9,2)", nullable: false),
                    YearCovered = table.Column<int>(type: "INT(4)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_payroll", x => x.PayrollId);
                });

            migrationBuilder.CreateIndex(
                name: "IX_payroll_EEId",
                table: "payroll",
                column: "EEId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "payroll");
        }
    }
}
