using Microsoft.EntityFrameworkCore;
using RailwayManagementSystemAPI.Data;
using RailwayManagementSystemAPI.Models;
using Route = RailwayManagementSystemAPI.Models.Route;

namespace RailwayManagementSystemAPI.Seeding
{
    public class DatabaseSeeder
    {
        private readonly RailwayContext _context;

        public DatabaseSeeder(RailwayContext context)
        {
            _context = context;
        }

        public async Task SeedAsync()
        {
            // apply any pending migrations automatically
            await _context.Database.MigrateAsync();

            await SeedTrainTypesAsync();
            await SeedStationsAsync();
            await SeedTrainsAsync();
            await SeedRoutesAsync();
            await SeedTripsAsync();
            await SeedDelaysAsync();
            await SeedSchedulesAsync();
        }

        private async Task SeedTrainTypesAsync()
        {
            if (await _context.TrainTypes.AnyAsync())
                return;

            var trainTypes = new List<TrainType>
            {
                new TrainType
                {
                    Name = "ICE 3",
                    MaxSpeed = 330,
                    Capacity = 400,
                    Manufacturer = "Siemens",
                    Type = TypeOfTrain.HighSpeed
                },
                new TrainType
                {
                    Name = "Coradia Stream",
                    MaxSpeed = 200,
                    Capacity = 300,
                    Manufacturer = "Alstom",
                    Type = TypeOfTrain.Passenger
                },
                new TrainType
                {
                    Name = "Flirt 3",
                    MaxSpeed = 160,
                    Capacity = 200,
                    Manufacturer = "Stadler",
                    Type = TypeOfTrain.Commuter
                },
                new TrainType
                {
                    Name = "Vectron",
                    MaxSpeed = 160,
                    Capacity = 0,
                    Manufacturer = "Siemens",
                    Type = TypeOfTrain.Freight
                }
            };

            await _context.TrainTypes.AddRangeAsync(trainTypes);
            await _context.SaveChangesAsync();
        }

        private async Task SeedStationsAsync()
        {
            if (await _context.Stations.AnyAsync())
                return;

            var stations = new List<Station>
            {
                new Station
                {
                    Name = "Belgrade Center",
                    City = "Belgrade",
                    Country = "Serbia",
                    NumberOfPlatforms = 10
                },
                new Station
                {
                    Name = "Novi Sad",
                    City = "Novi Sad",
                    Country = "Serbia",
                    NumberOfPlatforms = 6
                },
                new Station
                {
                    Name = "Subotica",
                    City = "Subotica",
                    Country = "Serbia",
                    NumberOfPlatforms = 4
                },
                new Station
                {
                    Name = "Nis",
                    City = "Nis",
                    Country = "Serbia",
                    NumberOfPlatforms = 5
                },
                new Station
                {
                    Name = "Novi Beograd",
                    City = "Belgrade",
                    Country = "Serbia",
                    NumberOfPlatforms = 3
                }
            };

            await _context.Stations.AddRangeAsync(stations);
            await _context.SaveChangesAsync();
        }

        private async Task SeedTrainsAsync()
        {
            if (await _context.Trains.AnyAsync())
                return;

            var highSpeedType = await _context.TrainTypes
                .FirstAsync(tt => tt.Type == TypeOfTrain.HighSpeed);
            var passengerType = await _context.TrainTypes
                .FirstAsync(tt => tt.Type == TypeOfTrain.Passenger);
            var commuterType = await _context.TrainTypes
                .FirstAsync(tt => tt.Type == TypeOfTrain.Commuter);

            var trains = new List<Train>
            {
                new Train { SerialNumber = "SRB-HS-001", TrainTypeId = highSpeedType.Id },
                new Train { SerialNumber = "SRB-HS-002", TrainTypeId = highSpeedType.Id },
                new Train { SerialNumber = "SRB-PS-001", TrainTypeId = passengerType.Id },
                new Train { SerialNumber = "SRB-PS-002", TrainTypeId = passengerType.Id },
                new Train { SerialNumber = "SRB-CM-001", TrainTypeId = commuterType.Id }
            };

            await _context.Trains.AddRangeAsync(trains);
            await _context.SaveChangesAsync();
        }

