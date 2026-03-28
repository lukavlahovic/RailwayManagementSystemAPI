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
                    TypeOfTrain = tt.Type
                })
                .ToListAsync();

            return Ok(types);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetTrainTypeById(int id)
        {
            var type = await _context.TrainTypes
                .Select(tt => new TrainTypeResponseDto
                {
                    Id = tt.Id,
                    Name = tt.Name,
                    MaxSpeed = tt.MaxSpeed,
                    Capacity = tt.Capacity,
                    TypeOfTrain = tt.Type
                })
                .FirstOrDefaultAsync(tt => tt.Id == id);

            if (type == null)
                return NotFound();

            return Ok(type);
        }
    }
}
