using Microsoft.EntityFrameworkCore.Migrations;

namespace Pms.Adjustments.Migrations
{
    public partial class renamedDeductedToApplied : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Deducted",
                table: "adjustment_billing",
                newName: "Applied");
            //migrationBuilder.Sql("ALTER TABLE adjustment_billing RENAME COLUMN Deducted TO Applied;");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Applied",
                table: "adjustment_billing",
                newName: "Deducted");
            //migrationBuilder.Sql("ALTER TABLE adjustment_billing RENAME COLUMN Applied TO Deducted;");
        }
    }
}
