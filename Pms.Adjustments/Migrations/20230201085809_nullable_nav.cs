using Microsoft.EntityFrameworkCore.Migrations;

namespace Pms.Adjustments.Migrations
{
    public partial class nullable_nav : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_adjustment_billing_BillingRecords_RecordId",
                table: "adjustment_billing");

            migrationBuilder.DropPrimaryKey(
                name: "PK_BillingRecords",
                table: "BillingRecords");

            migrationBuilder.RenameTable(
                name: "BillingRecords",
                newName: "billingrecords");

            migrationBuilder.RenameIndex(
                name: "IX_BillingRecords_EEId",
                table: "billingrecords",
                newName: "IX_billingrecords_EEId");

            migrationBuilder.AlterColumn<string>(
                name: "EEId",
                table: "billingrecords",
                type: "VARCHAR(8)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "VARCHAR(8)");

            migrationBuilder.AlterColumn<string>(
                name: "EEId",
                table: "adjustment_billing",
                type: "VARCHAR(8)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "VARCHAR(8)");

            migrationBuilder.AddPrimaryKey(
                name: "PK_billingrecords",
                table: "billingrecords",
                column: "RecordId");

            migrationBuilder.AddForeignKey(
                name: "FK_adjustment_billing_billingrecords_RecordId",
                table: "adjustment_billing",
                column: "RecordId",
                principalTable: "billingrecords",
                principalColumn: "RecordId",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_adjustment_billing_billingrecords_RecordId",
                table: "adjustment_billing");

            migrationBuilder.DropPrimaryKey(
                name: "PK_billingrecords",
                table: "billingrecords");

            migrationBuilder.RenameTable(
                name: "billingrecords",
                newName: "BillingRecords");

            migrationBuilder.RenameIndex(
                name: "IX_billingrecords_EEId",
                table: "BillingRecords",
                newName: "IX_BillingRecords_EEId");

            migrationBuilder.AlterColumn<string>(
                name: "EEId",
                table: "BillingRecords",
                type: "VARCHAR(8)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "VARCHAR(8)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "EEId",
                table: "adjustment_billing",
                type: "VARCHAR(8)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "VARCHAR(8)",
                oldNullable: true);

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
    }
}
