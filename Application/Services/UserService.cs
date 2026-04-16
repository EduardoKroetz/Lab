using Lab.Api.Application.DTOs.Users;
using Lab.Api.Domain.Entities;
using Lab.Api.Domain.Exceptions;
using Microsoft.AspNetCore.Identity;

namespace Lab.Api.Application.Services;

public class UserService
{
    private readonly UserManager<ApplicationUser> _userManager;

    public UserService(UserManager<ApplicationUser> userManager)
    {
        _userManager = userManager;
    }

    public async Task<GetCurrentUserDto> GetCurrentUserAsync(Guid userId)
    {
        var user = await _userManager.FindByIdAsync(userId.ToString());
        if (user == null)
            throw new NotFoundException("Usuário não encontrado");

        var dto = new GetCurrentUserDto
        {
            Id = user.Id,
            UserName = user.UserName,
            Email = user.Email,
            Phone = user.PhoneNumber
        };

        return dto;
    }

}
