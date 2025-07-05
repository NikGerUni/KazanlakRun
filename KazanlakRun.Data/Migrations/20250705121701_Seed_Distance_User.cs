using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace KazanlakRun.Data.Migrations
{
    /// <inheritdoc />
    public partial class Seed_Distance_User : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "7699db7d-964f-4782-8209-d76562e0fece",
                columns: new[] { "ConcurrencyStamp", "PasswordHash", "SecurityStamp" },
                values: new object[] { "236911aa-91dc-47af-abfb-7edf6d4bfed0", "AQAAAAIAAYagAAAAEK2vZ1CH/7D2QRMtKSz46ljxH2BNU0uxtiZmaU0FD3c2nWk7B7oniV+4mYcvVvxh8Q==", "fe7f7537-7e1c-466d-834e-b3fce34da4d9" });

            migrationBuilder.InsertData(
                table: "AspNetUsers",
                columns: new[] { "Id", "AccessFailedCount", "ConcurrencyStamp", "Email", "EmailConfirmed", "LockoutEnabled", "LockoutEnd", "NormalizedEmail", "NormalizedUserName", "PasswordHash", "PhoneNumber", "PhoneNumberConfirmed", "SecurityStamp", "TwoFactorEnabled", "UserName" },
                values: new object[] { "11111111-2222-3333-4444-555555555555", 0, "19522947-6b14-4b0f-bb0d-d88d1b467adf", "user@abv.bg", true, false, null, "USER@ABV.BG", "USER@ABV.BG", "AQAAAAIAAYagAAAAEPh/AAFREatNoabpvK1f3t0flXSeX0Y4YUfGxKFI2Z/kWKv+VZnALiUQkfQVqNCtlA==", null, false, "0a44cd3e-10e0-41ef-be26-d159d912ec78", false, "user@abv.bg" });

            migrationBuilder.InsertData(
                table: "Distances",
                columns: new[] { "Id", "Distans", "RegRunners" },
                values: new object[,]
                {
                    { 1, "10 km", 100 },
                    { 2, "20 km", 80 },
                    { 3, "40 km", 60 }
                });

            migrationBuilder.InsertData(
                table: "AspNetUserRoles",
                columns: new[] { "RoleId", "UserId" },
                values: new object[] { "f1e2d3c4-b5a6-4948-9309-c56782b0faab", "11111111-2222-3333-4444-555555555555" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AspNetUserRoles",
                keyColumns: new[] { "RoleId", "UserId" },
                keyValues: new object[] { "f1e2d3c4-b5a6-4948-9309-c56782b0faab", "11111111-2222-3333-4444-555555555555" });

            migrationBuilder.DeleteData(
                table: "Distances",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Distances",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "Distances",
                keyColumn: "Id",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "11111111-2222-3333-4444-555555555555");

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "7699db7d-964f-4782-8209-d76562e0fece",
                columns: new[] { "ConcurrencyStamp", "PasswordHash", "SecurityStamp" },
                values: new object[] { "9669f490-4089-4f7f-8b74-2d2d15e23c23", "AQAAAAIAAYagAAAAEHrTreRyJpsZKTJwg+x4lgQFNyJQbAjpreP4Q4UZe0cpceMtje4OJxajkRAy1ZzbsA==", "62afd5fb-7539-4d65-9fdb-3cb5813ca079" });
        }
    }
}
