using AutoMapper;
using Lab.Application.Common.Interfaces;
using Lab.Application.Common.Models;
using Lab.Application.DTOs.IncidentImpacts;
using Lab.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Lab.Application.Services;

public class IncidentImpactService
{
    private readonly IApplicationDbContext _dbContext;
    private readonly IMapper _mapper;

    public IncidentImpactService(IApplicationDbContext dbContext, IMapper mapper)
    {
        _dbContext = dbContext;
        _mapper = mapper;
    }

    public async Task<Result<List<GetIncidentImpactResponse>>> GetListAsync()
    {
        var incidentImpacts = await _dbContext.IncidentImpacts
            .AsNoTracking()
            .Include(incidentImpact => incidentImpact.Incident)
            .ToListAsync();

        var responses = incidentImpacts.Select(incidentImpact => _mapper.Map<GetIncidentImpactResponse>(incidentImpact)).ToList();

        return Result<List<GetIncidentImpactResponse>>.Success(responses);
    }

    public async Task<Result<GetIncidentImpactResponse>> GetByIdAsync(Guid id)
    {
        var incidentImpact = await _dbContext.IncidentImpacts
            .AsNoTracking()
            .Include(incidentImpact => incidentImpact.Incident)
            .FirstOrDefaultAsync(incidentImpact => incidentImpact.Id == id);

        if (incidentImpact == null)
            return Result<GetIncidentImpactResponse>.Failure("Impacto de incidente não encontrado.");

        return Result<GetIncidentImpactResponse>.Success(_mapper.Map<GetIncidentImpactResponse>(incidentImpact));
    }

    public async Task<Result<GetIncidentImpactResponse>> CreateAsync(UpsertIncidentImpactRequest request)
    {
        var incident = await _dbContext.Incidents.FindAsync(request.IncidentId);
        if (incident == null)
            return Result<GetIncidentImpactResponse>.Failure("Incidente não encontrado.");

        var incidentImpact = new IncidentImpact(incident, request.Type, request.Level);

        await _dbContext.IncidentImpacts.AddAsync(incidentImpact);
        await _dbContext.SaveChangesAsync();

        return await GetByIdAsync(incidentImpact.Id);
    }

    public async Task<Result<GetIncidentImpactResponse>> UpdateAsync(Guid id, UpsertIncidentImpactRequest request)
    {
        var incident = await _dbContext.Incidents.FindAsync(request.IncidentId);
        if (incident == null)
            return Result<GetIncidentImpactResponse>.Failure("Incidente não encontrado.");

        var incidentImpact = await _dbContext.IncidentImpacts.FindAsync(id);
        if (incidentImpact == null)
            return Result<GetIncidentImpactResponse>.Failure("Impacto de incidente não encontrado.");

        incidentImpact.Update(incident, request.Type, request.Level);

        await _dbContext.SaveChangesAsync();

        return await GetByIdAsync(id);
    }

    public async Task<Result> DeleteAsync(Guid id)
    {
        var incidentImpact = await _dbContext.IncidentImpacts.FindAsync(id);
        if (incidentImpact == null)
            return Result.Failure("Impacto de incidente não encontrado.");

        _dbContext.IncidentImpacts.Remove(incidentImpact);
        await _dbContext.SaveChangesAsync();

        return Result.Success();
    }
}
