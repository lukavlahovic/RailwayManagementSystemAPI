using Microsoft.EntityFrameworkCore;
using RailwayManagementSystemAPI.Data;
using RailwayManagementSystemAPI.Dtos;
using RailwayManagementSystemAPI.Models;
using RailwayManagementSystemAPI.Exceptions;

namespace RailwayManagementSystemAPI.Services
{
    public class StationService : IStationService
    {
        private readonly RailwayContext _context;

        public StationService(RailwayContext context)
        {
            _context = context;
        }

        public async Task<List<StationResponseDto>> GetAllStationsAsync()
        {
            return await _context.Stations
                .Select(s => new StationResponseDto
                {
                    Id = s.Id,
                    Name = s.Name,
                    City = s.City,
                    Country = s.Country,
                    NumberOfPlatforms = s.NumberOfPlatforms
                })
                .ToListAsync();
        }
        public async Task<StationResponseDto> GetStationByIdAsync(int id)
        {
            var station = await _context.Stations
                .Where(s => s.Id == id)
                .Select(s => new StationResponseDto
                {
                    Id = s.Id,
                    Name = s.Name,
                    City = s.City,
                    Country = s.Country,
                    NumberOfPlatforms = s.NumberOfPlatforms
                })
                .FirstOrDefaultAsync();

            if (station == null)
                throw new NotFoundException($"Station with id {id} not found");

            return station;
        }
        public async Task<StationResponseDto> CreateStationAsync(StationDto dto)
        {
            var station = new Station
            {
                Name = dto.Name,
                City = dto.City,
                Country = dto.Country,
                NumberOfPlatforms = dto.NumberOfPlatforms
            };

            await _context.AddAsync(station);
            await _context.SaveChangesAsync();

            return new StationResponseDto
            {
                Id = station.Id,
                Name = station.Name,
                City = station.City,
                Country = station.Country,
                NumberOfPlatforms = station.NumberOfPlatforms
            };
        }
        public async Task UpdateStationAsync(int id, StationDto dto)
        {
            var rowsAffected = await _context.Stations
                .Where(s => s.Id == id)
                .ExecuteUpdateAsync(setter => setter
                    .SetProperty(s => s.Name, dto.Name)
                    .SetProperty(s => s.City, dto.City)
                    .SetProperty(s => s.Country, dto.Country)
                    .SetProperty(s => s.NumberOfPlatforms, dto.NumberOfPlatforms)
                );

            if (rowsAffected == 0)
                throw new NotFoundException($"Station with id {id} not found");
        }
        public async Task DeleteStationAsync(int id)
        {
            var rowsAffected = await _context.Stations
                .Where(s => s.Id == id)
                .ExecuteDeleteAsync();

            if (rowsAffected == 0)
                throw new NotFoundException($"Station with id {id} not found");
        }
    }
}
