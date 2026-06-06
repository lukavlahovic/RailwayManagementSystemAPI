using RailwayManagementSystemAPI.Models;

namespace RailwayManagementSystemAPI.Dtos
{
    public class CreateScheduleDto
    {
        public int TrainId { get; set; }

        public int RouteId { get; set; }

        public TimeSpan DepartureTime { get; set; }

        public ScheduleType ScheduleType { get; set; }

        public DateTime ValidFrom { get; set; }
        public DateTime? ValidTo { get; set; }
    }
}