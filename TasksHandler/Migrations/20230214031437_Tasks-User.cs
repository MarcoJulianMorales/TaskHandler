using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TasksHandler.Migrations
{
    /// <inheritdoc />
    public partial class TasksUser : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "UserCreatedId",
                table: "Tasks",
                type: "nvarchar(450)",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Tasks_UserCreatedId",
                table: "Tasks",
                column: "UserCreatedId");

            migrationBuilder.AddForeignKey(
                name: "FK_Tasks_AspNetUsers_UserCreatedId",
                table: "Tasks",
                column: "UserCreatedId",
                principalTable: "AspNetUsers",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Tasks_AspNetUsers_UserCreatedId",
                table: "Tasks");

            migrationBuilder.DropIndex(
                name: "IX_Tasks_UserCreatedId",
                table: "Tasks");

            migrationBuilder.DropColumn(
                name: "UserCreatedId",
                table: "Tasks");
        }
    }
}
