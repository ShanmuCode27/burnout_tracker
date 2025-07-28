using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BurnoutTracker.Migrations
{
    /// <inheritdoc />
    public partial class addMoreInfotoDeveloperBurnoutStatesEntity : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "BurnoutScore",
                table: "DeveloperBurnoutStates",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<DateTime>(
                name: "LatestWorkTimeUtc",
                table: "DeveloperBurnoutStates",
                type: "datetime(6)",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "NightWorkCount",
                table: "DeveloperBurnoutStates",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "PullRequestCount",
                table: "DeveloperBurnoutStates",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "RevertCount",
                table: "DeveloperBurnoutStates",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "ReviewChangesCount",
                table: "DeveloperBurnoutStates",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "BurnoutScore",
                table: "DeveloperBurnoutStates");

            migrationBuilder.DropColumn(
                name: "LatestWorkTimeUtc",
                table: "DeveloperBurnoutStates");

            migrationBuilder.DropColumn(
                name: "NightWorkCount",
                table: "DeveloperBurnoutStates");

            migrationBuilder.DropColumn(
                name: "PullRequestCount",
                table: "DeveloperBurnoutStates");

            migrationBuilder.DropColumn(
                name: "RevertCount",
                table: "DeveloperBurnoutStates");

            migrationBuilder.DropColumn(
                name: "ReviewChangesCount",
                table: "DeveloperBurnoutStates");
        }
    }
}
