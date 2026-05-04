using AutoMapper;
using AutoMapper.QueryableExtensions;
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
        private readonly IMapper _mapper;
        private readonly ILogger<TrainTypeService> _logger;

        public TrainTypeService(RailwayContext context, IMapper mapper, ILogger<TrainTypeService> logger)
        {
            _context = context;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<TrainTypeResponseDto> CreateTrainTypeAsync(CreateTrainTypeDto dto)
        {
            var trainType = _mapper.Map<TrainType>(dto);

            await _context.TrainTypes.AddAsync(trainType);
            await _context.SaveChangesAsync();

            _logger.LogInformation("TrainType with id {TrainTypeId} was created", trainType.Id);

            return _mapper.Map<TrainTypeResponseDto>(trainType);
        }

        public async Task<List<TrainTypeResponseDto>> GetAllTrainTypesAsync()
        {
            return await _context.TrainTypes
                .ProjectTo<TrainTypeResponseDto>(_mapper.ConfigurationProvider)
                .ToListAsync();
        }

        public async Task<TrainTypeResponseDto> GetTrainTypeByIdAsync(int id)
        {
            var trainType = await _context.TrainTypes
                .Where(tt => tt.Id == id)
                .ProjectTo<TrainTypeResponseDto>(_mapper.ConfigurationProvider)
                .FirstOrDefaultAsync();

            if (trainType == null)
            {
                _logger.LogWarning("TrainType with id {TrainTypeId} was not found", id);
                throw new NotFoundException($"TrainType with id {id} not found");
            }

            return trainType;
        }
    }
}
