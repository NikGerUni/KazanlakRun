using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace KazanlakRun.Data.Migrations
{
    /// <inheritdoc />
    public partial class Initial : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AidStations",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ShortName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AidStations", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Distances",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Distans = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    RegRunners = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Distances", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Roles",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Roles", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Goods",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Measure = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Quantity = table.Column<int>(type: "int", nullable: false),
                    AidStationId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Goods", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Goods_AidStations_AidStationId",
                        column: x => x.AidStationId,
                        principalTable: "AidStations",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Volunteers",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Names = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Email = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Phone = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    AidStationId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Volunteers", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Volunteers_AidStations_AidStationId",
                        column: x => x.AidStationId,
                        principalTable: "AidStations",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AidStationDistances",
                columns: table => new
                {
                    AidStationId = table.Column<int>(type: "int", nullable: false),
                    DistanceId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AidStationDistances", x => new { x.AidStationId, x.DistanceId });
                    table.ForeignKey(
                        name: "FK_AidStationDistances_AidStations_AidStationId",
                        column: x => x.AidStationId,
                        principalTable: "AidStations",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_AidStationDistances_Distances_DistanceId",
                        column: x => x.DistanceId,
                        principalTable: "Distances",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "VolunteerRoles",
                columns: table => new
                {
                    VolunteerId = table.Column<int>(type: "int", nullable: false),
                    RoleId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_VolunteerRoles", x => new { x.VolunteerId, x.RoleId });
                    table.ForeignKey(
                        name: "FK_VolunteerRoles_Roles_RoleId",
                        column: x => x.RoleId,
                        principalTable: "Roles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_VolunteerRoles_Volunteers_VolunteerId",
                        column: x => x.VolunteerId,
                        principalTable: "Volunteers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "AidStations",
                columns: new[] { "Id", "Name", "ShortName" },
                values: new object[,]
                {
                    { 1, "Aid stationn 1", "A1" },
                    { 2, "Aid stationn 2", "A2" },
                    { 3, "Aid stationn 3", "A3" },
                    { 4, "Aid stationn 4", "A4" },
                    { 5, "Aid stationn 5", "A5" }
                });

            migrationBuilder.InsertData(
                table: "Roles",
                columns: new[] { "Id", "Name" },
                values: new object[,]
                {
                    { 1, "doctor" },
                    { 2, "security" },
                    { 3, "radio" },
                    { 4, "food preparation" },
                    { 5, "time recorder" }
                });

            migrationBuilder.InsertData(
                table: "Goods",
                columns: new[] { "Id", "AidStationId", "Measure", "Name", "Quantity" },
                values: new object[,]
                {
                    { 1, 1, "pcs", "Table", 10 },
                    { 2, 1, "pcs", "Chair", 20 },
                    { 3, 1, "pcs", "Tent", 7 },
                    { 4, 2, "pcs", "Plates", 40 },
                    { 5, 2, "pcs", "Cups", 200 },
                    { 6, 2, "kg", "Salt", 1 },
                    { 7, 3, "l", "Drinking water", 100 },
                    { 8, 3, "l", "Isotonic", 50 },
                    { 9, 3, "pcs", "Lemons", 100 },
                    { 10, 4, "kg", "Apples", 100 },
                    { 11, 4, "pcs", "Bananas", 100 },
                    { 12, 4, "pcs", "Chocolate", 50 },
                    { 13, 5, "bottles", "Soap", 10 }
                });

            migrationBuilder.InsertData(
                table: "Volunteers",
                columns: new[] { "Id", "AidStationId", "Email", "Names", "Phone" },
                values: new object[,]
                {
                    { 1, 1, "nnnnnn@nnn.bg", "Nikola Nikolov", "0888998899" },
                    { 2, 2, "iiiiii@.iii.bg", "Ivan Ivanov", "0888999999" },
                    { 3, 3, "gggggg@ggg.bg", "Georg Georgiev", "0888777777" },
                    { 4, 1, "petarp@p.bg", "Petar Petrov", "0888111111" },
                    { 5, 2, "maria@i.bg", "Maria Ivanova", "0888222222" },
                    { 6, 3, "stoyand@d.bg", "Stoyan Dimitrov", "0888333333" },
                    { 7, 4, "elena@p.bg", "Elena Petrova", "0888444444" },
                    { 8, 5, "vlad@st.bg", "Vladimir Stoyanov", "0888555555" },
                    { 9, 1, "tsveta@k.bg", "Tsveta Koleva", "0888666666" },
                    { 10, 2, "nikolay@m.bg", "Nikolay Marinov", "0888778778" }
                });

            migrationBuilder.CreateIndex(
                name: "IX_AidStationDistances_DistanceId",
                table: "AidStationDistances",
                column: "DistanceId");

            migrationBuilder.CreateIndex(
                name: "IX_Goods_AidStationId",
                table: "Goods",
                column: "AidStationId");

            migrationBuilder.CreateIndex(
                name: "IX_VolunteerRoles_RoleId",
                table: "VolunteerRoles",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "IX_Volunteers_AidStationId",
                table: "Volunteers",
                column: "AidStationId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AidStationDistances");

            migrationBuilder.DropTable(
                name: "Goods");

            migrationBuilder.DropTable(
                name: "VolunteerRoles");

            migrationBuilder.DropTable(
                name: "Distances");

            migrationBuilder.DropTable(
                name: "Roles");

            migrationBuilder.DropTable(
                name: "Volunteers");

            migrationBuilder.DropTable(
                name: "AidStations");
        }
    }
}
