using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BeaversTests.TestsManager.Infrastructure.DataAccess.Migrations
{
    /// <inheritdoc />
    public partial class TestDriversFix : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsDefault",
                table: "TestDrivers",
                type: "boolean",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsDefault",
                table: "TestDrivers");
        }
    }
}
