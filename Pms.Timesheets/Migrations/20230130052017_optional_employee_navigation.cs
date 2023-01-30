using Microsoft.EntityFrameworkCore.Migrations;

namespace Pms.Timesheets.Migrations
{
    public partial class optional_employee_navigation : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "EEId",
                table: "timesheet",
                type: "VARCHAR(8)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "VARCHAR(8)");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "EEId",
                table: "timesheet",
                type: "VARCHAR(8)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "VARCHAR(8)",
                oldNullable: true);
        }
    }
}
