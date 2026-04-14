using Lab.Api.Data;
using Lab.Api.DTOs;
using Lab.Api.DTOs.Services;
using Lab.Api.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Lab.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class ServicesController : ControllerBase
{
    private readonly LabDbContext _dbContext;

    public ServicesController(LabDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    [HttpGet]
    public async Task<IActionResult> GetListAsync()
    {
        var services = await _dbContext.Services.AsNoTracking().ToListAsync();

        var dto = services.Select(s => new GetServiceDto(s.Id, s.Name, s.Description, s.Price)).ToList();

        return Ok(new ResponseDto(dto));
    }

    [HttpGet("{id:guid}", Name = nameof(GetServiceByIdAsync))]
    public async Task<IActionResult> GetServiceByIdAsync(Guid id)
    {
        var service = await _dbContext.Services.FindAsync(id);
        if (service == null)
            return NotFound(new ResponseDto("Serviço não encontrado."));

        var dto = new GetServiceDto(service.Id, service.Name, service.Description, service.Price);

        return Ok(new ResponseDto(dto));
    }

    [HttpPost]
    public async Task<IActionResult> PostAsync([FromBody] UpsertServiceDto dto)
    {
        var service = new Service
        {  
            Name = dto.Name,
            Description = dto.Description,
            Price = dto.Price
        };

        await _dbContext.Services.AddAsync(service);
        await _dbContext.SaveChangesAsync();

        return CreatedAtRoute(nameof(GetServiceByIdAsync), new { id = service.Id }, new ResponseDto(service));
    }

    [HttpPut("{id:guid}")]
    public async Task<IActionResult> PutAsync(Guid id, [FromBody] UpsertServiceDto dto)
    {
        var service = await _dbContext.Services.FindAsync(id);
        if (service == null)
            return NotFound(new ResponseDto("Serviço não encontrado."));

        service.Name = dto.Name;
        service.Description = dto.Description;
        service.Price = dto.Price;

        await _dbContext.SaveChangesAsync();

        return Ok(new ResponseDto(service));
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> DeleteAsync(Guid id)
    {
        var service = await _dbContext.Services.FindAsync(id);
        if (service == null)
            return NotFound(new ResponseDto("Serviço não encontrado."));

        _dbContext.Services.Remove(service);

        await _dbContext.SaveChangesAsync();

        return NoContent();
    }
}
