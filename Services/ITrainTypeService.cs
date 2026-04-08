using RailwayManagementSystemAPI.Dtos;

namespace RailwayManagementSystemAPI.Services
{
    public interface ITrainTypeService
    {
        Task<List<TrainTypeResponseDto>> GetAllTrainTypesAsync();
        Task<TrainTypeResponseDto> GetTrainTypeByIdAsync(int id);
        Task<TrainTypeResponseDto> CreateTrainTypeAsync(CreateTrainTypeDto dto);
    }
}
