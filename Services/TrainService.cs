using Microsoft.EntityFrameworkCore;
using RailwayManagementSystemAPI.Data;
using RailwayManagementSystemAPI.Dtos;
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

        public async Task<(TrainResponseDto? train, string? error)> CreateTrainAsync(CreateTrainDto dto)
        {
            var exist = await _context.TrainTypes
                .AnyAsync(tt => tt.Id == dto.TrainTypeId);

            if (!exist)
                return (null, $"TrainTypeId {dto.TrainTypeId} does not exist!");

            var train = new Train
            {
                SerialNumber = dto.SerialNumber,
                TrainTypeId = dto.TrainTypeId
            };

            await _context.Trains.AddAsync(train);
            await _context.SaveChangesAsync();

            var resonse = await GetTrainByIdAsync(train.Id);
            return (resonse, null);
        }

        public async Task<bool> DeleteTrainAsync(int id)
        {
            var rowsAffected = await _context.Trains
                .Where(t => t.Id == id)
                .ExecuteDeleteAsync();

            return rowsAffected > 0;
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

        public async Task<TrainResponseDto?> GetTrainByIdAsync(int id)
        {
            return await _context.Trains
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
        }

        public async Task<string?> UpdateTrainAsync(int id, CreateTrainDto dto)
        {
            var exist = await _context.TrainTypes
                .AnyAsync(tt => tt.Id == dto.TrainTypeId);

            if (!exist)
                return $"TrainTypeId {dto.TrainTypeId} does not exist!";

            var rowsAffected = await _context.Trains
                .Where(t => t.Id == id)
                .ExecuteUpdateAsync(setters => setters
                    .SetProperty(t => t.TrainTypeId, dto.TrainTypeId)
                    .SetProperty(t => t.SerialNumber, dto.SerialNumber)
                );

            if (rowsAffected == 0)
                return "NotFound";

            return null;
        }
    }
}
