using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RailwayManagementSystemAPI.Migrations
{
    /// <inheritdoc />
    public partial class AlterRouteStation : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "DepartureOffsetMinutes",
                table: "RouteStations",
                newName: "StopDuration");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "StopDuration",
                table: "RouteStations",
                newName: "DepartureOffsetMinutes");
        }
    }
}
