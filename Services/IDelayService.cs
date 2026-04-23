using Microsoft.AspNetCore.Mvc;
using RailwayManagementSystemAPI.Dtos;

namespace RailwayManagementSystemAPI.Services
{
    public interface IDelayService
    {
        Task<DelayResponseDto> CreateDelay(CreateDelayDto dto);

        Task<DelayResponseDto> GetDelayById(int id);

        Task<List<DelayResponseDto>> GetDelaysByTrip(int tripId);
    }
}
