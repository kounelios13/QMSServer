using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace QMS.Migrations
{
    /// <inheritdoc />
    public partial class RelationshipTokenDesktop : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "FrontDeskTerminalDeviceId",
                table: "Tickets",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "FrontDeskTerminalId",
                table: "Tickets",
                type: "TEXT",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Tickets_FrontDeskTerminalDeviceId",
                table: "Tickets",
                column: "FrontDeskTerminalDeviceId");

            migrationBuilder.CreateIndex(
                name: "IX_Tickets_FrontDeskTerminalId",
                table: "Tickets",
                column: "FrontDeskTerminalId");

            migrationBuilder.AddForeignKey(
                name: "FK_Tickets_FrontDeskTerminals_FrontDeskTerminalDeviceId",
                table: "Tickets",
                column: "FrontDeskTerminalDeviceId",
                principalTable: "FrontDeskTerminals",
                principalColumn: "DeviceId");

            migrationBuilder.AddForeignKey(
                name: "FK_Tickets_FrontDeskTerminals_FrontDeskTerminalId",
                table: "Tickets",
                column: "FrontDeskTerminalId",
                principalTable: "FrontDeskTerminals",
                principalColumn: "DeviceId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Tickets_FrontDeskTerminals_FrontDeskTerminalDeviceId",
                table: "Tickets");

            migrationBuilder.DropForeignKey(
                name: "FK_Tickets_FrontDeskTerminals_FrontDeskTerminalId",
                table: "Tickets");

            migrationBuilder.DropIndex(
                name: "IX_Tickets_FrontDeskTerminalDeviceId",
                table: "Tickets");

            migrationBuilder.DropIndex(
                name: "IX_Tickets_FrontDeskTerminalId",
                table: "Tickets");

            migrationBuilder.DropColumn(
                name: "FrontDeskTerminalDeviceId",
                table: "Tickets");

            migrationBuilder.DropColumn(
                name: "FrontDeskTerminalId",
                table: "Tickets");
        }
    }
}
