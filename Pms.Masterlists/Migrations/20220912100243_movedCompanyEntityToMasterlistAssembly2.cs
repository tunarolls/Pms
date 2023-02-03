using Microsoft.EntityFrameworkCore.Migrations;

namespace Pms.Masterlists.Migrations
{
    public partial class movedCompanyEntityToMasterlistAssembly2 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "payrollcodes",
                columns: table => new
                {
                    PayrollCodeId = table.Column<string>(type: "VARCHAR(12)", nullable: false),
                    Name = table.Column<string>(type: "VARCHAR(10)", nullable: false),
                    CompanyId = table.Column<string>(type: "VARCHAR(35)", nullable: false),
                    Site = table.Column<string>(type: "VARCHAR(20)", nullable: false),
                    Process = table.Column<byte>(type: "TINYINT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PayrollCode", x => x.PayrollCodeId);
                });

            migrationBuilder.CreateTable(
                name: "company",
                columns: table => new
                {
                    CompanyId = table.Column<string>(type: "VARCHAR(35)", nullable: false),
                    Site = table.Column<string>(type: "VARCHAR(20)", nullable: true),
                    Acronym = table.Column<string>(type: "VARCHAR(10)", nullable: false),
                    RegisteredName = table.Column<string>(type: "VARCHAR(100)", nullable: false),
                    Region = table.Column<string>(type: "VARCHAR(10)", nullable: true),
                    TIN = table.Column<string>(type: "VARCHAR(20)", nullable: true),
                    BranchCode = table.Column<byte>(type: "TINYINT", nullable: false),
                    MinimumRate = table.Column<double>(type: "DOUBLE(6,2)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_company", x => x.CompanyId);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "payrollcodes");

            migrationBuilder.DropTable(
                name: "company");
        }
    }
}
