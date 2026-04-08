using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RailwayManagementSystemAPI.Data;
using RailwayManagementSystemAPI.Dtos;
using RailwayManagementSystemAPI.Models;
using RailwayManagementSystemAPI.Services;

namespace RailwayManagementSystemAPI.Controllers
{
    [ApiController]
    [Route("api/trains")]
    public class TrainController : ControllerBase
    {
        private readonly ITrainService _trainService;

        public TrainController(ITrainService trainService)
        {
            _trainService = trainService;
        }

        /// <summary>
        /// Retrieves all trains with their associated train type details.
        /// </summary>
        /// <returns>An IActionResult containing a list of TrainResponseDto objects.</returns>
        [HttpGet]
        public async Task<IActionResult> GetAllTrains()
        {
            var trains = await _trainService.GetAllTrainsAsync();

            return Ok(trains);
        }

        /// <summary>
        /// Retrieves a train by its unique identifier.
        /// </summary>
        /// <param name="id">The unique identifier of the train.</param>
        /// <returns>An IActionResult containing the train data if found; otherwise, NotFound.</returns>
        [HttpGet("{id}")]
        public async Task<IActionResult> GetTrainById(int id)
        {
            var train = await _trainService.GetTrainByIdAsync(id);

            if (train == null)
                return NotFound();

            return Ok(train);
        }

        /// <summary>
        /// Creates a new train and returns the created resource.
        /// </summary>
        /// <param name="trainDto">The data for the train to create.</param>
        /// <returns>A 201 Created response with the created train, or 400 Bad Request if the train type does not exist.</returns>
        [HttpPost]
        public async Task<IActionResult> CreateTrain([FromBody] CreateTrainDto trainDto)
        {
            var (train,error) = await _trainService.CreateTrainAsync(trainDto);
            if (error != null)
                return BadRequest(error);

            return CreatedAtAction(nameof(GetTrainById), new { id = train!.Id }, train);
        }

        /// <summary>
        /// Updates an existing train with the specified values.
        /// </summary>
        /// <param name="id">The identifier of the train to update.</param>
        /// <param name="trainDto">The data transfer object containing updated train information.</param>
        /// <returns>A 204 No Content response if the update is successful; 400 Bad Request if the specified TrainTypeId does not
        /// exist; 404 Not Found if the train is not found.</returns>
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateTrain(int id, [FromBody] CreateTrainDto trainDto)
        {
            var error = await _trainService.UpdateTrainAsync(id, trainDto);

            if (error == "NotFound")
                return NotFound();
            if (error != null)
                return BadRequest(error);

            return NoContent();
        }

        /// <summary>
        /// Deletes the train with the specified identifier.
        /// </summary>
        /// <param name="id">The identifier of the train to delete.</param>
        /// <returns>A 204 No Content response if the train was deleted; otherwise, a 404 Not Found response if the train does
        /// not exist.</returns>
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteTrain(int id)
        {
            var isDeleted = await _trainService.DeleteTrainAsync(id);

            if (!isDeleted)
                return NotFound();

            return NoContent();
        }
    }
}
