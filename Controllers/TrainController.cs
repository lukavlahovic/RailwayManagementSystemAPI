using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RailwayManagementSystemAPI.Data;
using RailwayManagementSystemAPI.Dtos;
using RailwayManagementSystemAPI.Models;

namespace RailwayManagementSystemAPI.Controllers
{
    [ApiController]
    [Route("api/trains")]
    public class TrainController : ControllerBase
    {
        private readonly RailwayContext _context;

        public TrainController(RailwayContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Retrieves all trains with their associated train type details.
        /// </summary>
        /// <returns>An IActionResult containing a list of TrainResponseDto objects.</returns>
        [HttpGet]
        public async Task<IActionResult> GetAllTrains()
        {
            var trains = await _context.Trains
                .Select(t => new TrainResponseDto
                {
                    Id = t.Id,
                    SerialNumber = t.SerialNumber,
                    TrainType = new TrainTypeResponseDto
                    {
                        Id = t.TrainType.Id,
                        Name = t.TrainType.Name,
                        MaxSpeed = t.TrainType.MaxSpeed,
                        Capacity = t.TrainType.Capacity,
                        Manufacturer = t.TrainType.Manufacturer,
                        TypeOfTrain = t.TrainType.Type
                    }
                })
                .ToListAsync();

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
            var train = await _context.Trains
                .Select(t => new TrainResponseDto
                {
                    Id = t.Id,
                    SerialNumber = t.SerialNumber,
                    TrainType = new TrainTypeResponseDto
                    {
                        Id = t.TrainType.Id,
                        Name = t.TrainType.Name,
                        MaxSpeed = t.TrainType.MaxSpeed,
                        Capacity = t.TrainType.Capacity,
                        Manufacturer = t.TrainType.Manufacturer,
                        TypeOfTrain = t.TrainType.Type
                    }
                })
                .FirstOrDefaultAsync(t => t.Id == id);

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
            var exist = await _context.TrainTypes
                .AnyAsync(tt => tt.Id == trainDto.TrainTypeId);

            if (!exist)
                return BadRequest($"TrainTypeId {trainDto.TrainTypeId} does not exist!");

            var train = new Train
            {
                SerialNumber = trainDto.SerialNumber,
                TrainTypeId = trainDto.TrainTypeId
            };

            await _context.Trains.AddAsync(train);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetTrainById), new { id = train.Id }, train);
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
            var exist = await _context.TrainTypes
                .AnyAsync(tt => tt.Id == trainDto.TrainTypeId);

            if (!exist)
                return BadRequest($"TrainTypeId {trainDto.TrainTypeId} does not exist!");

            var rowsAffected = await _context.Trains
                .Where(t => t.Id == id)
                .ExecuteUpdateAsync(setters => setters
                    .SetProperty(t => t.TrainTypeId, trainDto.TrainTypeId)
                    .SetProperty(t => t.SerialNumber, trainDto.SerialNumber)
                );

            if (rowsAffected == 0)
                return NotFound();

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
            var rowsAffected = await _context.Trains
                .Where(t => t.Id == id)
                .ExecuteDeleteAsync();

            if (rowsAffected == 0)
                return NotFound();

            return NoContent();
        }
    }
}
