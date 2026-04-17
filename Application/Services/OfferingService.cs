using AutoMapper;
using Lab.Api.Application.DTOs.Offerings;
using Lab.Api.Domain.Entities;
using Lab.Api.Domain.Exceptions;
using Lab.Api.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Lab.Api.Application.Services;

public class OfferingService
{
    private readonly LabDbContext _dbContext;
    private readonly IMapper _mapper;

    public OfferingService(LabDbContext dbContext, IMapper mapper)
    {
        _dbContext = dbContext;
        _mapper = mapper;
    }

    public async Task<List<GetOfferingDto>> GetListAsync()
    {
        var offerings = await _dbContext.Offerings.AsNoTracking().ToListAsync();

        return offerings.Select(o => _mapper.Map<GetOfferingDto>(o)).ToList();
    }

    public async Task<GetOfferingDto> GetByIdAsync(Guid id)
    {
        var offering = await _dbContext.Offerings.FindAsync(id);
        if (offering == null)
            throw new NotFoundException("Oferta não encontrada.");

        return _mapper.Map<GetOfferingDto>(offering);
    }

    public async Task<GetOfferingDto> CreateAsync(UpsertOfferingDto dto)
    {
        var offering = new Offering(dto.Name, dto.Description, dto.Price);

        await _dbContext.Offerings.AddAsync(offering);
        await _dbContext.SaveChangesAsync();

        return _mapper.Map<GetOfferingDto>(offering);
    }

    public async Task<GetOfferingDto> UpdateAsync(Guid id, UpsertOfferingDto dto)
    {
        var offering = await _dbContext.Offerings.FindAsync(id);
        if (offering == null)
            throw new NotFoundException("Oferta não encontrada.");

        offering.Update(dto.Name, dto.Description, dto.Price);

        await _dbContext.SaveChangesAsync();

        return _mapper.Map<GetOfferingDto>(offering);
    }

    public async Task DeleteAsync(Guid id)
    {
        var offering = await _dbContext.Offerings.FindAsync(id);
        if (offering == null)
            throw new NotFoundException("Oferta não encontrada.");

        _dbContext.Offerings.Remove(offering);
        await _dbContext.SaveChangesAsync();
    }
}
