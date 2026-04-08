using RailwayManagementSystemAPI.Dtos;

namespace RailwayManagementSystemAPI.Services
{
    public interface IStationService
    {
        Task<List<StationResponseDto>> GetAllStationsAsync();
        Task<StationResponseDto> GetStationByIdAsync(int id);
        Task<StationResponseDto> CreateStationAsync(StationDto dto);
        Task UpdateStationAsync(int id, StationDto dto);
        Task DeleteStationAsync(int id);
    }
}
