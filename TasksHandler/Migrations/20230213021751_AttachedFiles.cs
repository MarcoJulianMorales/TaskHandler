using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TasksHandler.Migrations
{
    /// <inheritdoc />
    public partial class AttachedFiles : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Steps_Steps_StepsId",
                table: "Steps");

            migrationBuilder.DropIndex(
                name: "IX_Steps_StepsId",
                table: "Steps");

            migrationBuilder.DropColumn(
                name: "StepsId",
                table: "Steps");

            migrationBuilder.CreateTable(
                name: "AttachedFiles",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    TaskId = table.Column<int>(type: "int", nullable: false),
                    Url = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Tittle = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Orden = table.Column<int>(type: "int", nullable: false),
                    CreationDate = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AttachedFiles", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AttachedFiles_Tasks_TaskId",
                        column: x => x.TaskId,
                        principalTable: "Tasks",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AttachedFiles_TaskId",
                table: "AttachedFiles",
                column: "TaskId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AttachedFiles");

            migrationBuilder.AddColumn<Guid>(
                name: "StepsId",
                table: "Steps",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Steps_StepsId",
                table: "Steps",
                column: "StepsId");

            migrationBuilder.AddForeignKey(
                name: "FK_Steps_Steps_StepsId",
                table: "Steps",
                column: "StepsId",
                principalTable: "Steps",
                principalColumn: "Id");
        }
    }
}
