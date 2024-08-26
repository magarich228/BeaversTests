using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BeaversTests.TestsManager.Infrastructure.DataAccess.Migrations
{
    /// <inheritdoc />
    public partial class TestDriversAdded : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "TestPackageType",
                table: "TestPackages");

            migrationBuilder.AddColumn<string>(
                name: "TestDriverKey",
                table: "TestPackages",
                type: "character varying(25)",
                maxLength: 25,
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateTable(
                name: "TestDrivers",
                columns: table => new
                {
                    Key = table.Column<string>(type: "character varying(25)", maxLength: 25, nullable: false),
                    Description = table.Column<string>(type: "character varying(250)", maxLength: 250, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TestDrivers", x => x.Key);
                });

            migrationBuilder.CreateIndex(
                name: "IX_TestPackages_TestDriverKey",
                table: "TestPackages",
                column: "TestDriverKey");

            migrationBuilder.AddForeignKey(
                name: "FK_TestPackages_TestDrivers_TestDriverKey",
                table: "TestPackages",
                column: "TestDriverKey",
                principalTable: "TestDrivers",
                principalColumn: "Key",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_TestPackages_TestDrivers_TestDriverKey",
                table: "TestPackages");

            migrationBuilder.DropTable(
                name: "TestDrivers");

            migrationBuilder.DropIndex(
                name: "IX_TestPackages_TestDriverKey",
                table: "TestPackages");

            migrationBuilder.DropColumn(
                name: "TestDriverKey",
                table: "TestPackages");

            migrationBuilder.AddColumn<string>(
                name: "TestPackageType",
                table: "TestPackages",
                type: "character varying(50)",
                maxLength: 50,
                nullable: false,
                defaultValue: "");
        }
    }
}
