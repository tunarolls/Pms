using Microsoft.EntityFrameworkCore.Migrations;

namespace Pms.Payrolls.Migrations
{
    public partial class removedBankCategory : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            //migrationBuilder.DropColumn(
            //    name: "Bank",
            //    table: "payroll");

            //migrationBuilder.DropColumn(
            //    name: "BankCategory",
            //    table: "payroll");

            //migrationBuilder.Sql("ALTER TABLE payroll RENAME COLUMN RegPay TO RegularPay");
            ////migrationBuilder.RenameColumn(
            ////    name: "RegPay",
            ////    table: "payroll",
            ////    newName: "RegularPay");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            //migrationBuilder.Sql("ALTER TABLE payroll RENAME COLUMN RegularPay TO RegPay");
            ////migrationBuilder.RenameColumn(
            ////    name: "RegularPay",
            ////    table: "payroll",
            ////    newName: "RegPay");

            //migrationBuilder.AddColumn<int>(
            //    name: "Bank",
            //    table: "payroll",
            //    type: "int",
            //    nullable: false,
            //    defaultValue: 0);

            //migrationBuilder.AddColumn<string>(
            //    name: "BankCategory",
            //    table: "payroll",
            //    type: "VARCHAR(6)",
            //    nullable: true);
        }
    }
}
