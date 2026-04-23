using Lab.Api.Extensions;
using Lab.Application.DTOs.Vulnerabilities;
using Lab.Application.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Lab.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class VulnerabilitiesController : ControllerBase
{
    private readonly VulnerabilityService _vulnerabilityService;

    public VulnerabilitiesController(VulnerabilityService vulnerabilityService)
    {
        _vulnerabilityService = vulnerabilityService;
    }

    [HttpGet]
    public async Task<IActionResult> GetListAsync()
    {
        var result = await _vulnerabilityService.GetListAsync();

        return result.ToActionResult();
    }

    [HttpGet("{id:guid}", Name = nameof(GetVulnerabilityByIdAsync))]
    public async Task<IActionResult> GetVulnerabilityByIdAsync(Guid id)
    {
        var result = await _vulnerabilityService.GetByIdAsync(id);

        return result.ToActionResult();
    }

    [HttpPost]
    public async Task<IActionResult> PostAsync([FromBody] UpsertVulnerabilityRequest request)
    {
        var result = await _vulnerabilityService.CreateAsync(request);
        if (!result.Succeeded)
            return result.ToActionResult();

        return CreatedAtRoute(nameof(GetVulnerabilityByIdAsync), new { id = result.Value!.Id }, result.Value);
    }

    [HttpPut("{id:guid}")]
    public async Task<IActionResult> PutAsync(Guid id, [FromBody] UpsertVulnerabilityRequest request)
    {
        var result = await _vulnerabilityService.UpdateAsync(id, request);

        return result.ToActionResult();
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> DeleteAsync(Guid id)
    {
        var result = await _vulnerabilityService.DeleteAsync(id);

        return result.ToActionResult();
    }
}
