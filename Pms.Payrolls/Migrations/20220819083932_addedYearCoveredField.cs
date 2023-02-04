using Microsoft.EntityFrameworkCore.Migrations;

namespace Pms.Payrolls.Migrations
{
    public partial class addedYearCoveredField : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "YearCovered",
                table: "payroll",
                type: "INT(4)",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "YearCovered",
                table: "payroll");
        }
    }
}
