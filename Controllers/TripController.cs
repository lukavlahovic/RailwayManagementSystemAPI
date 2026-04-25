using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RailwayManagementSystemAPI.Dtos;
using RailwayManagementSystemAPI.Models;
using RailwayManagementSystemAPI.Services;

namespace RailwayManagementSystemAPI.Controllers
{
    [ApiController]
    [Route("api/trips")]
    public class TripController : ControllerBase
    {
        private readonly ITripService _tripService;

        public TripController(ITripService tripService)
        {
            _tripService = tripService;
        }

        [HttpPost]
        [Authorize(Roles = "Admin,Operator")]
        public async Task<IActionResult> CreateTrip([FromBody] CreateTripDto dto)
        {
            var response = await _tripService.CreateTrip(dto);

            return CreatedAtAction(nameof(GetTripById), new { id = response.Id}, response);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetTripById(int id)
        {
            var trip = await _tripService.GetTripById(id);

            return Ok(trip);
        }

        [HttpGet("station/{stationId}")]
        public async Task<IActionResult> GetTripsByStation(int stationId)
        {
            var trips = await _tripService.GetTripsByStation(stationId);

            return Ok(trips);
        }

        [HttpGet("date")]
        public async Task<IActionResult> GetTripsByDate([FromQuery] DateTime date)
        {
            var trips = await _tripService.GetTripsByDate(date);

            return Ok(trips);
        }

        [HttpGet("station/{stationId}/schedule")]
        public async Task<IActionResult> GetStationSchedule(int stationId)
        {
            var result = await _tripService.GetStationSchedule(stationId);

            return Ok(result);
        }

        [HttpGet("search")]
        public async Task<IActionResult> SearchTrips([FromQuery] TripSearchQuery query)
        {
            var response = await _tripService.SearchTrips(query);

            return Ok(response);
        }
    }
}
