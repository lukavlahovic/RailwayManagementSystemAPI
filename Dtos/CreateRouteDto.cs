using RailwayManagementSystemAPI.Models;
using System.ComponentModel.DataAnnotations;

namespace RailwayManagementSystemAPI.Dtos
{
    public class CreateRouteDto
    {
        public string Name { get; set; } = string.Empty;

        public List<RouteStationDto> Stations { get; set; } = [];
    }
}