        private async Task SeedRoutesAsync()
        {
            if (await _context.Routes.AnyAsync())
                return;

            var belgrade = await _context.Stations.FirstAsync(s => s.Name == "Belgrade Center");
            var noviBeograd = await _context.Stations.FirstAsync(s => s.Name == "Novi Beograd");
            var noviSad = await _context.Stations.FirstAsync(s => s.Name == "Novi Sad");
            var subotica = await _context.Stations.FirstAsync(s => s.Name == "Subotica");
            var nis = await _context.Stations.FirstAsync(s => s.Name == "Nis");

            var routes = new List<Route>
            {
                new Route
                {
                    Name = "Belgrade - Subotica",
                    RouteStations = new List<RouteStation>
                    {
                        new RouteStation
                        {
                            StationId = belgrade.Id,
                            Order = 1,
                            ArrivalOffsetMinutes = 0,
                            StopDuration = 10
                        },
                        new RouteStation
                        {
                            StationId = noviSad.Id,
                            Order = 2,
                            ArrivalOffsetMinutes = 45,
                            StopDuration = 5
                        },
                        new RouteStation
                        {
                            StationId = subotica.Id,
                            Order = 3,
                            ArrivalOffsetMinutes = 120,
                            StopDuration = 10
                        }
                    }
                },
                new Route
                {
                    Name = "Belgrade - Nis",
                    RouteStations = new List<RouteStation>
                    {
                        new RouteStation
                        {
                            StationId = belgrade.Id,
                            Order = 1,
                            ArrivalOffsetMinutes = 0,
                            StopDuration = 10
                        },
                        new RouteStation
                        {
                            StationId = nis.Id,
                            Order = 2,
                            ArrivalOffsetMinutes = 180,
                            StopDuration = 10
                        }
                    }
                },
                new Route
                {
                    Name = "Belgrade - Novi Sad",
                    RouteStations = new List<RouteStation>
                    {
                        new RouteStation
                        {
                            StationId = belgrade.Id,
                            Order = 1,
                            ArrivalOffsetMinutes = 0,
                            StopDuration = 5
                        },
                        new RouteStation
                        {
                            StationId = noviBeograd.Id,
                            Order = 2,
                            ArrivalOffsetMinutes = 15,
                            StopDuration = 5
                        },
                        new RouteStation
                        {
                            StationId = noviSad.Id,
                            Order = 3,
                            ArrivalOffsetMinutes = 60,
                            StopDuration = 5
                        }
                    }
                }
            };

            await _context.Routes.AddRangeAsync(routes);
            await _context.SaveChangesAsync();
        }

