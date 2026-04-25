using Microsoft.AspNetCore.Mvc;
using RailwayManagementSystemAPI.Dtos;
using RailwayManagementSystemAPI.Models;
using RailwayManagementSystemAPI.Services;

namespace RailwayManagementSystemAPI.Controllers
{
    [ApiController]
    [Route("api/auth")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;

        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }

        [HttpPost("register/admin")]
        public async Task<IActionResult> RegisterAdmin([FromBody] RegisterDto dto)
        {
            var response = await _authService.Register(dto, UserRole.Admin);

            return Ok(response);
        }

        [HttpPost("register/operator")]
        public async Task<IActionResult> RegisterOperator([FromBody] RegisterDto dto)
        {
            var response = await _authService.Register(dto, UserRole.Operator);

            return Ok(response);
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginDto dto)
        {
            var response = await _authService.Login(dto);

            return Ok(response);
        }
    }
}
