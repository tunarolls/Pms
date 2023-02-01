using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Pms.Adjustments.Migrations
{
    public partial class addedBillingRecordEntity : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AdjustmentName",
                table: "adjustment_billing");

            migrationBuilder.DropColumn(
                name: "BankCategory",
                table: "adjustment_billing");

            migrationBuilder.DropColumn(
                name: "PayrollCode",
                table: "adjustment_billing");

            migrationBuilder.AddColumn<byte>(
                name: "AdjustmentOption",
                table: "adjustment_billing",
                type: "TINYINT",
                nullable: false,
                defaultValue: (byte)0);

            migrationBuilder.CreateTable(
                name: "BillingRecord",
                columns: table => new
                {
                    RecordId = table.Column<string>(type: "VARCHAR(45)", nullable: false),
                    EEId = table.Column<string>(type: "VARCHAR(8)", nullable: false),
                    AdjustmentType = table.Column<byte>(type: "TINYINT", nullable: false),
                    Balance = table.Column<double>(type: "DOUBLE(8,2)", nullable: false),
                    Advances = table.Column<double>(type: "DOUBLE(8,2)", nullable: false),
                    Amortization = table.Column<double>(type: "DOUBLE(8,2)", nullable: false),
                    EffectivityDate = table.Column<DateTime>(type: "DATE", nullable: false),
                    DeductionOption = table.Column<byte>(type: "TINYINT", nullable: false),
                    Status = table.Column<byte>(type: "TINYINT", nullable: false),
                    DateCreated = table.Column<DateTime>(type: "TIMESTAMP", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BillingRecord", x => x.RecordId);
                });

            migrationBuilder.CreateIndex(
                name: "IX_adjustment_billing_RecordId",
                table: "adjustment_billing",
                column: "RecordId");

            migrationBuilder.CreateIndex(
                name: "IX_BillingRecord_EEId",
                table: "BillingRecord",
                column: "EEId");

            migrationBuilder.AddForeignKey(
                name: "FK_adjustment_billing_BillingRecord_RecordId",
                table: "adjustment_billing",
                column: "RecordId",
                principalTable: "BillingRecord",
                principalColumn: "RecordId",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_adjustment_billing_BillingRecord_RecordId",
                table: "adjustment_billing");

            migrationBuilder.DropTable(
                name: "BillingRecord");

            migrationBuilder.DropIndex(
                name: "IX_adjustment_billing_RecordId",
                table: "adjustment_billing");

            migrationBuilder.DropColumn(
                name: "AdjustmentOption",
                table: "adjustment_billing");

            migrationBuilder.AddColumn<string>(
                name: "AdjustmentName",
                table: "adjustment_billing",
                type: "VARCHAR(45)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "BankCategory",
                table: "adjustment_billing",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PayrollCode",
                table: "adjustment_billing",
                type: "text",
                nullable: true);
        }
    }
}
