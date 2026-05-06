using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using TaskFlow.Application.DTOs;
using TaskFlow.Application.Interfaces;
using TaskFlow.Domain.Entities;
using TaskFlow.Domain.Enums;
using TaskFlow.Domain.Interfaces;

namespace TaskFlow.Application.Services;

public class AuthService : IAuthService
{
    private readonly IRepository<User> _userRepository;
    private readonly IConfiguration _configuration;
    private readonly ITokenBlacklistService _tokenBlacklistService;

    public AuthService(IRepository<User> userRepository, IConfiguration configuration, ITokenBlacklistService tokenBlacklistService)
    {
        _userRepository = userRepository;
        _configuration = configuration;
        _tokenBlacklistService = tokenBlacklistService;
    }

    public async Task<LoginResponse> RegisterAsync(RegisterRequest request, string ipAddress)
    {
        var existingUsers = await _userRepository.FindAsync(u => u.Email == request.Email);
        if (existingUsers.Any())
            throw new Exception("Email already registered");

        var user = new User
        {
            Name = request.Name,
            Email = request.Email,
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.Password),
            Role = UserRole.User,
            CreatedAt = DateTime.UtcNow,
            IsActive = true
        };

        await _userRepository.AddAsync(user);
        return await GenerateTokensAsync(user);
    }

    public async Task<LoginResponse> LoginAsync(LoginRequest request, string ipAddress)
    {
        var users = await _userRepository.FindAsync(u => u.Email == request.Email);
        var user = users.FirstOrDefault();
        
        if (user == null || !BCrypt.Net.BCrypt.Verify(request.Password, user.PasswordHash))
            throw new Exception("Invalid email or password");

        if (!user.IsActive)
            throw new Exception("Account is deactivated");

        return await GenerateTokensAsync(user);
    }

    public async Task<LoginResponse> RefreshTokenAsync(string refreshToken, string ipAddress)
    {
        var users = await _userRepository.FindAsync(u => u.RefreshToken == refreshToken);
        var user = users.FirstOrDefault();
        
        if (user == null || user.RefreshTokenExpiryTime <= DateTime.UtcNow)
            throw new Exception("Invalid or expired refresh token");

        return await GenerateTokensAsync(user);
    }

    public async Task LogoutAsync(Guid userId, string token)
    {
        var users = await _userRepository.FindAsync(u => u.Id == userId);
        var user = users.FirstOrDefault();
        
        if (user != null)
        {
            user.RefreshToken = null;
            user.RefreshTokenExpiryTime = null;
            await _userRepository.UpdateAsync(user);
        }
        
        await _tokenBlacklistService.BlacklistTokenAsync(token, DateTime.UtcNow.AddHours(1));
    }

    public async Task<UserDto> GetCurrentUserAsync(Guid userId)
    {
        var user = await _userRepository.GetByIdAsync(userId);
        if (user == null)
            throw new Exception("User not found");
            
        return new UserDto
        {
            Id = user.Id,
            Name = user.Name,
            Email = user.Email,
            Role = user.Role.ToString()
        };
    }

    private async Task<LoginResponse> GenerateTokensAsync(User user)
    {
        var accessToken = GenerateAccessToken(user);
        var refreshToken = GenerateRefreshToken();
        
        user.RefreshToken = refreshToken;
        user.RefreshTokenExpiryTime = DateTime.UtcNow.AddDays(7);
        await _userRepository.UpdateAsync(user);
        
        return new LoginResponse
        {
            AccessToken = accessToken,
            RefreshToken = refreshToken,
            ExpiresIn = 3600,
            User = new UserDto
            {
                Id = user.Id,
                Name = user.Name,
                Email = user.Email,
                Role = user.Role.ToString()
            }
        };
    }

    private string GenerateAccessToken(User user)
    {
        var tokenHandler = new JwtSecurityTokenHandler();
        var key = Encoding.ASCII.GetBytes(_configuration["JwtSettings:Secret"] ?? "SuperSecretKey2024!");
        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(new[]
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Email, user.Email),
                new Claim(ClaimTypes.Name, user.Name),
                new Claim(ClaimTypes.Role, user.Role.ToString())
            }),
            Expires = DateTime.UtcNow.AddMinutes(60),
            Issuer = _configuration["JwtSettings:Issuer"],
            Audience = _configuration["JwtSettings:Audience"],
            SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
        };
        
        var token = tokenHandler.CreateToken(tokenDescriptor);
        return tokenHandler.WriteToken(token);
    }

    private string GenerateRefreshToken()
    {
        var randomNumber = new byte[32];
        using var rng = RandomNumberGenerator.Create();
        rng.GetBytes(randomNumber);
        return Convert.ToBase64String(randomNumber);
    }
}
