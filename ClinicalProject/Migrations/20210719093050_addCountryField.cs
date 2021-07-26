using Microsoft.EntityFrameworkCore.Migrations;

namespace ClinicProject.Migrations
{
    public partial class addCountryField : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Country",
                table: "Patients",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Country",
                table: "Doctors",
                type: "nvarchar(max)",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Country",
                table: "Patients");

            migrationBuilder.DropColumn(
                name: "Country",
                table: "Doctors");
        }
    }
}
