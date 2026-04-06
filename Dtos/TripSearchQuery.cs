namespace RailwayManagementSystemAPI.Dtos
{
    public class TripSearchQuery
    {
        public int? FromStationId { get; set; }
        public int? ToStationId { get; set; }

        public DateTime? Date { get; set; }

        public TimeSpan? MinDepartureTime { get; set; }
        public TimeSpan? MaxDepartureTime { get; set; }

        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = 10;
    }
}
