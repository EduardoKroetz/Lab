using Lab.Api.Application.DTOs.Auth;
using Lab.Api.Domain.Entities;
using Lab.Api.Domain.Exceptions;
using Lab.Api.Infrastructure.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Lab.Api.Application.Services;

public class AuthService
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly TokenService _tokenService;

    public AuthService(UserManager<ApplicationUser> userManager, TokenService tokenService)
    {
        _userManager = userManager;
        _tokenService = tokenService;
    }

    public async Task<string> LoginAsync(LoginUserDto dto)
    {
        var user = await _userManager.FindByEmailAsync(dto.Email);
        if (user == null)
            throw new BadRequestException("Usuário não encontrado.");

        var success = await _userManager.CheckPasswordAsync(user, dto.Password);
        if (!success)
            throw new BadRequestException("Credenciais inválidas.");

        return _tokenService.GenerateAccessToken(user);
    }

    public async Task RegisterAsync(RegisterUserDto dto)
    {
        var userExists = await _userManager.Users.AnyAsync(u => u.Email == dto.Email);
        if (userExists)
            throw new BadRequestException("O e-mail informado já foi cadastrado.");

        var newUser = new ApplicationUser
        {
            UserName = dto.Email,
            Email = dto.Email
        };

        var result = await _userManager.CreateAsync(newUser, dto.Password);
        if (!result.Succeeded)
            throw new BadRequestException(string.Join(" ", result.Errors.Select(e => e.Description)));
    }
}
