using RailwayManagementSystemAPI.Models;

namespace RailwayManagementSystemAPI.Dtos
{
    public class ScheduleResponseDto
    {
        public int Id { get; set; }
        public string Train { get; set; } = string.Empty;
        public string Route { get; set; } = string.Empty;
        public TimeSpan DepartureTime { get; set; }
        public ScheduleType ScheduleType { get; set; }
        public DateTime ValidFrom { get; set; }
        public DateTime? ValidTo { get; set; }
        public bool IsActive { get; set; }
    }
}