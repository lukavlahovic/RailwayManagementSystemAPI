using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging.Abstractions;
using RailwayManagementSystemAPI.Dtos;
using RailwayManagementSystemAPI.Exceptions;
using RailwayManagementSystemAPI.Models;
using RailwayManagementSystemAPI.Services;
using Route = RailwayManagementSystemAPI.Models.Route;

namespace RailwayManagementSystemAPI.Tests.Services
{
    public class RouteServiceTests : TestBase
    {
        private readonly RouteService _service;

        public RouteServiceTests()
        {
            _service = new RouteService(Context, Mapper, NullLogger<RouteService>.Instance);
        }

        private async Task<List<Station>> CreateStationsAsync()
        {
            var stations = new List<Station>
            {
                new Station { Name = "Belgrade Center", City = "Belgrade", Country = "Serbia", NumberOfPlatforms = 10 },
                new Station { Name = "Novi Sad", City = "Novi Sad", Country = "Serbia", NumberOfPlatforms = 6 },
                new Station { Name = "Subotica", City = "Subotica", Country = "Serbia", NumberOfPlatforms = 4 },
                new Station { Name = "Nis", City = "Nis", Country = "Serbia", NumberOfPlatforms = 5 },
                new Station { Name = "Novi Beograd", City = "Belgrade", Country = "Serbia", NumberOfPlatforms = 3 }
            };

            await Context.Stations.AddRangeAsync(stations);
            await Context.SaveChangesAsync();
            return stations;
        }

        private async Task<Route> CreateRouteAsync()
        {
            var stations = await CreateStationsAsync();

            var route = new Route
            {
                Name = "Belgrade - Subotica",
                RouteStations = new List<RouteStation>
                    {
                        new RouteStation
                        {
                            StationId = stations[0].Id, // belgrade
                            Order = 1,
                            ArrivalOffsetMinutes = 0,
                            StopDuration = 10
                        },
                        new RouteStation
                        {
                            StationId = stations[1].Id, // novi sad
                            Order = 2,
                            ArrivalOffsetMinutes = 45,
                            StopDuration = 5
                        },
                        new RouteStation
                        {
                            StationId = stations[2].Id, // subotica
                            Order = 3,
                            ArrivalOffsetMinutes = 120,
                            StopDuration = 10
                        }
                    }
            };
            Context.Routes.Add(route);
            await Context.SaveChangesAsync();

            return route;
        }

        // --- GetRoutesAsync ---

        [Fact]
        public async Task GetRoutesAsync_ReturnsAllRoutes()
        {
            // Arrange
            var stations = await CreateStationsAsync();

            Context.Routes.AddRange(
                new Route
                {
                    Name = "Belgrade - Subotica",
                    RouteStations = new List<RouteStation>
                    {
                        new RouteStation
                        {
                            StationId = stations.ElementAt(0).Id, // belgrade
                            Order = 1,
                            ArrivalOffsetMinutes = 0,
                            StopDuration = 10
                        },
                        new RouteStation
                        {
                            StationId = stations.ElementAt(1).Id, // novi sad
                            Order = 2,
                            ArrivalOffsetMinutes = 45,
                            StopDuration = 5
                        },
                        new RouteStation
                        {
                            StationId = stations.ElementAt(2).Id, // subotica
                            Order = 3,
                            ArrivalOffsetMinutes = 120,
                            StopDuration = 10
                        }
                    }
                },
                new Route
                {
                    Name = "Belgrade - Nis",
                    RouteStations = new List<RouteStation>
                    {
                        new RouteStation
                        {
                            StationId = stations.ElementAt(0).Id, // belgrade
                            Order = 1,
                            ArrivalOffsetMinutes = 0,
                            StopDuration = 10
                        },
                        new RouteStation
                        {
                            StationId = stations.ElementAt(3).Id, // nis
                            Order = 2,
                            ArrivalOffsetMinutes = 180,
                            StopDuration = 10
                        }
                    }
                }
            );
            await Context.SaveChangesAsync();

            // Act
            var result = await _service.GetRoutesAsync(new PaginationQuery());

            // Assert
            Assert.NotNull(result);
            Assert.Equal(2, result.Items.Count);
        }

        [Fact]
        public async Task GetRoutesAsync_ReturnsCorrectRouteData()
        {
            // Arrange
            var route = await CreateRouteAsync();

            // Act
            var result = await _service.GetRoutesAsync(new PaginationQuery());

            // Assert
            Assert.NotNull(result);
            Assert.Equal("Belgrade - Subotica", result.Items[0].Name);
            Assert.Equal(3, result.Items[0].Stations.Count);
        }

