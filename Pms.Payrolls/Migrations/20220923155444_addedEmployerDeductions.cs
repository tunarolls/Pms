using Microsoft.EntityFrameworkCore.Migrations;

namespace Pms.Payrolls.Migrations
{
    public partial class addedEmployerDeductions : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<double>(
                name: "EmployerPagibig",
                table: "payroll",
                type: "DOUBLE(9,2)",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<double>(
                name: "EmployerPhilHealth",
                table: "payroll",
                type: "DOUBLE(9,2)",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<double>(
                name: "EmployerSSS",
                table: "payroll",
                type: "DOUBLE(9,2)",
                nullable: false,
                defaultValue: 0.0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "EmployerPagibig",
                table: "payroll");

            migrationBuilder.DropColumn(
                name: "EmployerPhilHealth",
                table: "payroll");

            migrationBuilder.DropColumn(
                name: "EmployerSSS",
                table: "payroll");
        }
    }
}
