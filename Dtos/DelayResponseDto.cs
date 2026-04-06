using RailwayManagementSystemAPI.Models;

namespace RailwayManagementSystemAPI.Dtos
{
    public class DelayResponseDto
    {
        public int Id { get; set; }
        public int TripId { get; set; }
        public string StationName { get; set; } = string.Empty;
        public int DelayMinutes { get; set; }
        public TypeOfDelay TypeOfDelay { get; set; }
        public DateTime CreatedAt { get; set; }
        public string Note { get; set; } = string.Empty;
    }
}
