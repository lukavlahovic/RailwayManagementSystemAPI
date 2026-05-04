using RailwayManagementSystemAPI.Dtos;
using RailwayManagementSystemAPI.Models;
using RailwayManagementSystemAPI.Validators;

namespace RailwayManagementSystemAPI.Tests.Validators
{
    public class CreateTrainTypeDtoValidatorTests
    {
        private readonly CreateTrainTypeDtoValidator _validator = new();

        [Fact]
        public async Task Should_Pass_WhenAllFieldsAreValid()
        {
            var dto = new CreateTrainTypeDto { Name = "ICE 3", MaxSpeed = 330, Capacity = 400, Manufacturer = "Siemens", Type = TypeOfTrain.HighSpeed };
            var result = await _validator.ValidateAsync(dto);
            Assert.True(result.IsValid);
        }

        [Theory]
        [InlineData("")]
        [InlineData("abcdefghijabcdefghijabcdefghijabcdefghijabcdefghijabcdefghijabcdefghijabcdefghijabcdefghijabcdefghijk")]
        public async Task Should_Fail_WhenNameIsEmptyOrLongerThenHundred(string name)
        {
            var dto = new CreateTrainTypeDto { Name = name, MaxSpeed = 330, Capacity = 400, Manufacturer = "Siemens", Type = TypeOfTrain.HighSpeed };
            var result = await _validator.ValidateAsync(dto);
            Assert.False(result.IsValid);
            Assert.Contains(result.Errors, e => e.PropertyName == "Name");
        }

        [Theory]
        [InlineData(0)]
        [InlineData(-1)]
        [InlineData(501)]
        public async Task Should_Fail_WhenMaxSpeedIsIncorrect(int maxSpeed)
        {
            var dto = new CreateTrainTypeDto { Name = "ICE 3", MaxSpeed = maxSpeed, Capacity = 400, Manufacturer = "Siemens", Type = TypeOfTrain.HighSpeed };
            var result = await _validator.ValidateAsync(dto);
            Assert.False(result.IsValid);
            Assert.Contains(result.Errors, e => e.PropertyName == "MaxSpeed");
        }

        [Theory]
        [InlineData(0)]
        [InlineData(-1)]
        [InlineData(2001)]
        public async Task Should_Fail_WhenCapacityIsIncorrect(int capacity)
        {
            var dto = new CreateTrainTypeDto { Name = "ICE 3", MaxSpeed = 330, Capacity = capacity, Manufacturer = "Siemens", Type = TypeOfTrain.HighSpeed };
            var result = await _validator.ValidateAsync(dto);
            Assert.False(result.IsValid);
            Assert.Contains(result.Errors, e => e.PropertyName == "Capacity");
        }

        [Fact]
        public async Task Should_Fail_WhenManufacturerIsLongerThenHundred()
        {
            var dto = new CreateTrainTypeDto 
            { 
                Name = "ICE 3", MaxSpeed = 330, Capacity = 400, 
                Manufacturer = "abcdefghijabcdefghijabcdefghijabcdefghijabcdefghijabcdefghijabcdefghijabcdefghijabcdefghijabcdefghijk", 
                Type = TypeOfTrain.HighSpeed 
            };
            var result = await _validator.ValidateAsync(dto);
            Assert.False(result.IsValid);
            Assert.Contains(result.Errors, e => e.PropertyName == "Manufacturer");
        }

        [Fact]
        public async Task Should_Fail_WhenTypeIsNotValid()
        {
            var dto = new CreateTrainTypeDto { Name = "ICE 3", MaxSpeed = 330, Capacity = 400, Manufacturer = "Siemens", Type = (TypeOfTrain)99 };
            var result = await _validator.ValidateAsync(dto);
            Assert.False(result.IsValid);
            Assert.Contains(result.Errors, e => e.PropertyName == "Type");
        }
    }
}
