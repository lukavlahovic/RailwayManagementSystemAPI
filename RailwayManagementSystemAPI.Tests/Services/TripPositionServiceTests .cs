using Microsoft.Extensions.Logging.Abstractions;
using RailwayManagementSystemAPI.Dtos;
using RailwayManagementSystemAPI.Exceptions;
using RailwayManagementSystemAPI.Models;
using RailwayManagementSystemAPI.Services;

namespace RailwayManagementSystemAPI.Tests.Services
{
    public class TripPositionServiceTests : TestBase
    {
        private readonly ITripService _service;

        public TripPositionServiceTests()
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

        private async Task<Trip> CreateTripAsync(int trainId, int routeId, DateTime departure, DateTime arrival)
        {
            var trip = new Trip
            {
                TrainId = trainId,
                RouteId = routeId,
                DepartureTime = departure,
                ArrivalTime = arrival
            };
            Context.Trip.Add(trip);
            await Context.SaveChangesAsync();
            return trip;
        }

        // --- GetTripPositionAsync ---

        [Fact]
        public async Task GetTripPositionAsync_ReturnsNotDeparted_WhenBeforeDeparture()
        {
            // Arrange
            var train = await CreateTrainAsync();
            var (route, stations) = await CreateRouteAsync();
            var trip = await CreateTripAsync(
                train.Id, route.Id,
                DateTime.Now.AddHours(2),   // departs in 2 hours
                DateTime.Now.AddHours(4)
            );

            // Act
            var result = await _service.GetTripPositionAsync(trip.Id);

            // Assert
            Assert.Equal(TripStatus.NotDeparted, result.Status);
            Assert.Equal("SRB-001", result.Train);
            Assert.Equal("Belgrade - Subotica", result.Route);
        }

        [Fact]
        public async Task GetTripPositionAsync_ReturnsAtStation_WhenWithinStopWindow()
        {
            // Arrange
            var train = await CreateTrainAsync();
            var (route, stations) = await CreateRouteAsync();

            // departed 47 minutes ago — station 2 arrives at 45min, stop duration 5min
            // so at 47min elapsed the train is stopped at station 2 (45 to 50)
            var trip = await CreateTripAsync(
                train.Id, route.Id,
                DateTime.Now.AddMinutes(-47),
                DateTime.Now.AddHours(2)
            );

            // Act
            var result = await _service.GetTripPositionAsync(trip.Id);

            // Assert
            Assert.Equal(TripStatus.AtStation, result.Status);
            Assert.Equal("Novi Sad", result.LastStation);
            Assert.Equal("Subotica", result.NextStation);
        }

        [Fact]
        public async Task GetTripPositionAsync_ReturnsInTransit_WhenBetweenStations()
        {
            // Arrange
            var train = await CreateTrainAsync();
            var (route, stations) = await CreateRouteAsync();

            // departed 30 minutes ago — between station 1 (offset 0, stop 10) and station 2 (offset 45)
            // at 30 min elapsed train has left station 1 (departed at 10min) and not yet reached station 2
            var trip = await CreateTripAsync(
                train.Id, route.Id,
                DateTime.Now.AddMinutes(-30),
                DateTime.Now.AddHours(2)
            );

            // Act
            var result = await _service.GetTripPositionAsync(trip.Id);

            // Assert
            Assert.Equal(TripStatus.InTransit, result.Status);
            Assert.Equal("Belgrade", result.LastStation);
            Assert.Equal("Novi Sad", result.NextStation);
            Assert.NotNull(result.MinutesToNextStation);
        }

        [Fact]
        public async Task GetTripPositionAsync_ReturnsWaitingForCompletion_WhenPastAllStations()
        {
            // Arrange
            var train = await CreateTrainAsync();
            var (route, stations) = await CreateRouteAsync();

            // departed 5 hours ago — last station offset is 120min so train has long passed it
            var trip = await CreateTripAsync(
                train.Id, route.Id,
                DateTime.Now.AddHours(-5),
                DateTime.Now.AddHours(-3)
            );

            // Act
            var result = await _service.GetTripPositionAsync(trip.Id);

            // Assert
            Assert.Equal(TripStatus.WaitingForCompletion, result.Status);
            Assert.Equal(100, result.ProgressPercent);
        }

