using RailwayManagementSystemAPI.Dtos;

namespace RailwayManagementSystemAPI.Services
{
    public interface IStationService
    {
        Task<PagedResult<StationResponseDto>> GetAllStationsAsync(PaginationQuery paginationQuery);
        Task<StationResponseDto> GetStationByIdAsync(int id);
        Task<StationResponseDto> CreateStationAsync(StationDto dto);
        Task UpdateStationAsync(int id, StationDto dto);
        Task DeleteStationAsync(int id);
    }
}
