namespace RailwayManagementSystemAPI.Dtos
{
    public class StationScheduleDto
    {
        public string Train { get; set; } = string.Empty;

        public string Route { get; set; } = string.Empty;

        public DateTime ArrivalTime { get; set; }

        public double MinutesUntilArrival { get; set; }
    }
}
