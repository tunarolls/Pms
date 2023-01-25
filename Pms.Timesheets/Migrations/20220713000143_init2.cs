using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Pms.Timesheets.Migrations
{
    public partial class init2 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            //migrationBuilder.DropColumn(
            //    name: "CutoffDate",
            //    table: "timesheet");

            //migrationBuilder.AddColumn<string>(
            //    name: "CutoffId",
            //    table: "timesheet",
            //    type: "VARCHAR(6)",
            //    nullable: false,
            //    defaultValue: "");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            //migrationBuilder.DropColumn(
            //    name: "CutoffId",
            //    table: "timesheet");

            //migrationBuilder.AddColumn<DateTime>(
            //    name: "CutoffDate",
            //    table: "timesheet",
            //    type: "DATE",
            //    nullable: false,
            //    defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));
        }
    }
}
