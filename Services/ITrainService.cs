using RailwayManagementSystemAPI.Dtos;

namespace RailwayManagementSystemAPI.Services
{
    public interface ITrainService
    {
        Task<List<TrainResponseDto>> GetAllTrainsAsync();
        Task<TrainResponseDto> GetTrainByIdAsync(int id);
        Task<TrainResponseDto> CreateTrainAsync(CreateTrainDto dto);
        Task UpdateTrainAsync(int id, CreateTrainDto dto);
        Task DeleteTrainAsync(int id);
    }
}
