using Microsoft.EntityFrameworkCore.Migrations;

namespace Pms.Masterlists.Migrations
{
    public partial class addedGender : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            //migrationBuilder.AddColumn<string>(
            //    name: "Gender",
            //    table: "masterlist",
            //    type: "VARCHAR(1)",
            //    nullable: true);

            //migrationBuilder.AddColumn<string>(
            //    name: "NameExtension",
            //    table: "masterlist",
            //    type: "text",
            //    nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            //migrationBuilder.DropColumn(
            //    name: "Gender",
            //    table: "masterlist");

            //migrationBuilder.DropColumn(
            //    name: "NameExtension",
            //    table: "masterlist");
        }
    }
}
