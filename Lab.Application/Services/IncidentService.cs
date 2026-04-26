using AutoMapper;
using Lab.Application.Common.Interfaces;
using Lab.Application.Common.Models;
using Lab.Application.DTOs.IncidentImpacts;
using Lab.Application.DTOs.Incidents;
using Lab.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Lab.Application.Services;

public class IncidentService
{
    private readonly IApplicationDbContext _dbContext;
    private readonly IMapper _mapper;

    public IncidentService(IApplicationDbContext dbContext, IMapper mapper)
    {
        _dbContext = dbContext;
        _mapper = mapper;
    }

    public async Task<Result<List<GetIncidentListResponse>>> GetListAsync()
    {
        var incidents = await _dbContext.Incidents
            .AsNoTracking()
            .Include(incident => incident.RelatedRisk)
            .Include(incident => incident.IncidentImpacts)
            .ToListAsync();

        var responses = incidents.Select(incident => _mapper.Map<GetIncidentListResponse>(incident)).ToList();

        return Result<List<GetIncidentListResponse>>.Success(responses);
    }

    public async Task<Result<GetIncidentDetailResponse>> GetByIdAsync(Guid id)
    {
        var incident = await _dbContext.Incidents
            .AsNoTracking()
            .Include(incident => incident.RelatedRisk)
            .Include(incident => incident.IncidentImpacts)
            .FirstOrDefaultAsync(incident => incident.Id == id);

        if (incident == null)
            return Result<GetIncidentDetailResponse>.Failure("Incidente nao encontrado.");

        return Result<GetIncidentDetailResponse>.Success(_mapper.Map<GetIncidentDetailResponse>(incident));
    }

    public async Task<Result<GetIncidentDetailResponse>> CreateAsync(UpsertIncidentRequest request)
    {
        if (request.RelatedRiskId.HasValue && !await _dbContext.Risks.AnyAsync(x => x.Id == request.RelatedRiskId.Value))
            return Result<GetIncidentDetailResponse>.Failure("Risco relacionado não encontrado.");

        var incident = new Incident(request.Description, request.DateOccurred, request.Status, request.RelatedRiskId);

        await _dbContext.Incidents.AddAsync(incident);
        await _dbContext.SaveChangesAsync();

        return await GetByIdAsync(incident.Id);
    }

    public async Task<Result<GetIncidentDetailResponse>> UpdateAsync(Guid id, UpsertIncidentRequest request)
    {
        if (request.RelatedRiskId.HasValue && !await _dbContext.Risks.AnyAsync(x => x.Id == request.RelatedRiskId.Value))
            return Result<GetIncidentDetailResponse>.Failure("Risco relacionado não encontrado.");

        var incident = await _dbContext.Incidents.FindAsync(id);
        if (incident == null)
            return Result<GetIncidentDetailResponse>.Failure("Incidente nao encontrado.");

        incident.Update(request.Description, request.DateOccurred, request.Status, request.RelatedRiskId);

        await _dbContext.SaveChangesAsync();

        return await GetByIdAsync(id);
    }

    public async Task<Result> DeleteAsync(Guid id)
    {
        var incident = await _dbContext.Incidents.FindAsync(id);
        if (incident == null)
            return Result.Failure("Incidente nao encontrado.");

        _dbContext.Incidents.Remove(incident);
        await _dbContext.SaveChangesAsync();

        return Result.Success();
    }

    public async Task<Result> AddImpactAsync(Guid incidentId, UpsertIncidentImpactRequest request)
    {
        var incident = await _dbContext.Incidents
            .Include(x => x.IncidentImpacts)
            .FirstOrDefaultAsync(x => x.Id == incidentId);

        if (incident == null)
            return Result.Failure("Incidente nao encontrado.");

        incident.AddImpact(request.Type, request.Level);

        await _dbContext.SaveChangesAsync();

        return Result.Success();
    }

    public async Task<Result> RemoveImpactAsync(Guid incidentId, Guid impactId)
    {
        var incident = await _dbContext.Incidents
            .Include(x => x.IncidentImpacts)
            .FirstOrDefaultAsync(x => x.Id == incidentId);

        if (incident == null)
            return Result.Failure("Incidente nao encontrado.");

        incident.RemoveImpact(impactId);

        await _dbContext.SaveChangesAsync();

        return Result.Success();
    }

    public async Task<Result<List<GetIncidentImpactResponse>>> GetListImpactsAsync(Guid incidentId)
    {
        var incidentExists = await _dbContext.Incidents.AnyAsync(x => x.Id == incidentId);
        if (!incidentExists)
            return Result<List<GetIncidentImpactResponse>>.Failure("Incidente nao encontrado.");

        var impacts = await _dbContext.IncidentImpacts
            .AsNoTracking()
            .Include(x => x.Incident)
            .Where(x => x.IncidentId == incidentId)
            .Select(x => _mapper.Map<GetIncidentImpactResponse>(x))
            .ToListAsync();

        return Result<List<GetIncidentImpactResponse>>.Success(impacts);
    }
}
