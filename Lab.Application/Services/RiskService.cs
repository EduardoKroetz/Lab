using AutoMapper;
using Lab.Application.Common.Interfaces;
using Lab.Application.DTOs.Risks;
using Lab.Domain.Common.Models;
using Lab.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Lab.Application.Services;

public class RiskService
{
    private readonly IApplicationDbContext _dbContext;
    private readonly IMapper _mapper;

    public RiskService(IApplicationDbContext dbContext, IMapper mapper)
    {
        _dbContext = dbContext;
        _mapper = mapper;
    }

    public async Task<Result<List<GetRiskResponse>>> GetListAsync()
    {
        var risks = await _dbContext.Risks
            .AsNoTracking()
            .Include(risk => risk.Asset)
            .Include(risk => risk.Threat)
            .Include(risk => risk.Vulnerability)
            .ToListAsync();

        var responses = risks.Select(risk => _mapper.Map<GetRiskResponse>(risk)).ToList();

        return Result<List<GetRiskResponse>>.Success(responses);
    }

    public async Task<Result<GetRiskResponse>> GetByIdAsync(Guid id)
    {
        var risk = await _dbContext.Risks
            .AsNoTracking()
            .Include(risk => risk.Asset)
            .Include(risk => risk.Threat)
            .Include(risk => risk.Vulnerability)
            .FirstOrDefaultAsync(risk => risk.Id == id);

        if (risk == null)
            return Result<GetRiskResponse>.Failure("Risco não encontrado.");

        return Result<GetRiskResponse>.Success(_mapper.Map<GetRiskResponse>(risk));
    }

    public async Task<Result<GetRiskResponse>> CreateAsync(UpsertRiskRequest request)
    {
        var asset = await _dbContext.Assets.FindAsync(request.AssetId);
        if (asset == null)
            return Result<GetRiskResponse>.Failure("Ativo não encontrado.");

        var threat = await _dbContext.Threats.FindAsync(request.ThreatId);
        if (threat == null)
            return Result<GetRiskResponse>.Failure("Ameaça não encontrada.");

        var vulnerability = await _dbContext.Vulnerabilities.FindAsync(request.VulnerabilityId);
        if (vulnerability == null)
            return Result<GetRiskResponse>.Failure("Vulnerabilidade não encontrada.");

        var risk = new Risk(asset, threat, vulnerability, request.Probability, request.Impact, request.Level, request.Status);

        await _dbContext.Risks.AddAsync(risk);
        await _dbContext.SaveChangesAsync();

        return await GetByIdAsync(risk.Id);
    }

    public async Task<Result<GetRiskResponse>> UpdateAsync(Guid id, UpsertRiskRequest request)
    {
        var asset = await _dbContext.Assets.FindAsync(request.AssetId);
        if (asset == null)
            return Result<GetRiskResponse>.Failure("Ativo não encontrado.");

        var threat = await _dbContext.Threats.FindAsync(request.ThreatId);
        if (threat == null)
            return Result<GetRiskResponse>.Failure("Ameaça não encontrada.");

        var vulnerability = await _dbContext.Vulnerabilities.FindAsync(request.VulnerabilityId);
        if (vulnerability == null)
            return Result<GetRiskResponse>.Failure("Vulnerabilidade não encontrada.");

        var risk = await _dbContext.Risks.FindAsync(id);
        if (risk == null)
            return Result<GetRiskResponse>.Failure("Risco não encontrado.");

        risk.Update(asset, threat, vulnerability, request.Probability, request.Impact, request.Level, request.Status);

        await _dbContext.SaveChangesAsync();

        return await GetByIdAsync(id);
    }

    public async Task<Result> DeleteAsync(Guid id)
    {
        var risk = await _dbContext.Risks.FindAsync(id);
        if (risk == null)
            return Result.Failure("Risco não encontrado.");

        _dbContext.Risks.Remove(risk);
        await _dbContext.SaveChangesAsync();

        return Result.Success();
    }
}
