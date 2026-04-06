namespace RailwayManagementSystemAPI.Dtos
{
    public class TripSearchResponseDto
    {
        public int TripId { get; set; }

        public string Train { get; set; } = string.Empty;

        public string Route { get; set; } = string.Empty;

        public DateTime? DepartureFromStationTime { get; set; }
        public DateTime? ArrivalToStation { get; set; }

        public double? DurationMinutes { get; set; }
    }
}
