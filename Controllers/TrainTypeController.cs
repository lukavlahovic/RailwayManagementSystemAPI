using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RailwayManagementSystemAPI.Dtos;
using RailwayManagementSystemAPI.Models;
using RailwayManagementSystemAPI.Services;

namespace RailwayManagementSystemAPI.Controllers
{
    [ApiController]
    [Route("api/train-types")]
    public class TrainTypeController : ControllerBase
    {
        private readonly ITrainTypeService _trainTypeService;

        public TrainTypeController(ITrainTypeService trainTypeService)
        {
            _trainTypeService = trainTypeService;
        }

        /// <summary>
        /// Creates a new train type and returns the created resource.
        /// </summary>
        /// <param name="dto">The data transfer object containing information for the new train type.</param>
        /// <returns>A response with the created train type and a location header.</returns>
        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> CreateTrainType([FromBody] CreateTrainTypeDto dto)
        {
            var trainType = await _trainTypeService.CreateTrainTypeAsync(dto);

            return CreatedAtAction(nameof(GetTrainTypeById), new { id = trainType.Id }, trainType);
        }

        /// <summary>
        /// Retrieves all train types from the database.
        /// </summary>
        /// <returns>An IActionResult containing a list of TrainTypeResponseDto objects.</returns>
        [HttpGet]
        public async Task<IActionResult> GetAllTrainTypes()
        {
            var trainTypes = await _trainTypeService.GetAllTrainTypesAsync();

            return Ok(trainTypes);
        }

        /// <summary>
        /// Retrieves a train type by its unique identifier.
        /// </summary>
        /// <param name="id">The unique identifier of the train type.</param>
        /// <returns>An IActionResult containing the train type data if found; otherwise, a NotFound result.</returns>
        [HttpGet("{id}")]
        public async Task<IActionResult> GetTrainTypeById(int id)
        {
            var trainType = await _trainTypeService.GetTrainTypeByIdAsync(id);

            return Ok(trainType);
        }
    }
}
