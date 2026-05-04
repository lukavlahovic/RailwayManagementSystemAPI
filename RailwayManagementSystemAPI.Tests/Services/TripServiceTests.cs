using Microsoft.Extensions.Logging.Abstractions;
using RailwayManagementSystemAPI.Dtos;
using RailwayManagementSystemAPI.Exceptions;
using RailwayManagementSystemAPI.Models;
using RailwayManagementSystemAPI.Services;
using Route = RailwayManagementSystemAPI.Models.Route;

namespace RailwayManagementSystemAPI.Tests.Services
{
    public class TripServiceTests : TestBase
    {
        private readonly ITripService _service;

        public TripServiceTests()
        {
            _service = new TripService(Context, Mapper, NullLogger<TripService>.Instance);
        }

        private async Task<Station> CreateStationAsync(string name = "Belgrade")
        {
            var station = new Station { Name = name, City = name, Country = "Serbia", NumberOfPlatforms = 5 };
            Context.Stations.Add(station);
            await Context.SaveChangesAsync();
            return station;
        }

        private async Task<Train> CreateTrainAsync()
        {
            var trainType = new TrainType { Name = "ICE 3", MaxSpeed = 330, Capacity = 400, Manufacturer = "Siemens", Type = TypeOfTrain.HighSpeed };
            Context.TrainTypes.Add(trainType);
            await Context.SaveChangesAsync();

            var train = new Train { SerialNumber = "SRB-001", TrainTypeId = trainType.Id };
            Context.Trains.Add(train);
            await Context.SaveChangesAsync();
            return train;
        }

        private async Task<(Route route, List<Station> stations)> CreateRouteAsync()
        {
            var station1 = await CreateStationAsync("Belgrade");
            var station2 = await CreateStationAsync("Novi Sad");
            var station3 = await CreateStationAsync("Subotica");

            var stations = new List<Station> { station1, station2, station3 };

            var route = new Route
            {
                Name = "Belgrade - Subotica",
                RouteStations = new List<RouteStation>
                {
                    new RouteStation { StationId = station1.Id, Order = 1, ArrivalOffsetMinutes = 0, StopDuration = 10 },
                    new RouteStation { StationId = station2.Id, Order = 2, ArrivalOffsetMinutes = 45, StopDuration = 5 },
                    new RouteStation { StationId = station3.Id, Order = 3, ArrivalOffsetMinutes = 120, StopDuration = 10 }
                }
            };

            Context.Routes.Add(route);
            await Context.SaveChangesAsync();
            return (route, stations);
        }

        private async Task<Trip> CreateTripAsync(int trainId, int routeId)
        {
            var trip = new Trip
            {
                TrainId = trainId,
                RouteId = routeId,
                DepartureTime = DateTime.Today.AddHours(8),
                ArrivalTime = DateTime.Today.AddHours(10)
            };
            Context.Trip.Add(trip);
            await Context.SaveChangesAsync();
            return trip;
        }

        // --- GetTripByIdAsync ---

        [Fact]
        public async Task GetTripByIdAsync_ReturnsTrip_WhenFound()
        {
            // Arrange
            var train = await CreateTrainAsync();
            var (route, stations) = await CreateRouteAsync();
            var trip = await CreateTripAsync(train.Id, route.Id);

            // Act
            var result = await _service.GetTripByIdAsync(trip.Id);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(trip.Id, result.Id);
            Assert.Equal("SRB-001", result.SerialNumber);
            Assert.Equal("Belgrade - Subotica", result.RouteName);
        }

        [Fact]
        public async Task GetTripByIdAsync_ThrowsNotFoundException_WhenNotFound()
        {
            await Assert.ThrowsAsync<NotFoundException>(() =>
                _service.GetTripByIdAsync(999));
        }

        // --- CreateTripAsync ---

        [Fact]
        public async Task CreateTripAsync_CreatesAndReturnsTrip()
        {
            // Arrange
            var train = await CreateTrainAsync();
            var (route, stations) = await CreateRouteAsync();

            var dto = new CreateTripDto
            {
                TrainId = train.Id,
                RouteId = route.Id,
                DepartureTime = DateTime.Today.AddHours(8),
                ArrivalTime = DateTime.Today.AddHours(10)
            };

            // Act
            var result = await _service.CreateTripAsync(dto);

            // Assert
            Assert.NotNull(result);
            Assert.True(result.Id > 0);
            Assert.Equal("SRB-001", result.SerialNumber);
            Assert.Equal("Belgrade - Subotica", result.RouteName);
        }

