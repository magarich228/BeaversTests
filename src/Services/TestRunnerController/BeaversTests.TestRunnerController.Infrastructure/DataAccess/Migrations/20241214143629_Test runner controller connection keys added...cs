using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BeaversTests.TestRunnerController.Infrastructure.DataAccess.Migrations
{
    /// <inheritdoc />
    public partial class Testrunnercontrollerconnectionkeysadded : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Key",
                table: "TestAgents",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "OwnerId",
                table: "TestAgents",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateTable(
                name: "ControllerUserKeys",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Key = table.Column<string>(type: "text", nullable: false),
                    OwnerId = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ControllerUserKeys", x => x.Id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ControllerUserKeys");

            migrationBuilder.DropColumn(
                name: "Key",
                table: "TestAgents");

            migrationBuilder.DropColumn(
                name: "OwnerId",
                table: "TestAgents");
        }
    }
}