        private async Task SeedTripsAsync()
        {
            if (await _context.Trip.AnyAsync())
                return;

            var routeBelgradeSubotica = await _context.Routes
                .FirstAsync(r => r.Name == "Belgrade - Subotica");
            var routeBelgradeNis = await _context.Routes
                .FirstAsync(r => r.Name == "Belgrade - Nis");
            var routeBelgradeNS = await _context.Routes
                .FirstAsync(r => r.Name == "Belgrade - Novi Sad");

            var highSpeedTrain1 = await _context.Trains
                .FirstAsync(t => t.SerialNumber == "SRB-HS-001");
            var highSpeedTrain2 = await _context.Trains
                .FirstAsync(t => t.SerialNumber == "SRB-HS-002");
            var passengerTrain1 = await _context.Trains
                .FirstAsync(t => t.SerialNumber == "SRB-PS-001");
            var commuterTrain = await _context.Trains
                .FirstAsync(t => t.SerialNumber == "SRB-CM-001");

            var today = DateTime.Today;

            var trips = new List<Trip>
            {
                // Belgrade - Subotica route — mix of on time and late
                new Trip
                {
                    TrainId = highSpeedTrain1.Id,
                    RouteId = routeBelgradeSubotica.Id,
                    DepartureTime = today.AddDays(-7).AddHours(8),
                    ArrivalTime = today.AddDays(-7).AddHours(10),
                    ActualArrivalTime = today.AddDays(-7).AddHours(10) // on time
                },
                new Trip
                {
                    TrainId = highSpeedTrain1.Id,
                    RouteId = routeBelgradeSubotica.Id,
                    DepartureTime = today.AddDays(-6).AddHours(8),
                    ArrivalTime = today.AddDays(-6).AddHours(10),
                    ActualArrivalTime = today.AddDays(-6).AddHours(10).AddMinutes(20) // 20 min late
                },
                new Trip
                {
                    TrainId = highSpeedTrain1.Id,
                    RouteId = routeBelgradeSubotica.Id,
                    DepartureTime = today.AddDays(-5).AddHours(8),
                    ArrivalTime = today.AddDays(-5).AddHours(10),
                    ActualArrivalTime = today.AddDays(-5).AddHours(10).AddMinutes(35) // 35 min late
                },
                new Trip
                {
                    TrainId = highSpeedTrain2.Id,
                    RouteId = routeBelgradeSubotica.Id,
                    DepartureTime = today.AddDays(-7).AddHours(12),
                    ArrivalTime = today.AddDays(-7).AddHours(14),
                    ActualArrivalTime = today.AddDays(-7).AddHours(14).AddMinutes(10) // 10 min late
                },
                new Trip
                {
                    TrainId = highSpeedTrain2.Id,
                    RouteId = routeBelgradeSubotica.Id,
                    DepartureTime = today.AddDays(-6).AddHours(12),
                    ArrivalTime = today.AddDays(-6).AddHours(14),
                    ActualArrivalTime = today.AddDays(-6).AddHours(14) // on time
                },
                new Trip
                {
                    TrainId = highSpeedTrain2.Id,
                    RouteId = routeBelgradeSubotica.Id,
                    DepartureTime = today.AddDays(-5).AddHours(12),
                    ArrivalTime = today.AddDays(-5).AddHours(14),
                    ActualArrivalTime = today.AddDays(-5).AddHours(14).AddMinutes(45) // 45 min late
                },

                // Belgrade - Nis route — mostly late
                new Trip
                {
                    TrainId = passengerTrain1.Id,
                    RouteId = routeBelgradeNis.Id,
                    DepartureTime = today.AddDays(-7).AddHours(9),
                    ArrivalTime = today.AddDays(-7).AddHours(12),
                    ActualArrivalTime = today.AddDays(-7).AddHours(12).AddMinutes(30) // 30 min late
                },
                new Trip
                {
                    TrainId = passengerTrain1.Id,
                    RouteId = routeBelgradeNis.Id,
                    DepartureTime = today.AddDays(-6).AddHours(9),
                    ArrivalTime = today.AddDays(-6).AddHours(12),
                    ActualArrivalTime = today.AddDays(-6).AddHours(12).AddMinutes(55) // 55 min late
                },
                new Trip
                {
                    TrainId = passengerTrain1.Id,
                    RouteId = routeBelgradeNis.Id,
                    DepartureTime = today.AddDays(-5).AddHours(9),
                    ArrivalTime = today.AddDays(-5).AddHours(12),
                    ActualArrivalTime = today.AddDays(-5).AddHours(12).AddMinutes(20) // 20 min late
                },

                // Novi Beograd - Novi Sad commuter — mostly on time
                new Trip
                {
                    TrainId = commuterTrain.Id,
                    RouteId = routeBelgradeNS.Id,
                    DepartureTime = today.AddDays(-7).AddHours(7),
                    ArrivalTime = today.AddDays(-7).AddHours(8),
                    ActualArrivalTime = today.AddDays(-7).AddHours(8) // on time
                },
                new Trip
                {
                    TrainId = commuterTrain.Id,
                    RouteId = routeBelgradeNS.Id,
                    DepartureTime = today.AddDays(-7).AddHours(17),
                    ArrivalTime = today.AddDays(-7).AddHours(18),
                    ActualArrivalTime = today.AddDays(-7).AddHours(18).AddMinutes(5) // 5 min late
                },
                new Trip
                {
                    TrainId = commuterTrain.Id,
                    RouteId = routeBelgradeNS.Id,
                    DepartureTime = today.AddDays(-6).AddHours(7),
                    ArrivalTime = today.AddDays(-6).AddHours(8),
                    ActualArrivalTime = today.AddDays(-6).AddHours(8) // on time
                },
                new Trip
                {
                    TrainId = commuterTrain.Id,
                    RouteId = routeBelgradeNS.Id,
                    DepartureTime = today.AddDays(-6).AddHours(17),
                    ArrivalTime = today.AddDays(-6).AddHours(18),
                    ActualArrivalTime = today.AddDays(-6).AddHours(18).AddMinutes(15) // 15 min late
                },
                new Trip
                {
                    TrainId = commuterTrain.Id,
                    RouteId = routeBelgradeNS.Id,
                    DepartureTime = today.AddDays(-5).AddHours(7),
                    ArrivalTime = today.AddDays(-5).AddHours(8),
                    ActualArrivalTime = today.AddDays(-5).AddHours(8) // on time
                }
            };

            await _context.Trip.AddRangeAsync(trips);
            await _context.SaveChangesAsync();
        }

