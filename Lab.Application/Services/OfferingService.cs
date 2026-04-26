using AutoMapper;
using Lab.Application.Common.Interfaces;
using Lab.Application.Common.Models;
using Lab.Application.DTOs.Offerings;
using Lab.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Lab.Application.Services;

public class OfferingService
{
    private readonly IApplicationDbContext _dbContext;
    private readonly IMapper _mapper;

    public OfferingService(IApplicationDbContext dbContext, IMapper mapper)
    {
        _dbContext = dbContext;
        _mapper = mapper;
    }

    public async Task<Result<List<GetOfferingResponse>>> GetListAsync()
    {
        var offerings = await _dbContext.Offerings
            .AsNoTracking()
            .ToListAsync();

        var responses = offerings.Select(o => _mapper.Map<GetOfferingResponse>(o)).ToList();

        return Result<List<GetOfferingResponse>>.Success(responses);
    }

    public async Task<Result<GetOfferingResponse>> GetByIdAsync(Guid id)
    {
        var offering = await _dbContext.Offerings
            .AsNoTracking()
            .FirstOrDefaultAsync(o => o.Id == id);

        if (offering == null)
            return Result<GetOfferingResponse>.Failure("Oferta não encontrada.");

        return Result<GetOfferingResponse>.Success(_mapper.Map<GetOfferingResponse>(offering));
    }

    public async Task<Result<GetOfferingResponse>> CreateAsync(UpsertOfferingRequest request)
    {
        var offering = new Offering(request.Name, request.Description, request.Price);

        await _dbContext.Offerings.AddAsync(offering);
        await _dbContext.SaveChangesAsync();

        return await GetByIdAsync(offering.Id);
    }

    public async Task<Result<GetOfferingResponse>> UpdateAsync(Guid id, UpsertOfferingRequest request)
    {
        var offering = await _dbContext.Offerings.FindAsync(id);
        if (offering == null)
            return Result<GetOfferingResponse>.Failure("Oferta não encontrada.");

        offering.Update(request.Name, request.Description, request.Price);

        await _dbContext.SaveChangesAsync();

        return await GetByIdAsync(id);
    }

    public async Task<Result> DeleteAsync(Guid id)
    {
        var offering = await _dbContext.Offerings.FindAsync(id);
        if (offering == null)
            return Result.Failure("Oferta não encontrada.");

        _dbContext.Offerings.Remove(offering);
        await _dbContext.SaveChangesAsync();

        return Result.Success();
    }
}
