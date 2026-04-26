using AutoMapper;
using Lab.Application.Common.Interfaces;
using Lab.Application.Common.Models;
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

    public async Task<Result<List<GetIncidentResponse>>> GetListAsync()
    {
        var incidents = await _dbContext.Incidents
            .AsNoTracking()
            .Include(incident => incident.RelatedRisk)
            .ToListAsync();

        var responses = incidents.Select(incident => _mapper.Map<GetIncidentResponse>(incident)).ToList();

        return Result<List<GetIncidentResponse>>.Success(responses);
    }

    public async Task<Result<GetIncidentResponse>> GetByIdAsync(Guid id)
    {
        var incident = await _dbContext.Incidents
            .AsNoTracking()
            .Include(incident => incident.RelatedRisk)
            .FirstOrDefaultAsync(incident => incident.Id == id);

        if (incident == null)
            return Result<GetIncidentResponse>.Failure("Incidente não encontrado.");

        return Result<GetIncidentResponse>.Success(_mapper.Map<GetIncidentResponse>(incident));
    }

    public async Task<Result<GetIncidentResponse>> CreateAsync(UpsertIncidentRequest request)
    {
        Risk? relatedRisk = null;

        if (request.RelatedRiskId.HasValue)
        {
            relatedRisk = await _dbContext.Risks.FindAsync(request.RelatedRiskId.Value);
            if (relatedRisk == null)
                return Result<GetIncidentResponse>.Failure("Risco relacionado não encontrado.");
        }

        var incident = new Incident(request.Description, request.DateOccurred, request.Status, relatedRisk);

        await _dbContext.Incidents.AddAsync(incident);
        await _dbContext.SaveChangesAsync();

        return await GetByIdAsync(incident.Id);
    }

    public async Task<Result<GetIncidentResponse>> UpdateAsync(Guid id, UpsertIncidentRequest request)
    {
        Risk? relatedRisk = null;

        if (request.RelatedRiskId.HasValue)
        {
            relatedRisk = await _dbContext.Risks.FindAsync(request.RelatedRiskId.Value);
            if (relatedRisk == null)
                return Result<GetIncidentResponse>.Failure("Risco relacionado não encontrado.");
        }

        var incident = await _dbContext.Incidents.FindAsync(id);
        if (incident == null)
            return Result<GetIncidentResponse>.Failure("Incidente não encontrado.");

        incident.Update(request.Description, request.DateOccurred, request.Status, relatedRisk);

        await _dbContext.SaveChangesAsync();

        return await GetByIdAsync(id);
    }

    public async Task<Result> DeleteAsync(Guid id)
    {
        var incident = await _dbContext.Incidents.FindAsync(id);
        if (incident == null)
            return Result.Failure("Incidente não encontrado.");

        _dbContext.Incidents.Remove(incident);
        await _dbContext.SaveChangesAsync();

        return Result.Success();
    }
}
