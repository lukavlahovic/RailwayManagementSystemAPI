namespace RailwayManagementSystemAPI.Dtos
{
    public class RouteResponseDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;

        public List<RouteStationResponseDto> Stations { get; set; } = [];
    }
}
