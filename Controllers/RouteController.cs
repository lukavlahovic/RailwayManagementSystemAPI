using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RailwayManagementSystemAPI.Data;
using RailwayManagementSystemAPI.Dtos;
using RailwayManagementSystemAPI.Models;

namespace RailwayManagementSystemAPI.Controllers
{
    [ApiController]
    [Route("api/routes")]
    public class RouteController : ControllerBase
    {
        private readonly RailwayContext _context;

        public RouteController(RailwayContext context)
        {
            _context = context;
        }

        [HttpPost]
        public async Task<IActionResult> CreateRoute([FromBody] CreateRouteDto dto)
        {
            var route = new Models.Route { Name = dto.Name! };

            var stationIds = dto.Stations.Select(s => s.StationId).ToList();

            var existingStations = await _context.Stations
                .Where(s => stationIds.Contains(s.Id))
                .Select(s => s.Id)
                .Distinct()
                .ToListAsync();

            var missing = stationIds.Except(existingStations);

            if (missing.Any())
                return BadRequest($"Stations not found: {string.Join(",", missing)}");

            route.RouteStations = dto.Stations!
                .Select(s => new RouteStation
                {
                    StationId = s.StationId,
                    Order = s.Order
                })
                .ToList();

            await _context.Routes.AddAsync(route);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetRouteById), new { id = route.Id }, route);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetRouteById(int id)
        {
            RouteResponseDto? response = await _context.Routes
                .Where(r => r.Id == id)
                .Select(r => new RouteResponseDto
                {
                    Id = r.Id,
                    Name = r.Name,
                    Stations = r.RouteStations
                        .OrderBy(rs => rs.Order)
                        .Select(rs => new RouteStationResponseDto
                        {
                            StationName = rs.Station.Name,
                            Order = rs.Order
                        }).ToList()
                })
                .FirstOrDefaultAsync();

            if (response == null)
                return NotFound();

            return Ok(response);
        }

        [HttpGet]
        public async Task<IActionResult> GetRoutes()
        {
            List<RouteResponseDto> routes = await _context.Routes
                .Select(r => new RouteResponseDto
                {
                    Id = r.Id,
                    Name = r.Name,
                    Stations = r.RouteStations
                        .OrderBy(rs => rs.Order)
                        .Select(rs => new RouteStationResponseDto
                        {
                            StationName = rs.Station.Name,
                            Order = rs.Order
                        })
                        .ToList()
                })
                .ToListAsync();

            return Ok(routes);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateRoute(int id, [FromBody] CreateRouteDto dto)
        {
            var duplicates = dto.Stations
                .GroupBy(s => s.Order)
                .Any(g => g.Count() > 1);

            if (duplicates)
                return BadRequest("Duplicate order values are not allowed.");

            var route = await _context.Routes
                .Include(r => r.RouteStations)
                .FirstOrDefaultAsync(r => r.Id == id);

            if (route == null)
                return NotFound();

            route.Name = dto.Name;

            _context.RouteStations.RemoveRange(route.RouteStations);

            route.RouteStations = dto.Stations
                .Select(s => new RouteStation
                {
                    StationId = s.StationId,
                    Order = s.Order
                })
                .ToList();

            await _context.SaveChangesAsync();

            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteRoute(int id)
        {
            var rowsAffected = await _context.Routes
                .Where(r => r.Id == id)
                .ExecuteDeleteAsync();

            if (rowsAffected == 0)
                return NotFound();

            return NoContent();
        }
    }
}
