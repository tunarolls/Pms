using Microsoft.EntityFrameworkCore.Migrations;

namespace Pms.Timesheets.Migrations
{
    public partial class modifiedTypePage : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            //migrationBuilder.AlterColumn<short>(
            //    name: "Page",
            //    table: "timesheet",
            //    type: "SMALLINT",
            //    nullable: false,
            //    comment: "Time System API Page",
            //    oldClrType: typeof(byte),
            //    oldType: "TINYINT",
            //    oldComment: "Time System API Page");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            //migrationBuilder.AlterColumn<byte>(
            //    name: "Page",
            //    table: "timesheet",
            //    type: "TINYINT",
            //    nullable: false,
            //    comment: "Time System API Page",
            //    oldClrType: typeof(short),
            //    oldType: "SMALLINT",
            //    oldComment: "Time System API Page");
        }
    }
}
