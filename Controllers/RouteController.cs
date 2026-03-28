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

            return Ok(route);
        }
    }
}
