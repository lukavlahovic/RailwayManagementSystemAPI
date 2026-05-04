using RailwayManagementSystemAPI.Dtos;
using RailwayManagementSystemAPI.Validators;

namespace RailwayManagementSystemAPI.Tests.Validators
{
    public class CreateTripDtoValidatorTests
    {
        private readonly CreateTripDtoValidator _validator = new();

        [Fact]
        public async Task Should_Pass_WhenAllFieldsAreValid()
        {
            var dto = new CreateTripDto
            {
                TrainId = 1,
                RouteId = 1,
                DepartureTime = DateTime.Now.AddHours(1),
                ArrivalTime = DateTime.Now.AddHours(3)
            };
            var result = await _validator.ValidateAsync(dto);
            Assert.True(result.IsValid);
        }

        [Theory]
        [InlineData(0)]
        [InlineData(-1)]
        public async Task Should_Fail_WhenTrainIdIsInvalid(int trainId)
        {
            var dto = new CreateTripDto
            {
                TrainId = trainId,
                RouteId = 1,
                DepartureTime = DateTime.Now.AddHours(1),
                ArrivalTime = DateTime.Now.AddHours(3)
            };
            var result = await _validator.ValidateAsync(dto);
            Assert.False(result.IsValid);
            Assert.Contains(result.Errors, e => e.PropertyName == "TrainId");
        }

        [Theory]
        [InlineData(0)]
        [InlineData(-1)]
        public async Task Should_Fail_WhenRouteIdIsInvalid(int routeId)
        {
            var dto = new CreateTripDto
            {
                TrainId = 1,
                RouteId = routeId,
                DepartureTime = DateTime.Now.AddHours(1),
                ArrivalTime = DateTime.Now.AddHours(3)
            };
            var result = await _validator.ValidateAsync(dto);
            Assert.False(result.IsValid);
            Assert.Contains(result.Errors, e => e.PropertyName == "RouteId");
        }

        [Fact]
        public async Task Should_Fail_WhenDepartureTimeIsInThePast()
        {
            var dto = new CreateTripDto
            {
                TrainId = 1,
                RouteId = 1,
                DepartureTime = DateTime.Now.AddHours(-1),
                ArrivalTime = DateTime.Now.AddHours(3)
            };
            var result = await _validator.ValidateAsync(dto);
            Assert.False(result.IsValid);
            Assert.Contains(result.Errors, e => e.PropertyName == "DepartureTime");
        }

        [Fact]
        public async Task Should_Fail_WhenArrivalTimeIsBeforeDepartureTime()
        {
            var dto = new CreateTripDto
            {
                TrainId = 1,
                RouteId = 1,
                DepartureTime = DateTime.Now.AddHours(3),
                ArrivalTime = DateTime.Now.AddHours(1)
            };
            var result = await _validator.ValidateAsync(dto);
            Assert.False(result.IsValid);
            Assert.Contains(result.Errors, e => e.PropertyName == "ArrivalTime");
        }
    }
}