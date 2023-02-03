using Microsoft.EntityFrameworkCore.Migrations;

namespace Pms.Timesheets.Migrations
{
    public partial class init5 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Location",
                table: "timesheet",
                type: "VARCHAR(50)",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Location",
                table: "timesheet");
        }
    }
}
