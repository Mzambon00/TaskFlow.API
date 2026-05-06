using TaskFlow.Application.DTOs;

namespace TaskFlow.Application.Interfaces;

public interface IAuthService
{
    Task<LoginResponse> RegisterAsync(RegisterRequest request, string ipAddress);
    Task<LoginResponse> LoginAsync(LoginRequest request, string ipAddress);
    Task<LoginResponse> RefreshTokenAsync(string refreshToken, string ipAddress);
    Task LogoutAsync(Guid userId, string token);
    Task<UserDto> GetCurrentUserAsync(Guid userId);
}
