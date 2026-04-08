using RailwayManagementSystemAPI.Dtos;

namespace RailwayManagementSystemAPI.Services
{
    public interface ITrainService
    {
        Task<List<TrainResponseDto>> GetAllTrainsAsync();
        Task<TrainResponseDto?> GetTrainByIdAsync(int id);
        Task<(TrainResponseDto? train, string? error)> CreateTrainAsync(CreateTrainDto dto);
        Task<string?> UpdateTrainAsync(int id, CreateTrainDto dto);
        Task<bool> DeleteTrainAsync(int id);
    }
}
