namespace RailwayManagementSystemAPI.Models
{
    public class Route
    {
        public int Id { get; set; }

        public string Name { get; set; } = string.Empty;

        public ICollection<RouteStation> RouteStations { get; set; } = new List<RouteStation>();
    }
}
