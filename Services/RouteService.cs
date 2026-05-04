using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.EntityFrameworkCore;
using RailwayManagementSystemAPI.Data;
using RailwayManagementSystemAPI.Dtos;
using RailwayManagementSystemAPI.Exceptions;
using RailwayManagementSystemAPI.Models;
using System.Linq;

namespace RailwayManagementSystemAPI.Services
{
    public class RouteService : IRouteService
    {
        private readonly RailwayContext _context;
        private readonly IMapper _mapper;
        private readonly ILogger<RouteService> _logger;

        public RouteService(RailwayContext context, IMapper mapper, ILogger<RouteService> logger)
        {
            _context = context;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<RouteResponseDto> CreateRouteAsync(CreateRouteDto dto)
        {
            var stationIds = dto.Stations.Select(s => s.StationId).ToList();

            var existingStations = await _context.Stations
                .Where(s => stationIds.Contains(s.Id))
                .Select(s => s.Id)
                .Distinct()
                .ToListAsync();

            var missing = stationIds.Except(existingStations);
            if (missing.Any())
            {
                _logger.LogWarning("Stations not found {Missing}", string.Join(",", missing));
                throw new BadRequestException($"Stations not found: {string.Join(",", missing)}");
            }

            var route = _mapper.Map<Models.Route>(dto);

            await _context.Routes.AddAsync(route);
            await _context.SaveChangesAsync();

            _logger.LogInformation("Created Route with id {RouteId}", route.Id);

            return await GetRouteByIdAsync(route.Id);
        }

        public async Task DeleteRouteAsync(int id)
        {
            var rowsAffected = await _context.Routes
                .Where(r => r.Id == id)
                .ExecuteDeleteAsync();

            if (rowsAffected == 0)
            {
                _logger.LogWarning("Route with id {RouteId} was not found for deletion", id);
                throw new NotFoundException($"Route with id {id} not found");
            }

            _logger.LogInformation("Route with id {RouteId} was deleted", id);
        }

        public async Task<RouteResponseDto> GetRouteByIdAsync(int id)
        {
            var route = await _context.Routes
                .ProjectTo<RouteResponseDto>(_mapper.ConfigurationProvider)
                .FirstOrDefaultAsync(r => r.Id == id);

            if (route == null)
            {
                _logger.LogWarning("Route with id {RouteId} was not found", id);
                throw new NotFoundException($"Route with id {id} not found");
            }

            return route;
        }

        public async Task<PagedResult<RouteResponseDto>> GetRoutesAsync(PaginationQuery paginationQuery)
        {
            paginationQuery.Page = paginationQuery.Page < 1 ? 1 : paginationQuery.Page;
            paginationQuery.PageSize = paginationQuery.PageSize > 50 ? 50 : paginationQuery.PageSize;

            var totalCount = await _context.Routes.CountAsync();

            var routes = await _context.Routes
                .Skip((paginationQuery.Page - 1) * paginationQuery.PageSize)
                .Take(paginationQuery.PageSize)
                .ProjectTo<RouteResponseDto>(_mapper.ConfigurationProvider)
                .ToListAsync();

            var response = new PagedResult<RouteResponseDto>
            {
                Items = routes,
                TotalCount = totalCount,
                Page = paginationQuery.Page,
                PageSize = paginationQuery.PageSize
            };

            return response;
        }

        public async Task UpdateRouteAsync(int id, CreateRouteDto dto)
        {
            var route = await _context.Routes
                .Include(r => r.RouteStations)
                .FirstOrDefaultAsync(r => r.Id == id);

            if (route == null)
            {
                _logger.LogWarning("Route with id {RouteId} was not found for update", id);
                throw new NotFoundException($"Route with id {id} not found");
            }

            var stationIds = dto.Stations.Select(s => s.StationId).ToList();
            var existingIds = await _context.Stations
                .Where(s => stationIds.Contains(s.Id))
                .Select(s => s.Id)
                .ToListAsync();

            var missing = stationIds.Except(existingIds).ToList();
            if (missing.Any())
            {
                _logger.LogWarning("Stations not found {Missing}", string.Join(", ", missing));
                throw new BadRequestException($"Stations not found: {string.Join(", ", missing)}");
            }

            _context.RouteStations.RemoveRange(route.RouteStations);

            route.Name = dto.Name;
            route.RouteStations = _mapper.Map<List<RouteStation>>(dto.Stations);

            await _context.SaveChangesAsync();

            _logger.LogInformation("Route with id {RouteId} was updated", id);
        }
    }
}
