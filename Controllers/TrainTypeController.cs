using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RailwayManagementSystemAPI.Data;
using RailwayManagementSystemAPI.Dtos;
using RailwayManagementSystemAPI.Models;

namespace RailwayManagementSystemAPI.Controllers
{
    [ApiController]
    [Route("api/train-types")]
    public class TrainTypeController : ControllerBase
    {
        private readonly RailwayContext _context;

        public TrainTypeController(RailwayContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Creates a new train type and returns the created resource.
        /// </summary>
        /// <param name="dto">The data transfer object containing information for the new train type.</param>
        /// <returns>A response with the created train type and a location header.</returns>
        [HttpPost]
        public async Task<IActionResult> CreateTrainType([FromBody] CreateTrainTypeDto dto)
        {
            var trainType = new TrainType
            {
                Name = dto.Name,
                MaxSpeed = dto.MaxSpeed,
                Capacity = dto.Capacity,
                Manufacturer = dto.Manufacturer
            };

            await _context.TrainTypes.AddAsync(trainType);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetTrainTypeById), new { id = trainType.Id }, trainType);
        }

        /// <summary>
        /// Retrieves all train types from the database.
        /// </summary>
        /// <returns>An IActionResult containing a list of TrainTypeResponseDto objects.</returns>
        [HttpGet]
        public async Task<IActionResult> GetAllTrainTypes()
        {
            var types = await _context.TrainTypes
                .Select(tt => new TrainTypeResponseDto
                {
                    Id = tt.Id,
                    Name = tt.Name,
                    MaxSpeed = tt.MaxSpeed,
                    Capacity = tt.Capacity,
                    Manufacturer = tt.Manufacturer,
                    TypeOfTrain = tt.Type
                })
                .ToListAsync();

            return Ok(types);
        }

        /// <summary>
        /// Retrieves a train type by its unique identifier.
        /// </summary>
        /// <param name="id">The unique identifier of the train type.</param>
        /// <returns>An IActionResult containing the train type data if found; otherwise, a NotFound result.</returns>
        [HttpGet("{id}")]
        public async Task<IActionResult> GetTrainTypeById(int id)
        {
            var type = await _context.TrainTypes
                .Where(tt => tt.Id == id)
                .Select(tt => new TrainTypeResponseDto
                {
                    Id = tt.Id,
                    Name = tt.Name,
                    MaxSpeed = tt.MaxSpeed,
                    Capacity = tt.Capacity,
                    Manufacturer = tt.Manufacturer,
                    TypeOfTrain = tt.Type
                })
                .FirstOrDefaultAsync();

            if (type == null)
                return NotFound();

            return Ok(type);
        }
    }
}
