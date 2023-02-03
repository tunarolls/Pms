using Microsoft.EntityFrameworkCore.Migrations;

namespace Pms.Masterlists.Migrations
{
    public partial class definedMaxLengthToJobCode : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "JobCode",
                table: "masterlist",
                type: "VARCHAR(25)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "JobCode",
                table: "masterlist",
                type: "text",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "VARCHAR(25)",
                oldNullable: true);
        }
    }
}
