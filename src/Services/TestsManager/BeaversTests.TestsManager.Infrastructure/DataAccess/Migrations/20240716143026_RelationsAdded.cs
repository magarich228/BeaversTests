using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BeaversTests.TestsManager.Infrastructure.DataAccess.Migrations
{
    /// <inheritdoc />
    public partial class RelationsAdded : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "TestProjects",
                type: "character varying(50)",
                maxLength: 50,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AlterColumn<string>(
                name: "Description",
                table: "TestProjects",
                type: "character varying(1000)",
                maxLength: 1000,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "TestPackageType",
                table: "TestPackages",
                type: "character varying(50)",
                maxLength: 50,
                nullable: false,
                oldClrType: typeof(int),
                oldType: "integer");

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "TestPackages",
                type: "character varying(100)",
                maxLength: 100,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AlterColumn<string>(
                name: "Description",
                table: "TestPackages",
                type: "character varying(1000)",
                maxLength: 1000,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true);

            migrationBuilder.AddUniqueConstraint(
                name: "AK_TestProjects_Name",
                table: "TestProjects",
                column: "Name");

            migrationBuilder.AddUniqueConstraint(
                name: "AK_TestPackages_Name_TestProjectId",
                table: "TestPackages",
                columns: new[] { "Name", "TestProjectId" });

            migrationBuilder.CreateIndex(
                name: "IX_TestPackages_TestProjectId",
                table: "TestPackages",
                column: "TestProjectId");

            migrationBuilder.AddForeignKey(
                name: "FK_TestPackages_TestProjects_TestProjectId",
                table: "TestPackages",
                column: "TestProjectId",
                principalTable: "TestProjects",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_TestPackages_TestProjects_TestProjectId",
                table: "TestPackages");

            migrationBuilder.DropUniqueConstraint(
                name: "AK_TestProjects_Name",
                table: "TestProjects");

            migrationBuilder.DropUniqueConstraint(
                name: "AK_TestPackages_Name_TestProjectId",
                table: "TestPackages");

            migrationBuilder.DropIndex(
                name: "IX_TestPackages_TestProjectId",
                table: "TestPackages");

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "TestProjects",
                type: "text",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(50)",
                oldMaxLength: 50);

            migrationBuilder.AlterColumn<string>(
                name: "Description",
                table: "TestProjects",
                type: "text",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "character varying(1000)",
                oldMaxLength: 1000,
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "TestPackageType",
                table: "TestPackages",
                type: "integer",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(50)",
                oldMaxLength: 50);

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "TestPackages",
                type: "text",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(100)",
                oldMaxLength: 100);

            migrationBuilder.AlterColumn<string>(
                name: "Description",
                table: "TestPackages",
                type: "text",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "character varying(1000)",
                oldMaxLength: 1000,
                oldNullable: true);
        }
    }
}
