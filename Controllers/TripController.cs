using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RailwayManagementSystemAPI.Data;
using RailwayManagementSystemAPI.Dtos;
using RailwayManagementSystemAPI.Models;

namespace RailwayManagementSystemAPI.Controllers
{
    [ApiController]
    [Route("api/trips")]
    public class TripController : ControllerBase
    {
        private readonly RailwayContext _context;

        public TripController(RailwayContext context)
        {
            _context = context;
        }

        [HttpPost]
        public async Task<IActionResult> CreateTrip([FromBody] CreateTripDto dto)
        {
            if(dto.DepartureTime >= dto.ArrivalTime)
                return BadRequest("Arrival time must be after departure time!");

            var trainExists = await _context.Trains.AnyAsync(t => t.Id == dto.TrainId);

            if(!trainExists)
                return BadRequest("Invalid TrainId");

            var routeExists = await _context.Routes.AnyAsync(r => r.Id == dto.RouteId);

            if (!routeExists)
                return BadRequest("Invalid RouteId");

            var trip = new Trip
            {
                TrainId = dto.TrainId,
                RouteId = dto.RouteId,
                DepartureTime = dto.DepartureTime,
                ArrivalTime = dto.ArrivalTime
            };

            await _context.Trip.AddAsync(trip);
            await _context.SaveChangesAsync();

            var response = await _context.Trip
                .Where(t => t.Id == trip.Id)
                .Select(t => new TripResponseDto
                {
                    SerialNumber = t.Train.SerialNumber,
                    TrainTypeName = t.Train.TrainType.Name,
                    RouteName = t.Route.Name,
                    DepartureTime = t.DepartureTime,
                    ArrivalTime = t.ArrivalTime
                })
                .FirstOrDefaultAsync();

            return CreatedAtAction(nameof(GetTripById), new { id = trip.Id}, response);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetTripById(int id)
        {
            var trip = await _context.Trip
                .Where(t => t.Id == id)
                .Select(t => new TripResponseDto
                {
                    SerialNumber = t.Train.SerialNumber,
                    TrainTypeName = t.Train.TrainType.Name,
                    RouteName = t.Route.Name,
                    DepartureTime = t.DepartureTime,
                    ArrivalTime = t.ArrivalTime
                })
                .FirstOrDefaultAsync();

            if (trip == null)
                return NotFound();

            return Ok(trip);
        }

        [HttpGet("station/{stationId}")]
        public async Task<IActionResult> GetTripsByStation(int stationId)
        {
            var trips = await _context.Trip
                .Where(t => t.Route.RouteStations
                    .Any(rs => rs.StationId == stationId))
                .Select(t => new TripScheduleDto
                {
                    TripId = t.Id,
                    Train = t.Train.TrainType.Name,
                    Route = t.Route.Name,
                    DepartureTime = t.DepartureTime,
                    ArrivalTime = t.ArrivalTime
                })
                .OrderBy(t => t.DepartureTime)
                .ToListAsync();

            return Ok(trips);
        }

        [HttpGet("date")]
        public async Task<IActionResult> GetTripsByDate([FromQuery] DateTime date)
        {
            var trips = await _context.Trip
                .Where(t => t.DepartureTime.Date == date.Date)
                .Select(t => new TripScheduleDto
                {
                    TripId = t.Id,
                    Train = t.Train.TrainType.Name,
                    Route = t.Route.Name,
                    DepartureTime = t.DepartureTime,
                    ArrivalTime = t.ArrivalTime
                })
                .OrderBy(t => t.DepartureTime)
                .ToListAsync();

            return Ok(trips);
        }

        [HttpGet("station/{station}/schedule")]
        public async Task<IActionResult> getStationSchedule(int stationId)
        {
            var now = DateTime.Now;

            var schedule = await _context.Trip
                .Where(t => t.ArrivalTime > now)
                .Where(t => t.Route.RouteStations
                    .Any(rs => rs.StationId == stationId))
                .Select(t => new
                {
                    Train = t.Train.SerialNumber,
                    Route = t.Route.Name,
                    t.DepartureTime,
                    ArrivalOffsetMinutes = t.Route.RouteStations
                        .Where(rs => rs.StationId == stationId)
                        .Select(rs => rs.ArrivalOffsetMinutes)
                        .FirstOrDefault()
                })
                .ToListAsync();

            var result = schedule
                .Select(s =>
                {
                    var arrival = s.DepartureTime.AddMinutes(s.ArrivalOffsetMinutes);
                    var minutes = (arrival - now).TotalMinutes;

                    return new StationScheduleDto
                    {
                        Train = s.Train,
                        Route = s.Route,
                        ArrivalTime = arrival,
                        MinutesUntilArrival = minutes
                    };
                })
                .Where(x => x.MinutesUntilArrival >= -5) // show departed train for 5 minutes
                .OrderBy(x => x.ArrivalTime)
                .ToList();

            return Ok();
        }
    }
}
