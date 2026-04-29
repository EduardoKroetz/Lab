using Lab.Application.DTOs.Threats;
using Lab.Application.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Lab.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class ThreatsController : ControllerBase
{
    private readonly ThreatService _threatService;

    public ThreatsController(ThreatService threatService)
    {
        _threatService = threatService;
    }

    [HttpGet]
    public async Task<IActionResult> GetListAsync()
    {
        var result = await _threatService.GetListAsync();

        return Ok(result);
    }

    [HttpGet("{id:guid}", Name = nameof(GetThreatByIdAsync))]
    public async Task<IActionResult> GetThreatByIdAsync(Guid id)
    {
        var result = await _threatService.GetByIdAsync(id);

        return Ok(result);
    }

    [HttpPost]
    public async Task<IActionResult> PostAsync([FromBody] UpsertThreatRequest request)
    {
        var result = await _threatService.CreateAsync(request);

        return CreatedAtRoute(nameof(GetThreatByIdAsync), new { id = result.Id }, result);
    }

    [HttpPut("{id:guid}")]
    public async Task<IActionResult> PutAsync(Guid id, [FromBody] UpsertThreatRequest request)
    {
        var result = await _threatService.UpdateAsync(id, request);

        return Ok(result);
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> DeleteAsync(Guid id)
    {
        await _threatService.DeleteAsync(id);

        return NoContent();
    }
}
