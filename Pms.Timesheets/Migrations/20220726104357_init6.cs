using Microsoft.EntityFrameworkCore.Migrations;

namespace Pms.Timesheets.Migrations
{
    public partial class init6 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<byte>(
                name: "IsConfirmed",
                table: "timesheet",
                type: "TINYINT",
                nullable: false,
                oldClrType: typeof(double),
                oldType: "DOUBLE(8,2)");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<double>(
                name: "IsConfirmed",
                table: "timesheet",
                type: "DOUBLE(8,2)",
                nullable: false,
                oldClrType: typeof(byte),
                oldType: "TINYINT");
        }
    }
}
