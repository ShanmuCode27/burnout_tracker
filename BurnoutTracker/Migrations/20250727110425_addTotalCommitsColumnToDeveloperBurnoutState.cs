using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BurnoutTracker.Migrations
{
    /// <inheritdoc />
    public partial class addTotalCommitsColumnToDeveloperBurnoutState : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "TotalCommitCount",
                table: "DeveloperBurnoutStates",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "TotalCommitCount",
                table: "DeveloperBurnoutStates");
        }
    }
}
