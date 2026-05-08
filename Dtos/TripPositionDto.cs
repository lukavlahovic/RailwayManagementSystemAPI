namespace RailwayManagementSystemAPI.Dtos
{
    public enum TripStatus
    {
        NotDeparted,
        AtStation,
        InTransit,
        WaitingForCompletion,
        Completed
    }

    public class TripPositionDto
    {
        public int TripId { get; set; }
        public string Train { get; set; } = string.Empty;
        public string Route { get; set; } = string.Empty;
        public TripStatus Status { get; set; }

        // InTransit / AtStation fields
        public string? LastStation { get; set; }
        public string? NextStation { get; set; }
        public double? MinutesToNextStation { get; set; }
        public double ProgressPercent { get; set; }
        public int TotalDelayMinutes { get; set; }
        public DateTime? EstimatedFinalArrival { get; set; }

        // Completed fields
        public DateTime? PlannedArrival { get; set; }
        public DateTime? ActualArrival { get; set; }
    }
}
