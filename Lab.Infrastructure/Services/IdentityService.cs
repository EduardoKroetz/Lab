using Lab.Application.Common.Interfaces;
using Lab.Domain.Common.Models;
using Lab.Infrastructure.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Lab.Infrastructure.Services;

public class IdentityService : IIdentityService
{
    private readonly UserManager<ApplicationUser> _userManager;

    public IdentityService(UserManager<ApplicationUser> userManager)
    {
        _userManager = userManager;
    }

    public async Task<Result<IUser>> GetUserIdByEmailAsync(string email)
    {
        var user = await _userManager.FindByEmailAsync(email);
        if (user == null)
            return Result<IUser>.Failure("Usuário não encontrado.");

        return Result<IUser>.Success(user);
    }


    public async Task<Result<IUser>> AuthenticateAsync(string email, string password)
    {
        var user = await _userManager.FindByEmailAsync(email);
        if (user == null)
            return Result<IUser>.Failure("E-mail ou senha incorretos.");

        var success = await _userManager.CheckPasswordAsync(user, password);
        if (!success)
            return Result<IUser>.Failure("E-mail ou senha incorretos.");

        return Result<IUser>.Success(user);
    }

    public async Task<Result<IUser>> CreateUserAsync(string email, string password, Guid tenantId)
    {
        var userExists = await _userManager.Users.AnyAsync(u => u.Email == email);
        if (userExists)
            return Result<IUser>.Failure("O e-mail informado já foi cadastrado.");

        var newUser = new ApplicationUser
        {
            UserName = email,
            Email = email
        };

        var result = await _userManager.CreateAsync(newUser, password);
        if (!result.Succeeded)
            throw new InvalidOperationException("Não foi possível criar o usuário.");

        return Result<IUser>.Success(newUser);
    }

    public async Task<Result<IUser>> GetUserByIdAsync(Guid userId)
    {
        var user = await _userManager.FindByIdAsync(userId.ToString());
        if (user == null)
            return Result<IUser>.Failure("Usuário não encontrado.");

        return Result<IUser>.Success(user);
    }

}
