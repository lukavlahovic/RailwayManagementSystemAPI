using RailwayManagementSystemAPI.Models;
using System.ComponentModel.DataAnnotations;

namespace RailwayManagementSystemAPI.Dtos
{
    public class CreateRouteDto
    {
        [Required]
        public string Name { get; set; } = string.Empty;

        [Required]
        public List<RouteStation> Stations { get; set; } = new List<RouteStation>();
    }
}
