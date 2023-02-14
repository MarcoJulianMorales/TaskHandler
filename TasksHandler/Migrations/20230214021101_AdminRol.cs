using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TasksHandler.Migrations
{
    /// <inheritdoc />
    public partial class AdminRol : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"IF NOT EXISTS (Select Id from AspNetRoles where Id = '5423c3ca-b458-4c26-9ff4-9bbf872a0185')
                                    BEGIN
	                                    INSERT AspNetRoles (Id, [Name], [NormalizedName])
	                                    VALUES ('5423c3ca-b458-4c26-9ff4-9bbf872a0185', 'admin', 'ADMIN')
                                    END");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"DELETE AspNetRoles WHERE Id = '5423c3ca-b458-4c26-9ff4-9bbf872a0185'");
        }
    }
}
