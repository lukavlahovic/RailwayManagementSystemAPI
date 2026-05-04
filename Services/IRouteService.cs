using RailwayManagementSystemAPI.Dtos;

namespace RailwayManagementSystemAPI.Services
{
    public interface IRouteService
    {
        Task<RouteResponseDto> CreateRouteAsync(CreateRouteDto dto);
        Task<PagedResult<RouteResponseDto>> GetRoutesAsync(PaginationQuery paginationQuery);
        Task<RouteResponseDto> GetRouteByIdAsync(int id);
        Task UpdateRouteAsync(int id, CreateRouteDto dto);
        Task DeleteRouteAsync(int id);
    }
}
