using Microsoft.EntityFrameworkCore.Migrations;

namespace Pms.Payrolls.Migrations
{
    public partial class init : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            //migrationBuilder.CreateTable(
            //    name: "payroll",
            //    columns: table => new
            //    {
            //        PayrollId = table.Column<string>(type: "VARCHAR(35)", nullable: false),
            //        EEId = table.Column<string>(type: "VARCHAR(8)", nullable: false),
            //        PayrollCode = table.Column<string>(type: "VARCHAR(6)", nullable: true),
            //        BankCategory = table.Column<string>(type: "VARCHAR(6)", nullable: true),
            //        CutoffId = table.Column<string>(type: "VARCHAR(20)", nullable: false),
            //        GrossPay = table.Column<double>(type: "DOUBLE(9,2)", nullable: false),
            //        RegPay = table.Column<double>(type: "DOUBLE(9,2)", nullable: false),
            //        NetPay = table.Column<double>(type: "DOUBLE(9,2)", nullable: false),
            //        Adjust1Total = table.Column<double>(type: "DOUBLE(9,2)", nullable: false),
            //        Adjust2Total = table.Column<double>(type: "DOUBLE(9,2)", nullable: false),
            //        GovernmentTotal = table.Column<double>(type: "DOUBLE(9,2)", nullable: false)
            //    },
            //    constraints: table =>
            //    {
            //        table.PrimaryKey("PK_payroll", x => x.PayrollId);
            //    });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            //migrationBuilder.DropTable(
            //    name: "payroll");
        }
    }
}
