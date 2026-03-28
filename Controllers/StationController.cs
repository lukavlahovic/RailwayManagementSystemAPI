using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RailwayManagementSystemAPI.Data;
using RailwayManagementSystemAPI.Dtos;
using RailwayManagementSystemAPI.Models;

namespace RailwayManagementSystemAPI.Controllers
{
    [ApiController]
    [Route("api/stations")]
    public class StationController : ControllerBase
    {
        private readonly RailwayContext _context;

        public StationController(RailwayContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Retrieves all stations from the data store.
        /// </summary>
        /// <returns>An IActionResult containing the list of stations.</returns>
        [HttpGet]
        public async Task<IActionResult> GetAllStations()
        {
            var stations = await _context.Stations.ToListAsync();
            return Ok(stations);
        }

        /// <summary>
        /// Retrieves a station with the specified identifier.
        /// </summary>
        /// <param name="id">The unique identifier of the station.</param>
        /// <returns>An IActionResult containing the station if found; otherwise, NotFound.</returns>
        [HttpGet("{id}")]
        public async Task<IActionResult> GetStationById(int id)
        {
            var station = await _context.Stations.FindAsync(id);

            if (station == null)
                return NotFound();

            return Ok(station);
        }

        /// <summary>
        /// Creates a new station and returns the created resource.
        /// </summary>
        /// <param name="stationDto">The data for the station to create.</param>
        /// <returns>A 201 Created response with the created station.</returns>
        [HttpPost]
        public async Task<IActionResult> CreateStation([FromBody] StationDto stationDto)
        {
            var station = new Station
            {
                Name = stationDto.Name,
                City = stationDto.City,
                Country = stationDto.Country,
                NumberOfPlatforms = stationDto.NumberOfPlatforms
            };

            await _context.Stations.AddAsync(station);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetStationById), new { id = station.Id }, station);
        }

        /// <summary>
        /// Updates the details of an existing station.
        /// </summary>
        /// <param name="id">The unique identifier of the station to update.</param>
        /// <param name="stationDto">The updated station data.</param>
        /// <returns>An HTTP 204 response if the update is successful; HTTP 404 if the station is not found.</returns>
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateStation(int id, [FromBody] StationDto stationDto)
        {
            var rowsAffected = await _context.Stations
                .Where(s => s.Id == id)
                .ExecuteUpdateAsync(setter => setter
                    .SetProperty(s => s.Name, stationDto.Name)
                    .SetProperty(s => s.City, stationDto.City)
                    .SetProperty(s => s.Country, stationDto.Country)
                    .SetProperty(s => s.NumberOfPlatforms, stationDto.NumberOfPlatforms)
                );

            if (rowsAffected == 0)
                return NotFound();

            return NoContent();
        }

        /// <summary>
        /// Deletes the station with the specified identifier.
        /// </summary>
        /// <param name="id">The identifier of the station to delete.</param>
        /// <returns>A 204 No Content response if the station was deleted; otherwise, a 404 Not Found response if the station
        /// does not exist.</returns>
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteStation(int id)
        {
            var rowsAffected = await _context.Stations
                .Where(s => s.Id == id)
                .ExecuteDeleteAsync();

            if (rowsAffected == 0)
                return NotFound();

            return NoContent();
        }
    }
}
