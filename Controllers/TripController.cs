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
            var response = await _tripService.CreateTripAsync(dto);

            return CreatedAtAction(nameof(GetTripById), new { id = response.Id}, response);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetTripById(int id)
        {
            var trip = await _tripService.GetTripByIdAsync(id);

            return Ok(trip);
        }

        [HttpGet("station/{stationId}")]
        public async Task<IActionResult> GetTripsByStation(int stationId)
        {
            var trips = await _tripService.GetTripsByStationAsync(stationId);

            return Ok(trips);
        }

        [HttpGet("date")]
        public async Task<IActionResult> GetTripsByDate([FromQuery] DateTime date)
        {
            var trips = await _tripService.GetTripsByDateAsync(date);

            return Ok(trips);
        }

        [HttpGet("station/{stationId}/schedule")]
        public async Task<IActionResult> GetStationSchedule(int stationId)
        {
            var result = await _tripService.GetStationScheduleAsync(stationId);

            return Ok(result);
        }

        [HttpGet("search")]
        public async Task<IActionResult> SearchTrips([FromQuery] TripSearchQuery query)
        {
            var response = await _tripService.SearchTripsAsync(query);

            return Ok(response);
        }

        [HttpGet("/api/trips/{id}/position")]
        public async Task<IActionResult> GetTripPosition(int id)
        {
            var response = await _tripService.GetTripPositionAsync(id);

            return Ok(response);
        }

        [HttpGet("/api/trips/{id}/completed")]
        [Authorize(Roles = "Admin,Operator")]
        public async Task<IActionResult> CompleteTrip(int id, [FromBody] CompleteTripDto dto)
        {
            await _tripService.CompleteTripAsync(id, dto);

            return NoContent();
        }
    }
}
