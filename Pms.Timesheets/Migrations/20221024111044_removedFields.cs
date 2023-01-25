using Microsoft.EntityFrameworkCore.Migrations;

namespace Pms.Timesheets.Migrations
{
    public partial class removedFields : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            //migrationBuilder.DropColumn(
            //    name: "Bank",
            //    table: "timesheet");

            //migrationBuilder.DropColumn(
            //    name: "Fullname",
            //    table: "timesheet");

            //migrationBuilder.DropColumn(
            //    name: "Location",
            //    table: "timesheet");

            //migrationBuilder.DropColumn(
            //    name: "PayrollCode",
            //    table: "timesheet");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            //migrationBuilder.AddColumn<byte>(
            //    name: "Bank",
            //    table: "timesheet",
            //    type: "TINYINT",
            //    nullable: false,
            //    defaultValue: (byte)0);

            //migrationBuilder.AddColumn<string>(
            //    name: "Fullname",
            //    table: "timesheet",
            //    type: "VARCHAR(60)",
            //    nullable: true);

            //migrationBuilder.AddColumn<string>(
            //    name: "Location",
            //    table: "timesheet",
            //    type: "VARCHAR(50)",
            //    nullable: true);

            //migrationBuilder.AddColumn<string>(
            //    name: "PayrollCode",
            //    table: "timesheet",
            //    type: "VARCHAR(6)",
            //    nullable: true);
        }
    }
}
