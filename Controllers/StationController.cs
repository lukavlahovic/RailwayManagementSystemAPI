using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RailwayManagementSystemAPI.Dtos;
using RailwayManagementSystemAPI.Models;
using RailwayManagementSystemAPI.Services;

namespace RailwayManagementSystemAPI.Controllers
{
    [ApiController]
    [Route("api/stations")]
    public class StationController : ControllerBase
    {
        private readonly IStationService _stationService;

        public StationController(IStationService stationService)
        {
            _stationService = stationService;
        }

        /// <summary>
        /// Retrieves all stations from the data store.
        /// </summary>
        /// <returns>An IActionResult containing the list of stations.</returns>
        [HttpGet]
        public async Task<IActionResult> GetAllStations()
        {
            var stations = await _stationService.GetAllStationsAsync();
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
            var station = await _stationService.GetStationByIdAsync(id);

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
            var station = await _stationService.CreateStationAsync(stationDto);

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
            var isUpdated = await _stationService.UpdateStationAsync(id, stationDto);

            if (!isUpdated)
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
            var isDeleted = await _stationService.DeleteStationAsync(id);

            if (!isDeleted)
                return NotFound();

            return NoContent();
        }
    }
}
