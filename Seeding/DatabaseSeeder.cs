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
                new Trip
                {
                    TrainId = highSpeedTrain1.Id,
                    RouteId = routeBelgradeSubotica.Id,
                    DepartureTime = today.AddHours(8),
                    ArrivalTime = today.AddHours(10)
                },
                new Trip
                {
                    TrainId = highSpeedTrain2.Id,
                    RouteId = routeBelgradeSubotica.Id,
                    DepartureTime = today.AddHours(12),
                    ArrivalTime = today.AddHours(14)
                },
                new Trip
                {
                    TrainId = passengerTrain1.Id,
                    RouteId = routeBelgradeNis.Id,
                    DepartureTime = today.AddHours(9),
                    ArrivalTime = today.AddHours(12)
                },
                new Trip
                {
                    TrainId = commuterTrain.Id,
                    RouteId = routeBelgradeNS.Id,
                    DepartureTime = today.AddHours(7),
                    ArrivalTime = today.AddHours(8)
                },
                new Trip
                {
                    TrainId = commuterTrain.Id,
                    RouteId = routeBelgradeNS.Id,
                    DepartureTime = today.AddHours(17),
                    ArrivalTime = today.AddHours(18)
                }
            };

            await _context.Trip.AddRangeAsync(trips);
            await _context.SaveChangesAsync();
        }
    }
}
