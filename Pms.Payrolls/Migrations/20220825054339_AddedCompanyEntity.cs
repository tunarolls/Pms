using Microsoft.EntityFrameworkCore.Migrations;

namespace Pms.Payrolls.Migrations
{
    public partial class AddedCompanyEntity : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            //migrationBuilder.CreateTable(
            //    name: "company",
            //    columns: table => new
            //    {
            //        CompanyId = table.Column<string>(type: "VARCHAR(35)", nullable: false),
            //        Site = table.Column<string>(type: "VARCHAR(20)", nullable: false),
            //        Acronym = table.Column<string>(type: "VARCHAR(10)", nullable: false),
            //        RegisteredName = table.Column<string>(type: "VARCHAR(100)", nullable: false),
            //        Region = table.Column<string>(type: "VARCHAR(10)", nullable: false),
            //        TIN = table.Column<string>(type: "VARCHAR(20)", nullable: false),
            //        BranchCode = table.Column<string>(type: "VARCHAR(10)", nullable: false)
            //    },
            //    constraints: table =>
            //    {
            //        table.PrimaryKey("PK_company", x => x.CompanyId);
            //    });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            //migrationBuilder.DropTable(
            //    name: "company");
        }
    }
}
