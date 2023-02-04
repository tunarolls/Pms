using Microsoft.EntityFrameworkCore.Migrations;

namespace Pms.Payrolls.Migrations
{
    public partial class turnover_changes : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "PayrollCode",
                table: "payroll",
                type: "VARCHAR(6)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "VARCHAR(6)",
                oldNullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "PayrollCode",
                table: "payroll",
                type: "VARCHAR(6)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "VARCHAR(6)");
        }
    }
}
