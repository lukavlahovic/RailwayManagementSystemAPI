using Microsoft.Extensions.Logging.Abstractions;
using RailwayManagementSystemAPI.Dtos;
using RailwayManagementSystemAPI.Exceptions;
using RailwayManagementSystemAPI.Models;
using RailwayManagementSystemAPI.Services;
using Route = RailwayManagementSystemAPI.Models.Route;

namespace RailwayManagementSystemAPI.Tests.Services
{
    public class DelayServiceTests : TestBase
    {
        private readonly IDelayService _service;

        public DelayServiceTests()
        {
            _service = new DelayService(Context, Mapper, NullLogger<DelayService>.Instance);
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

        private async Task<Trip> CreateTripAsync(int trainId, int routeId)
        {
            var trip = new Trip
            {
                TrainId = trainId,
                RouteId = routeId,
                DepartureTime = DateTime.Now.AddHours(1),
                ArrivalTime = DateTime.Now.AddHours(3)
            };
            Context.Trip.Add(trip);
            await Context.SaveChangesAsync();
            return trip;
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

        // --- GetDelayByIdAsync ---

        [Fact]
        public async Task GetDelayByIdAsync_ReturnsDelay_WhenFound()
        {
            // Arrange
            var train = await CreateTrainAsync();
            var (route, stations) = await CreateRouteAsync();
            var trip = await CreateTripAsync(train.Id, route.Id);
            var delay = new Delay 
            { 
                TripId = trip.Id, StationId = stations[0].Id, DelayMinutes = 10, TypeOfDelay = TypeOfDelay.TrackMaintenance, Note = "Note" 
            };
            Context.Delays.Add(delay);
            await Context.SaveChangesAsync();

            // Act
            var result = await _service.GetDelayByIdAsync(delay.Id);

            // Assert
            Assert.NotNull(result);
            Assert.True(result.Id > 0);
            Assert.Equal(trip.Id, result.TripId);
            Assert.Equal("Belgrade", result.StationName);
            Assert.Equal(10, result.DelayMinutes);
            Assert.Equal(TypeOfDelay.TrackMaintenance, result.TypeOfDelay);
            Assert.Equal("Note", result.Note);
        }

        [Fact]
        public async Task GetDelayByIdAsync_ThrowsNotFoundException_WhenNotFound()
        {
            await Assert.ThrowsAsync<NotFoundException>(() =>
                _service.GetDelayByIdAsync(999)
            );
        }

        // --- GetDelaysByTrip ---

        [Fact]
        public async Task GetDelaysByTrip_ReturnsDelays_WhenFound()
        {
            // Arrange
            var train = await CreateTrainAsync();
            var (route, stations) = await CreateRouteAsync();
            var trip = await CreateTripAsync(train.Id, route.Id);
            var delay = new Delay
            {
                TripId = trip.Id,
                StationId = stations[0].Id,
                DelayMinutes = 10,
                TypeOfDelay = TypeOfDelay.TrackMaintenance,
                Note = "Note"
            };
            Context.Delays.Add(delay);
            await Context.SaveChangesAsync();

            // Act
            var result = await _service.GetDelaysByTripAsync(trip.Id);

            // Assert
            Assert.NotNull(result);
            Assert.True(result[0].Id > 0);
            Assert.Equal(trip.Id, result[0].TripId);
            Assert.Equal("Belgrade", result[0].StationName);
            Assert.Equal(10, result[0].DelayMinutes);
            Assert.Equal(TypeOfDelay.TrackMaintenance, result[0].TypeOfDelay);
            Assert.Equal("Note", result[0].Note);
        }

        [Fact]
        public async Task GetDelaysByTripAsync_ReturnsEmptyList_WhenNoDelays()
        {
            // Arrange
            var train = await CreateTrainAsync();
            var (route, stations) = await CreateRouteAsync();
            var trip = await CreateTripAsync(train.Id, route.Id);

            // Act
            var result = await _service.GetDelaysByTripAsync(trip.Id);

            // Assert
            Assert.Empty(result);
        }

        [Fact]
        public async Task GetDelaysByTripAsync_ThrowsNotFoundException_WhenNotFound()
        {
            await Assert.ThrowsAsync<NotFoundException>(() =>
                _service.GetDelaysByTripAsync(999)
            );
        }

        // --- CreateDelayAsync ---

        [Fact]
        public async Task CreateDelayAsync_CreatesAndReturnsDelay()
        {
            // Arrange
            var train = await CreateTrainAsync();
            var (route, stations) = await CreateRouteAsync();
            var trip = await CreateTripAsync(train.Id, route.Id);
            var dto = new CreateDelayDto
            {
                TripId = trip.Id,
                StationId = stations[0].Id,
                DelayMinutes = 10,
                TypeOfDelay = TypeOfDelay.TrackMaintenance,
                Note = "Note"
            };

            // Act
            var result = await _service.CreateDelayAsync(dto);

            // Assert
            Assert.NotNull(result);
            Assert.True(result.Id > 0);
            Assert.Equal(trip.Id, result.TripId);
            Assert.Equal("Belgrade", result.StationName);
            Assert.Equal(10, result.DelayMinutes);
            Assert.Equal(TypeOfDelay.TrackMaintenance, result.TypeOfDelay);
            Assert.Equal("Note", result.Note);
        }

        [Fact]
        public async Task CreateDelayAsync_PersistsToDatabase()
        {
            // Arrange
            var train = await CreateTrainAsync();
            var (route, stations) = await CreateRouteAsync();
            var trip = await CreateTripAsync(train.Id, route.Id);
            var dto = new CreateDelayDto
            {
                TripId = trip.Id,
                StationId = stations[0].Id,
                DelayMinutes = 10,
                TypeOfDelay = TypeOfDelay.TrackMaintenance,
                Note = "Note"
            };

            // Act
            var result = await _service.CreateDelayAsync(dto);

            // Assert
            var inDb = await Context.Delays.FindAsync(result.Id);
            Assert.NotNull(inDb);
            Assert.Equal(trip.Id, inDb.TripId);
            Assert.Equal(stations[0].Id, inDb.StationId);
            Assert.Equal(10, inDb.DelayMinutes);
            Assert.Equal(TypeOfDelay.TrackMaintenance, inDb.TypeOfDelay);
            Assert.Equal("Note", inDb.Note);
        }

        [Fact]
        public async Task CreateDelayAsync_ThrowsBadRequestException_WhenTripNotFound()
        {
            // Arrange
            var station = await CreateStationAsync();
            var dto = new CreateDelayDto
            {
                TripId = 999,
                StationId = station.Id,
                DelayMinutes = 10,
                TypeOfDelay = TypeOfDelay.TrackMaintenance,
                Note = "Note"
            };

            await Assert.ThrowsAsync<BadRequestException>(() =>
                _service.CreateDelayAsync(dto)
            );
        }

        [Fact]
        public async Task CreateDelayAsync_ThrowsBadRequestException_WhenStationNotFound()
        {
            // Arrange
            var train = await CreateTrainAsync();
            var (route, stations) = await CreateRouteAsync();
            var trip = await CreateTripAsync(train.Id, route.Id);
            var dto = new CreateDelayDto
            {
                TripId = trip.Id,
                StationId = 999,
                DelayMinutes = 10,
                TypeOfDelay = TypeOfDelay.TrackMaintenance,
                Note = "Note"
            };

            await Assert.ThrowsAsync<BadRequestException>(() =>
                _service.CreateDelayAsync(dto)
            );
        }
    }
}
