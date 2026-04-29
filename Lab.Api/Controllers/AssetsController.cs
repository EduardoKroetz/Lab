using Lab.Application.DTOs.Assets;
using Lab.Application.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Lab.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class AssetsController : ControllerBase
{
    private readonly AssetService _assetService;

    public AssetsController(AssetService assetService)
    {
        _assetService = assetService;
    }

    [HttpGet]
    public async Task<IActionResult> GetListAsync()
    {
        var result = await _assetService.GetListAsync();

        return Ok(result);
    }

    [HttpGet("{id:guid}", Name = nameof(GetAssetByIdAsync))]
    public async Task<IActionResult> GetAssetByIdAsync(Guid id)
    {
        var result = await _assetService.GetByIdAsync(id);

        return Ok(result);
    }

    [HttpPost]
    public async Task<IActionResult> PostAsync([FromBody] UpsertAssetRequest request)
    {
        var result = await _assetService.CreateAsync(request);

        return CreatedAtRoute(nameof(GetAssetByIdAsync), new { id = result.Id }, result);
    }

    [HttpPut("{id:guid}")]
    public async Task<IActionResult> PutAsync(Guid id, [FromBody] UpsertAssetRequest request)
    {
        var result = await _assetService.UpdateAsync(id, request);

        return Ok(result);
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> DeleteAsync(Guid id)
    {
        await _assetService.DeleteAsync(id);

        return NoContent();
    }
}
