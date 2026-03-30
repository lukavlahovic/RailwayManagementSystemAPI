using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RailwayManagementSystemAPI.Migrations
{
    /// <inheritdoc />
    public partial class AddTimeOffsetToRouteStation : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "ArrivalOffsetMinutes",
                table: "RouteStations",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "DepartureOffsetMinutes",
                table: "RouteStations",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ArrivalOffsetMinutes",
                table: "RouteStations");

            migrationBuilder.DropColumn(
                name: "DepartureOffsetMinutes",
                table: "RouteStations");
        }
    }
}
