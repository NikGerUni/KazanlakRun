using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace KazanlakRun.Data.Migrations
{
    /// <inheritdoc />
    public partial class MakeAidStationNullable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Volunteers_AidStations_AidStationId",
                table: "Volunteers");

            migrationBuilder.AlterColumn<int>(
                name: "AidStationId",
                table: "Volunteers",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "11111111-2222-3333-4444-555555555555",
                columns: new[] { "ConcurrencyStamp", "PasswordHash", "SecurityStamp" },
                values: new object[] { "ad4ac903-10c2-4aeb-966d-b394f54a708e", "AQAAAAIAAYagAAAAEJ7/7Vz8wq4kcFkE3BZd8jncATuKfhSlcUmdmNIkerEOjP3Mpj0EZvw8qJXFFc5gcg==", "c7a1c02b-de57-4e03-be2a-78965a147221" });

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "7699db7d-964f-4782-8209-d76562e0fece",
                columns: new[] { "ConcurrencyStamp", "PasswordHash", "SecurityStamp" },
                values: new object[] { "d32f8a2b-caa3-4a01-9320-1e968c0777ad", "AQAAAAIAAYagAAAAEEFmBYYPL/qj2ByQcq93WzyzTPwy6cEoxZwZPYP7bRxHjwc8V06Q+BN5a9zK2PHNLA==", "e2a54845-bf69-4a73-852f-5c3d54e5bb3d" });

            migrationBuilder.AddForeignKey(
                name: "FK_Volunteers_AidStations_AidStationId",
                table: "Volunteers",
                column: "AidStationId",
                principalTable: "AidStations",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Volunteers_AidStations_AidStationId",
                table: "Volunteers");

            migrationBuilder.AlterColumn<int>(
                name: "AidStationId",
                table: "Volunteers",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "11111111-2222-3333-4444-555555555555",
                columns: new[] { "ConcurrencyStamp", "PasswordHash", "SecurityStamp" },
                values: new object[] { "19522947-6b14-4b0f-bb0d-d88d1b467adf", "AQAAAAIAAYagAAAAEPh/AAFREatNoabpvK1f3t0flXSeX0Y4YUfGxKFI2Z/kWKv+VZnALiUQkfQVqNCtlA==", "0a44cd3e-10e0-41ef-be26-d159d912ec78" });

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "7699db7d-964f-4782-8209-d76562e0fece",
                columns: new[] { "ConcurrencyStamp", "PasswordHash", "SecurityStamp" },
                values: new object[] { "236911aa-91dc-47af-abfb-7edf6d4bfed0", "AQAAAAIAAYagAAAAEK2vZ1CH/7D2QRMtKSz46ljxH2BNU0uxtiZmaU0FD3c2nWk7B7oniV+4mYcvVvxh8Q==", "fe7f7537-7e1c-466d-834e-b3fce34da4d9" });

            migrationBuilder.AddForeignKey(
                name: "FK_Volunteers_AidStations_AidStationId",
                table: "Volunteers",
                column: "AidStationId",
                principalTable: "AidStations",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
