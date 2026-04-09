using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.EntityFrameworkCore;
using RailwayManagementSystemAPI.Data;
using RailwayManagementSystemAPI.Dtos;
using RailwayManagementSystemAPI.Exceptions;
using RailwayManagementSystemAPI.Models;

namespace RailwayManagementSystemAPI.Services
{
    public class RouteService : IRouteService
    {
        private readonly RailwayContext _context;
        private readonly IMapper _mapper;

        public RouteService(RailwayContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<RouteResponseDto> CreateRoute(CreateRouteDto dto)
        {
            var stationIds = dto.Stations.Select(s => s.StationId).ToList();

            var existingStations = await _context.Stations
                .Where(s => stationIds.Contains(s.Id))
                .Select(s => s.Id)
                .Distinct()
                .ToListAsync();

            var missing = stationIds.Except(existingStations);
            if (missing.Any())
                throw new BadRequestException($"Stations not found: {string.Join(",", missing)}");

            var route = _mapper.Map<Models.Route>(dto);

            await _context.Routes.AddAsync(route);
            await _context.SaveChangesAsync();

            return await GetRouteByIdAsync(route.Id);
        }

        public async Task DeleteRouteAsync(int id)
        {
            var rowsAffected = await _context.Routes
                .Where(r => r.Id == id)
                .ExecuteDeleteAsync();

            if (rowsAffected == 0)
                throw new NotFoundException($"Route with id {id} not found");
        }

        public async Task<RouteResponseDto> GetRouteByIdAsync(int id)
        {
            var route = await _context.Routes
                .ProjectTo<RouteResponseDto>(_mapper.ConfigurationProvider)
                .FirstOrDefaultAsync(r => r.Id == id);

            if (route == null)
                throw new NotFoundException($"Route with id {id} not found");

            return route;
        }

        public async Task<List<RouteResponseDto>> GetRoutesAsync()
        {
            return await _context.Routes
                .ProjectTo<RouteResponseDto>(_mapper.ConfigurationProvider)
                .ToListAsync();
        }

        public async Task UpdateRouteAsync(int id, CreateRouteDto dto)
        {
            var route = await _context.Routes
                .Include(r => r.RouteStations)
                .FirstOrDefaultAsync(r => r.Id == id);

            if (route == null)
                throw new NotFoundException($"Route with id {id} not found");

            var stationIds = dto.Stations.Select(s => s.StationId).ToList();
            var existingIds = await _context.Stations
                .Where(s => stationIds.Contains(s.Id))
                .Select(s => s.Id)
                .ToListAsync();

            var missing = stationIds.Except(existingIds).ToList();
            if (missing.Any())
                throw new BadRequestException($"Stations not found: {string.Join(", ", missing)}");

            _context.RouteStations.RemoveRange(route.RouteStations);

            route.Name = dto.Name;
            route.RouteStations = _mapper.Map<List<RouteStation>>(dto.Stations);

            await _context.SaveChangesAsync();
        }
    }
}
