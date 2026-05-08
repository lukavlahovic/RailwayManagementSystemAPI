using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RailwayManagementSystemAPI.Migrations
{
    /// <inheritdoc />
    public partial class AddActualArrivalAndCoordinates : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "ActualArrivalTime",
                table: "Trip",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<double>(
                name: "Latitude",
                table: "Stations",
                type: "float",
                nullable: true);

            migrationBuilder.AddColumn<double>(
                name: "Longitude",
                table: "Stations",
                type: "float",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ActualArrivalTime",
                table: "Trip");

            migrationBuilder.DropColumn(
                name: "Latitude",
                table: "Stations");

            migrationBuilder.DropColumn(
                name: "Longitude",
                table: "Stations");
        }
    }
}
