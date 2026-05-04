using RailwayManagementSystemAPI.Dtos;

namespace RailwayManagementSystemAPI.Services
{
    public interface IRouteService
    {
        Task<RouteResponseDto> CreateRouteAsync(CreateRouteDto dto);
        Task<List<RouteResponseDto>> GetRoutesAsync();
        Task<RouteResponseDto> GetRouteByIdAsync(int id);
        Task UpdateRouteAsync(int id, CreateRouteDto dto);
        Task DeleteRouteAsync(int id);
    }
}