        private async Task SeedDelaysAsync()
        {
            if (await _context.Delays.AnyAsync())
                return;

            var trips = await _context.Trip
                .Where(t => t.ActualArrivalTime != null)
                .ToListAsync();

            var belgrade = await _context.Stations.FirstAsync(s => s.Name == "Belgrade Center");
            var noviSad = await _context.Stations.FirstAsync(s => s.Name == "Novi Sad");
            var nis = await _context.Stations.FirstAsync(s => s.Name == "Nis");
            var noviBeograd = await _context.Stations.FirstAsync(s => s.Name == "Novi Beograd");

            var delays = new List<Delay>();

            foreach (var trip in trips)
            {
                var route = await _context.Routes
                    .Include(r => r.RouteStations)
                    .FirstAsync(r => r.Id == trip.RouteId);

                var routeName = route.Name;

                if (routeName == "Belgrade - Subotica")
                {
                    // some trips have delays at Belgrade, some at Novi Sad
                    if (trip.ActualArrivalTime > trip.ArrivalTime)
                    {
                        var delayMinutes = (int)(trip.ActualArrivalTime!.Value - trip.ArrivalTime).TotalMinutes;

                        delays.Add(new Delay
                        {
                            TripId = trip.Id,
                            StationId = delayMinutes > 30 ? noviSad.Id : belgrade.Id,
                            DelayMinutes = delayMinutes,
                            TypeOfDelay = delayMinutes > 30 ? TypeOfDelay.TrackMaintenance : TypeOfDelay.Technical,
                            Note = delayMinutes > 30 ? "Track maintenance on Novi Sad section" : "Technical issue at Belgrade",
                            CreatedAt = trip.DepartureTime.AddMinutes(30)
                        });
                    }
                }
                else if (routeName == "Belgrade - Nis")
                {
                    if (trip.ActualArrivalTime > trip.ArrivalTime)
                    {
                        var delayMinutes = (int)(trip.ActualArrivalTime!.Value - trip.ArrivalTime).TotalMinutes;

                        delays.Add(new Delay
                        {
                            TripId = trip.Id,
                            StationId = nis.Id,
                            DelayMinutes = delayMinutes / 2,
                            TypeOfDelay = TypeOfDelay.Weather,
                            Note = "Weather conditions on Nis route",
                            CreatedAt = trip.DepartureTime.AddMinutes(60)
                        });

                        delays.Add(new Delay
                        {
                            TripId = trip.Id,
                            StationId = belgrade.Id,
                            DelayMinutes = delayMinutes / 2,
                            TypeOfDelay = TypeOfDelay.StationCongestion,
                            Note = "Station congestion at Belgrade",
                            CreatedAt = trip.DepartureTime.AddMinutes(10)
                        });
                    }
                }
                else if (routeName == "Belgrade - Novi Sad")
                {
                    if (trip.ActualArrivalTime > trip.ArrivalTime)
                    {
                        var delayMinutes = (int)(trip.ActualArrivalTime!.Value - trip.ArrivalTime).TotalMinutes;

                        delays.Add(new Delay
                        {
                            TripId = trip.Id,
                            StationId = noviBeograd.Id,
                            DelayMinutes = delayMinutes,
                            TypeOfDelay = TypeOfDelay.PassengerIncident,
                            Note = "Passenger incident at Novi Beograd",
                            CreatedAt = trip.DepartureTime.AddMinutes(5)
                        });
                    }
                }
            }

            await _context.Delays.AddRangeAsync(delays);
            await _context.SaveChangesAsync();
        }

