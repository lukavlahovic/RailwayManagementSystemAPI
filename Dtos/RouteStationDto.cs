using System.ComponentModel.DataAnnotations;

namespace RailwayManagementSystemAPI.Dtos
{
    public class RouteStationDto
    {
        [Required]
        public int StationId { get; set; }

        [Range(1,int.MaxValue)]
        public int Order { get; set; }

        [Range(0, int.MaxValue)]
        public int ArrivalOffsetMinutes { get; set; }

        [Range(0, int.MaxValue)]
        public int StopDuration { get; set; }
    }
}
