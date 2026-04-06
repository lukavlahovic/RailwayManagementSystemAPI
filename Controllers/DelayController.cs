using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RailwayManagementSystemAPI.Data;
using RailwayManagementSystemAPI.Dtos;
using RailwayManagementSystemAPI.Models;

namespace RailwayManagementSystemAPI.Controllers
{
    [ApiController]
    [Route("api/delays")]
    public class DelayController : ControllerBase
    {
        private readonly RailwayContext _context;

        public DelayController(RailwayContext context)
        {
            _context = context;
        }

        [HttpPost]
        public async Task<IActionResult> CreateDelay(CreateDelayDto dto)
        {
            var trip = await _context.Trip.FindAsync(dto.TripId);
            if (trip == null)
                return BadRequest("Invalid TripId");

            var stationExistsOnTrip = await _context.RouteStations
                .AnyAsync(rs => rs.RouteId == trip.RouteId && rs.StationId == dto.StationId);

            if (!stationExistsOnTrip)
                return BadRequest("Invalid StationId");

            var delay = new Delay
            {
                TripId = dto.TripId,
                StationId = dto.StationId,
                DelayMinutes = dto.DelayMinutes,
                TypeOfDelay = dto.TypeOfDelay,
                Note = dto.Note
            };

            await _context.Delays.AddAsync(delay);
            await _context.SaveChangesAsync();

            var response = await _context.Delays
                .Where(d => d.Id == delay.Id)
                .Select(d => new DelayResponseDto 
                {
                    Id = delay.Id,
                    TripId = delay.TripId,
                    StationName = delay.Station.Name,
                    DelayMinutes = delay.DelayMinutes,
                    TypeOfDelay = delay.TypeOfDelay,
                    CreatedAt = delay.CreatedAt,
                    Note = delay.Note
                })
                .FirstOrDefaultAsync();

            return CreatedAtAction(nameof(GetDelayById), new { id = delay.Id }, response);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetDelayById(int id)
        {
            var delay = await _context.Delays
                .Where(d => d.Id == id)
                .Select(d => new DelayResponseDto
                {
                    Id = d.Id,
                    TripId = d.TripId,
                    StationName = d.Station.Name,
                    DelayMinutes = d.DelayMinutes,
                    TypeOfDelay = d.TypeOfDelay,
                    CreatedAt = d.CreatedAt,
                    Note = d.Note
                })
                .FirstOrDefaultAsync();

            if (delay == null)
                return NotFound();

            return Ok(delay);
        }
    }
}
