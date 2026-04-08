using Microsoft.EntityFrameworkCore;
using RailwayManagementSystemAPI.Data;
using RailwayManagementSystemAPI.Dtos;
using RailwayManagementSystemAPI.Exceptions;
using RailwayManagementSystemAPI.Models;

namespace RailwayManagementSystemAPI.Services
{
    public class TrainTypeService : ITrainTypeService
    {
        private readonly RailwayContext _context;

        public TrainTypeService(RailwayContext context)
        {
            _context = context;
        }

        public async Task<TrainTypeResponseDto> CreateTrainTypeAsync(CreateTrainTypeDto dto)
        {
            var trainType = new TrainType
            {
                Name = dto.Name,
                MaxSpeed = dto.MaxSpeed,
                Capacity = dto.Capacity,
                Manufacturer = dto.Manufacturer,
                Type = dto.Type
            };

            await _context.TrainTypes.AddAsync(trainType);
            await _context.SaveChangesAsync();

            return new TrainTypeResponseDto
            {
                Id = trainType.Id,
                Name = trainType.Name,
                MaxSpeed = trainType.MaxSpeed,
                Capacity = trainType.Capacity,
                Manufacturer = trainType.Manufacturer,
                TypeOfTrain = trainType.Type
            };
        }

        public async Task<List<TrainTypeResponseDto>> GetAllTrainTypesAsync()
        {
            return await _context.TrainTypes
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
        }

        public async Task<TrainTypeResponseDto> GetTrainTypeByIdAsync(int id)
        {
            var trainType = await _context.TrainTypes
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

            if (trainType == null)
                throw new NotFoundException($"TrainType with id {id} not found");

            return trainType;
        }
    }
}
