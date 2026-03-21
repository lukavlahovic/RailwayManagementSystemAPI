using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RailwayManagementSystemAPI.Data;
using RailwayManagementSystemAPI.Dtos;
using RailwayManagementSystemAPI.Models;

namespace RailwayManagementSystemAPI.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class TrainController : ControllerBase
    {
        private readonly RailwayContext _context;

        public TrainController(RailwayContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Retrieves all trains from the data store.
        /// </summary>
        /// <returns>An IActionResult containing the list of trains.</returns>
        [HttpGet]
        public async Task<IActionResult> GetAllTrains()
        {
            var trains = await _context.Trains.ToListAsync();
            return Ok(trains);
        }

        /// <summary>
        /// Retrieves a train by its unique identifier.
        /// </summary>
        /// <param name="id">The unique identifier of the train.</param>
        /// <returns>An IActionResult containing the train if found; otherwise, a NotFound result.</returns>
        [HttpGet("{id}")]
        public async Task<IActionResult> GetTrainById(int id)
        {
            var train = await _context.Trains.Where(s => s.Id == id).ToListAsync();

            if (train == null)
                return NotFound();

            return Ok(train);
        }

        /// <summary>
        /// Creates a new train and returns a response with the location of the created resource.
        /// </summary>
        /// <param name="trainDto">The train entity to add.</param>
        /// <returns>A CreatedAtActionResult containing the created train.</returns>
        [HttpPost]
        public async Task<IActionResult> CreateTrain([FromBody] TrainDto trainDto)
        {
            var train = new Train
            {
                Name = trainDto.Name,
                MaxSpeed = trainDto.MaxSpeed,
                Capacity = trainDto.Capacity,
                Type = trainDto.Type
            };
            await _context.Trains.AddAsync(train);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetTrainById), new { id = train.Id }, train);
        }

        /// <summary>
        /// Updates the details of an existing train.
        /// </summary>
        /// <param name="id">The identifier of the train to update.</param>
        /// <param name="trainDto">The updated train data.</param>
        /// <returns>A 204 No Content response if the update is successful; 400 Bad Request if the identifier does not match; 404
        /// Not Found if the train does not exist.</returns>
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateTrain(int id, [FromBody] TrainDto trainDto)
        {
            var rowsAffected = await _context.Trains
                .Where(t => t.Id == id)
                .ExecuteUpdateAsync(setters => setters
                    .SetProperty(t => t.Name, trainDto.Name)
                    .SetProperty(t => t.MaxSpeed, trainDto.MaxSpeed)
                    .SetProperty(t => t.Capacity, trainDto.Capacity)
                    .SetProperty(t => t.Type, trainDto.Type)
                );

            if (rowsAffected == 0)
                return NotFound();

            return NoContent();
        }

        /// <summary>
        /// Deletes the train with the specified identifier.
        /// </summary>
        /// <param name="id">The unique identifier of the train to delete.</param>
        /// <returns>A 204 No Content response if the train was deleted; otherwise, a 404 Not Found response if the train does
        /// not exist.</returns>
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteTrain(int id)
        {
            var rowsAffected = await _context.Trains
                .Where(t => t.Id == id)
                .ExecuteDeleteAsync();

            if (rowsAffected == 0)
                return NotFound();

            return NoContent();
        }
    }
}
