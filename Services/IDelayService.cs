using Microsoft.AspNetCore.Mvc;
using RailwayManagementSystemAPI.Dtos;

namespace RailwayManagementSystemAPI.Services
{
    public interface IDelayService
    {
        Task<DelayResponseDto> CreateDelayAsync(CreateDelayDto dto);

        Task<DelayResponseDto> GetDelayByIdAsync(int id);

        Task<List<DelayResponseDto>> GetDelaysByTripAsync(int tripId);
    }
}
