namespace RailwayManagementSystemAPI.Models
{
    public class Station
    {
        public int Id { get; set; }
        public string Name { get; set; } = null!;
        public string City { get; set; } = null!;
        public string Country { get; set; } = null!;
        public int NumberOfPlatforms { get; set; }
    }
}
