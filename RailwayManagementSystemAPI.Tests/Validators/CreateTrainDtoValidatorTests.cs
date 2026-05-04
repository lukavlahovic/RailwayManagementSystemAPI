using RailwayManagementSystemAPI.Dtos;
using RailwayManagementSystemAPI.Validators;

namespace RailwayManagementSystemAPI.Tests.Validators
{
    public class CreateTrainDtoValidatorTests
    {
        private readonly CreateTrainDtoValidator _validator = new();

        [Fact]
        public async Task Should_Pass_WhenAllFieldsAreValid()
        {
            var dto = new CreateTrainDto { SerialNumber = "SRB-001", TrainTypeId = 1 };
            var result = await _validator.ValidateAsync(dto);
            Assert.True(result.IsValid);
        }

        [Theory]
        [InlineData(0)]
        [InlineData(-1)]
        public async Task Should_Fail_WhenTrainTypeIsLessThenZero(int typeT)
        {
            var dto = new CreateTrainDto { SerialNumber = "SRB-001", TrainTypeId = typeT };
            var result = await _validator.ValidateAsync(dto);
            Assert.False(result.IsValid);
            Assert.Contains(result.Errors, e => e.PropertyName == "TrainTypeId");
        }

        [Fact]
        public async Task Should_Fail_WhenSerialNumberIsEmpty()
        {
            var dto = new CreateTrainDto { SerialNumber = "", TrainTypeId = 1 };
            var result = await _validator.ValidateAsync(dto);
            Assert.False(result.IsValid);
            Assert.Contains(result.Errors, e => e.PropertyName == "SerialNumber");
        }

        [Fact]
        public async Task Should_Fail_WhenSerialNumberIsLongerThenFifty()
        {
            var dto = new CreateTrainDto { SerialNumber = "abcdefghijabcdefghijabcdefghijabcdefghijabcdefghijk", TrainTypeId = 1 };
            var result = await _validator.ValidateAsync(dto);
            Assert.False(result.IsValid);
            Assert.Contains(result.Errors, e => e.PropertyName == "SerialNumber");
        }
    }
}
