using InventorySystem.API.DTOs;
using InventorySystem.API.DTOs.Auth;

namespace InventorySystem.API.Services.Auth
{
    public interface IAuthService
    {
        Task<UserResponseDto> RegisterAsync(RegisterDto dto);
        Task<AuthResponseDto?> LoginAsync(LoginDto dto);
    }
}
