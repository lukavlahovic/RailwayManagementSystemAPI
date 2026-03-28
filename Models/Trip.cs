namespace RailwayManagementSystemAPI.Models
{
    public class Trip
    {
        public int Id { get; set; }

        public int TrainId { get; set; }
        public Train Train { get; set; } = null!;

        public int RouteId { get; set; }
        public Route Route { get; set; } = null!;

        public DateTime DepartureTime { get; set; }
        public DateTime ArrivalTime { get; set; }
    }
}
