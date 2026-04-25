using RailwayManagementSystemAPI.Dtos;
using RailwayManagementSystemAPI.Models;

namespace RailwayManagementSystemAPI.Services
{
    public interface IAuthService
    {
        Task<AuthResponseDto> Register(RegisterDto dto, UserRole role);
        Task<AuthResponseDto> Login(LoginDto dto);
        Task<bool> AdminExists();
    }
}
