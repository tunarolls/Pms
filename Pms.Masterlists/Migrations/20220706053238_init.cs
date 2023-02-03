using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Pms.Masterlists.Migrations
{
    public partial class init : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "masterlist",
                columns: table => new
                {
                    EEId = table.Column<string>(type: "VARCHAR(8)", nullable: false),
                    FirstName = table.Column<string>(type: "VARCHAR(45)", nullable: true),
                    LastName = table.Column<string>(type: "VARCHAR(45)", nullable: true),
                    MiddleName = table.Column<string>(type: "VARCHAR(45)", nullable: true),
                    Location = table.Column<string>(type: "VARCHAR(45)", nullable: true),
                    Site = table.Column<string>(type: "VARCHAR(15)", nullable: true),
                    CardNumber = table.Column<string>(type: "VARCHAR(20)", nullable: true),
                    AccountNumber = table.Column<string>(type: "VARCHAR(20)", nullable: true),
                    PayrollCode = table.Column<string>(type: "VARCHAR(6)", nullable: true),
                    BankCategory = table.Column<string>(type: "VARCHAR(10)", nullable: true),
                    BankName = table.Column<string>(type: "VARCHAR(10)", nullable: true),
                    Pagibig = table.Column<string>(type: "VARCHAR(20)", nullable: true),
                    PhilHealth = table.Column<string>(type: "VARCHAR(20)", nullable: true),
                    SSS = table.Column<string>(type: "VARCHAR(20)", nullable: true),
                    TIN = table.Column<string>(type: "VARCHAR(20)", nullable: true),
                    BirthDate = table.Column<DateTime>(type: "DATE", nullable: false),
                    Active = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    DateModified = table.Column<DateTime>(type: "DATETIME", nullable: false),
                    DateCreated = table.Column<DateTime>(type: "datetime", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_masterlist", x => x.EEId);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "masterlist");
        }
    }
}
