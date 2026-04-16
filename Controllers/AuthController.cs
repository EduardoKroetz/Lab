using Lab.Api.Application.DTOs.Auth;
using Lab.Api.Domain.Entities;
using Lab.Api.DTOs;
using Lab.Api.Infrastructure.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Lab.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly TokenService _tokenService;

    public AuthController(UserManager<ApplicationUser> userManager, TokenService tokenService)
    {
        _userManager = userManager;
        _tokenService = tokenService;
    }

    [HttpPost("login")]
    [AllowAnonymous]
    public async Task<IActionResult> Login([FromBody] LoginUserDto dto)
    {
        var user = await _userManager.FindByEmailAsync(dto.Email);
        if (user == null)
            return BadRequest(new ResponseDto("Usuário não encontrado."));

        var success = await _userManager.CheckPasswordAsync(user, dto.Password);
        if (!success)
            return BadRequest(new ResponseDto("Credenciais inválidas."));

        var token = _tokenService.GenerateAccessToken(user);

        return Ok(new { token });
    }

    [HttpPost("register")]
    [AllowAnonymous]
    public async Task<IActionResult> RegisterAsync([FromBody] RegisterUserDto dto)
    {
        var userExists = await _userManager.Users.AnyAsync(u => u.Email == dto.Email);
        if (userExists)
            return BadRequest(new ResponseDto("O e-mail informado já foi cadastrado."));

        var newUser = new ApplicationUser
        {
            UserName = dto.Email,
            Email = dto.Email
        };

        var result = await _userManager.CreateAsync(newUser, dto.Password);
        if (!result.Succeeded)
            return BadRequest(new ResponseDto(result.Errors.Select(e => e.Description).ToList()));

        return Ok(new ResponseDto());
    }

}
