using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Voluntary.App.Data.Migrations
{
    public partial class TaskIdNullable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Volunteers_Tasks_TaskId",
                table: "Volunteers");

            migrationBuilder.AlterColumn<Guid>(
                name: "TaskId",
                table: "Volunteers",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier");

            migrationBuilder.AddForeignKey(
                name: "FK_Volunteers_Tasks_TaskId",
                table: "Volunteers",
                column: "TaskId",
                principalTable: "Tasks",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Volunteers_Tasks_TaskId",
                table: "Volunteers");

            migrationBuilder.AlterColumn<Guid>(
                name: "TaskId",
                table: "Volunteers",
                type: "uniqueidentifier",
                nullable: false,
                oldClrType: typeof(Guid),
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Volunteers_Tasks_TaskId",
                table: "Volunteers",
                column: "TaskId",
                principalTable: "Tasks",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
