using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BurnoutTracker.Migrations
{
    /// <inheritdoc />
    public partial class addDeveloperBurnoutState : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "DeveloperBurnoutStates",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    UserRepositoryConnectionId = table.Column<long>(type: "bigint", nullable: false),
                    DeveloperLogin = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    State = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    WeeklyCommitCount = table.Column<int>(type: "int", nullable: false),
                    RecordedAt = table.Column<DateTime>(type: "datetime(6)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DeveloperBurnoutStates", x => x.Id);
                    table.ForeignKey(
                        name: "FK_DeveloperBurnoutStates_UserRepositoryConnections_UserReposit~",
                        column: x => x.UserRepositoryConnectionId,
                        principalTable: "UserRepositoryConnections",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateIndex(
                name: "IX_DeveloperBurnoutStates_UserRepositoryConnectionId",
                table: "DeveloperBurnoutStates",
                column: "UserRepositoryConnectionId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "DeveloperBurnoutStates");
        }
    }
}
