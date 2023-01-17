using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Pms.Adjustments.Migrations
{
    public partial class init : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "BillingRecords",
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
                    table.PrimaryKey("PK_BillingRecords", x => x.RecordId);
                });

            migrationBuilder.CreateTable(
                name: "adjustment_billing",
                columns: table => new
                {
                    BillingId = table.Column<string>(type: "VARCHAR(45)", nullable: false),
                    EEId = table.Column<string>(type: "VARCHAR(8)", nullable: false),
                    RecordId = table.Column<string>(type: "VARCHAR(45)", nullable: true),
                    CutoffId = table.Column<string>(type: "VARCHAR(6)", nullable: false),
                    Amount = table.Column<double>(type: "DOUBLE(8,2)", nullable: false),
                    Deducted = table.Column<byte>(type: "TINYINT", nullable: false),
                    Remarks = table.Column<string>(type: "text", nullable: false),
                    AdjustmentType = table.Column<byte>(type: "TINYINT", nullable: false),
                    AdjustmentOption = table.Column<byte>(type: "TINYINT", nullable: false),
                    DateCreated = table.Column<DateTime>(type: "TIMESTAMP", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_adjustment_billing", x => x.BillingId);
                    table.ForeignKey(
                        name: "FK_adjustment_billing_BillingRecords_RecordId",
                        column: x => x.RecordId,
                        principalTable: "BillingRecords",
                        principalColumn: "RecordId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_adjustment_billing_EEId",
                table: "adjustment_billing",
                column: "EEId");

            migrationBuilder.CreateIndex(
                name: "IX_adjustment_billing_RecordId",
                table: "adjustment_billing",
                column: "RecordId");

            migrationBuilder.CreateIndex(
                name: "IX_BillingRecords_EEId",
                table: "BillingRecords",
                column: "EEId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "adjustment_billing");

            migrationBuilder.DropTable(
                name: "BillingRecords");
        }
    }
}
