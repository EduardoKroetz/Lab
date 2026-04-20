using Lab.Api.Application.DTOs;
using Lab.Api.Application.Services;
using Lab.Api.Extensions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Lab.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class UsersController : ControllerBase
{
    private readonly UserService _userService;

    public UsersController(UserService userService)
    {
        _userService = userService;
    }

    [HttpGet("current")]
    public async Task<IActionResult> GetCurrentUserAsync()
    {
        var userId = User.GetUserId();

        var dto = await _userService.GetCurrentUserAsync(userId);

        return Ok(new ResponseDto(dto));
    }
}
