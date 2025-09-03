using DealershipStockManagement.Application.Dtos;
using DealershipStockManagement.Domain.Entities;
using DealershipStockManagement.Domain.Interfaces;

namespace DealershipStockManagement.Application.Services;

public class AuthService : IAuthService
{
    private readonly IUserRepository _userRepository;
    private readonly JwtService _jwtService; 

    public AuthService(IUserRepository userRepository, JwtService jwtService)
    {
        _userRepository = userRepository;
        _jwtService = jwtService;
    }

    public async Task<(bool Success, string Message)> RegisterAsync(RegisterDto dto)
    {
        var existingUser = await _userRepository.GetByUsernameAsync(dto.Username);
        if (existingUser != null)
            return (false, "Username already exists");

        // Hash password
        var (hash, salt) = PasswordHasher.HashPassword(dto.Password);

        var user = new User
        {
            Username = dto.Username,
            PasswordHash = hash,
            PasswordSalt = salt, 
            Role = dto.Role ?? "User"
        };

        await _userRepository.AddAsync(user);
        return (true, "User registered successfully");
    }

    public async Task<string?> LoginAsync(LoginDto dto)
    {
        var user = await _userRepository.GetByUsernameAsync(dto.Username);
        if (user == null) return null;

        // Verify password using stored hash and salt
        bool verified = PasswordHasher.VerifyPassword(dto.Password, user.PasswordHash, user.PasswordSalt);
        if (!verified) return null;

        // Generate JWT token
        return _jwtService.GenerateToken(user);
    }
}