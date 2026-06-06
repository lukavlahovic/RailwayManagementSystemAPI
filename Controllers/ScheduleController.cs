using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RailwayManagementSystemAPI.Dtos;
using RailwayManagementSystemAPI.Services;

namespace RailwayManagementSystemAPI.Controllers
{
    [ApiController]
    [Route("api/schedules")]
    public class ScheduleController : ControllerBase
    {
        private readonly IScheduleService _service;

        public ScheduleController(IScheduleService service)
        {
            _service = service;
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> CreateSchedule([FromBody] CreateScheduleDto dto)
        {
            var response = await _service.CreateScheduleAsync(dto);

            return CreatedAtAction(nameof(GetScheduleById), new { id = response.Id}, response);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetScheduleById(int id)
        {
            var schedule = await _service.GetScheduleByIdAsync(id);

            return Ok(schedule);
        }

        [HttpGet]
        public async Task<IActionResult> GetAllSchedules([FromQuery] PaginationQuery query)
        {
            var schedules = await _service.GetAllSchedulesAsync(query);

            return Ok(schedules);
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> UpdateSchedule(int id, [FromBody] CreateScheduleDto dto)
        {
            await _service.UpdateScheduleAsync(id, dto);

            return NoContent();
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteSchedule(int id)
        {
            await _service.DeleteScheduleAsync(id);

            return NoContent();
        }

        [HttpPatch("{id}/toggle")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> ToggleActive(int id)
        {
            await _service.ToggleActiveAsync(id);

            return NoContent();
        }
    }
}