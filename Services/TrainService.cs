using AutoMapper;
using AutoMapper.QueryableExtensions;
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
        private readonly IMapper _mapper;

        public TrainService(RailwayContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<TrainResponseDto> CreateTrainAsync(CreateTrainDto dto)
        {
            var trainTypeExists = await _context.TrainTypes
                .AnyAsync(tt => tt.Id == dto.TrainTypeId);

            if (!trainTypeExists)
                throw new BadRequestException($"TrainTypeId {dto.TrainTypeId} does not exist!");

            var train = _mapper.Map<Train>(dto);

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
                throw new NotFoundException($"Train with id {id} not found");
        }

        public async Task<List<TrainResponseDto>> GetAllTrainsAsync()
        {
            return await _context.Trains
                .ProjectTo<TrainResponseDto>(_mapper.ConfigurationProvider)
                .ToListAsync();
        }

        public async Task<TrainResponseDto> GetTrainByIdAsync(int id)
        {
            var train = await _context.Trains
                .ProjectTo<TrainResponseDto>(_mapper.ConfigurationProvider)
                .FirstOrDefaultAsync(t => t.Id == id);

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
                throw new NotFoundException($"Train with id {id} not found");
        }
    }
}
