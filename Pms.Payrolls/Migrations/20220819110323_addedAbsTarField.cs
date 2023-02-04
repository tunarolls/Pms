using Microsoft.EntityFrameworkCore.Migrations;

namespace Pms.Payrolls.Migrations
{
    public partial class addedAbsTarField : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<double>(
                name: "AbsTar",
                table: "payroll",
                type: "DOUBLE(6,2)",
                nullable: false,
                defaultValue: 0.0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AbsTar",
                table: "payroll");
        }
    }
}
