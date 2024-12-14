using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BeaversTests.TestsManager.Infrastructure.DataAccess.Migrations
{
    /// <inheritdoc />
    public partial class TestDrivervalidationstatusfieldsadded : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ValidationMessage",
                table: "TestDrivers",
                type: "character varying(1000)",
                maxLength: 1000,
                nullable: true,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "ValidationResult",
                table: "TestDrivers",
                type: "text",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ValidationMessage",
                table: "TestDrivers");

            migrationBuilder.DropColumn(
                name: "ValidationResult",
                table: "TestDrivers");
        }
    }
}
