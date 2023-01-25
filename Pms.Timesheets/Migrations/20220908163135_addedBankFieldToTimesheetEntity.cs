using Microsoft.EntityFrameworkCore.Migrations;

namespace Pms.Timesheets.Migrations
{
    public partial class addedBankFieldToTimesheetEntity : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            //migrationBuilder.AddColumn<byte>(
            //    name: "Bank",
            //    table: "timesheet",
            //    type: "TINYINT",
            //    nullable: false,
            //    defaultValue: (byte)0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            //migrationBuilder.DropColumn(
            //    name: "Bank",
            //    table: "timesheet");
        }
    }
}