        [Fact]
        public async Task GetRoutesAsync_ReturnsEmptyList_WhenNoRoutes()
        {
            var result = await _service.GetRoutesAsync(new PaginationQuery());

            Assert.Empty(result.Items);
        }

        // --- GetRouteByIdAsync ---

        [Fact]
        public async Task GetRouteByIdAsync_ReturnsRoute_WhenFound()
        {
            // Arrange
            var route = await CreateRouteAsync();

            // Act
            var result = await _service.GetRouteByIdAsync(route.Id);

            // Assert
            Assert.NotNull(result);
            Assert.Equal("Belgrade - Subotica", result.Name);
            Assert.Equal(3, result.Stations.Count);
        }

        [Fact]
        public async Task GetRouteByIdAsync_ThrowsNotFoundException_WhenNotFound()
        {
            await Assert.ThrowsAsync<NotFoundException>(() =>
                _service.GetRouteByIdAsync(999)
            );
        }

        // --- CreateRoute ---

        [Fact]
        public async Task CreateRouteAsync_CreatesAndReturnsRoutes()
        {
            // Arrange
            var stations = await CreateStationsAsync();

            var dto = new CreateRouteDto
            {
                Name = "Belgrade - Subotica",
                Stations = new List<RouteStationDto>
                {
                    new RouteStationDto
                    {
                        StationId = stations.ElementAt(0).Id, // belgrade
                        Order = 1,
                        ArrivalOffsetMinutes = 0,
                        StopDuration = 10
                    },
                    new RouteStationDto
                    {
                        StationId = stations.ElementAt(1).Id, // novi sad
                        Order = 2,
                        ArrivalOffsetMinutes = 45,
                        StopDuration = 5
                    },
                    new RouteStationDto
                    {
                        StationId = stations.ElementAt(2).Id, // subotica
                        Order = 3,
                        ArrivalOffsetMinutes = 120,
                        StopDuration = 10
                    }
                }
            };

            // Act
            var result = await _service.CreateRouteAsync(dto);

            // Assert
            Assert.NotNull(result);
            Assert.True(result.Id > 0);
            Assert.Equal("Belgrade - Subotica", result.Name);
            Assert.Equal(3, result.Stations.Count);
        }

        [Fact]
        public async Task CreateRouteAsync_PersistsToDatabase()
        {
            // Arrange
            var stations = await CreateStationsAsync();

            var dto = new CreateRouteDto
            {
                Name = "Belgrade - Subotica",
                Stations = new List<RouteStationDto>
                {
                    new RouteStationDto
                    {
                        StationId = stations.ElementAt(0).Id, // belgrade
                        Order = 1,
                        ArrivalOffsetMinutes = 0,
                        StopDuration = 10
                    },
                    new RouteStationDto
                    {
                        StationId = stations.ElementAt(1).Id, // novi sad
                        Order = 2,
                        ArrivalOffsetMinutes = 45,
                        StopDuration = 5
                    },
                    new RouteStationDto
                    {
                        StationId = stations.ElementAt(2).Id, // subotica
                        Order = 3,
                        ArrivalOffsetMinutes = 120,
                        StopDuration = 10
                    }
                }
            };

            // Act
            var result = await _service.CreateRouteAsync(dto);

            // Assert
            var inDB = await Context.Routes
                .Include(r => r.RouteStations)
                .FirstOrDefaultAsync(r => r.Id == result.Id);
            Assert.NotNull(inDB);
            Assert.True(inDB.Id > 0);
            Assert.Equal("Belgrade - Subotica", inDB.Name);
            Assert.Equal(3, inDB.RouteStations.Count);
        }

        [Fact]
        public async Task CreateRouteAsync_ThrowsBadRequestException_WhenStationNotFound()
        {
            // Arrange
            var dto = new CreateRouteDto
            {
                Name = "Belgrade - Subotica",
                Stations = new List<RouteStationDto>
                {
                    new RouteStationDto
                    {
                        StationId = 999,
                        Order = 1,
                        ArrivalOffsetMinutes = 0,
                        StopDuration = 10
                    }
                }
            };

            await Assert.ThrowsAsync<BadRequestException>(() =>
                _service.CreateRouteAsync(dto)
            );
        }

        // --- UpdateRouteAsync ---

