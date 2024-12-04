using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DockScripter.Migrations
{
    /// <inheritdoc />
    public partial class add_script_files : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "EntryFilePath",
                table: "ScriptEntities",
                type: "TEXT",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "ScriptFiles",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    S3Key = table.Column<string>(type: "TEXT", maxLength: 1024, nullable: false),
                    ScriptId = table.Column<Guid>(type: "TEXT", nullable: false),
                    CreationDateTimeUtc = table.Column<DateTime>(type: "TEXT", nullable: false),
                    LastUpdatedDateTimeUtc = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ScriptFiles", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ScriptFiles_ScriptEntities_ScriptId",
                        column: x => x.ScriptId,
                        principalTable: "ScriptEntities",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ScriptFiles_ScriptId",
                table: "ScriptFiles",
                column: "ScriptId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ScriptFiles");

            migrationBuilder.DropColumn(
                name: "EntryFilePath",
                table: "ScriptEntities");
        }
    }
}
