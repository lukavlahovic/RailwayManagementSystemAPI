using RailwayManagementSystemAPI.Dtos;

namespace RailwayManagementSystemAPI.Services
{
    public interface IScheduleService
    {
        Task<ScheduleResponseDto> CreateScheduleAsync(CreateScheduleDto dto);

        Task<ScheduleResponseDto> GetScheduleByIdAsync(int id);

        Task<PagedResult<ScheduleResponseDto>> GetAllSchedulesAsync(PaginationQuery query);

        Task UpdateScheduleAsync(int id, CreateScheduleDto dto);

        Task DeleteScheduleAsync(int id);

        Task ToggleActiveAsync(int id);
    }
}