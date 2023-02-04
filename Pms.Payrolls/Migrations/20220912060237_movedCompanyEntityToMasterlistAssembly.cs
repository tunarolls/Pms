using Microsoft.EntityFrameworkCore.Migrations;

namespace Pms.Payrolls.Migrations
{
    public partial class movedCompanyEntityToMasterlistAssembly : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "company");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "company",
                columns: table => new
                {
                    CompanyId = table.Column<string>(type: "VARCHAR(35)", nullable: false),
                    Acronym = table.Column<string>(type: "VARCHAR(10)", nullable: false),
                    BranchCode = table.Column<byte>(type: "TINYINT", nullable: false),
                    MinimumRate = table.Column<double>(type: "DOUBLE(6,2)", nullable: false),
                    Region = table.Column<string>(type: "VARCHAR(10)", nullable: true),
                    RegisteredName = table.Column<string>(type: "VARCHAR(100)", nullable: false),
                    Site = table.Column<string>(type: "VARCHAR(20)", nullable: true),
                    TIN = table.Column<string>(type: "VARCHAR(20)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_company", x => x.CompanyId);
                });
        }
    }
}
