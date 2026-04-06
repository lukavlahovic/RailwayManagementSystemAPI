namespace RailwayManagementSystemAPI.Dtos
{
    public class TripResponseDto
    {
        public int Id { get; set; }
        public string SerialNumber { get; set; } = string.Empty;
        public string TrainTypeName { get; set; } = string.Empty;

        public string RouteName { get; set; } = string.Empty;

        public DateTime DepartureTime { get; set; }
        public DateTime ArrivalTime { get; set; }
    }
}
