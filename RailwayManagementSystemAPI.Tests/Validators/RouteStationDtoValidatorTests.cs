using RailwayManagementSystemAPI.Dtos;
using RailwayManagementSystemAPI.Validators;

namespace RailwayManagementSystemAPI.Tests.Validators
{
    public class RouteStationDtoValidatorTests
    {
        private readonly RouteStationDtoValidator _validator = new();

        [Fact]
        public async Task Should_Pass_WhenAllFieldsAreValid()
        {
            var dto = new RouteStationDto { StationId = 1, Order = 1, ArrivalOffsetMinutes = 0, StopDuration = 5 };
            var result = await _validator.ValidateAsync(dto);
            Assert.True(result.IsValid);
        }

        [Theory]
        [InlineData(0)]
        [InlineData(-1)]
        public async Task Should_Fail_WhenStationIdIsInvalid(int stationId)
        {
            var dto = new RouteStationDto { StationId = stationId, Order = 1, ArrivalOffsetMinutes = 0, StopDuration = 5 };
            var result = await _validator.ValidateAsync(dto);
            Assert.False(result.IsValid);
            Assert.Contains(result.Errors, e => e.PropertyName == "StationId");
        }

        [Theory]
        [InlineData(0)]
        [InlineData(-1)]
        public async Task Should_Fail_WhenOrderIsInvalid(int order)
        {
            var dto = new RouteStationDto { StationId = 1, Order = order, ArrivalOffsetMinutes = 0, StopDuration = 5 };
            var result = await _validator.ValidateAsync(dto);
            Assert.False(result.IsValid);
            Assert.Contains(result.Errors, e => e.PropertyName == "Order");
        }

        [Fact]
        public async Task Should_Fail_WhenArrivalOffsetMinutesIsNegative()
        {
            var dto = new RouteStationDto { StationId = 1, Order = 1, ArrivalOffsetMinutes = -1, StopDuration = 5 };
            var result = await _validator.ValidateAsync(dto);
            Assert.False(result.IsValid);
            Assert.Contains(result.Errors, e => e.PropertyName == "ArrivalOffsetMinutes");
        }

        [Fact]
        public async Task Should_Fail_WhenStopDurationIsNegative()
        {
            var dto = new RouteStationDto { StationId = 1, Order = 1, ArrivalOffsetMinutes = 0, StopDuration = -1 };
            var result = await _validator.ValidateAsync(dto);
            Assert.False(result.IsValid);
            Assert.Contains(result.Errors, e => e.PropertyName == "StopDuration");
        }
    }
}