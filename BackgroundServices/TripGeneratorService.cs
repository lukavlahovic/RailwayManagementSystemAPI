using Microsoft.EntityFrameworkCore;
using RailwayManagementSystemAPI.Data;
using RailwayManagementSystemAPI.Models;

namespace RailwayManagementSystemAPI.BackgroundServices
{
    public class TripGeneratorService : BackgroundService
    {
        private readonly IServiceScopeFactory _scopeFactory;
        private readonly ILogger<TripGeneratorService> _logger;

        public TripGeneratorService(IServiceScopeFactory scopeFactory, ILogger<TripGeneratorService> logger)
        {
            _scopeFactory = scopeFactory;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("Trip Generator Service started");

            while (!stoppingToken.IsCancellationRequested)
            {
                var now = DateTime.Now;
                // calculate time until next midnight
                var nextMidnight = DateTime.Today.AddDays(1);
                var delay = nextMidnight - now;

                _logger.LogInformation("Next trip generation scheduled at {NextMidnight}", nextMidnight);

                await Task.Delay(delay, stoppingToken);

                await GenerateTripsForTomorrow();
            }
        }

        private async Task GenerateTripsForTomorrow()
        {
            using var scope = _scopeFactory.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<RailwayContext>();

            var tomorrow = DateTime.Today.AddDays(1);
            var dayOfWeek = tomorrow.DayOfWeek;

            _logger.LogInformation("Generating trips for {Date}", tomorrow.ToString("yyyy-MM-dd"));

            // determine which schedule types apply to tomorrow
            var applicableTypes = new List<ScheduleType> { ScheduleType.Daily };

            if (dayOfWeek == DayOfWeek.Saturday || dayOfWeek == DayOfWeek.Sunday)
                applicableTypes.Add(ScheduleType.Weekend);
            else
                applicableTypes.Add(ScheduleType.Workday);

            // get active schedules that apply to tomorrow
            var schedules = await context.Schedules
                .Include(s => s.Route)
                    .ThenInclude(r => r.RouteStations)
                .Where(s => s.IsActive)
                .Where(s => s.ValidFrom.Date <= tomorrow)
                .Where(s => s.ValidTo == null || s.ValidTo.Value.Date >= tomorrow)
                .Where(s => applicableTypes.Contains(s.ScheduleType))
                .ToListAsync();

            _logger.LogInformation("Found {Count} active schedules for {Date}", 
                schedules.Count, tomorrow.ToString("yyyy-MM-dd"));

            var tripsCreated = 0;

            foreach (var schedule in schedules)
            {
                // check if trip already exists for this schedule on this date
                // avoids duplicates if service runs twice
                var tripExists = await context.Trip
                    .AnyAsync(t => 
                        t.TrainId == schedule.TrainId &&
                        t.RouteId == schedule.RouteId &&
                        t.DepartureTime.Date == tomorrow);

                if (tripExists)
                {
                    _logger.LogInformation("Trip already exists for schedule {ScheduleId} on {Date}", 
                        schedule.Id, tomorrow.ToString("yyyy-MM-dd"));
                    continue;
                }

                // calculate arrival time from last station offset
                var lastStation = schedule.Route.RouteStations
                    .OrderByDescending(rs => rs.Order)
                    .First();

                var departureTime = tomorrow.Add(schedule.DepartureTime);
                var arrivalTime = departureTime.AddMinutes(lastStation.ArrivalOffsetMinutes);

                var trip = new Trip
                {
                    TrainId = schedule.TrainId,
                    RouteId = schedule.RouteId,
                    DepartureTime = departureTime,
                    ArrivalTime = arrivalTime
                };

                await context.Trip.AddAsync(trip);
                tripsCreated++;
            }

            await context.SaveChangesAsync();

            _logger.LogInformation("Generated {Count} trips for {Date}", 
                tripsCreated, tomorrow.ToString("yyyy-MM-dd"));
        }
    }
}