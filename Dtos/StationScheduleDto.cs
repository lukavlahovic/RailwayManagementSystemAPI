namespace RailwayManagementSystemAPI.Dtos
{
    public class StationScheduleDto
    {
        public string Train { get; set; } = string.Empty;

        public string Route { get; set; } = string.Empty;

        public DateTime RealArrivalTime { get; set; }
        public DateTime PlannedArrivalTime { get; set; }

        public int TotalDelayMinutes { get; set; }

        public double MinutesUntilArrival { get; set; }
    }
}
