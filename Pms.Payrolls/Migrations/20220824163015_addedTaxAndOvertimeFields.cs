using Microsoft.EntityFrameworkCore.Migrations;

namespace Pms.Payrolls.Migrations
{
    public partial class addedTaxAndOvertimeFields : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {

            migrationBuilder.AddColumn<double>(
                name: "EmployeePagibig",
                table: "payroll",
                type: "DOUBLE(9,2)",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<double>(
                name: "EmployeePhilHealth",
                table: "payroll",
                type: "DOUBLE(9,2)",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<double>(
                name: "EmployeeSSS",
                table: "payroll",
                type: "DOUBLE(9,2)",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<double>(
                name: "HolidayOvertime",
                table: "payroll",
                type: "DOUBLE(6,2)",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<double>(
                name: "NightDifferential",
                table: "payroll",
                type: "DOUBLE(6,2)",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<double>(
                name: "Overtime",
                table: "payroll",
                type: "DOUBLE(6,2)",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<double>(
                name: "RestDayOvertime",
                table: "payroll",
                type: "DOUBLE(6,2)",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<double>(
                name: "WithholdingTax",
                table: "payroll",
                type: "DOUBLE(9,2)",
                nullable: false,
                defaultValue: 0.0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "EmployeePagibig",
                table: "payroll");

            migrationBuilder.DropColumn(
                name: "EmployeePhilHealth",
                table: "payroll");

            migrationBuilder.DropColumn(
                name: "EmployeeSSS",
                table: "payroll");

            migrationBuilder.DropColumn(
                name: "HolidayOvertime",
                table: "payroll");

            migrationBuilder.DropColumn(
                name: "NightDifferential",
                table: "payroll");

            migrationBuilder.DropColumn(
                name: "Overtime",
                table: "payroll");

            migrationBuilder.DropColumn(
                name: "RestDayOvertime",
                table: "payroll");

            migrationBuilder.DropColumn(
                name: "WithholdingTax",
                table: "payroll");

        }
    }
}
