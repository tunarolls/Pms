using Microsoft.EntityFrameworkCore.Migrations;

namespace Pms.Masterlists.Migrations
{
    public partial class test : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "TIN",
                table: "masterlist",
                type: "VARCHAR(20)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "VARCHAR(20)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Site",
                table: "masterlist",
                type: "VARCHAR(25)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "VARCHAR(25)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "SSS",
                table: "masterlist",
                type: "VARCHAR(20)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "VARCHAR(20)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "PhilHealth",
                table: "masterlist",
                type: "VARCHAR(20)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "VARCHAR(20)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "PayrollCode",
                table: "masterlist",
                type: "VARCHAR(6)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "VARCHAR(6)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Pagibig",
                table: "masterlist",
                type: "VARCHAR(20)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "VARCHAR(20)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "NameExtension",
                table: "masterlist",
                type: "VARCHAR(6)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "VARCHAR(6)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "MiddleName",
                table: "masterlist",
                type: "VARCHAR(45)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "VARCHAR(45)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Location",
                table: "masterlist",
                type: "VARCHAR(45)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "VARCHAR(45)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "LastName",
                table: "masterlist",
                type: "VARCHAR(45)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "VARCHAR(45)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "JobCode",
                table: "masterlist",
                type: "VARCHAR(25)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "VARCHAR(25)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Gender",
                table: "masterlist",
                type: "VARCHAR(1)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "VARCHAR(1)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "FirstName",
                table: "masterlist",
                type: "VARCHAR(45)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "VARCHAR(45)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "CompanyId",
                table: "masterlist",
                type: "VARCHAR(25)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "VARCHAR(25)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "CardNumber",
                table: "masterlist",
                type: "VARCHAR(30)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "VARCHAR(30)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "AccountNumber",
                table: "masterlist",
                type: "VARCHAR(30)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "VARCHAR(30)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "TIN",
                table: "company",
                type: "VARCHAR(20)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "VARCHAR(20)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Site",
                table: "company",
                type: "VARCHAR(20)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "VARCHAR(20)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Region",
                table: "company",
                type: "VARCHAR(10)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "VARCHAR(10)",
                oldNullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "TIN",
                table: "masterlist",
                type: "VARCHAR(20)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "VARCHAR(20)");

            migrationBuilder.AlterColumn<string>(
                name: "Site",
                table: "masterlist",
                type: "VARCHAR(25)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "VARCHAR(25)");

            migrationBuilder.AlterColumn<string>(
                name: "SSS",
                table: "masterlist",
                type: "VARCHAR(20)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "VARCHAR(20)");

            migrationBuilder.AlterColumn<string>(
                name: "PhilHealth",
                table: "masterlist",
                type: "VARCHAR(20)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "VARCHAR(20)");

            migrationBuilder.AlterColumn<string>(
                name: "PayrollCode",
                table: "masterlist",
                type: "VARCHAR(6)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "VARCHAR(6)");

            migrationBuilder.AlterColumn<string>(
                name: "Pagibig",
                table: "masterlist",
                type: "VARCHAR(20)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "VARCHAR(20)");

            migrationBuilder.AlterColumn<string>(
                name: "NameExtension",
                table: "masterlist",
                type: "VARCHAR(6)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "VARCHAR(6)");

            migrationBuilder.AlterColumn<string>(
                name: "MiddleName",
                table: "masterlist",
                type: "VARCHAR(45)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "VARCHAR(45)");

            migrationBuilder.AlterColumn<string>(
                name: "Location",
                table: "masterlist",
                type: "VARCHAR(45)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "VARCHAR(45)");

            migrationBuilder.AlterColumn<string>(
                name: "LastName",
                table: "masterlist",
                type: "VARCHAR(45)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "VARCHAR(45)");

            migrationBuilder.AlterColumn<string>(
                name: "JobCode",
                table: "masterlist",
                type: "VARCHAR(25)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "VARCHAR(25)");

            migrationBuilder.AlterColumn<string>(
                name: "Gender",
                table: "masterlist",
                type: "VARCHAR(1)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "VARCHAR(1)");

            migrationBuilder.AlterColumn<string>(
                name: "FirstName",
                table: "masterlist",
                type: "VARCHAR(45)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "VARCHAR(45)");

            migrationBuilder.AlterColumn<string>(
                name: "CompanyId",
                table: "masterlist",
                type: "VARCHAR(25)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "VARCHAR(25)");

            migrationBuilder.AlterColumn<string>(
                name: "CardNumber",
                table: "masterlist",
                type: "VARCHAR(30)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "VARCHAR(30)");

            migrationBuilder.AlterColumn<string>(
                name: "AccountNumber",
                table: "masterlist",
                type: "VARCHAR(30)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "VARCHAR(30)");

            migrationBuilder.AlterColumn<string>(
                name: "TIN",
                table: "company",
                type: "VARCHAR(20)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "VARCHAR(20)");

            migrationBuilder.AlterColumn<string>(
                name: "Site",
                table: "company",
                type: "VARCHAR(20)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "VARCHAR(20)");

            migrationBuilder.AlterColumn<string>(
                name: "Region",
                table: "company",
                type: "VARCHAR(10)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "VARCHAR(10)");
        }
    }
}
