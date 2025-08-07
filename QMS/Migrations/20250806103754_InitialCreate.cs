using System;
using Microsoft.EntityFrameworkCore.Migrations;
using QMS.Db;
#nullable disable

namespace QMS.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "FrontDeskTerminals",
                columns: table => new
                {
                    DeviceId = table.Column<string>(type: "TEXT", nullable: false),
                    LastSeen = table.Column<DateTime>(type: "TEXT", nullable: false),
                    IPAddress = table.Column<string>(type: "TEXT", maxLength: 45, nullable: false),
                    DeviceName = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FrontDeskTerminals", x => x.DeviceId);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "FrontDeskTerminals");
        }
    }
}
