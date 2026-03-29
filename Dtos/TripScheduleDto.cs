namespace RailwayManagementSystemAPI.Dtos
{
    public class TripScheduleDto
    {
        public int TripId { get; set; }

        public string Train { get; set; } = string.Empty;

        public string Route { get; set; } = string.Empty;

        public DateTime DepartureTime { get; set; }
        public DateTime ArrivalTime { get; set; }
    }
}
