using Microsoft.EntityFrameworkCore;
using RailwayManagementSystemAPI.Data;
using RailwayManagementSystemAPI.Dtos;
using RailwayManagementSystemAPI.Exceptions;
using RailwayManagementSystemAPI.Models;

namespace RailwayManagementSystemAPI.Services
{
    public class TrainService : ITrainService
    {
        private readonly RailwayContext _context;

        public TrainService(RailwayContext context)
        {
            _context = context;
        }

        public async Task<TrainResponseDto> CreateTrainAsync(CreateTrainDto dto)
        {
            var trainTypeExists = await _context.TrainTypes
                .AnyAsync(tt => tt.Id == dto.TrainTypeId);

            if (!trainTypeExists)
                throw new BadRequestException($"TrainTypeId {dto.TrainTypeId} does not exist!");

            var train = new Train
            {
                SerialNumber = dto.SerialNumber,
                TrainTypeId = dto.TrainTypeId
            };

            await _context.Trains.AddAsync(train);
            await _context.SaveChangesAsync();

            return await GetTrainByIdAsync(train.Id);
        }

        public async Task DeleteTrainAsync(int id)
        {
            var rowsAffected = await _context.Trains
                .Where(t => t.Id == id)
                .ExecuteDeleteAsync();

            if (rowsAffected == 0)
                throw new NotFoundException("NotFound");
        }

        public async Task<List<TrainResponseDto>> GetAllTrainsAsync()
        {
            return await _context.Trains
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
        }

        public async Task<TrainResponseDto> GetTrainByIdAsync(int id)
        {
            var train = await _context.Trains
                .Where(t => t.Id == id)
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
                .FirstOrDefaultAsync();

            if (train == null)
                throw new NotFoundException($"Train with id {id} not found");

            return train;
        }

        public async Task UpdateTrainAsync(int id, CreateTrainDto dto)
        {
            var trainTypeExists = await _context.TrainTypes
                .AnyAsync(tt => tt.Id == dto.TrainTypeId);

            if (!trainTypeExists)
                throw new BadRequestException($"TrainTypeId {dto.TrainTypeId} does not exist!");

            var rowsAffected = await _context.Trains
                .Where(t => t.Id == id)
                .ExecuteUpdateAsync(setters => setters
                    .SetProperty(t => t.TrainTypeId, dto.TrainTypeId)
                    .SetProperty(t => t.SerialNumber, dto.SerialNumber)
                );

            if (rowsAffected == 0)
                throw new NotFoundException("NotFound");
        }
    }
}
