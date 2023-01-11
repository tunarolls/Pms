using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Pms.Masterlists.Migrations
{
    public partial class init : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "company",
                columns: table => new
                {
                    CompanyId = table.Column<string>(type: "VARCHAR(35)", nullable: false),
                    Acronym = table.Column<string>(type: "VARCHAR(10)", nullable: false),
                    BranchCode = table.Column<byte>(type: "TINYINT", nullable: false),
                    MinimumRate = table.Column<double>(type: "DOUBLE(6,2)", nullable: false),
                    Region = table.Column<string>(type: "VARCHAR(10)", nullable: false),
                    RegisteredName = table.Column<string>(type: "VARCHAR(100)", nullable: false),
                    Site = table.Column<string>(type: "VARCHAR(20)", nullable: false),
                    TIN = table.Column<string>(type: "VARCHAR(20)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_company", x => x.CompanyId);
                });

            migrationBuilder.CreateTable(
                name: "masterlist",
                columns: table => new
                {
                    EEId = table.Column<string>(type: "VARCHAR(8)", nullable: false),
                    Active = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    CompanyId = table.Column<string>(type: "VARCHAR(25)", nullable: false),
                    DateResigned = table.Column<DateTime>(type: "DATE", nullable: false),
                    JobCode = table.Column<string>(type: "VARCHAR(25)", nullable: false),
                    Location = table.Column<string>(type: "VARCHAR(45)", nullable: false),
                    Site = table.Column<string>(type: "VARCHAR(25)", nullable: false),
                    BirthDate = table.Column<DateTime>(type: "DATE", nullable: false),
                    FirstName = table.Column<string>(type: "VARCHAR(45)", nullable: false),
                    Gender = table.Column<string>(type: "VARCHAR(1)", nullable: false),
                    LastName = table.Column<string>(type: "VARCHAR(45)", nullable: false),
                    MiddleName = table.Column<string>(type: "VARCHAR(45)", nullable: false),
                    NameExtension = table.Column<string>(type: "VARCHAR(6)", nullable: false),
                    Pagibig = table.Column<string>(type: "VARCHAR(20)", nullable: false),
                    PhilHealth = table.Column<string>(type: "VARCHAR(20)", nullable: false),
                    SSS = table.Column<string>(type: "VARCHAR(20)", nullable: false),
                    TIN = table.Column<string>(type: "VARCHAR(20)", nullable: false),
                    AccountNumber = table.Column<string>(type: "VARCHAR(30)", nullable: false),
                    Bank = table.Column<byte>(type: "TINYINT", nullable: false),
                    CardNumber = table.Column<string>(type: "VARCHAR(30)", nullable: false),
                    PayrollCode = table.Column<string>(type: "VARCHAR(6)", nullable: false),
                    DateCreated = table.Column<DateTime>(type: "datetime", nullable: false),
                    DateModified = table.Column<DateTime>(type: "DATETIME", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_masterlist", x => x.EEId);
                });

            migrationBuilder.CreateTable(
                name: "PayrollCodes",
                columns: table => new
                {
                    PayrollCodeId = table.Column<string>(type: "VARCHAR(12)", nullable: false),
                    CompanyId = table.Column<string>(type: "VARCHAR(35)", nullable: false),
                    Name = table.Column<string>(type: "VARCHAR(10)", nullable: false),
                    Process = table.Column<byte>(type: "TINYINT", nullable: false),
                    Site = table.Column<string>(type: "VARCHAR(20)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PayrollCodes", x => x.PayrollCodeId);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "company");

            migrationBuilder.DropTable(
                name: "masterlist");

            migrationBuilder.DropTable(
                name: "PayrollCodes");
        }
    }
}
