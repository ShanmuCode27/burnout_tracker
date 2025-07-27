using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BurnoutTracker.Migrations
{
    /// <inheritdoc />
    public partial class addBranchColumnOnUserRepositoryConnection : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Branch",
                table: "UserRepositoryConnections",
                type: "longtext",
                nullable: false)
                .Annotation("MySql:CharSet", "utf8mb4");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Branch",
                table: "UserRepositoryConnections");
        }
    }
}
