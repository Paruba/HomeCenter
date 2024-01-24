using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace Boiler.Server.Migrations
{
    /// <inheritdoc />
    public partial class DeletedCameraConfigTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CameraConfig",
                schema: "public");

            migrationBuilder.AddColumn<int>(
                name: "Period",
                schema: "public",
                table: "Camera",
                type: "integer",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Period",
                schema: "public",
                table: "Camera");

            migrationBuilder.CreateTable(
                name: "CameraConfig",
                schema: "public",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    CameraId = table.Column<int>(type: "integer", nullable: false),
                    Period = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CameraConfig", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CameraConfig_Camera_CameraId",
                        column: x => x.CameraId,
                        principalSchema: "public",
                        principalTable: "Camera",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_CameraConfig_CameraId",
                schema: "public",
                table: "CameraConfig",
                column: "CameraId");
        }
    }
}
