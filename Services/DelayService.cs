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

        public DelayService(RailwayContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }
        public async Task<DelayResponseDto> CreateDelay(CreateDelayDto dto)
        {
            var trip = await _context.Trip.FindAsync(dto.TripId);
            if (trip == null)
                throw new BadRequestException("Invalid TripId");

            var stationExistsOnTrip = await _context.RouteStations
                .AnyAsync(rs => rs.RouteId == trip.RouteId && rs.StationId == dto.StationId);

            if (!stationExistsOnTrip)
                throw new BadRequestException("Invalid StationId");

            var delay = _mapper.Map<Delay>(dto);

            await _context.Delays.AddAsync(delay);
            await _context.SaveChangesAsync();

            return await GetDelayById(delay.Id);
        }

        public async Task<DelayResponseDto> GetDelayById(int id)
        {
            var delay = await _context.Delays
                .Where(d => d.Id == id)
                .ProjectTo<DelayResponseDto>(_mapper.ConfigurationProvider)
                .FirstOrDefaultAsync();

            if (delay == null)
                throw new NotFoundException($"Delay with id {id} not found");

            return delay;
        }

        public async Task<List<DelayResponseDto>> GetDelaysByTrip(int tripId)
        {
            var tripExists = _context.Trip.FindAsync(tripId);
            if (tripExists == null)
                throw new BadRequestException($"Trip with id {tripId} not found");

            var delays = await _context.Delays
                .Where(d => d.TripId == tripId)
                .OrderBy(d => d.CreatedAt)
                .ProjectTo<DelayResponseDto>(_mapper.ConfigurationProvider)
                .ToListAsync();

            return delays;
        }
    }
}
