using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace KazanlakRun.Data.Migrations
{
    /// <inheritdoc />
    public partial class Goods_allow_null : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<double>(
                name: "Quantity",
                table: "Goods",
                type: "float",
                nullable: true,
                oldClrType: typeof(double),
                oldType: "float");

            migrationBuilder.AlterColumn<string>(
                name: "Measure",
                table: "Goods",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "11111111-2222-3333-4444-555555555555",
                columns: new[] { "ConcurrencyStamp", "PasswordHash", "SecurityStamp" },
                values: new object[] { "da99f337-ef01-4395-b64b-176523c1a7f5", "AQAAAAIAAYagAAAAEIWtKhJ9PYk7JdHJydrB5LuRZAZmCCM66ea90fk7DCfkFGUz6D5SUGXBIVruzXtDPQ==", "9b2ae9c7-e753-4f9d-8e96-7044face8c27" });

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "7699db7d-964f-4782-8209-d76562e0fece",
                columns: new[] { "ConcurrencyStamp", "PasswordHash", "SecurityStamp" },
                values: new object[] { "ce991cce-f297-40c8-ac9d-c5c86e9224e8", "AQAAAAIAAYagAAAAELw8CHsPuFYvb3alc2/CAxxfMZttBT8wYnjkbh8YeaoRpzqpQOR3xu3xQt4IS4VE9A==", "bdfdd3ef-b9c5-4421-99d1-242ed84d5344" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<double>(
                name: "Quantity",
                table: "Goods",
                type: "float",
                nullable: false,
                defaultValue: 0.0,
                oldClrType: typeof(double),
                oldType: "float",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Measure",
                table: "Goods",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

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
        }
    }
}
