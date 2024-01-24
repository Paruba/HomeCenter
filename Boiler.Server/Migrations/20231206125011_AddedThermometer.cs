using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Boiler.Server.Migrations
{
    /// <inheritdoc />
    public partial class AddedThermometer : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ThermometerId",
                schema: "public",
                table: "Temperature",
                type: "text",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "Thermometer",
                schema: "public",
                columns: table => new
                {
                    Id = table.Column<string>(type: "text", nullable: false),
                    Name = table.Column<string>(type: "text", nullable: false),
                    UserId = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Thermometer", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Thermometer_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalSchema: "public",
                        principalTable: "AspNetUsers",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_Temperature_ThermometerId",
                schema: "public",
                table: "Temperature",
                column: "ThermometerId");

            migrationBuilder.CreateIndex(
                name: "IX_Thermometer_UserId",
                schema: "public",
                table: "Thermometer",
                column: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_Temperature_Thermometer_ThermometerId",
                schema: "public",
                table: "Temperature",
                column: "ThermometerId",
                principalSchema: "public",
                principalTable: "Thermometer",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Temperature_Thermometer_ThermometerId",
                schema: "public",
                table: "Temperature");

            migrationBuilder.DropTable(
                name: "Thermometer",
                schema: "public");

            migrationBuilder.DropIndex(
                name: "IX_Temperature_ThermometerId",
                schema: "public",
                table: "Temperature");

            migrationBuilder.DropColumn(
                name: "ThermometerId",
                schema: "public",
                table: "Temperature");
        }
    }
}
