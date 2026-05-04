using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RailwayManagementSystemAPI.Dtos;
using RailwayManagementSystemAPI.Models;
using RailwayManagementSystemAPI.Services;

namespace RailwayManagementSystemAPI.Controllers
{
    [ApiController]
    [Route("api/delays")]
    public class DelayController : ControllerBase
    {
        private readonly IDelayService _delayService;

        public DelayController(IDelayService delayService)
        {
            _delayService = delayService;
        }

        [HttpPost]
        [Authorize(Roles = "Admin,Operator")]
        public async Task<IActionResult> CreateDelay(CreateDelayDto dto)
        {
            var response = await _delayService.CreateDelayAsync(dto);

            return CreatedAtAction(nameof(GetDelayById), new { id = response.Id }, response);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetDelayById(int id)
        {
            var delay = await _delayService.GetDelayByIdAsync(id);

            return Ok(delay);
        }

        [HttpGet("trip/{tripId}")]
        public async Task<IActionResult> GetDelaysByTrip(int tripId)
        {
            var delays = await _delayService.GetDelaysByTripAsync(tripId);

            return Ok(delays);
        }
    }
}
