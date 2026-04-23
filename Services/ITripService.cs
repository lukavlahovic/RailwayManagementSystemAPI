using Microsoft.AspNetCore.Mvc;
using RailwayManagementSystemAPI.Dtos;

namespace RailwayManagementSystemAPI.Services
{
    public interface ITripService
    {
        Task<TripResponseDto> CreateTrip(CreateTripDto dto);

        Task<TripResponseDto> GetTripById(int id);

        Task<List<TripScheduleDto>> GetTripsByStation(int stationId);

        Task<List<TripScheduleDto>> GetTripsByDate(DateTime date);

        Task<List<StationScheduleDto>> GetStationSchedule(int stationId);

        Task<PagedResult<TripSearchResponseDto>> SearchTrips(TripSearchQuery query);
    }
}
