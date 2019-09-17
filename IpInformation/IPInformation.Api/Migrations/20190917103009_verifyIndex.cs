using Microsoft.EntityFrameworkCore.Migrations;

namespace IPInformation.Api.Migrations
{
    public partial class verifyIndex : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "Ip",
                table: "IPDetails",
                nullable: true,
                oldClrType: typeof(string),
                oldNullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_IPDetails_Ip",
                table: "IPDetails",
                column: "Ip");
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
                oldClrType: typeof(string),
                oldNullable: true);
        }
    }
}
