using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BeaversTests.TestsManager.Infrastructure.DataAccess.Migrations
{
    /// <inheritdoc />
    public partial class TestProjectUserCreatoridadded : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "UserCreatorId",
                table: "TestProjects",
                type: "character varying(50)",
                maxLength: 50,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "ValidationMessage",
                table: "TestPackages",
                type: "character varying(1000)",
                maxLength: 1000,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "ValidationStatus",
                table: "TestPackages",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.InsertData(
                table: "TestDrivers",
                columns: new[] { "Key", "Description", "IsDefault" },
                values: new object[] { "NUnit", null, true });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "TestDrivers",
                keyColumn: "Key",
                keyValue: "NUnit");

            migrationBuilder.DropColumn(
                name: "UserCreatorId",
                table: "TestProjects");

            migrationBuilder.DropColumn(
                name: "ValidationMessage",
                table: "TestPackages");

            migrationBuilder.DropColumn(
                name: "ValidationStatus",
                table: "TestPackages");
        }
    }
}
