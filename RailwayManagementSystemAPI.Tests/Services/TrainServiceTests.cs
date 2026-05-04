using Microsoft.Extensions.Logging.Abstractions;
using RailwayManagementSystemAPI.Dtos;
using RailwayManagementSystemAPI.Exceptions;
using RailwayManagementSystemAPI.Models;
using RailwayManagementSystemAPI.Services;

namespace RailwayManagementSystemAPI.Tests.Services
{
    public class TrainServiceTests : TestBase
    {
        private readonly TrainService _service;

        public TrainServiceTests()
        {
            _service = new TrainService(Context, Mapper, NullLogger<TrainService>.Instance);
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

        // --- GetAllTrainsAsync ---

        [Fact]
        public async Task GetAllTrainsAsync_ReturnsAllTrains()
        {
            // Arrange
            var trainType = await CreateTrainTypeAsync();
            Context.Trains.AddRange(
                new Train { SerialNumber = "SRB-001", TrainTypeId = trainType.Id },
                new Train { SerialNumber = "SRB-002", TrainTypeId = trainType.Id }
            );
            await Context.SaveChangesAsync();

            // Act
            var result = await _service.GetAllTrainsAsync(new PaginationQuery());

            //Assert
            Assert.Equal(2, result.Items.Count);
        }

        [Fact]
        public async Task GetAllTrainsAsync_ReturnsEmptyList_WhenNoTrains()
        {
            var result = await _service.GetAllTrainsAsync(new PaginationQuery());

            Assert.Empty(result.Items);
        }

        [Fact]
        public async Task GetAllTrainsAsync_ReturnsCorrectTrainTypeData()
        {
            // Arrange
            var trainType = await CreateTrainTypeAsync();
            Context.Trains.Add(new Train { SerialNumber = "SRB-001", TrainTypeId = trainType.Id });
            await Context.SaveChangesAsync();

            // Act
            var result = await _service.GetAllTrainsAsync(new PaginationQuery());

            // Assert
            Assert.Equal("ICE 3", result.Items[0].TrainType.Name);
            Assert.Equal(330, result.Items[0].TrainType.MaxSpeed);
            Assert.Equal(TypeOfTrain.HighSpeed, result.Items[0].TrainType.TypeOfTrain);
        }

        // --- GetTrainByIdAsync ---

        [Fact]
        public async Task GetTrainByIdAsync_ReturnsTrain_WhenFound()
        {
            // Arrange
            var trainType = await CreateTrainTypeAsync();
            var train = new Train { SerialNumber = "SRB-001", TrainTypeId = trainType.Id };
            Context.Trains.Add(train);
            await Context.SaveChangesAsync();

            // Act
            var result = await _service.GetTrainByIdAsync(train.Id);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(train.Id, result.Id);
            Assert.Equal("SRB-001", result.SerialNumber);
            Assert.Equal(train.TrainTypeId, result.TrainType.Id);
            Assert.Equal("ICE 3", result.TrainType.Name);
        }

        [Fact]
        public async Task GetTrainByIdAsync_ThrowsNotFoundException_WhenNotFound()
        {
            await Assert.ThrowsAsync<NotFoundException>(() =>
                _service.GetTrainByIdAsync(999)
            );
        }

        // --- CreateTrainAsync ---

        [Fact]
        public async Task CreateTrainAsync_CreatesAndReturnsTrain_WhenTrainTypeExists()
        {
            // Arrange
            var trainType = await CreateTrainTypeAsync();
            var dto = new CreateTrainDto
            {
                SerialNumber = "SRB-002",
                TrainTypeId = trainType.Id
            };

            //Act
            var result = await _service.CreateTrainAsync(dto);

            //Assert
            Assert.NotNull(result);
            Assert.True(result.Id > 0);
            Assert.Equal("SRB-002", result.SerialNumber);
            Assert.Equal(trainType.Id, result.TrainType.Id);
            Assert.Equal("ICE 3", result.TrainType.Name);
        }

        [Fact]
        public async Task CreateTrainAsync_PersistsToDatabase()
        {
            // Arrange
            var trainType = await CreateTrainTypeAsync();
            var dto = new CreateTrainDto
            {
                SerialNumber = "SRB-002",
                TrainTypeId = trainType.Id
            };

            // Act
            var result = await _service.CreateTrainAsync(dto);

            //Assert
            var inDB = await Context.Trains.FindAsync(result.Id);
            Assert.NotNull(inDB);
            Assert.Equal("SRB-002", inDB.SerialNumber);
            Assert.Equal(trainType.Id, inDB.TrainTypeId);
        }

        [Fact]
        public async Task CreateTrainAsync_ThrowsBadRequestException_WhenTrainTypeDoesNotExist()
        {
            // Arrange
            var dto = new CreateTrainDto { SerialNumber = "SRB-002", TrainTypeId = 999 };

            await Assert.ThrowsAsync<BadRequestException>(() => 
                _service.CreateTrainAsync(dto)
            );
        }

        // --- UpdateTrainAsync ---

        [Fact]
        public async Task UpdateTrainAsync_UpdatesTrain_WhenFound()
        {
            // Arrange
            var trainType = await CreateTrainTypeAsync();
            var train = new Train { SerialNumber = "SRB-001", TrainTypeId = trainType.Id };
            Context.Trains.Add(train);
            await Context.SaveChangesAsync();

            var newTrainType = new TrainType
            {
                Name = "Coradia",
                MaxSpeed = 200,
                Capacity = 300,
                Manufacturer = "Alstom",
                Type = TypeOfTrain.Passenger
            };
            Context.TrainTypes.Add(newTrainType);
            await Context.SaveChangesAsync();

            var dto = new CreateTrainDto
            {
                SerialNumber = "SRB-001-UPDATED",
                TrainTypeId = newTrainType.Id
            };

            // Act
            await _service.UpdateTrainAsync(train.Id, dto);

            Context.Entry(train).State = Microsoft.EntityFrameworkCore.EntityState.Detached;

            // Assert
            var updated = await Context.Trains.FindAsync(train.Id);
            Assert.Equal("SRB-001-UPDATED", updated!.SerialNumber);
            Assert.Equal(newTrainType.Id, updated.TrainTypeId);
        }

        [Fact]
        public async Task UpdateTrainAsync_ThrowsNotFoundException_WhenTrainNotFound()
        {
            // Arrange
            var trainType = await CreateTrainTypeAsync();
            var dto = new CreateTrainDto
            {
                SerialNumber = "SRB-001",
                TrainTypeId = trainType.Id
            };

            // Act & Assert
            await Assert.ThrowsAsync<NotFoundException>(() =>
                _service.UpdateTrainAsync(999, dto));
        }

        [Fact]
        public async Task UpdateTrainAsync_ThrowsBadRequestException_WhenTrainTypeDoesNotExist()
        {
            // Arrange
            var trainType = await CreateTrainTypeAsync();
            var train = new Train { SerialNumber = "SRB-001", TrainTypeId = trainType.Id };
            Context.Trains.Add(train);
            await Context.SaveChangesAsync();

            var dto = new CreateTrainDto
            {
                SerialNumber = "SRB-001",
                TrainTypeId = 999
            };

            // Act & Assert
            await Assert.ThrowsAsync<BadRequestException>(() =>
                _service.UpdateTrainAsync(train.Id, dto));
        }

        // ─── DeleteTrainAsync ──────────────────────────────────────────────

        [Fact]
        public async Task DeleteTrainAsync_DeletesTrain_WhenFound()
        {
            // Arrange
            var trainType = await CreateTrainTypeAsync();
            var train = new Train { SerialNumber = "SRB-001", TrainTypeId = trainType.Id };
            Context.Trains.Add(train);
            await Context.SaveChangesAsync();

            // Act
            await _service.DeleteTrainAsync(train.Id);

            Context.Entry(train).State = Microsoft.EntityFrameworkCore.EntityState.Detached;

            // Assert
            var deleted = await Context.Trains.FindAsync(train.Id);
            Assert.Null(deleted);
        }

        [Fact]
        public async Task DeleteTrainAsync_ThrowsNotFoundException_WhenNotFound()
        {
            // Act & Assert
            await Assert.ThrowsAsync<NotFoundException>(() =>
                _service.DeleteTrainAsync(999));
        }
    }
}
