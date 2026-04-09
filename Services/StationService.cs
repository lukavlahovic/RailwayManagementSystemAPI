using AutoMapper;
using Microsoft.EntityFrameworkCore;
using RailwayManagementSystemAPI.Data;
using RailwayManagementSystemAPI.Dtos;
using RailwayManagementSystemAPI.Exceptions;
using RailwayManagementSystemAPI.Models;

namespace RailwayManagementSystemAPI.Services
{
    public class StationService : IStationService
    {
        private readonly RailwayContext _context;
        private readonly IMapper _mapper;

        public StationService(RailwayContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<List<StationResponseDto>> GetAllStationsAsync()
        {
            var stations = await _context.Stations.ToListAsync();
            return _mapper.Map<List<StationResponseDto>>(stations);
        }
        public async Task<StationResponseDto> GetStationByIdAsync(int id)
        {
            var station = await _context.Stations.FindAsync(id);

            if (station == null)
                throw new NotFoundException($"Station with id {id} not found");

            return _mapper.Map<StationResponseDto>(station);
        }
        public async Task<StationResponseDto> CreateStationAsync(StationDto dto)
        {
            var station = _mapper.Map<Station>(dto);

            await _context.AddAsync(station);
            await _context.SaveChangesAsync();

            return _mapper.Map<StationResponseDto>(station);
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
