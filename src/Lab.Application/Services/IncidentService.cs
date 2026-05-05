using AutoMapper;
using Lab.Application.Common.Interfaces;
using Lab.Application.DTOs.IncidentImpacts;
using Lab.Application.DTOs.Incidents;
using Lab.Domain.Entities;
using Lab.Domain.Exceptions;
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

    private async Task<Incident> GetIncidentWithRelationsAsync(Guid incidentId)
    {
        var incident = await _dbContext.Incidents
             .AsNoTracking()
             .Include(incident => incident.IncidentImpacts)
             .FirstOrDefaultAsync(incident => incident.Id == incidentId);

        if (incident == null)
            throw new NotFoundException("Incidente não encontrado.");

        return incident;
    }

    public async Task<List<GetIncidentListResponse>> GetListAsync()
    {
        var incidents = await _dbContext.Incidents
            .AsNoTracking()
            .Include(incident => incident.Risk)
            .Include(incident => incident.IncidentImpacts)
            .ToListAsync();

        return incidents.Select(incident => _mapper.Map<GetIncidentListResponse>(incident)).ToList();
    }

    public async Task<GetIncidentDetailResponse> GetByIdAsync(Guid id)
    {
        var incident = await _dbContext.Incidents
            .AsNoTracking()
            .Include(incident => incident.Risk)
            .Include(incident => incident.IncidentImpacts)
            .FirstOrDefaultAsync(incident => incident.Id == id);

        if (incident == null)
            throw new NotFoundException("Incidente não encontrado.");

        return _mapper.Map<GetIncidentDetailResponse>(incident);
    }

    public async Task<GetIncidentDetailResponse> CreateAsync(UpsertIncidentRequest request)
    {
        if (!await _dbContext.Risks.AnyAsync(x => x.Id == request.RiskId))
            throw new NotFoundException("Risco relacionado não encontrado.");

        var incident = new Incident(request.Description, request.DateOccurred, request.Status, request.RiskId);

        await _dbContext.Incidents.AddAsync(incident);
        await _dbContext.SaveChangesAsync();

        return await GetByIdAsync(incident.Id);
    }

    public async Task<GetIncidentDetailResponse> UpdateAsync(Guid id, UpsertIncidentRequest request)
    {
        if (!await _dbContext.Risks.AnyAsync(x => x.Id == request.RiskId))
            throw new NotFoundException("Risco relacionado não encontrado.");

        var incident = await _dbContext.Incidents.FindAsync(id);
        if (incident == null)
            throw new NotFoundException("Incidente não encontrado.");

        incident.Update(request.Description, request.DateOccurred, request.Status, request.RiskId);

        await _dbContext.SaveChangesAsync();

        return await GetByIdAsync(id);
    }

    public async Task DeleteAsync(Guid id)
    {
        var incident = await _dbContext.Incidents.FindAsync(id);
        if (incident == null)
            throw new NotFoundException("Incidente não encontrado.");

        _dbContext.Incidents.Remove(incident);
        await _dbContext.SaveChangesAsync();
    }

    public async Task AddImpactAsync(Guid incidentId, UpsertIncidentImpactRequest request)
    {
        var incident = await GetIncidentWithRelationsAsync(incidentId);

        incident.AddImpact(request.Type, request.SeverityScore, request.Description);

        await _dbContext.SaveChangesAsync();
    }

    public async Task RemoveImpactAsync(Guid incidentId, Guid impactId)
    {
        var incident = await GetIncidentWithRelationsAsync(incidentId);

        incident.RemoveImpact(impactId);

        await _dbContext.SaveChangesAsync();
    }

    public async Task<List<GetIncidentImpactResponse>> GetListImpactsAsync(Guid incidentId)
    {
        var incidentExists = await _dbContext.Incidents.AnyAsync(x => x.Id == incidentId);
        if (!incidentExists)
            throw new NotFoundException("Incidente não encontrado.");

        return await _dbContext.IncidentImpacts
            .AsNoTracking()
            .Include(x => x.Incident)
            .Where(x => x.IncidentId == incidentId)
            .Select(x => _mapper.Map<GetIncidentImpactResponse>(x))
            .ToListAsync();
    }
}
