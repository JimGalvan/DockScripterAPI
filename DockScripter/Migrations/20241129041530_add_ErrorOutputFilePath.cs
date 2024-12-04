using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DockScripter.Migrations
{
    /// <inheritdoc />
    public partial class add_ErrorOutputFilePath : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ErrorOutputFilePath",
                table: "ExecutionResultEntities",
                type: "TEXT",
                maxLength: 1024,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "OutputFilePath",
                table: "ExecutionResultEntities",
                type: "TEXT",
                maxLength: 1024,
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ErrorOutputFilePath",
                table: "ExecutionResultEntities");

            migrationBuilder.DropColumn(
                name: "OutputFilePath",
                table: "ExecutionResultEntities");
        }
    }
}
