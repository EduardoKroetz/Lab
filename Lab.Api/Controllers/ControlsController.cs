using Lab.Api.Extensions;
using Lab.Application.DTOs.Controls;
using Lab.Application.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Lab.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class ControlsController : ControllerBase
{
    private readonly ControlService _controlService;

    public ControlsController(ControlService controlService)
    {
        _controlService = controlService;
    }

    [HttpGet]
    public async Task<IActionResult> GetListAsync()
    {
        var result = await _controlService.GetListAsync();

        return result.ToActionResult();
    }

    [HttpGet("{id:guid}", Name = nameof(GetControlByIdAsync))]
    public async Task<IActionResult> GetControlByIdAsync(Guid id)
    {
        var result = await _controlService.GetByIdAsync(id);

        return result.ToActionResult();
    }

    [HttpPost]
    public async Task<IActionResult> PostAsync([FromBody] UpsertControlRequest request)
    {
        var result = await _controlService.CreateAsync(request);
        if (!result.Succeeded)
            return result.ToActionResult();

        return CreatedAtRoute(nameof(GetControlByIdAsync), new { id = result.Value!.Id }, result.Value);
    }

    [HttpPut("{id:guid}")]
    public async Task<IActionResult> PutAsync(Guid id, [FromBody] UpsertControlRequest request)
    {
        var result = await _controlService.UpdateAsync(id, request);

        return result.ToActionResult();
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> DeleteAsync(Guid id)
    {
        var result = await _controlService.DeleteAsync(id);

        return result.ToActionResult();
    }
}
