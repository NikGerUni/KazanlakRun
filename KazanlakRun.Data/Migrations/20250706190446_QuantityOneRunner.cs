using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace KazanlakRun.Data.Migrations
{
    /// <inheritdoc />
    public partial class QuantityOneRunner : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Goods_AidStations_AidStationId",
                table: "Goods");

            migrationBuilder.AlterColumn<double>(
                name: "Quantity",
                table: "Goods",
                type: "float",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<int>(
                name: "AidStationId",
                table: "Goods",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddColumn<double>(
                name: "QuantityOneRunner",
                table: "Goods",
                type: "float",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "11111111-2222-3333-4444-555555555555",
                columns: new[] { "ConcurrencyStamp", "PasswordHash", "SecurityStamp" },
                values: new object[] { "6d47a312-e477-4075-a152-07f66ac9064d", "AQAAAAIAAYagAAAAEA71gdLe0AjPqCtMC49JokSptrhdBMoiyZ8cZz+QukJSkJSBwyg4q8zyBreCMkGK4A==", "dc7509e0-1a4f-4e05-8949-3eed6bb8cbac" });

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "7699db7d-964f-4782-8209-d76562e0fece",
                columns: new[] { "ConcurrencyStamp", "PasswordHash", "SecurityStamp" },
                values: new object[] { "3c425abd-d8bb-48be-8553-526c613c275a", "AQAAAAIAAYagAAAAEAb0yoQX8z1uUzmu+PDsdvFzpMoSU/+6XBpxeJOqJMVzHR5mM9LD2hiBSolSMSbYPg==", "ec30afce-53ea-4eb6-bf86-d3ee6839e3b9" });

            migrationBuilder.UpdateData(
                table: "Goods",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "AidStationId", "Quantity", "QuantityOneRunner" },
                values: new object[] { null, 10.0, 0.0 });

            migrationBuilder.UpdateData(
                table: "Goods",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "AidStationId", "Quantity", "QuantityOneRunner" },
                values: new object[] { null, 20.0, 0.0 });

            migrationBuilder.UpdateData(
                table: "Goods",
                keyColumn: "Id",
                keyValue: 3,
                columns: new[] { "AidStationId", "Quantity", "QuantityOneRunner" },
                values: new object[] { null, 7.0, 0.0 });

            migrationBuilder.UpdateData(
                table: "Goods",
                keyColumn: "Id",
                keyValue: 4,
                columns: new[] { "AidStationId", "Quantity", "QuantityOneRunner" },
                values: new object[] { null, 40.0, 0.0 });

            migrationBuilder.UpdateData(
                table: "Goods",
                keyColumn: "Id",
                keyValue: 5,
                columns: new[] { "AidStationId", "Quantity", "QuantityOneRunner" },
                values: new object[] { null, 200.0, 1.0 });

            migrationBuilder.UpdateData(
                table: "Goods",
                keyColumn: "Id",
                keyValue: 6,
                columns: new[] { "AidStationId", "Quantity", "QuantityOneRunner" },
                values: new object[] { null, 1.0, 0.0030000000000000001 });

            migrationBuilder.UpdateData(
                table: "Goods",
                keyColumn: "Id",
                keyValue: 7,
                columns: new[] { "AidStationId", "Quantity", "QuantityOneRunner" },
                values: new object[] { null, 100.0, 0.5 });

            migrationBuilder.UpdateData(
                table: "Goods",
                keyColumn: "Id",
                keyValue: 8,
                columns: new[] { "AidStationId", "Quantity", "QuantityOneRunner" },
                values: new object[] { null, 50.0, 0.20000000000000001 });

            migrationBuilder.UpdateData(
                table: "Goods",
                keyColumn: "Id",
                keyValue: 9,
                columns: new[] { "AidStationId", "Quantity", "QuantityOneRunner" },
                values: new object[] { null, 100.0, 0.5 });

            migrationBuilder.UpdateData(
                table: "Goods",
                keyColumn: "Id",
                keyValue: 10,
                columns: new[] { "AidStationId", "Quantity", "QuantityOneRunner" },
                values: new object[] { null, 100.0, 0.5 });

            migrationBuilder.UpdateData(
                table: "Goods",
                keyColumn: "Id",
                keyValue: 11,
                columns: new[] { "AidStationId", "Quantity", "QuantityOneRunner" },
                values: new object[] { null, 100.0, 0.5 });

            migrationBuilder.UpdateData(
                table: "Goods",
                keyColumn: "Id",
                keyValue: 12,
                columns: new[] { "AidStationId", "Quantity", "QuantityOneRunner" },
                values: new object[] { null, 50.0, 0.01 });

            migrationBuilder.UpdateData(
                table: "Goods",
                keyColumn: "Id",
                keyValue: 13,
                columns: new[] { "AidStationId", "Quantity", "QuantityOneRunner" },
                values: new object[] { null, 10.0, 0.0 });

            migrationBuilder.AddForeignKey(
                name: "FK_Goods_AidStations_AidStationId",
                table: "Goods",
                column: "AidStationId",
                principalTable: "AidStations",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Goods_AidStations_AidStationId",
                table: "Goods");

            migrationBuilder.DropColumn(
                name: "QuantityOneRunner",
                table: "Goods");

            migrationBuilder.AlterColumn<int>(
                name: "Quantity",
                table: "Goods",
                type: "int",
                nullable: false,
                oldClrType: typeof(double),
                oldType: "float");

            migrationBuilder.AlterColumn<int>(
                name: "AidStationId",
                table: "Goods",
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
                values: new object[] { "ad4ac903-10c2-4aeb-966d-b394f54a708e", "AQAAAAIAAYagAAAAEJ7/7Vz8wq4kcFkE3BZd8jncATuKfhSlcUmdmNIkerEOjP3Mpj0EZvw8qJXFFc5gcg==", "c7a1c02b-de57-4e03-be2a-78965a147221" });

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "7699db7d-964f-4782-8209-d76562e0fece",
                columns: new[] { "ConcurrencyStamp", "PasswordHash", "SecurityStamp" },
                values: new object[] { "d32f8a2b-caa3-4a01-9320-1e968c0777ad", "AQAAAAIAAYagAAAAEEFmBYYPL/qj2ByQcq93WzyzTPwy6cEoxZwZPYP7bRxHjwc8V06Q+BN5a9zK2PHNLA==", "e2a54845-bf69-4a73-852f-5c3d54e5bb3d" });

            migrationBuilder.UpdateData(
                table: "Goods",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "AidStationId", "Quantity" },
                values: new object[] { 1, 10 });

            migrationBuilder.UpdateData(
                table: "Goods",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "AidStationId", "Quantity" },
                values: new object[] { 1, 20 });

            migrationBuilder.UpdateData(
                table: "Goods",
                keyColumn: "Id",
                keyValue: 3,
                columns: new[] { "AidStationId", "Quantity" },
                values: new object[] { 1, 7 });

            migrationBuilder.UpdateData(
                table: "Goods",
                keyColumn: "Id",
                keyValue: 4,
                columns: new[] { "AidStationId", "Quantity" },
                values: new object[] { 2, 40 });

            migrationBuilder.UpdateData(
                table: "Goods",
                keyColumn: "Id",
                keyValue: 5,
                columns: new[] { "AidStationId", "Quantity" },
                values: new object[] { 2, 200 });

            migrationBuilder.UpdateData(
                table: "Goods",
                keyColumn: "Id",
                keyValue: 6,
                columns: new[] { "AidStationId", "Quantity" },
                values: new object[] { 2, 1 });

            migrationBuilder.UpdateData(
                table: "Goods",
                keyColumn: "Id",
                keyValue: 7,
                columns: new[] { "AidStationId", "Quantity" },
                values: new object[] { 3, 100 });

            migrationBuilder.UpdateData(
                table: "Goods",
                keyColumn: "Id",
                keyValue: 8,
                columns: new[] { "AidStationId", "Quantity" },
                values: new object[] { 3, 50 });

            migrationBuilder.UpdateData(
                table: "Goods",
                keyColumn: "Id",
                keyValue: 9,
                columns: new[] { "AidStationId", "Quantity" },
                values: new object[] { 3, 100 });

            migrationBuilder.UpdateData(
                table: "Goods",
                keyColumn: "Id",
                keyValue: 10,
                columns: new[] { "AidStationId", "Quantity" },
                values: new object[] { 4, 100 });

            migrationBuilder.UpdateData(
                table: "Goods",
                keyColumn: "Id",
                keyValue: 11,
                columns: new[] { "AidStationId", "Quantity" },
                values: new object[] { 4, 100 });

            migrationBuilder.UpdateData(
                table: "Goods",
                keyColumn: "Id",
                keyValue: 12,
                columns: new[] { "AidStationId", "Quantity" },
                values: new object[] { 4, 50 });

            migrationBuilder.UpdateData(
                table: "Goods",
                keyColumn: "Id",
                keyValue: 13,
                columns: new[] { "AidStationId", "Quantity" },
                values: new object[] { 5, 10 });

            migrationBuilder.AddForeignKey(
                name: "FK_Goods_AidStations_AidStationId",
                table: "Goods",
                column: "AidStationId",
                principalTable: "AidStations",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