        [Fact]
        public async Task UpdateRouteAsync_UpdatesRoute_WhenFound()
        {
            // Arrange
            var stations = await CreateStationsAsync();

            var route = new Route
            {
                Name = "Route",
                RouteStations = new List<RouteStation>
                {
                    new RouteStation
                    {
                        StationId = stations.ElementAt(0).Id, // belgrade
                        Order = 1,
                        ArrivalOffsetMinutes = 0,
                        StopDuration = 10
                    },
                    new RouteStation
                    {
                        StationId = stations.ElementAt(1).Id, // novi sad
                        Order = 2,
                        ArrivalOffsetMinutes = 45,
                        StopDuration = 5
                    },
                    new RouteStation
                    {
                        StationId = stations.ElementAt(2).Id, // subotica
                        Order = 3,
                        ArrivalOffsetMinutes = 120,
                        StopDuration = 10
                    }
                }
            };

            Context.Routes.Add(route);
            await Context.SaveChangesAsync();

            var dto = new CreateRouteDto
            {
                Name = "Route Update",
                Stations = new List<RouteStationDto>
                {
                    new RouteStationDto
                    {
                        StationId = stations.ElementAt(0).Id, // belgrade
                        Order = 1,
                        ArrivalOffsetMinutes = 0,
                        StopDuration = 10
                    },
                    new RouteStationDto
                    {
                        StationId = stations.ElementAt(3).Id, // nis
                        Order = 2,
                        ArrivalOffsetMinutes = 180,
                        StopDuration = 10
                    }
                }
            };

            // Act
            await _service.UpdateRouteAsync(route.Id, dto);

            Context.Entry(route).State = Microsoft.EntityFrameworkCore.EntityState.Detached;

            //Assert
            var updated = await Context.Routes
                .Include(r => r.RouteStations)
                .FirstOrDefaultAsync(r => r.Id == route.Id);
            Assert.NotNull(updated);
            Assert.Equal("Route Update", updated.Name);
            Assert.Equal(2, updated.RouteStations.Count);
        }

        [Fact]
        public async Task UpdateRouteAsync_ThrowsNotFoundException_WhenRouteNotFound()
        {
            // Arrange
            var stations = await CreateStationsAsync();

            var dto = new CreateRouteDto
            {
                Name = "Route",
                Stations = new List<RouteStationDto>
                {
                    new RouteStationDto
                    {
                        StationId = stations.ElementAt(0).Id, // belgrade
                        Order = 1,
                        ArrivalOffsetMinutes = 0,
                        StopDuration = 10
                    },
                    new RouteStationDto
                    {
                        StationId = stations.ElementAt(3).Id, // nis
                        Order = 2,
                        ArrivalOffsetMinutes = 180,
                        StopDuration = 10
                    }
                }
            };

            await Assert.ThrowsAsync<NotFoundException>(() =>
                _service.UpdateRouteAsync(999, dto)
            );
        }

        [Fact]
        public async Task UpdateRouteAsync_ThrowsBadRequestException_WhenStationNotFound()
        {
            // Arrange
            var stations = await CreateStationsAsync();

            var route = new Route
            {
                Name = "Route",
                RouteStations = new List<RouteStation>
                {
                    new RouteStation
                    {
                        StationId = stations.ElementAt(0).Id, // belgrade
                        Order = 1,
                        ArrivalOffsetMinutes = 0,
                        StopDuration = 10
                    }
                }
            };

            Context.Routes.Add(route);
            await Context.SaveChangesAsync();

            var dto = new CreateRouteDto
            {
                Name = "Route",
                Stations = new List<RouteStationDto>
                {
                    new RouteStationDto
                    {
                        StationId = 999,
                        Order = 1,
                        ArrivalOffsetMinutes = 0,
                        StopDuration = 10
                    }
                }
            };

            await Assert.ThrowsAsync<BadRequestException>(() =>
                _service.UpdateRouteAsync(route.Id, dto)
            );
        }

        // --- DeleteRouteAsync ---

        [Fact]
        public async Task DeleteRouteAsync_DeletesRoute_WhenFound()
        {
            // Arrange
            var route = await CreateRouteAsync();

            // Act
            await _service.DeleteRouteAsync(route.Id);

            Context.Entry(route).State = Microsoft.EntityFrameworkCore.EntityState.Detached;

            // Assert
            var deleted = await Context.Routes.FindAsync(route.Id);
            Assert.Null(deleted);
        }

        [Fact]
        public async Task DeleteRouteAsync_ThrowsNotFoundException_WhenNotFound()
        {
            await Assert.ThrowsAsync<NotFoundException>(() =>
                _service.DeleteRouteAsync(999)
            );
        }
    }
}
