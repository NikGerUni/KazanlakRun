using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.EntityFrameworkCore.Migrations;
using System;

#nullable disable

namespace KazanlakRun.Data.Migrations
{
    /// <inheritdoc />
    public partial class RemoveAidStationIdFromGood : Migration
    {
        /// <inheritdoc />

            protected override void Up(MigrationBuilder migrationBuilder)
            {
                // 1) Премахваме FK към AidStations
                migrationBuilder.DropForeignKey(
                    name: "FK_Goods_AidStations_AidStationId",
                    table: "Goods");

                // 2) Премахваме индекса, който EF вече е създал по AidStationId
                migrationBuilder.DropIndex(
                    name: "IX_Goods_AidStationId",
                    table: "Goods");

                // 3) Сега можем да махнем колоната
                migrationBuilder.DropColumn(
                    name: "AidStationId",
                    table: "Goods");

                // --- вашите UpdateData(...) вече тук ---
                migrationBuilder.UpdateData(
                    table: "AspNetUsers",
                    keyColumn: "Id",
                    keyValue: "11111111-2222-3333-4444-555555555555",
                    columns: new[] { "ConcurrencyStamp", "PasswordHash", "SecurityStamp" },
                    values: new object[] { "b9028c0b-f69a-42a3-b354-dc25bc372527", "AQAAAAIAAYagAAAAEC9SKbcZix67XP9tyh8AVXQ8jHQc0XcxKC4QogAoD1w9W0tU0bRdPcwQd4W0g923cQ==", "3ab34984-e431-43ba-8b44-986e65b153df" });

                migrationBuilder.UpdateData(
                    table: "AspNetUsers",
                    keyColumn: "Id",
                    keyValue: "7699db7d-964f-4782-8209-d76562e0fece",
                    columns: new[] { "ConcurrencyStamp", "PasswordHash", "SecurityStamp" },
                    values: new object[] { "77d16a54-d206-44db-8680-1676ca4b9d9a", "AQAAAAIAAYagAAAAEN5B6Rtm6mGLW6Daxfxsqaa5xnfmWeidFX/pdkle95hjYH2raWDM//5V2rCWMUG9gQ==", "5ddefc55-d355-4ce6-a468-f7ea6760e8f9" });
            }

            protected override void Down(MigrationBuilder migrationBuilder)
            {
                // 1) Добавяме колоната обратно
                migrationBuilder.AddColumn<int>(
                    name: "AidStationId",
                    table: "Goods",
                    type: "int",
                    nullable: true);

                // 2) Връщаме индекса
                migrationBuilder.CreateIndex(
                    name: "IX_Goods_AidStationId",
                    table: "Goods",
                    column: "AidStationId");

                // 3) Връщаме FK-а
                migrationBuilder.AddForeignKey(
                    name: "FK_Goods_AidStations_AidStationId",
                    table: "Goods",
                    column: "AidStationId",
                    principalTable: "AidStations",
                    principalColumn: "Id");

                // --- вашите UpdateData(...) за Down тук ---
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
        }

    }

