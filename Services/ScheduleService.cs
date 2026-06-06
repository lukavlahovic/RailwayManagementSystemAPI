using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.EntityFrameworkCore;
using RailwayManagementSystemAPI.Data;
using RailwayManagementSystemAPI.Dtos;
using RailwayManagementSystemAPI.Exceptions;
using RailwayManagementSystemAPI.Models;

namespace RailwayManagementSystemAPI.Services
{
    public class ScheduleService : IScheduleService
    {
        private readonly RailwayContext _context;
        private readonly IMapper _mapper;
        private readonly ILogger<ScheduleService> _logger;

        public ScheduleService(RailwayContext context, IMapper mapper, ILogger<ScheduleService> logger)
        {
            _context = context;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<ScheduleResponseDto> CreateScheduleAsync(CreateScheduleDto dto)
        {
            var trainExists = await _context.Trains
                .AnyAsync(t => t.Id == dto.TrainId);

            if (!trainExists)
            {
                _logger.LogWarning("Train with id {TrainId} does not exist", dto.TrainId);
                throw new BadRequestException("Invalid TrainId");
            }

            var routeExists = await _context.Routes
                .AnyAsync(r => r.Id == dto.RouteId);

            if (!routeExists)
            {
                _logger.LogWarning("Route with id {RouteId} does not exist", dto.RouteId);
                throw new BadRequestException("Invalid RouteId");
            }

            var schedule = _mapper.Map<Schedule>(dto);

            await _context.Schedules.AddAsync(schedule);
            await _context.SaveChangesAsync();

            return await GetScheduleByIdAsync(schedule.Id);
        }

        public async Task DeleteScheduleAsync(int id)
        {
            var rowsAffected = await _context.Schedules
                .Where(s => s.Id == id)
                .ExecuteDeleteAsync();

            if (rowsAffected == 0)
            {
                _logger.LogWarning("Schedule with id {ScheduleId} was not found for deletion", id);
                throw new NotFoundException($"Schedule with id {id} not found");
            }

            _logger.LogInformation("Schedule with id {ScheduleId} was deleted", id);
        }

        public async Task<PagedResult<ScheduleResponseDto>> GetAllSchedulesAsync(PaginationQuery query)
        {
            query.Page = query.Page < 1 ? 1 : query.Page;
            query.PageSize = query.PageSize > 50 ? 50 : query.PageSize;

            var totalCount = await _context.Schedules.CountAsync();

            var schedules = await _context.Schedules
                .Skip((query.Page - 1)*query.PageSize)
                .Take(query.PageSize)
                .ProjectTo<ScheduleResponseDto>(_mapper.ConfigurationProvider)
                .ToListAsync();

            var response = new PagedResult<ScheduleResponseDto>
            {
                Items = schedules,
                TotalCount = totalCount,
                Page = query.Page,
                PageSize = query.PageSize,
            };

            return response;
        }

        public async Task<ScheduleResponseDto> GetScheduleByIdAsync(int id)
        {
            var schedule = await _context.Schedules
                .ProjectTo<ScheduleResponseDto>(_mapper.ConfigurationProvider)
                .FirstOrDefaultAsync(s => s.Id == id);

            if (schedule == null)
            {
                _logger.LogWarning("Schedule with id {ScheduleId} was not found", id);
                throw new NotFoundException($"Schedule with id {id} not found");
            }

            return schedule;
        }

        public async Task UpdateScheduleAsync(int id, CreateScheduleDto dto)
        {
            var trainExists = await _context.Trains
                .AnyAsync(t => t.Id == dto.TrainId);

            if (!trainExists)
            {
                _logger.LogWarning("Train with id {TrainId} does not exist", dto.TrainId);
                throw new BadRequestException($"TrainId {dto.TrainId} does not exist!");
            }

            var routeExists = await _context.Routes
                .AnyAsync(r => r.Id == dto.RouteId);

            if (!routeExists)
            {
                _logger.LogWarning("Route with id {RouteId} does not exist", dto.RouteId);
                throw new BadRequestException($"RouteId {dto.RouteId} does not exist!");
            }

            var rowsAffected = await _context.Schedules
                .Where(s => s.Id == id)
                .ExecuteUpdateAsync(setter => setter
                    .SetProperty(s => s.TrainId, dto.TrainId)
                    .SetProperty(s => s.RouteId, dto.RouteId)
                    .SetProperty(s => s.DepartureTime, dto.DepartureTime)
                    .SetProperty(s => s.ScheduleType, dto.ScheduleType)
                    .SetProperty(s => s.ValidFrom, dto.ValidFrom)
                    .SetProperty(s => s.ValidTo, dto.ValidTo)
                );

            if(rowsAffected == 0)
            {
                _logger.LogWarning("Schedule with id {ScheduleId} was not updated", id);
                throw new NotFoundException($"Schedule with id {id} not found");
            }

            _logger.LogInformation("Schedule with id {ScheduleId} was updated", id);
        }

        public async Task ToggleActiveAsync(int id)
        {
            var rowsAffected = await _context.Schedules
                .Where(s => s.Id == id)
                .ExecuteUpdateAsync(setter => setter
                    .SetProperty(s => s.IsActive, s => !s.IsActive)
                );

            if (rowsAffected == 0)
            {
                _logger.LogWarning("Schedule with id {ScheduleId} was not found for toggle", id);
                throw new NotFoundException($"Schedule with id {id} not found");
            }

            _logger.LogInformation("Schedule with id {ScheduleId} active status was toggled", id);
        }
    }
}