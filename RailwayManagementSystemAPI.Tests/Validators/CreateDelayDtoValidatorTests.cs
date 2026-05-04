using RailwayManagementSystemAPI.Dtos;
using RailwayManagementSystemAPI.Models;
using RailwayManagementSystemAPI.Validators;

namespace RailwayManagementSystemAPI.Tests.Validators
{
    public class CreateDelayDtoValidatorTests
    {
        private readonly CreateDelayDtoValidator _validator = new();

        [Fact]
        public async Task Should_Pass_WhenAllFieldsAreValid()
        {
            var dto = new CreateDelayDto
            {
                TripId = 1,
                StationId = 1,
                DelayMinutes = 10,
                TypeOfDelay = TypeOfDelay.Weather,
                Note = "Some note"
            };
            var result = await _validator.ValidateAsync(dto);
            Assert.True(result.IsValid);
        }

        [Theory]
        [InlineData(0)]
        [InlineData(-1)]
        public async Task Should_Fail_WhenTripIdIsInvalid(int tripId)
        {
            var dto = new CreateDelayDto
            {
                TripId = tripId,
                StationId = 1,
                DelayMinutes = 10,
                TypeOfDelay = TypeOfDelay.Weather
            };
            var result = await _validator.ValidateAsync(dto);
            Assert.False(result.IsValid);
            Assert.Contains(result.Errors, e => e.PropertyName == "TripId");
        }

        [Theory]
        [InlineData(0)]
        [InlineData(-1)]
        public async Task Should_Fail_WhenStationIdIsInvalid(int stationId)
        {
            var dto = new CreateDelayDto
            {
                TripId = 1,
                StationId = stationId,
                DelayMinutes = 10,
                TypeOfDelay = TypeOfDelay.Weather
            };
            var result = await _validator.ValidateAsync(dto);
            Assert.False(result.IsValid);
            Assert.Contains(result.Errors, e => e.PropertyName == "StationId");
        }

        [Theory]
        [InlineData(0)]
        [InlineData(-1)]
        public async Task Should_Fail_WhenDelayMinutesIsInvalid(int minutes)
        {
            var dto = new CreateDelayDto
            {
                TripId = 1,
                StationId = 1,
                DelayMinutes = minutes,
                TypeOfDelay = TypeOfDelay.Weather
            };
            var result = await _validator.ValidateAsync(dto);
            Assert.False(result.IsValid);
            Assert.Contains(result.Errors, e => e.PropertyName == "DelayMinutes");
        }

        [Fact]
        public async Task Should_Fail_WhenTypeOfDelayIsInvalid()
        {
            var dto = new CreateDelayDto
            {
                TripId = 1,
                StationId = 1,
                DelayMinutes = 10,
                TypeOfDelay = (TypeOfDelay)99
            };
            var result = await _validator.ValidateAsync(dto);
            Assert.False(result.IsValid);
            Assert.Contains(result.Errors, e => e.PropertyName == "TypeOfDelay");
        }

        [Fact]
        public async Task Should_Fail_WhenNoteExceedsTwoHundredFiftyChars()
        {
            var dto = new CreateDelayDto
            {
                TripId = 1,
                StationId = 1,
                DelayMinutes = 10,
                TypeOfDelay = TypeOfDelay.Weather,
                Note = new string('a', 251)
            };
            var result = await _validator.ValidateAsync(dto);
            Assert.False(result.IsValid);
            Assert.Contains(result.Errors, e => e.PropertyName == "Note");
        }
    }
}