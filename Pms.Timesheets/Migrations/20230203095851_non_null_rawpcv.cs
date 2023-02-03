using Microsoft.EntityFrameworkCore.Migrations;

namespace Pms.Timesheets.Migrations
{
    public partial class non_null_rawpcv : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "RawPCV",
                table: "timesheet",
                type: "VARCHAR(255)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "VARCHAR(255)",
                oldNullable: true);

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
                name: "RawPCV",
                table: "timesheet",
                type: "VARCHAR(255)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "VARCHAR(255)");

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
