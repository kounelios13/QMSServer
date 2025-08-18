using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace QMS.Migrations
{
    /// <inheritdoc />
    public partial class InitialMigration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterDatabase()
                .Annotation("MySQL:Charset", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "FrontDeskTerminals",
                columns: table => new
                {
                    DeviceId = table.Column<string>(type: "varchar(191)", maxLength: 191, nullable: false),
                    LastSeen = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    IPAddress = table.Column<string>(type: "varchar(45)", maxLength: 45, nullable: false),
                    DeviceName = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FrontDeskTerminals", x => x.DeviceId);
                })
                .Annotation("MySQL:Charset", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "Tickets",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "char(36)", nullable: false),
                    IssuedAt = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    IPAddress = table.Column<string>(type: "varchar(45)", maxLength: 45, nullable: true),
                    TicketNumber = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: false),
                    Status = table.Column<int>(type: "int", nullable: false, defaultValue: 0),
                    FrontDeskTerminalId = table.Column<string>(type: "varchar(191)", maxLength: 191, nullable: true),
                    FrontDeskTerminalDeviceId = table.Column<string>(type: "varchar(191)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Tickets", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Tickets_FrontDeskTerminals_FrontDeskTerminalDeviceId",
                        column: x => x.FrontDeskTerminalDeviceId,
                        principalTable: "FrontDeskTerminals",
                        principalColumn: "DeviceId");
                    table.ForeignKey(
                        name: "FK_Tickets_FrontDeskTerminals_FrontDeskTerminalId",
                        column: x => x.FrontDeskTerminalId,
                        principalTable: "FrontDeskTerminals",
                        principalColumn: "DeviceId");
                })
                .Annotation("MySQL:Charset", "utf8mb4");

            migrationBuilder.CreateIndex(
                name: "IX_Tickets_FrontDeskTerminalDeviceId",
                table: "Tickets",
                column: "FrontDeskTerminalDeviceId");

            migrationBuilder.CreateIndex(
                name: "IX_Tickets_FrontDeskTerminalId",
                table: "Tickets",
                column: "FrontDeskTerminalId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Tickets");

            migrationBuilder.DropTable(
                name: "FrontDeskTerminals");
        }
    }
}
