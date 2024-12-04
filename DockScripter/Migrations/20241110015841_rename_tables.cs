using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DockScripter.Migrations
{
    /// <inheritdoc />
    public partial class rename_tables : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_EnvironmentEntity_UserEntities_UserId",
                table: "EnvironmentEntity");

            migrationBuilder.DropForeignKey(
                name: "FK_ScriptEntity_UserEntities_UserId",
                table: "ScriptEntity");

            migrationBuilder.DropPrimaryKey(
                name: "PK_ScriptEntity",
                table: "ScriptEntity");

            migrationBuilder.DropPrimaryKey(
                name: "PK_EnvironmentEntity",
                table: "EnvironmentEntity");

            migrationBuilder.RenameTable(
                name: "ScriptEntity",
                newName: "ScriptEntities");

            migrationBuilder.RenameTable(
                name: "EnvironmentEntity",
                newName: "EnvironmentEntities");

            migrationBuilder.RenameIndex(
                name: "IX_ScriptEntity_UserId",
                table: "ScriptEntities",
                newName: "IX_ScriptEntities_UserId");

            migrationBuilder.RenameIndex(
                name: "IX_EnvironmentEntity_UserId",
                table: "EnvironmentEntities",
                newName: "IX_EnvironmentEntities_UserId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_ScriptEntities",
                table: "ScriptEntities",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_EnvironmentEntities",
                table: "EnvironmentEntities",
                column: "Id");

            migrationBuilder.CreateTable(
                name: "ExecutionResultEntities",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    Output = table.Column<string>(type: "TEXT", nullable: false),
                    ErrorOutput = table.Column<string>(type: "TEXT", nullable: true),
                    ExecutedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    ScriptId = table.Column<Guid>(type: "TEXT", nullable: false),
                    Status = table.Column<int>(type: "INTEGER", nullable: false),
                    CreationDateTimeUtc = table.Column<DateTime>(type: "TEXT", nullable: false),
                    LastUpdatedDateTimeUtc = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ExecutionResultEntities", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ExecutionResultEntities_ScriptEntities_ScriptId",
                        column: x => x.ScriptId,
                        principalTable: "ScriptEntities",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ExecutionResultEntities_ScriptId",
                table: "ExecutionResultEntities",
                column: "ScriptId");

            migrationBuilder.AddForeignKey(
                name: "FK_EnvironmentEntities_UserEntities_UserId",
                table: "EnvironmentEntities",
                column: "UserId",
                principalTable: "UserEntities",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ScriptEntities_UserEntities_UserId",
                table: "ScriptEntities",
                column: "UserId",
                principalTable: "UserEntities",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_EnvironmentEntities_UserEntities_UserId",
                table: "EnvironmentEntities");

            migrationBuilder.DropForeignKey(
                name: "FK_ScriptEntities_UserEntities_UserId",
                table: "ScriptEntities");

            migrationBuilder.DropTable(
                name: "ExecutionResultEntities");

            migrationBuilder.DropPrimaryKey(
                name: "PK_ScriptEntities",
                table: "ScriptEntities");

            migrationBuilder.DropPrimaryKey(
                name: "PK_EnvironmentEntities",
                table: "EnvironmentEntities");

            migrationBuilder.RenameTable(
                name: "ScriptEntities",
                newName: "ScriptEntity");

            migrationBuilder.RenameTable(
                name: "EnvironmentEntities",
                newName: "EnvironmentEntity");

            migrationBuilder.RenameIndex(
                name: "IX_ScriptEntities_UserId",
                table: "ScriptEntity",
                newName: "IX_ScriptEntity_UserId");

            migrationBuilder.RenameIndex(
                name: "IX_EnvironmentEntities_UserId",
                table: "EnvironmentEntity",
                newName: "IX_EnvironmentEntity_UserId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_ScriptEntity",
                table: "ScriptEntity",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_EnvironmentEntity",
                table: "EnvironmentEntity",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_EnvironmentEntity_UserEntities_UserId",
                table: "EnvironmentEntity",
                column: "UserId",
                principalTable: "UserEntities",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ScriptEntity_UserEntities_UserId",
                table: "ScriptEntity",
                column: "UserId",
                principalTable: "UserEntities",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
