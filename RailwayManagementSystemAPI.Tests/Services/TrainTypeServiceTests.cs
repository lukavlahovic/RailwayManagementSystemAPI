using Microsoft.Extensions.Logging.Abstractions;
using RailwayManagementSystemAPI.Dtos;
using RailwayManagementSystemAPI.Exceptions;
using RailwayManagementSystemAPI.Models;
using RailwayManagementSystemAPI.Services;

namespace RailwayManagementSystemAPI.Tests.Services
{
    public class TrainTypeServiceTests : TestBase
    {
        private readonly TrainTypeService _service;

        public TrainTypeServiceTests()
        {
            _service = new TrainTypeService(Context, Mapper, NullLogger<TrainTypeService>.Instance);
        }

        private async Task<TrainType> CreateTrainTypeAsync()
        {
            var trainType = new TrainType
            {
                Name = "ICE 3",
                MaxSpeed = 330,
                Capacity = 400,
                Manufacturer = "Siemens",
                Type = TypeOfTrain.HighSpeed
            };
            Context.TrainTypes.Add(trainType);
            await Context.SaveChangesAsync();
            return trainType;
        }

        // --- GetAllTrainTypesAsync ---

        [Fact]
        public async Task GetAllTrainTypesAsync_ReturnsAllTrainTypes()
        {
            // Arrange
            Context.TrainTypes.AddRange(
                new TrainType { Name = "ICE 3", MaxSpeed = 330, Capacity = 400, Manufacturer = "Siemens", Type = TypeOfTrain.HighSpeed},
                new TrainType { Name = "Coradia Stream", MaxSpeed = 200, Capacity = 300, Manufacturer = "Alstom", Type = TypeOfTrain.Passenger }
            );
            await Context.SaveChangesAsync();

            // Act
            var result = await _service.GetAllTrainTypesAsync();

            //Assert
            Assert.Equal(2, result.Count);
        }

        [Fact]
        public async Task GetAllTrainTypesAsync_ReturnsCorrectData()
        {
            // Arrange
            Context.TrainTypes.Add(
                new TrainType { Name = "ICE 3", MaxSpeed = 330, Capacity = 400, Manufacturer = "Siemens", Type = TypeOfTrain.HighSpeed }
            );
            await Context.SaveChangesAsync();

            // Act
            var result = await _service.GetAllTrainTypesAsync();

            // Assert
            Assert.Equal("ICE 3", result[0].Name);
            Assert.Equal(330, result[0].MaxSpeed);
            Assert.Equal(400, result[0].Capacity);
            Assert.Equal("Siemens", result[0].Manufacturer);
            Assert.Equal(TypeOfTrain.HighSpeed, result[0].TypeOfTrain);
        }

        [Fact]
        public async Task GetAllTrainTypesAsync_ReturnsEmptyList_WhenNoTrains()
        {
            var result = await _service.GetAllTrainTypesAsync();

            Assert.Empty(result);
        }

        // --- GetTrainTypeByIdAsync ---

        [Fact]
        public async Task GetTrainTypeByIdAsync_ReturnsTrainType_WhenFound()
        {
            // Arrange
            var trainType = await CreateTrainTypeAsync();

            // Act
            var result = await _service.GetTrainTypeByIdAsync(trainType.Id);

            // Assert
            Assert.NotNull(result);
            Assert.True(result.Id > 0);
            Assert.Equal("ICE 3", result.Name);
            Assert.Equal(330, result.MaxSpeed);
            Assert.Equal(400, result.Capacity);
            Assert.Equal("Siemens", result.Manufacturer);
            Assert.Equal(TypeOfTrain.HighSpeed, result.TypeOfTrain);
        }

        [Fact]
        public async Task GetTrainTypeByIdAsync_ThrowsNotFoundException_WhenNotFound()
        {
            await Assert.ThrowsAsync<NotFoundException>(() =>
                _service.GetTrainTypeByIdAsync(999)
            );
        }

        // --- CreateTrainTypeAsync ---

        [Fact]
        public async Task CreateTrainTypeAsync_CreatesAndReturnsTrainType()
        {
            // Arrange
            var dto = new CreateTrainTypeDto
            {
                Name = "ICE 3",
                MaxSpeed = 330,
                Capacity = 400,
                Manufacturer = "Siemens",
                Type = TypeOfTrain.HighSpeed
            };

            // Act
            var result = await _service.CreateTrainTypeAsync(dto);

            //Assert
            Assert.NotNull(result);
            Assert.True(result.Id > 0);
            Assert.Equal("ICE 3", result.Name);
            Assert.Equal(330, result.MaxSpeed);
            Assert.Equal(400, result.Capacity);
            Assert.Equal("Siemens", result.Manufacturer);
            Assert.Equal(TypeOfTrain.HighSpeed, result.TypeOfTrain);
        }

        [Fact]
        public async Task CreateTrainTypeAsync_PersistsToDatabase()
        {
            // Arrange
            var dto = new CreateTrainTypeDto
            {
                Name = "ICE 3",
                MaxSpeed = 330,
                Capacity = 400,
                Manufacturer = "Siemens",
                Type = TypeOfTrain.HighSpeed
            };

            // Act
            var result = await _service.CreateTrainTypeAsync(dto);

            //Assert
            var inDB = await Context.TrainTypes.FindAsync(result.Id);
            Assert.NotNull(inDB);
            Assert.Equal(result.Id, inDB.Id);
            Assert.Equal("ICE 3", inDB.Name);
            Assert.Equal(330, inDB.MaxSpeed);
            Assert.Equal(400, inDB.Capacity);
            Assert.Equal("Siemens", inDB.Manufacturer);
            Assert.Equal(TypeOfTrain.HighSpeed, inDB.Type);
        }
    }
}
