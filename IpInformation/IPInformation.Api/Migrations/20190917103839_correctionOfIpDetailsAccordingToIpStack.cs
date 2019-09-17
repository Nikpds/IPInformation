using Microsoft.EntityFrameworkCore.Migrations;

namespace IPInformation.Api.Migrations
{
    public partial class correctionOfIpDetailsAccordingToIpStack : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Country",
                table: "IPDetails",
                newName: "Country_name");

            migrationBuilder.RenameColumn(
                name: "Continent",
                table: "IPDetails",
                newName: "Continent_name");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Country_name",
                table: "IPDetails",
                newName: "Country");

            migrationBuilder.RenameColumn(
                name: "Continent_name",
                table: "IPDetails",
                newName: "Continent");
        }
    }
}
