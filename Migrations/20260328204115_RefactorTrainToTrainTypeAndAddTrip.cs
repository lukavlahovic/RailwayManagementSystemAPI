using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RailwayManagementSystemAPI.Migrations
{
    /// <inheritdoc />
    public partial class RefactorTrainToTrainTypeAndAddTrip : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Capacity",
                table: "Trains");

            migrationBuilder.DropColumn(
                name: "MaxSpeed",
                table: "Trains");

            migrationBuilder.DropColumn(
                name: "Name",
                table: "Trains");

            migrationBuilder.RenameColumn(
                name: "Type",
                table: "Trains",
                newName: "TrainTypeId");

            migrationBuilder.AddColumn<string>(
                name: "SerialNumber",
                table: "Trains",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateTable(
                name: "TrainTypes",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    MaxSpeed = table.Column<int>(type: "int", nullable: false),
                    Capacity = table.Column<int>(type: "int", nullable: false),
                    Manufacturer = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Type = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TrainTypes", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Trip",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TrainId = table.Column<int>(type: "int", nullable: false),
                    RouteId = table.Column<int>(type: "int", nullable: false),
                    DepartureTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ArrivalTime = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Trip", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Trip_Routes_RouteId",
                        column: x => x.RouteId,
                        principalTable: "Routes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Trip_Trains_TrainId",
                        column: x => x.TrainId,
                        principalTable: "Trains",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Trains_TrainTypeId",
                table: "Trains",
                column: "TrainTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_Trip_RouteId",
                table: "Trip",
                column: "RouteId");

            migrationBuilder.CreateIndex(
                name: "IX_Trip_TrainId",
                table: "Trip",
                column: "TrainId");

            migrationBuilder.AddForeignKey(
                name: "FK_Trains_TrainTypes_TrainTypeId",
                table: "Trains",
                column: "TrainTypeId",
                principalTable: "TrainTypes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Trains_TrainTypes_TrainTypeId",
                table: "Trains");

            migrationBuilder.DropTable(
                name: "TrainTypes");

            migrationBuilder.DropTable(
                name: "Trip");

            migrationBuilder.DropIndex(
                name: "IX_Trains_TrainTypeId",
                table: "Trains");

            migrationBuilder.DropColumn(
                name: "SerialNumber",
                table: "Trains");

            migrationBuilder.RenameColumn(
                name: "TrainTypeId",
                table: "Trains",
                newName: "Type");

            migrationBuilder.AddColumn<int>(
                name: "Capacity",
                table: "Trains",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "MaxSpeed",
                table: "Trains",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "Name",
                table: "Trains",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }
    }
}