        [Fact]
        public async Task GetTripPositionAsync_ReturnsCompleted_WhenActualArrivalSet()
        {
            // Arrange
            var train = await CreateTrainAsync();
            var (route, stations) = await CreateRouteAsync();
            var trip = await CreateTripAsync(
                train.Id, route.Id,
                DateTime.Now.AddHours(-5),
                DateTime.Now.AddHours(-3)
            );

            trip.ActualArrivalTime = DateTime.Now.AddHours(-3).AddMinutes(10);
            await Context.SaveChangesAsync();

            // Act
            var result = await _service.GetTripPositionAsync(trip.Id);

            // Assert
            Assert.Equal(TripStatus.Completed, result.Status);
            Assert.NotNull(result.ActualArrival);
            Assert.NotNull(result.PlannedArrival);
            Assert.Equal(100, result.ProgressPercent);
        }

        [Fact]
        public async Task GetTripPositionAsync_ThrowsNotFoundException_WhenTripNotFound()
        {
            await Assert.ThrowsAsync<NotFoundException>(() =>
                _service.GetTripPositionAsync(999));
        }

        [Fact]
        public async Task GetTripPositionAsync_AccountsForDelays()
        {
            // Arrange
            var train = await CreateTrainAsync();
            var (route, stations) = await CreateRouteAsync();
            var trip = await CreateTripAsync(
                train.Id, route.Id,
                DateTime.Now.AddMinutes(-30),
                DateTime.Now.AddHours(2)
            );

            // add 20 minutes of delay at first station
            Context.Delays.Add(new Delay
            {
                TripId = trip.Id,
                StationId = stations[0].Id,
                DelayMinutes = 20,
                TypeOfDelay = TypeOfDelay.Technical
            });
            await Context.SaveChangesAsync();

            // Act
            var result = await _service.GetTripPositionAsync(trip.Id);

            // Assert
            Assert.Equal(20, result.TotalDelayMinutes);
        }

        // --- CompleteTripAsync ---

        [Fact]
        public async Task CompleteTripAsync_CompletesSuccessfully()
        {
            // Arrange
            var train = await CreateTrainAsync();
            var (route, stations) = await CreateRouteAsync();
            var trip = await CreateTripAsync(
                train.Id, route.Id,
                DateTime.Now.AddHours(-5),
                DateTime.Now.AddHours(-3)
            );

            var dto = new CompleteTripDto
            {
                ActualArrivalTime = DateTime.Now.AddHours(-3).AddMinutes(15)
            };

            // Act
            await _service.CompleteTripAsync(trip.Id, dto);

            // Assert
            var updated = await Context.Trip.FindAsync(trip.Id);
            Assert.NotNull(updated!.ActualArrivalTime);
            Assert.Equal(dto.ActualArrivalTime, updated.ActualArrivalTime);
        }

        [Fact]
        public async Task CompleteTripAsync_ThrowsNotFoundException_WhenTripNotFound()
        {
            var dto = new CompleteTripDto { ActualArrivalTime = DateTime.Now };

            await Assert.ThrowsAsync<NotFoundException>(() =>
                _service.CompleteTripAsync(999, dto));
        }

        [Fact]
        public async Task CompleteTripAsync_ThrowsBadRequestException_WhenAlreadyCompleted()
        {
            // Arrange
            var train = await CreateTrainAsync();
            var (route, stations) = await CreateRouteAsync();
            var trip = await CreateTripAsync(
                train.Id, route.Id,
                DateTime.Now.AddHours(-5),
                DateTime.Now.AddHours(-3)
            );

            trip.ActualArrivalTime = DateTime.Now.AddHours(-3);
            await Context.SaveChangesAsync();

            var dto = new CompleteTripDto { ActualArrivalTime = DateTime.Now };

            // Act & Assert
            await Assert.ThrowsAsync<BadRequestException>(() =>
                _service.CompleteTripAsync(trip.Id, dto));
        }

        [Fact]
        public async Task CompleteTripAsync_ThrowsBadRequestException_WhenArrivalBeforeDeparture()
        {
            // Arrange
            var train = await CreateTrainAsync();
            var (route, stations) = await CreateRouteAsync();
            var trip = await CreateTripAsync(
                train.Id, route.Id,
                DateTime.Now.AddHours(-5),
                DateTime.Now.AddHours(-3)
            );

            var dto = new CompleteTripDto
            {
                ActualArrivalTime = DateTime.Now.AddHours(-10) // before departure
            };

            // Act & Assert
            await Assert.ThrowsAsync<BadRequestException>(() =>
                _service.CompleteTripAsync(trip.Id, dto));
        }
    }
}
