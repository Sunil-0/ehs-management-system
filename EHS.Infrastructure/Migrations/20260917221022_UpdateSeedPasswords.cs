using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EHS.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class UpdateSeedPasswords : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 1,
                column: "PasswordHash",
                value: "$2b$11$gBehB2FbUIFA2nZGbUrcqOjORV28Re5pA290nViD.baurAEtDAf1W");

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 2,
                column: "PasswordHash",
                value: "$2b$11$gBehB2FbUIFA2nZGbUrcqOjORV28Re5pA290nViD.baurAEtDAf1W");

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 3,
                column: "PasswordHash",
                value: "$2b$11$gBehB2FbUIFA2nZGbUrcqOjORV28Re5pA290nViD.baurAEtDAf1W");

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 4,
                column: "PasswordHash",
                value: "$2b$11$gBehB2FbUIFA2nZGbUrcqOjORV28Re5pA290nViD.baurAEtDAf1W");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 1,
                column: "PasswordHash",
                value: "temp");

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 2,
                column: "PasswordHash",
                value: "temp");

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 3,
                column: "PasswordHash",
                value: "temp");

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 4,
                column: "PasswordHash",
                value: "temp");
        }
    }
}
