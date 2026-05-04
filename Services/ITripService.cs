using Microsoft.AspNetCore.Mvc;
using RailwayManagementSystemAPI.Dtos;

namespace RailwayManagementSystemAPI.Services
{
    public interface ITripService
    {
        Task<TripResponseDto> CreateTripAsync(CreateTripDto dto);

        Task<TripResponseDto> GetTripByIdAsync(int id);

        Task<List<TripScheduleDto>> GetTripsByStationAsync(int stationId);

        Task<List<TripScheduleDto>> GetTripsByDateAsync(DateTime date);

        Task<List<StationScheduleDto>> GetStationScheduleAsync(int stationId);

        Task<PagedResult<TripSearchResponseDto>> SearchTripsAsync(TripSearchQuery query);
    }
}
