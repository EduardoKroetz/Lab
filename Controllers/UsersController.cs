using Lab.Api.DTOs;
using Lab.Api.DTOs.Users;
using Lab.Api.Entities;
using Lab.Api.Extensions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace Lab.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class UsersController : ControllerBase
{
    private readonly UserManager<ApplicationUser> _userManager;

    public UsersController(UserManager<ApplicationUser> userManager)
    {
        _userManager = userManager;
    }

    [HttpGet("current")] 
    public async Task<IActionResult> GetCurrentUserAsync()
    {
        var userId = User.GetUserId();

        var user = await _userManager.FindByIdAsync(userId.ToString());
        if (user == null)
            return NotFound(new ResponseDto("Usuário não encontrado"));

        var dto = new GetCurrentUserDto
        {
            Id = user.Id,
            UserName = user.UserName,
            Email = user.Email,
            Phone = user.PhoneNumber
        };

        return Ok(new ResponseDto(dto));
    }
}
