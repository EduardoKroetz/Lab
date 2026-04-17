using Lab.Api.Application.DTOs;
using Lab.Api.Application.DTOs.Auth;
using Lab.Api.Application.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Lab.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly AuthService _authService;

    public AuthController(AuthService authService)
    {
        _authService = authService;
    }

    [HttpPost("login")]
    [AllowAnonymous]
    public async Task<IActionResult> Login([FromBody] LoginUserDto dto)
    {
        var token = await _authService.LoginAsync(dto);

        return Ok(new { token });
    }

    [HttpPost("register")]
    [AllowAnonymous]
    public async Task<IActionResult> RegisterAsync([FromBody] RegisterUserDto dto)
    {
        await _authService.RegisterAsync(dto);

        return Ok(new ResponseDto());
    }
}
