using Microsoft.EntityFrameworkCore.Migrations;

namespace Pms.Payrolls.Migrations
{
    public partial class addedRegHours : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            //migrationBuilder.AddColumn<double>(
            //    name: "RegHours",
            //    table: "payroll",
            //    type: "DOUBLE(6,2)",
            //    nullable: false,
            //    defaultValue: 0.0);

            //migrationBuilder.CreateIndex(
            //    name: "IX_payroll_EEId",
            //    table: "payroll",
            //    column: "EEId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            //migrationBuilder.DropIndex(
            //    name: "IX_payroll_EEId",
            //    table: "payroll");

            //migrationBuilder.DropColumn(
            //    name: "RegHours",
            //    table: "payroll");
        }
    }
}
