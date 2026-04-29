using Lab.Application.Common.Interfaces;
using Lab.Domain.Exceptions;
using Microsoft.AspNetCore.Identity;

namespace Lab.Infrastructure.Identity;

public class IdentityService : IIdentityService
{
    private readonly UserManager<ApplicationUser> _userManager;

    public IdentityService(UserManager<ApplicationUser> userManager)
    {
        _userManager = userManager;
    }

    public async Task<IUser> GetUserIdByEmailAsync(string email)
    {
        var user = await _userManager.FindByEmailAsync(email) ?? throw new NotFoundException("Usuário não encontrado.");

        return user;
    }

    public async Task<IUser> AuthenticateAsync(string email, string password)
    {
        var user = await _userManager.FindByEmailAsync(email);
        if (user == null)
            throw new InvalidOperationException("E-mail ou senha incorretos.");

        var success = await _userManager.CheckPasswordAsync(user, password);
        if (!success)
            throw new InvalidOperationException("E-mail ou senha incorretos.");

        return user;
    }

    public async Task<IUser> CreateUserAsync(string email, string password, Guid tenantId)
    {
        var newUser = new ApplicationUser
        {
            UserName = email,
            Email = email,
            TenantId = tenantId
        };

        var result = await _userManager.CreateAsync(newUser, password);

        if (!result.Succeeded)
        {
            var errors = result.Errors.Select(e => new ValidationError(e.Code, e.Description));

            throw new ValidationException(errors);
        }

        return newUser;
    }

    public async Task<IUser> GetUserByIdAsync(Guid userId)
    {
        var user = await _userManager.FindByIdAsync(userId.ToString()) ?? throw new NotFoundException("Usuário não encontrado.");

        return user;
    }

}
