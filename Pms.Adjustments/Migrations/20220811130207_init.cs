using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Pms.Adjustments.Migrations
{
    public partial class init : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
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
                    Remarks = table.Column<string>(type: "text", nullable: true),
                    AdjustmentName = table.Column<string>(type: "VARCHAR(45)", nullable: false),
                    AdjustmentType = table.Column<byte>(type: "TINYINT", nullable: false),
                    DateCreated = table.Column<DateTime>(type: "TIMESTAMP", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_adjustment_billing", x => x.BillingId);
                });

            migrationBuilder.CreateIndex(
                name: "IX_adjustment_billing_EEId",
                table: "adjustment_billing",
                column: "EEId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "adjustment_billing");
        }
    }
}
