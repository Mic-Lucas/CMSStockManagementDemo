using DealershipStockManagement.Application.Dtos;
using DealershipStockManagement.Application.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DealershipStockManagement.Api.Controllers;

[ApiController]
[Route("api/auth")]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;

    public AuthController(IAuthService authService)
    {
        _authService = authService;
    }

    [HttpPost("register")]
    [AllowAnonymous]
    public async Task<IActionResult> Register([FromBody] RegisterDto dto)
    {
        var result = await _authService.RegisterAsync(dto);
        if (!result.Success) return BadRequest(result.Message);

        // Automatically log the user in and generate JWT
        var token = await _authService.LoginAsync(new LoginDto
        {
            Username = dto.Username,
            Password = dto.Password
        });

        return Ok(new { Token = token });
    }

    [HttpPost("login")]
    [AllowAnonymous]
    public async Task<IActionResult> Login([FromBody] LoginDto dto)
    {
        var token = await _authService.LoginAsync(dto);
        if (string.IsNullOrEmpty(token)) return Unauthorized();
        return Ok(new { Token = token });
    }
}