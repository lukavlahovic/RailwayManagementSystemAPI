using AutoMapper;
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

        public TrainTypeService(RailwayContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<TrainTypeResponseDto> CreateTrainTypeAsync(CreateTrainTypeDto dto)
        {
            var trainType = _mapper.Map<TrainType>(dto);

            await _context.TrainTypes.AddAsync(trainType);
            await _context.SaveChangesAsync();

            return _mapper.Map<TrainTypeResponseDto>(trainType);
        }

        public async Task<List<TrainTypeResponseDto>> GetAllTrainTypesAsync()
        {
            var trainTypes = await _context.TrainTypes.ToListAsync();

            return _mapper.Map<List<TrainTypeResponseDto>>(trainTypes);
        }

        public async Task<TrainTypeResponseDto> GetTrainTypeByIdAsync(int id)
        {
            var trainType = await _context.TrainTypes.FindAsync(id);

            if (trainType == null)
                throw new NotFoundException($"TrainType with id {id} not found");

            return _mapper.Map<TrainTypeResponseDto>(trainType);
        }
    }
}
