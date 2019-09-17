using Microsoft.EntityFrameworkCore.Migrations;

namespace IPInformation.Api.Migrations
{
    public partial class databaseValidationOnIP : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_IPDetails_Ip",
                table: "IPDetails");

            migrationBuilder.AlterColumn<string>(
                name: "Ip",
                table: "IPDetails",
                nullable: false,
                oldClrType: typeof(string),
                oldNullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_IPDetails_Ip",
                table: "IPDetails",
                column: "Ip",
                unique: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_IPDetails_Ip",
                table: "IPDetails");

            migrationBuilder.AlterColumn<string>(
                name: "Ip",
                table: "IPDetails",
                nullable: true,
                oldClrType: typeof(string));

            migrationBuilder.CreateIndex(
                name: "IX_IPDetails_Ip",
                table: "IPDetails",
                column: "Ip");
        }
    }
}
