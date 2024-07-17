using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BeaversTests.TestsManager.Infrastructure.DataAccess.Migrations
{
    /// <inheritdoc />
    public partial class TestPackageBytesRemoved : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "TestPackage",
                table: "TestPackages");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<byte[]>(
                name: "TestPackage",
                table: "TestPackages",
                type: "bytea",
                nullable: false,
                defaultValue: new byte[0]);
        }
    }
}
