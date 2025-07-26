using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BurnoutTracker.Migrations
{
    /// <inheritdoc />
    public partial class replacedGithubTokenwithUserRepositoryConnection : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "GitHubTokens");

            migrationBuilder.CreateTable(
                name: "UserRepositoryConnections",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    UserId = table.Column<long>(type: "bigint", nullable: false),
                    RepositoryUrl = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    SupportedRepositoryId = table.Column<long>(type: "bigint", nullable: false),
                    SupportedRepositoryId1 = table.Column<int>(type: "int", nullable: false),
                    AccessToken = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    ConnectedAt = table.Column<DateTime>(type: "datetime(6)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserRepositoryConnections", x => x.Id);
                    table.ForeignKey(
                        name: "FK_UserRepositoryConnections_SupportedRepositories_SupportedRep~",
                        column: x => x.SupportedRepositoryId1,
                        principalTable: "SupportedRepositories",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_UserRepositoryConnections_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateIndex(
                name: "IX_UserRepositoryConnections_SupportedRepositoryId1",
                table: "UserRepositoryConnections",
                column: "SupportedRepositoryId1");

            migrationBuilder.CreateIndex(
                name: "IX_UserRepositoryConnections_UserId",
                table: "UserRepositoryConnections",
                column: "UserId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "UserRepositoryConnections");

            migrationBuilder.CreateTable(
                name: "GitHubTokens",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    UserId = table.Column<long>(type: "bigint", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    Token = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GitHubTokens", x => x.Id);
                    table.ForeignKey(
                        name: "FK_GitHubTokens_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateIndex(
                name: "IX_GitHubTokens_UserId",
                table: "GitHubTokens",
                column: "UserId");
        }
    }
}
