namespace RailwayManagementSystemAPI.Models
{
    public enum ScheduleType
    {
        Daily,
        Workday,
        Weekend
    }

    public class Schedule
    {
        public int Id { get; set; }

        public Train Train { get; set;} = null!;
        public int TrainId { get; set; }

        public Route Route { get; set; } = null!;
        public int RouteId { get; set; }

        public TimeSpan DepartureTime { get; set; }

        public ScheduleType ScheduleType { get; set; }

        public DateTime ValidFrom { get; set; }
        public DateTime? ValidTo { get; set; }

        public bool IsActive { get; set; } = true;
    }
}