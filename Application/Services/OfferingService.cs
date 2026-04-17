using Lab.Api.Application.DTOs.Services;
using Lab.Api.Domain.Entities;
using Lab.Api.Domain.Exceptions;
using Lab.Api.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Lab.Api.Application.Services;

public class OfferingService
{
    private readonly LabDbContext _dbContext;

    public OfferingService(LabDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<List<GetOfferingDto>> GetListAsync()
    {
        var offerings = await _dbContext.Services.AsNoTracking().ToListAsync();

        var dto = offerings.Select(s => new GetOfferingDto(s.Id, s.Name, s.Description, s.Price)).ToList();

        return dto;
    }

    public async Task<GetOfferingDto> GetByIdAsync(Guid id)
    {
        var offering = await _dbContext.Services.FindAsync(id);
        if (offering == null)
            throw new NotFoundException("Oferta não encontrada.");

        var dto = new GetOfferingDto(offering.Id, offering.Name, offering.Description, offering.Price);

        return dto;
    }

    public async Task<GetOfferingDto> CreateAsync(UpsertServiceDto dto)
    {
        var offering = new Offering
        {
            Name = dto.Name,
            Description = dto.Description,
            Price = dto.Price
        };

        await _dbContext.Services.AddAsync(offering);
        await _dbContext.SaveChangesAsync();

        var responseDto = new GetOfferingDto(offering.Id, offering.Name, offering.Description, offering.Price);

        return responseDto;
    }

    public async Task<GetOfferingDto> UpdateAsync(Guid id, UpsertServiceDto dto)
    {
        var offering = await _dbContext.Services.FindAsync(id);
        if (offering == null)
            throw new NotFoundException("Oferta não encontrada.");

        offering.Name = dto.Name;
        offering.Description = dto.Description;
        offering.Price = dto.Price;

        await _dbContext.SaveChangesAsync();

        var responseDto = new GetOfferingDto(offering.Id, offering.Name, offering.Description, offering.Price);

        return responseDto;
    }

    public async Task DeleteAsync(Guid id)
    {
        var offering = await _dbContext.Services.FindAsync(id);
        if (offering == null)
            throw new NotFoundException("Oferta não encontrada.");

        _dbContext.Services.Remove(offering);

        await _dbContext.SaveChangesAsync();
    }
}
