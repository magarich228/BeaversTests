using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BeaversTests.TestsManager.Infrastructure.DataAccess.Migrations
{
    /// <inheritdoc />
    public partial class ValidationMessageNameFix : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "ValidationMessage",
                table: "TestDrivers",
                type: "character varying(10000)",
                maxLength: 10000,
                nullable: true,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "character varying(1000)",
                oldMaxLength: 1000,
                oldNullable: true,
                oldDefaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "ValidationMessage",
                table: "TestDrivers",
                type: "character varying(1000)",
                maxLength: 1000,
                nullable: true,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "character varying(10000)",
                oldMaxLength: 10000,
                oldNullable: true,
                oldDefaultValue: "");
        }
    }
}