        private async Task SeedSchedulesAsync()
        {
            if(await _context.Schedules.AnyAsync())
                return;

            var highSpeedTrain1 = await _context.Trains.FirstAsync(t => t.SerialNumber == "SRB-HS-001");
            var highSpeedTrain2 = await _context.Trains.FirstAsync(t => t.SerialNumber == "SRB-HS-002");
            var passengerTrain1 = await _context.Trains.FirstAsync(t => t.SerialNumber == "SRB-PS-001");
            var commuterTrain = await _context.Trains.FirstAsync(t => t.SerialNumber == "SRB-CM-001");

            var routeBelgradeSubotica = await _context.Routes.FirstAsync(r => r.Name == "Belgrade - Subotica");
            var routeBelgradeNis = await _context.Routes.FirstAsync(r => r.Name == "Belgrade - Nis");
            var routeBelgradeNS = await _context.Routes.FirstAsync(r => r.Name == "Belgrade - Novi Sad");

            var today = DateTime.Today;

            var schedules = new List<Schedule>
            {
                new Schedule
                {
                    TrainId = highSpeedTrain1.Id,
                    RouteId = routeBelgradeSubotica.Id,
                    DepartureTime = new TimeSpan(8, 0, 0),
                    ScheduleType = ScheduleType.Workday,
                    ValidFrom = today,
                    IsActive = true
                },
                new Schedule
                {
                    TrainId = highSpeedTrain2.Id,
                    RouteId = routeBelgradeSubotica.Id,
                    DepartureTime = new TimeSpan(12, 0, 0),
                    ScheduleType = ScheduleType.Workday,
                    ValidFrom = today,
                    IsActive = true
                },
                new Schedule
                {
                    TrainId = highSpeedTrain1.Id,
                    RouteId = routeBelgradeSubotica.Id,
                    DepartureTime = new TimeSpan(10, 0, 0),
                    ScheduleType = ScheduleType.Weekend,
                    ValidFrom = today,
                    IsActive = true
                },
                new Schedule
                {
                    TrainId = passengerTrain1.Id,
                    RouteId = routeBelgradeNis.Id,
                    DepartureTime = new TimeSpan(9, 0, 0),
                    ScheduleType = ScheduleType.Daily,
                    ValidFrom = today,
                    IsActive = true
                },
                new Schedule
                {
                    TrainId = commuterTrain.Id,
                    RouteId = routeBelgradeNS.Id,
                    DepartureTime = new TimeSpan(7, 0, 0),
                    ScheduleType = ScheduleType.Workday,
                    ValidFrom = today,
                    IsActive = true
                },
                new Schedule
                {
                    TrainId = commuterTrain.Id,
                    RouteId = routeBelgradeNS.Id,
                    DepartureTime = new TimeSpan(17, 0, 0),
                    ScheduleType = ScheduleType.Workday,
                    ValidFrom = today,
                    IsActive = true
                },
                new Schedule
                {
                    TrainId = commuterTrain.Id,
                    RouteId = routeBelgradeNS.Id,
                    DepartureTime = new TimeSpan(10, 0, 0),
                    ScheduleType = ScheduleType.Weekend,
                    ValidFrom = today,
                    IsActive = true
                }
            };

            await _context.Schedules.AddRangeAsync(schedules);
            await _context.SaveChangesAsync();
        }
    }
}