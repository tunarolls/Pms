using Microsoft.EntityFrameworkCore.Migrations;

namespace Pms.Adjustments.Migrations
{
    public partial class addedAdjustmentType : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "AdjustmentOption",
                table: "BillingRecords",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AdjustmentOption",
                table: "BillingRecords");
        }
    }
}