        [Fact]
        public async Task CreateTripAsync_PersistsToDatabase()
        {
            // Arrange
            var train = await CreateTrainAsync();
            var (route, stations) = await CreateRouteAsync();

            var dto = new CreateTripDto
            {
                TrainId = train.Id,
                RouteId = route.Id,
                DepartureTime = DateTime.Today.AddHours(8),
                ArrivalTime = DateTime.Today.AddHours(10)
            };

            // Act
            var result = await _service.CreateTripAsync(dto);

            // Assert
            var inDb = await Context.Trip.FindAsync(result.Id);
            Assert.NotNull(inDb);
            Assert.Equal(train.Id, inDb.TrainId);
            Assert.Equal(route.Id, inDb.RouteId);
        }

        [Fact]
        public async Task CreateTripAsync_ThrowsBadRequestException_WhenTrainNotFound()
        {
            // Arrange
            var (route, stations) = await CreateRouteAsync();

            var dto = new CreateTripDto
            {
                TrainId = 999,
                RouteId = route.Id,
                DepartureTime = DateTime.Today.AddHours(8),
                ArrivalTime = DateTime.Today.AddHours(10)
            };

            await Assert.ThrowsAsync<BadRequestException>(() =>
                _service.CreateTripAsync(dto));
        }

        [Fact]
        public async Task CreateTripAsync_ThrowsBadRequestException_WhenRouteNotFound()
        {
            // Arrange
            var train = await CreateTrainAsync();

            var dto = new CreateTripDto
            {
                TrainId = train.Id,
                RouteId = 999,
                DepartureTime = DateTime.Today.AddHours(8),
                ArrivalTime = DateTime.Today.AddHours(10)
            };

            await Assert.ThrowsAsync<BadRequestException>(() =>
                _service.CreateTripAsync(dto));
        }

        // --- GetTripsByStationAsync ---

        [Fact]
        public async Task GetTripsByStationAsync_ReturnsTrips_WhenFound()
        {
            // Arrange
            var train = await CreateTrainAsync();
            var (route, stations) = await CreateRouteAsync();
            var trip = await CreateTripAsync(train.Id, route.Id);

            // Act
            var result = await _service.GetTripsByStationAsync(stations[0].Id);

            // Assert
            Assert.NotNull(result);
            Assert.Single(result);
            Assert.Equal(trip.Id, result[0].TripId);
        }

        [Fact]
        public async Task GetTripsByStationAsync_ReturnsEmptyList_WhenNoTrips()
        {
            // Arrange
            var station = await CreateStationAsync("Nis");

            // Act
            var result = await _service.GetTripsByStationAsync(station.Id);

            // Assert
            Assert.Empty(result);
        }

        [Fact]
        public async Task GetTripsByStationAsync_ThrowsNotFoundException_WhenStationNotFound()
        {
            await Assert.ThrowsAsync<NotFoundException>(() =>
                _service.GetTripsByStationAsync(999));
        }

        // --- GetTripsByDateAsync ---

        [Fact]
        public async Task GetTripsByDateAsync_ReturnsTrips_WhenFound()
        {
            // Arrange
            var train = await CreateTrainAsync();
            var (route, stations) = await CreateRouteAsync();
            var trip = await CreateTripAsync(train.Id, route.Id);

            // Act
            var result = await _service.GetTripsByDateAsync(DateTime.Today);

            // Assert
            Assert.NotNull(result);
            Assert.Single(result);
            Assert.Equal(trip.Id, result[0].TripId);
        }

        [Fact]
        public async Task GetTripsByDateAsync_ReturnsEmptyList_WhenNoTripsOnDate()
        {
            // Arrange
            var train = await CreateTrainAsync();
            var (route, stations) = await CreateRouteAsync();
            await CreateTripAsync(train.Id, route.Id);

            // Act — search for tomorrow, trip is today
            var result = await _service.GetTripsByDateAsync(DateTime.Today.AddDays(1));

            // Assert
            Assert.Empty(result);
        }
    }
}