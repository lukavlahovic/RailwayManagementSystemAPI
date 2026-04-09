using System.ComponentModel.DataAnnotations;

namespace RailwayManagementSystemAPI.Dtos
{
    public class RouteStationDto
    {
        public int StationId { get; set; }

        public int Order { get; set; }

        public int ArrivalOffsetMinutes { get; set; }

        public int StopDuration { get; set; }
    }
}
