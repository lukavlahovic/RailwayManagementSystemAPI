using RailwayManagementSystemAPI.Dtos;
using RailwayManagementSystemAPI.Validators;

namespace RailwayManagementSystemAPI.Tests.Validators
{
    public class CreateRouteDtoValidatorTests
    {
        private readonly CreateRouteDtoValidator _validator = new();

        private List<RouteStationDto> ValidStations() =>
        [
            new RouteStationDto { StationId = 1, Order = 1, ArrivalOffsetMinutes = 0, StopDuration = 5 },
            new RouteStationDto { StationId = 2, Order = 2, ArrivalOffsetMinutes = 45, StopDuration = 5 }
        ];

        [Fact]
        public async Task Should_Pass_WhenAllFieldsAreValid()
        {
            var dto = new CreateRouteDto { Name = "Belgrade - Novi Sad", Stations = ValidStations() };
            var result = await _validator.ValidateAsync(dto);
            Assert.True(result.IsValid);
        }

        [Theory]
        [InlineData("")]
        public async Task Should_Fail_WhenNameIsEmpty(string name)
        {
            var dto = new CreateRouteDto { Name = name, Stations = ValidStations() };
            var result = await _validator.ValidateAsync(dto);
            Assert.False(result.IsValid);
            Assert.Contains(result.Errors, e => e.PropertyName == "Name");
        }

        [Fact]
        public async Task Should_Fail_WhenNameExceedsHundredChars()
        {
            var dto = new CreateRouteDto
            {
                Name = new string('a', 101),
                Stations = ValidStations()
            };
            var result = await _validator.ValidateAsync(dto);
            Assert.False(result.IsValid);
            Assert.Contains(result.Errors, e => e.PropertyName == "Name");
        }

        [Fact]
        public async Task Should_Fail_WhenStationsIsEmpty()
        {
            var dto = new CreateRouteDto { Name = "Belgrade - Novi Sad", Stations = [] };
            var result = await _validator.ValidateAsync(dto);
            Assert.False(result.IsValid);
            Assert.Contains(result.Errors, e => e.PropertyName == "Stations");
        }

        [Fact]
        public async Task Should_Fail_WhenDuplicateOrderValues()
        {
            var dto = new CreateRouteDto
            {
                Name = "Belgrade - Novi Sad",
                Stations =
                [
                    new RouteStationDto { StationId = 1, Order = 1, ArrivalOffsetMinutes = 0, StopDuration = 5 },
                    new RouteStationDto { StationId = 2, Order = 1, ArrivalOffsetMinutes = 45, StopDuration = 5 }
                ]
            };
            var result = await _validator.ValidateAsync(dto);
            Assert.False(result.IsValid);
            Assert.Contains(result.Errors, e => e.PropertyName == "Stations");
        }

        [Fact]
        public async Task Should_Fail_WhenDuplicateStations()
        {
            var dto = new CreateRouteDto
            {
                Name = "Belgrade - Novi Sad",
                Stations =
                [
                    new RouteStationDto { StationId = 1, Order = 1, ArrivalOffsetMinutes = 0, StopDuration = 5 },
                    new RouteStationDto { StationId = 1, Order = 2, ArrivalOffsetMinutes = 45, StopDuration = 5 }
                ]
            };
            var result = await _validator.ValidateAsync(dto);
            Assert.False(result.IsValid);
            Assert.Contains(result.Errors, e => e.PropertyName == "Stations");
        }

        [Fact]
        public async Task Should_Fail_WhenFirstStationOffsetIsNotZero()
        {
            var dto = new CreateRouteDto
            {
                Name = "Belgrade - Novi Sad",
                Stations =
                [
                    new RouteStationDto { StationId = 1, Order = 1, ArrivalOffsetMinutes = 10, StopDuration = 5 },
                    new RouteStationDto { StationId = 2, Order = 2, ArrivalOffsetMinutes = 45, StopDuration = 5 }
                ]
            };
            var result = await _validator.ValidateAsync(dto);
            Assert.False(result.IsValid);
            Assert.Contains(result.Errors, e => e.PropertyName == "Stations");
        }

        [Fact]
        public async Task Should_Fail_WhenOffsetsAreNotIncreasing()
        {
            var dto = new CreateRouteDto
            {
                Name = "Belgrade - Novi Sad",
                Stations =
                [
                    new RouteStationDto { StationId = 1, Order = 1, ArrivalOffsetMinutes = 0, StopDuration = 5 },
                    new RouteStationDto { StationId = 2, Order = 2, ArrivalOffsetMinutes = 0, StopDuration = 5 }
                ]
            };
            var result = await _validator.ValidateAsync(dto);
            Assert.False(result.IsValid);
            Assert.Contains(result.Errors, e => e.PropertyName == "Stations");
        }
    }
}