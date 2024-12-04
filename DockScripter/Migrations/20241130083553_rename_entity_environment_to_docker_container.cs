using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DockScripter.Migrations
{
    /// <inheritdoc />
    public partial class rename_entity_environment_to_docker_container : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "EnvironmentEntities");

            migrationBuilder.AddColumn<Guid>(
                name: "DockerContainerId",
                table: "ScriptEntities",
                type: "TEXT",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "DockerContainerEntities",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    DockerContainerName = table.Column<string>(type: "TEXT", maxLength: 255, nullable: true),
                    Status = table.Column<int>(type: "INTEGER", nullable: false),
                    DockerImage = table.Column<string>(type: "TEXT", maxLength: 255, nullable: false),
                    ContainerId = table.Column<string>(type: "TEXT", maxLength: 64, nullable: true),
                    UserId = table.Column<Guid>(type: "TEXT", nullable: false),
                    CreationDateTimeUtc = table.Column<DateTime>(type: "TEXT", nullable: false),
                    LastUpdatedDateTimeUtc = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DockerContainerEntities", x => x.Id);
                    table.ForeignKey(
                        name: "FK_DockerContainerEntities_UserEntities_UserId",
                        column: x => x.UserId,
                        principalTable: "UserEntities",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ScriptEntities_DockerContainerId",
                table: "ScriptEntities",
                column: "DockerContainerId");

            migrationBuilder.CreateIndex(
                name: "IX_DockerContainerEntities_UserId",
                table: "DockerContainerEntities",
                column: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_ScriptEntities_DockerContainerEntities_DockerContainerId",
                table: "ScriptEntities",
                column: "DockerContainerId",
                principalTable: "DockerContainerEntities",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ScriptEntities_DockerContainerEntities_DockerContainerId",
                table: "ScriptEntities");

            migrationBuilder.DropTable(
                name: "DockerContainerEntities");

            migrationBuilder.DropIndex(
                name: "IX_ScriptEntities_DockerContainerId",
                table: "ScriptEntities");

            migrationBuilder.DropColumn(
                name: "DockerContainerId",
                table: "ScriptEntities");

            migrationBuilder.CreateTable(
                name: "EnvironmentEntities",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    UserId = table.Column<Guid>(type: "TEXT", nullable: false),
                    ContainerId = table.Column<string>(type: "TEXT", maxLength: 64, nullable: true),
                    CreationDateTimeUtc = table.Column<DateTime>(type: "TEXT", nullable: false),
                    EnvironmentName = table.Column<string>(type: "TEXT", maxLength: 50, nullable: false),
                    LastUpdatedDateTimeUtc = table.Column<DateTime>(type: "TEXT", nullable: false),
                    Status = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EnvironmentEntities", x => x.Id);
                    table.ForeignKey(
                        name: "FK_EnvironmentEntities_UserEntities_UserId",
                        column: x => x.UserId,
                        principalTable: "UserEntities",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_EnvironmentEntities_UserId",
                table: "EnvironmentEntities",
                column: "UserId");
        }
    }
}
