using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BeaversTests.TestsManager.Infrastructure.DataAccess.Migrations
{
    /// <inheritdoc />
    public partial class TestDriverstablefix : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "TestDrivers",
                keyColumn: "Key",
                keyValue: "NUnit");

            migrationBuilder.DropColumn(
                name: "IsDefault",
                table: "TestDrivers");

            migrationBuilder.AddColumn<string>(
                name: "UserCreatorId",
                table: "TestDrivers",
                type: "character varying(50)",
                maxLength: 50,
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "UserCreatorId",
                table: "TestDrivers");

            migrationBuilder.AddColumn<bool>(
                name: "IsDefault",
                table: "TestDrivers",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.InsertData(
                table: "TestDrivers",
                columns: new[] { "Key", "Description", "IsDefault" },
                values: new object[] { "NUnit", null, true });
        }
    }
}
