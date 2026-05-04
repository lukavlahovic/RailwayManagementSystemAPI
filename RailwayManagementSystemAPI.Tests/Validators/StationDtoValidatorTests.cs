using RailwayManagementSystemAPI.Dtos;
using RailwayManagementSystemAPI.Validators;

namespace RailwayManagementSystemAPI.Tests.Validators
{
    public class StationDtoValidatorTests
    {
        private readonly StationDtoValidator _validator = new();

        [Fact]
        public async Task Should_Pass_WhenAllFieldsAreValid()
        {
            var dto = new StationDto { Name = "Belgrade", City = "Belgrade", Country = "Serbia", NumberOfPlatforms = 5 };
            var result = await _validator.ValidateAsync(dto);
            Assert.True(result.IsValid);
        }

        [Fact]
        public async Task Should_Fail_WhenNameIsEmpty()
        {
            var dto = new StationDto { Name = "", City = "Belgrade", Country = "Serbia", NumberOfPlatforms = 5 };
            var result = await _validator.ValidateAsync(dto);
            Assert.False(result.IsValid);
            Assert.Contains(result.Errors, e => e.PropertyName == "Name");
        }

        [Fact]
        public async Task Should_Fail_WhenNameIsLonger()
        {
            var dto = new StationDto { Name = "abcdefghijabcdefghijabcdefghijabcdefghijabcdefghijabcdefghijabcdefghijabcdefghijabcdefghijabcdefghijk", City = "Belgrade", Country = "Serbia", NumberOfPlatforms = 5 };
            var result = await _validator.ValidateAsync(dto);
            Assert.False(result.IsValid);
            Assert.Contains(result.Errors, e => e.PropertyName == "Name");
        }

        [Fact]
        public async Task Should_Fail_WhenCityIsEmpty()
        {
            var dto = new StationDto { Name = "Belgrade", City = "", Country = "Serbia", NumberOfPlatforms = 5 };
            var result = await _validator.ValidateAsync(dto);
            Assert.False(result.IsValid);
            Assert.Contains(result.Errors, e => e.PropertyName == "City");
        }

        [Fact]
        public async Task Should_Fail_WhenCountryIsEmpty()
        {
            var dto = new StationDto { Name = "Belgrade", City = "Belgrade", Country = "", NumberOfPlatforms = 5 };
            var result = await _validator.ValidateAsync(dto);
            Assert.False(result.IsValid);
            Assert.Contains(result.Errors, e => e.PropertyName == "Country");
        }

        [Fact]
        public async Task Should_Fail_WhenNumberOfPlatformsLessThenZero()
        {
            var dto = new StationDto { Name = "Belgrade", City = "Belgrade", Country = "Serbia", NumberOfPlatforms = -1 };
            var result = await _validator.ValidateAsync(dto);
            Assert.False(result.IsValid);
            Assert.Contains(result.Errors, e => e.PropertyName == "NumberOfPlatforms");
        }

        [Fact]
        public async Task Should_Fail_WhenNumberOfPlatformsGreaterThenTwenty()
        {
            var dto = new StationDto { Name = "Belgrade", City = "Belgrade", Country = "Serbia", NumberOfPlatforms = 21 };
            var result = await _validator.ValidateAsync(dto);
            Assert.False(result.IsValid);
            Assert.Contains(result.Errors, e => e.PropertyName == "NumberOfPlatforms");
        }
    }
}
