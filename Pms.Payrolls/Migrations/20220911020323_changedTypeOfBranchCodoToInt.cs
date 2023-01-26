using Microsoft.EntityFrameworkCore.Migrations;

namespace Pms.Payrolls.Migrations
{
    public partial class changedTypeOfBranchCodoToInt : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            //migrationBuilder.AlterColumn<byte>(
            //    name: "BranchCode",
            //    table: "company",
            //    type: "TINYINT",
            //    nullable: false,
            //    defaultValue: (byte)0,
            //    oldClrType: typeof(string),
            //    oldType: "VARCHAR(10)",
            //    oldNullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            //migrationBuilder.AlterColumn<string>(
            //    name: "BranchCode",
            //    table: "company",
            //    type: "VARCHAR(10)",
            //    nullable: true,
            //    oldClrType: typeof(byte),
            //    oldType: "TINYINT");
        }
    }
}
