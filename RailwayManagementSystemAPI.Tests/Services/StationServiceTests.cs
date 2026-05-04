using Microsoft.Extensions.Logging.Abstractions;
using RailwayManagementSystemAPI.Dtos;
using RailwayManagementSystemAPI.Exceptions;
using RailwayManagementSystemAPI.Models;
using RailwayManagementSystemAPI.Services;

namespace RailwayManagementSystemAPI.Tests.Services
{
    public class StationServiceTests : TestBase
    {
        private readonly StationService _service;

        public StationServiceTests()
        {
            _service = new StationService(Context, Mapper, NullLogger<StationService>.Instance);
        }

        // --- GetAllStationsAsync ---

        [Fact]
        public async Task GetAllStationsAsync_ReturnsAllStations()
        {
            // Arrange
            Context.Stations.AddRange(
                new Station { Name = "Belgrade", City = "Belgrade", Country = "Serbia", NumberOfPlatforms = 10 },
                new Station { Name = "Novi Sad", City = "Novi Sad", Country = "Serbia", NumberOfPlatforms = 6 }
            );
            await Context.SaveChangesAsync();

            // Act
            var result = await _service.GetAllStationsAsync(new PaginationQuery());

            // Assert
            Assert.Equal(2, result.Items.Count);
        }

        [Fact]
        public async Task GetAllStationsAsync_ReturnsEmptyList_WhenNoStation()
        {
            var result = await _service.GetAllStationsAsync(new PaginationQuery());

            Assert.Empty(result.Items);
        }

        [Fact]
        public async Task GetAllStationsAsync_ReturnsCorrectData()
        {
            // Arrange
            Context.Stations.Add(
                new Station { Name = "Belgrade", City = "Belgrade", Country = "Serbia", NumberOfPlatforms = 10 }
            );
            await Context.SaveChangesAsync();

            // Act
            var result = await _service.GetAllStationsAsync(new PaginationQuery());

            // Assert
            Assert.Equal("Belgrade", result.Items[0].Name);
            Assert.Equal("Belgrade", result.Items[0].City);
            Assert.Equal("Serbia", result.Items[0].Country);
            Assert.Equal(10, result.Items[0].NumberOfPlatforms);
        }

        // -- GetStationByIdAsync ---

        [Fact]
        public async Task GetStationByIdAsync_ReturnsStation_WhenFound()
        {
            // Arrange
            var station = new Station { Name = "Belgrade", City = "Belgrade", Country = "Serbia", NumberOfPlatforms = 10 };
            Context.Stations.Add(station);
            await Context.SaveChangesAsync();

            // Act
            var result = await _service.GetStationByIdAsync(station.Id);

            //Assert
            Assert.NotNull(result);
            Assert.Equal(station.Id, result.Id);
            Assert.Equal("Belgrade", result.Name);
            Assert.Equal("Belgrade", result.City);
            Assert.Equal("Serbia", result.Country);
            Assert.Equal(10, result.NumberOfPlatforms);
        }

        [Fact]
        public async Task GetStationByIdAsync_ThrowsNotFoundException_WhenNotFound()
        {
            await Assert.ThrowsAsync<NotFoundException>(() =>
                _service.GetStationByIdAsync(0)
            );
        }

        // --- CreateStationAsync ---

        [Fact]
        public async Task CreateStationAsync_CreatesAndReturnsStation()
        {
            // Arrange
            var station = new StationDto 
            {
                Name = "Nis",
                City = "Nis",
                Country = "Serbia",
                NumberOfPlatforms = 5
            };

            // Act
            var result = await _service.CreateStationAsync(station);

            // Assert
            Assert.NotNull(result);
            Assert.True(result.Id > 0);
            Assert.Equal("Nis", result.Name);
            Assert.Equal("Nis", result.City);
            Assert.Equal("Serbia", result.Country);
            Assert.Equal(5, result.NumberOfPlatforms);
        }

        [Fact]
        public async Task CreateStationAsync_PersistsToDatabase()
        {
            // Arrange
            var dto = new StationDto
            {
                Name = "Nis",
                City = "Nis",
                Country = "Serbia",
                NumberOfPlatforms = 5
            };

            // Act
            var result = await _service.CreateStationAsync(dto);

            // Assert
            var inDB = await Context.Stations.FindAsync(result.Id);
            Assert.NotNull(inDB);
            Assert.Equal(result.Id, inDB.Id);
            Assert.Equal("Nis", inDB.Name);
            Assert.Equal("Nis", inDB.City);
            Assert.Equal("Serbia", inDB.Country);
            Assert.Equal(5, inDB.NumberOfPlatforms);
        }

        // --- UpdateStationAsync ---

        [Fact]
        public async Task UpdateStationAsync_UpdatesStation_WhenFound()
        {
            // Arrange
            var station = new Station { Name = "Belgrade", City = "Belgrade", Country = "Serbia", NumberOfPlatforms = 10 };
            Context.Stations.Add(station);
            await Context.SaveChangesAsync();

            var dto = new StationDto
            {
                Name = "Belgrade Updated",
                City = "Belgrade",
                Country = "Serbia",
                NumberOfPlatforms = 12
            };

            // Act
            await _service.UpdateStationAsync(station.Id, dto);

            Context.Entry(station).State = Microsoft.EntityFrameworkCore.EntityState.Detached;

            // Assert
            var updated = await Context.Stations.FindAsync(station.Id);
            Assert.NotNull(updated);
            Assert.Equal("Belgrade Updated", updated.Name);
            Assert.Equal(12, updated.NumberOfPlatforms);
        }

        [Fact]
        public async Task UpdateStationAsync_ThrowsNotFoundException_WhenNotFound()
        {
            // Arrange
            var dto = new StationDto
            {
                Name = "Belgrade",
                City = "Belgrade",
                Country = "Serbia",
                NumberOfPlatforms = 12
            };

            await Assert.ThrowsAsync<NotFoundException>(() =>
                _service.UpdateStationAsync(999, dto)
            );
        }

        // --- DeleteStationAsync ---

        [Fact]
        public async Task DeleteStationAsync_DeletesStations_WhenFound()
        {
            // Arrange
            var station = new Station { Name = "Belgrade", City = "Belgrade", Country = "Serbia", NumberOfPlatforms = 10 };
            Context.Stations.Add(station);
            await Context.SaveChangesAsync();

            // Act
            await _service.DeleteStationAsync(station.Id);

            Context.Entry(station).State = Microsoft.EntityFrameworkCore.EntityState.Detached;

            //Assert
            var deleted = await Context.Stations.FindAsync(station.Id);
            Assert.Null(deleted);
        }

        [Fact]
        public async Task DeleteStationAsync_ThrowsNotFoundException_WhenNotFound()
        {
            await Assert.ThrowsAsync<NotFoundException>(() =>
                _service.DeleteStationAsync(999)
            );
        }
    }
}
