using Lab.Api.Domain.Entities;
using Lab.Api.Infrastructure.Data;
using Microsoft.AspNetCore.Mvc;

namespace Lab.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class TenantsController : ControllerBase
{
    private LabDbContext _labDbContext;

    public TenantsController(LabDbContext labDbContext)
    {
        _labDbContext = labDbContext;
    }

    [HttpPost]
    public async Task<IActionResult> PostAsync(Tenant tenant)
    {
        await _labDbContext.AddAsync(tenant);
        await _labDbContext.SaveChangesAsync();

        return Ok();
    }
}