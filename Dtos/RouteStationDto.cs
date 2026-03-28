using System.ComponentModel.DataAnnotations;

namespace RailwayManagementSystemAPI.Dtos
{
    public class RouteStationDto
    {
        [Required]
        public int StationId { get; set; }

        [Range(1,int.MaxValue)]
        public int Order { get; set; }
    }
}
