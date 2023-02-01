using Microsoft.EntityFrameworkCore.Migrations;

namespace Pms.Adjustments.Migrations
{
    public partial class addedRemarksToBillingRecordEntity : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_adjustment_billing_BillingRecord_RecordId",
                table: "adjustment_billing");

            migrationBuilder.DropPrimaryKey(
                name: "PK_BillingRecord",
                table: "BillingRecord");

            migrationBuilder.RenameTable(
                name: "BillingRecord",
                newName: "BillingRecords");

            migrationBuilder.RenameIndex(
                name: "IX_BillingRecord_EEId",
                table: "BillingRecords",
                newName: "IX_BillingRecords_EEId");

            migrationBuilder.AddColumn<string>(
                name: "Remarks",
                table: "BillingRecords",
                type: "text",
                nullable: true);

            migrationBuilder.AddPrimaryKey(
                name: "PK_BillingRecords",
                table: "BillingRecords",
                column: "RecordId");

            migrationBuilder.AddForeignKey(
                name: "FK_adjustment_billing_BillingRecords_RecordId",
                table: "adjustment_billing",
                column: "RecordId",
                principalTable: "BillingRecords",
                principalColumn: "RecordId",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_adjustment_billing_BillingRecords_RecordId",
                table: "adjustment_billing");

            migrationBuilder.DropPrimaryKey(
                name: "PK_BillingRecords",
                table: "BillingRecords");

            migrationBuilder.DropColumn(
                name: "Remarks",
                table: "BillingRecords");

            migrationBuilder.RenameTable(
                name: "BillingRecords",
                newName: "BillingRecord");

            migrationBuilder.RenameIndex(
                name: "IX_BillingRecords_EEId",
                table: "BillingRecord",
                newName: "IX_BillingRecord_EEId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_BillingRecord",
                table: "BillingRecord",
                column: "RecordId");

            migrationBuilder.AddForeignKey(
                name: "FK_adjustment_billing_BillingRecord_RecordId",
                table: "adjustment_billing",
                column: "RecordId",
                principalTable: "BillingRecord",
                principalColumn: "RecordId",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
