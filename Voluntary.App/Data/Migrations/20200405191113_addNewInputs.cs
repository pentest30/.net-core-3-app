using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Voluntary.App.Data.Migrations
{
    public partial class addNewInputs : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<DateTime>(
                name: "BirthDate",
                table: "Volunteers",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "datetime2");

            migrationBuilder.AddColumn<string>(
                name: "Address",
                table: "Volunteers",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "DistrictName",
                table: "Volunteers",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "FirstNameAr",
                table: "Volunteers",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Job",
                table: "Volunteers",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "LastNameAr",
                table: "Volunteers",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Neighborhood",
                table: "Volunteers",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Region",
                table: "Volunteers",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "RegistrationNumber",
                table: "Volunteers",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Sector",
                table: "Volunteers",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Address",
                table: "Volunteers");

            migrationBuilder.DropColumn(
                name: "DistrictName",
                table: "Volunteers");

            migrationBuilder.DropColumn(
                name: "FirstNameAr",
                table: "Volunteers");

            migrationBuilder.DropColumn(
                name: "Job",
                table: "Volunteers");

            migrationBuilder.DropColumn(
                name: "LastNameAr",
                table: "Volunteers");

            migrationBuilder.DropColumn(
                name: "Neighborhood",
                table: "Volunteers");

            migrationBuilder.DropColumn(
                name: "Region",
                table: "Volunteers");

            migrationBuilder.DropColumn(
                name: "RegistrationNumber",
                table: "Volunteers");

            migrationBuilder.DropColumn(
                name: "Sector",
                table: "Volunteers");

            migrationBuilder.AlterColumn<DateTime>(
                name: "BirthDate",
                table: "Volunteers",
                type: "datetime2",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldNullable: true);
        }
    }
}
