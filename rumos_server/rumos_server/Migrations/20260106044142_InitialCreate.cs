using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace rumos_server.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "modes",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_modes", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "platforms",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_platforms", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "presets",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(15)", maxLength: 15, nullable: false),
                    Img_url = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false, defaultValue: "")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_presets", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "rooms",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_rooms", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "devices",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    Ip_v4 = table.Column<string>(type: "nvarchar(15)", maxLength: 15, nullable: true),
                    platform_id = table.Column<int>(type: "int", nullable: false),
                    room_id = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_devices", x => x.Id);
                    table.ForeignKey(
                        name: "FK_devices_platforms_platform_id",
                        column: x => x.platform_id,
                        principalTable: "platforms",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_devices_rooms_room_id",
                        column: x => x.room_id,
                        principalTable: "rooms",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "preset_device_maps",
                columns: table => new
                {
                    preset_id = table.Column<int>(type: "int", nullable: false),
                    device_id = table.Column<int>(type: "int", nullable: false),
                    R = table.Column<int>(type: "int", nullable: false),
                    G = table.Column<int>(type: "int", nullable: false),
                    B = table.Column<int>(type: "int", nullable: false),
                    Brightness = table.Column<int>(type: "int", nullable: false),
                    mode_id = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_preset_device_maps", x => new { x.preset_id, x.device_id });
                    table.CheckConstraint("CK_Preset_device_map_B", "[B] >= 0 AND [B] <= 255");
                    table.CheckConstraint("CK_Preset_device_map_Brightness", "[Brightness] >= 0 AND [Brightness] <= 255");
                    table.CheckConstraint("CK_Preset_device_map_G", "[G] >= 0 AND [G] <= 255");
                    table.CheckConstraint("CK_Preset_device_map_R", "[R] >= 0 AND [R] <= 255");
                    table.ForeignKey(
                        name: "FK_preset_device_maps_devices_device_id",
                        column: x => x.device_id,
                        principalTable: "devices",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_preset_device_maps_modes_mode_id",
                        column: x => x.mode_id,
                        principalTable: "modes",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_preset_device_maps_presets_preset_id",
                        column: x => x.preset_id,
                        principalTable: "presets",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "modes",
                columns: new[] { "Id", "Name" },
                values: new object[,]
                {
                    { 1, "Normal" },
                    { 2, "Blink" },
                    { 3, "Fade" }
                });

            migrationBuilder.InsertData(
                table: "platforms",
                columns: new[] { "Id", "Description", "Name" },
                values: new object[,]
                {
                    { 1, "MQTT Protocol Devices", "MQTT" },
                    { 2, "gRPC Protocol Devices", "gRPC" }
                });

            migrationBuilder.InsertData(
                table: "rooms",
                columns: new[] { "Id", "Description", "Name" },
                values: new object[,]
                {
                    { 1, "Main living area", "Living Room" },
                    { 2, "Master bedroom", "Bedroom" },
                    { 3, "Kitchen area", "Kitchen" }
                });

            migrationBuilder.CreateIndex(
                name: "IX_devices_platform_id",
                table: "devices",
                column: "platform_id");

            migrationBuilder.CreateIndex(
                name: "IX_devices_room_id",
                table: "devices",
                column: "room_id");

            migrationBuilder.CreateIndex(
                name: "IX_preset_device_maps_device_id",
                table: "preset_device_maps",
                column: "device_id");

            migrationBuilder.CreateIndex(
                name: "IX_preset_device_maps_mode_id",
                table: "preset_device_maps",
                column: "mode_id");

            migrationBuilder.CreateIndex(
                name: "IX_preset_device_maps_preset_id_device_id",
                table: "preset_device_maps",
                columns: new[] { "preset_id", "device_id" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "preset_device_maps");

            migrationBuilder.DropTable(
                name: "devices");

            migrationBuilder.DropTable(
                name: "modes");

            migrationBuilder.DropTable(
                name: "presets");

            migrationBuilder.DropTable(
                name: "platforms");

            migrationBuilder.DropTable(
                name: "rooms");
        }
    }
}
