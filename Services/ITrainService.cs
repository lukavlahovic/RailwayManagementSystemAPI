using RailwayManagementSystemAPI.Dtos;

namespace RailwayManagementSystemAPI.Services
{
    public interface ITrainService
    {
        Task<PagedResult<TrainResponseDto>> GetAllTrainsAsync(PaginationQuery paginationQuery);
        Task<TrainResponseDto> GetTrainByIdAsync(int id);
        Task<TrainResponseDto> CreateTrainAsync(CreateTrainDto dto);
        Task UpdateTrainAsync(int id, CreateTrainDto dto);
        Task DeleteTrainAsync(int id);
    }
}
