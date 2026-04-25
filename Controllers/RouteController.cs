using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RailwayManagementSystemAPI.Dtos;
using RailwayManagementSystemAPI.Models;
using RailwayManagementSystemAPI.Services;

namespace RailwayManagementSystemAPI.Controllers
{
    [ApiController]
    [Route("api/routes")]
    public class RouteController : ControllerBase
    {
        private readonly IRouteService _routeService;

        public RouteController(IRouteService routeService)
        {
            _routeService = routeService;
        }

        /// <summary>
        /// Creates a new route with the specified stations.
        /// </summary>
        /// <param name="dto">The data transfer object containing route details and associated stations.</param>
        /// <returns>A 201 Created response with the created route, or 400 Bad Request if any stations are not found.</returns>
        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> CreateRoute([FromBody] CreateRouteDto dto)
        {
            var route = await _routeService.CreateRoute(dto);

            return CreatedAtAction(nameof(GetRouteById), new { id = route.Id }, route);
        }

        /// <summary>
        /// Retrieves a route by its unique identifier.
        /// </summary>
        /// <param name="id">The unique identifier of the route.</param>
        /// <returns>An IActionResult containing the route data if found; otherwise, a NotFound result.</returns>
        [HttpGet("{id}")]
        public async Task<IActionResult> GetRouteById(int id)
        {
            var response = await _routeService.GetRouteByIdAsync(id);

            return Ok(response);
        }

        /// <summary>
        /// Retrieves a list of routes with their associated stations.
        /// </summary>
        /// <returns>An IActionResult containing a collection of RouteResponseDto objects.</returns>
        [HttpGet]
        public async Task<IActionResult> GetRoutes()
        {
            var routes = await _routeService.GetRoutesAsync();

            return Ok(routes);
        }

        /// <summary>
        /// Updates an existing route and its stations.
        /// </summary>
        /// <param name="id">The identifier of the route to update.</param>
        /// <param name="dto">The data transfer object containing updated route information.</param>
        /// <returns>An IActionResult indicating the result of the operation.</returns>
        [HttpPut("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> UpdateRoute(int id, [FromBody] CreateRouteDto dto)
        {
            await _routeService.UpdateRouteAsync(id, dto);

            return NoContent();
        }

        /// <summary>
        /// Deletes the route with the specified identifier.
        /// </summary>
        /// <param name="id">The identifier of the route to delete.</param>
        /// <returns>A 204 No Content response if the route was deleted; otherwise, a 404 Not Found response.</returns>
        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteRoute(int id)
        {
            await _routeService.DeleteRouteAsync(id);

            return NoContent();
        }
    }
}
