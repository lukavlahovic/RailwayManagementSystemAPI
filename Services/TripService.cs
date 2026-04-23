using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.EntityFrameworkCore;
using RailwayManagementSystemAPI.Data;
using RailwayManagementSystemAPI.Dtos;
using RailwayManagementSystemAPI.Exceptions;
using RailwayManagementSystemAPI.Models;

namespace RailwayManagementSystemAPI.Services
{
    public class TripService : ITripService
    {
        private readonly RailwayContext _context;
        private readonly IMapper _mapper;

        public TripService(RailwayContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<TripResponseDto> CreateTrip(CreateTripDto dto)
        {
            if (dto.DepartureTime >= dto.ArrivalTime)
                throw new BadRequestException("Arrival time must be after departure time!");

            var trainExists = await _context.Trains.AnyAsync(t => t.Id == dto.TrainId);

            if (!trainExists)
                throw new BadRequestException("Invalid TrainId");

            var routeExists = await _context.Routes.AnyAsync(r => r.Id == dto.RouteId);

            if (!routeExists)
                throw new BadRequestException("Invalid RouteId");

            var trip = _mapper.Map<Trip>(dto);

            await _context.Trip.AddAsync(trip);
            await _context.SaveChangesAsync();

            return await GetTripById(trip.Id);
        }

        public async Task<List<StationScheduleDto>> GetStationSchedule(int stationId)
        {
            var stationExists = await _context.Stations.AnyAsync(s => s.Id == stationId);
            if (!stationExists)
                throw new NotFoundException($"Station with id {stationId} not found");

            var now = DateTime.Now;

            var schedule = await _context.Trip
                .Where(t => t.ArrivalTime > now)
                .Where(t => t.Route.RouteStations
                    .Any(rs => rs.StationId == stationId))
                .Select(t => new
                {
                    t.Id,
                    Train = t.Train.SerialNumber,
                    Route = t.Route.Name,
                    t.DepartureTime,
                    Station = t.Route.RouteStations
                        .Where(rs => rs.StationId == stationId)
                        .Select(rs => new
                        {
                            rs.Order,
                            rs.ArrivalOffsetMinutes

                        })
                        .FirstOrDefault(),
                    AllStations = t.Route.RouteStations
                        .Select(rs => new
                        {
                            rs.Order,
                            rs.StationId
                        })
                        .ToList()
                })
                .ToListAsync();

            var delays = await _context.Delays
                .Where(d => schedule.Select(t => t.Id).Contains(d.TripId))
                .ToListAsync();

            var result = schedule
                .Select(s =>
                {
                    var plannedArrival = s.DepartureTime.AddMinutes(s.Station!.ArrivalOffsetMinutes);

                    // taking to account only stations before the current station as well as the current station it self
                    var validStationIds = s.AllStations
                        .Where(s1 => s1.Order <= s.Station.Order)
                        .Select(s1 => s1.StationId)
                        .ToList();

                    //calculating delay
                    var totalDelay = delays
                        .Where(d => d.TripId == s.Id && validStationIds.Contains(d.StationId))
                        .Sum(d => d.DelayMinutes);

                    //delay for the station
                    var realArrival = plannedArrival.AddMinutes(totalDelay);
                    var minutes = (realArrival - now).TotalMinutes;

                    return new StationScheduleDto
                    {
                        Train = s.Train,
                        Route = s.Route,
                        PlannedArrivalTime = plannedArrival,
                        RealArrivalTime = realArrival,
                        TotalDelayMinutes = totalDelay,
                        MinutesUntilArrival = minutes
                    };
                })
                .Where(x => x.MinutesUntilArrival >= -5) // show departed train for 5 minutes
                .OrderBy(x => x.RealArrivalTime)
                .ToList();

            return result;
        }

        public async Task<TripResponseDto> GetTripById(int id)
        {
            var trip = await _context.Trip
                .Where(t => t.Id == id)
                .ProjectTo<TripResponseDto>(_mapper.ConfigurationProvider)
                .FirstOrDefaultAsync();

            if (trip == null)
                throw new NotFoundException($"Trip with id {id} not found");

            return trip;
        }

        public async Task<List<TripScheduleDto>> GetTripsByDate(DateTime date)
        {
            var trips = await _context.Trip
                .Where(t => t.DepartureTime.Date == date.Date)
                .OrderBy(t => t.DepartureTime)
                .ProjectTo<TripScheduleDto>(_mapper.ConfigurationProvider)
                .ToListAsync();

            return trips;
        }

        public async Task<List<TripScheduleDto>> GetTripsByStation(int stationId)
        {
            var stationExists = await _context.Stations.FindAsync(stationId);
            if (stationExists == null)
                throw new NotFoundException($"Station with id {stationId} not found");

            var trips = await _context.Trip
                .Where(t => t.Route.RouteStations
                    .Any(rs => rs.StationId == stationId))
                .OrderBy(t => t.DepartureTime)
                .ProjectTo<TripScheduleDto>(_mapper.ConfigurationProvider)
                .ToListAsync();

            return trips;
        }

        public async Task<PagedResult<TripSearchResponseDto>> SearchTrips(TripSearchQuery query)
        {
            var now = DateTime.Now;

            query.Page = query.Page < 1 ? 1 : query.Page;
            query.PageSize = query.PageSize > 50 ? 50 : query.PageSize;

            //making a query for search
            var tripsQuery = _context.Trip
                .Where(t => t.ArrivalTime > now)
                .AsQueryable();

            if (!query.FromStationId.HasValue && !query.ToStationId.HasValue)
                throw new BadRequestException("At least one of FromStationId or ToStationId must be provided.");

            if (query.FromStationId.HasValue)
            {
                if (query.FromStationId > 0)
                {
                    tripsQuery = tripsQuery
                        .Where(t => t.Route.RouteStations
                            .Any(rs => rs.StationId == query.FromStationId));
                }
                else
                    throw new BadRequestException("Invalid FromStationId");
            }

            if (query.ToStationId.HasValue)
            {
                if (query.ToStationId > 0)
                {
                    tripsQuery = tripsQuery
                        .Where(t => t.Route.RouteStations
                            .Any(rs => rs.StationId == query.ToStationId));
                }
                else
                    throw new BadRequestException("Invalid ToStationId");
            }

            if (query.Date.HasValue && query.Date.Value.Date < DateTime.Today)
                throw new BadRequestException("Date cannot be in the past.");

            if (query.Date.HasValue)
            {
                tripsQuery = tripsQuery
                    .Where(t => t.DepartureTime.Date == query.Date.Value.Date);
            }

            if (query.MinDepartureTime.HasValue && query.MaxDepartureTime.HasValue)
            {
                if (query.MinDepartureTime > query.MaxDepartureTime)
                    throw new BadRequestException("MinDepartureTime cannot be greater than MaxDepartureTime.");
            }

            if (query.MinDepartureTime.HasValue)
            {
                tripsQuery = tripsQuery
                    .Where(t => t.DepartureTime.TimeOfDay == query.MinDepartureTime);
            }

            if (query.MaxDepartureTime.HasValue)
            {
                tripsQuery = tripsQuery
                    .Where(t => t.DepartureTime.TimeOfDay == query.MaxDepartureTime);
            }

            var totalCount = await tripsQuery.CountAsync();

            var trips = await tripsQuery
                .OrderBy(t => t.DepartureTime)
                .Skip((query.Page - 1) * query.PageSize)
                .Take(query.PageSize)
                .Select(t => new
                {
                    t.Id,
                    t.Train.SerialNumber,
                    RouteName = t.Route.Name,
                    // time of departure from first station
                    t.DepartureTime,
                    FromStation = query.FromStationId != null
                        ? t.Route.RouteStations
                            .Where(rs => rs.StationId == query.FromStationId)
                            .Select(rs => new
                            {
                                rs.Order,
                                // when train is supposed to arrive to a station from first station(DepartureTime), plus the time of stop
                                // the time of departure from that station in minutes
                                DepartureOffset = rs.ArrivalOffsetMinutes + rs.StopDuration
                            }).FirstOrDefault()
                        : null,
                    ToStation = query.ToStationId != null
                        ? t.Route.RouteStations
                            .Where(rs => rs.StationId == query.ToStationId)
                            .Select(rs => new
                            {
                                rs.Order,
                                // time when train is to arrive to a station from the first station in minutes
                                rs.ArrivalOffsetMinutes
                            }).FirstOrDefault()
                        : null
                })
                .ToListAsync();

            var resultItems = trips
                .Where(t =>
                {
                    // if its A -> B
                    if (query.FromStationId.HasValue && query.ToStationId.HasValue)
                        return t.FromStation != null &&
                               t.ToStation != null &&
                               t.FromStation.Order < t.ToStation.Order;

                    //only A
                    if (query.FromStationId.HasValue)
                        return t.FromStation != null;

                    //only B
                    if (query.ToStationId.HasValue)
                        return t.ToStation != null;

                    return true;
                })
                .Select(t =>
                {
                    DateTime? departure = null;
                    DateTime? arrival = null;

                    if (t.FromStation != null)
                        // we get exact time when train leaves a station
                        departure = t.DepartureTime.AddMinutes(t.FromStation.DepartureOffset);

                    if (t.ToStation != null)
                        // we get exact time when train arrives to a station
                        arrival = t.DepartureTime.AddMinutes(t.ToStation.ArrivalOffsetMinutes);

                    //filter for time
                    if (query.MinDepartureTime.HasValue && departure.HasValue)
                    {
                        if (departure.Value.TimeOfDay < query.MinDepartureTime.Value)
                            return null;
                    }

                    if (query.MaxDepartureTime.HasValue && arrival.HasValue)
                        if (arrival.Value.TimeOfDay > query.MaxDepartureTime.Value)
                            return null;

                    return new TripSearchResponseDto
                    {
                        TripId = t.Id,
                        Train = t.SerialNumber,
                        Route = t.RouteName,
                        DepartureFromStationTime = departure,
                        ArrivalToStation = arrival,
                        DurationMinutes = (departure.HasValue && arrival.HasValue)
                            ? (arrival - departure)?.TotalMinutes
                            : null
                    };
                })
                .Where(x => x != null)
                .ToList();

            var response = new PagedResult<TripSearchResponseDto>
            {
                Items = resultItems!,
                TotalCount = totalCount,
                Page = query.Page,
                PageSize = query.PageSize
            };

            return response;
        }
    }
}
