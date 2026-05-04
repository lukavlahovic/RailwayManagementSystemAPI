using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.EntityFrameworkCore;
using RailwayManagementSystemAPI.Data;
using RailwayManagementSystemAPI.Dtos;
using RailwayManagementSystemAPI.Exceptions;
using RailwayManagementSystemAPI.Models;

namespace RailwayManagementSystemAPI.Services
{
    public class DelayService : IDelayService
    {
        private readonly RailwayContext _context;
        private readonly IMapper _mapper;
        private readonly ILogger<DelayService> _logger;

        public DelayService(RailwayContext context, IMapper mapper, ILogger<DelayService> logger)
        {
            _context = context;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<DelayResponseDto> CreateDelayAsync(CreateDelayDto dto)
        {
            var trip = await _context.Trip.FindAsync(dto.TripId);
            if (trip == null)
            {
                _logger.LogWarning("Trip with {TripId} is invalid", dto.TripId);
                throw new BadRequestException("Invalid TripId");
            }

            var stationExistsOnTrip = await _context.RouteStations
                .AnyAsync(rs => rs.RouteId == trip.RouteId && rs.StationId == dto.StationId);

            if (!stationExistsOnTrip)
            {
                _logger.LogWarning("Station with {StationId} is invalid", dto.StationId);
                throw new BadRequestException("Invalid StationId");
            }

            var delay = _mapper.Map<Delay>(dto);

            await _context.Delays.AddAsync(delay);
            await _context.SaveChangesAsync();

            _logger.LogInformation("Delay with id {DelayId} was created", delay.Id);

            return await GetDelayByIdAsync(delay.Id);
        }

        public async Task<DelayResponseDto> GetDelayByIdAsync(int id)
        {
            var delay = await _context.Delays
                .Where(d => d.Id == id)
                .ProjectTo<DelayResponseDto>(_mapper.ConfigurationProvider)
                .FirstOrDefaultAsync();

            if (delay == null)
            {
                _logger.LogWarning("Delay with id {DelayId} not found", id);
                throw new NotFoundException($"Delay with id {id} not found");
            }

            return delay;
        }

        public async Task<List<DelayResponseDto>> GetDelaysByTripAsync(int tripId)
        {
            var tripExists = await _context.Trip.AnyAsync(t => t.Id == tripId);
            if (!tripExists)
            {
                _logger.LogWarning("Trip with id {TripId} not found", tripId);
                throw new NotFoundException($"Trip with id {tripId} not found");
            }

            var delays = await _context.Delays
                .Where(d => d.TripId == tripId)
                .OrderBy(d => d.CreatedAt)
                .ProjectTo<DelayResponseDto>(_mapper.ConfigurationProvider)
                .ToListAsync();

            return delays;
        }
    }
}
