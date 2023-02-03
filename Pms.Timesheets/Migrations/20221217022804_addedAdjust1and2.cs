using Microsoft.EntityFrameworkCore.Migrations;

namespace Pms.Timesheets.Migrations
{
    public partial class addedAdjust1and2 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<double>(
                name: "Adjust1",
                table: "timesheet",
                type: "DOUBLE(8,2)",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<double>(
                name: "Adjust2",
                table: "timesheet",
                type: "DOUBLE(8,2)",
                nullable: false,
                defaultValue: 0.0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Adjust1",
                table: "timesheet");

            migrationBuilder.DropColumn(
                name: "Adjust2",
                table: "timesheet");
        }
    }
}
