namespace RailwayManagementSystemAPI.Models
{
    public enum TypeOfDelay
    {
        Weather, 
        Technical, 
        StationCongestion, 
        TrackMaintenance, 
        PassengerIncident, 
        ExternalFactor, 
        Other
    }

    public class Delay
    {
        public int Id { get; set; }

        public int TripId { get; set; }
        public Trip Trip { get; set; } = null!;

        public int StationId { get; set; }
        public Station Station { get; set; } = null!;

        public int DelayMinutes { get; set; }

        public TypeOfDelay TypeOfDelay { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public string Note { get; set; } = string.Empty;
    }
}
