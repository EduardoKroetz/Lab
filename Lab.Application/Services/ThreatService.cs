using AutoMapper;
using Lab.Application.Common.Interfaces;
using Lab.Application.Common.Models;
using Lab.Application.DTOs.Threats;
using Lab.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Lab.Application.Services;

public class ThreatService
{
    private readonly IApplicationDbContext _dbContext;
    private readonly IMapper _mapper;

    public ThreatService(IApplicationDbContext dbContext, IMapper mapper)
    {
        _dbContext = dbContext;
        _mapper = mapper;
    }

    private async Task<bool> IsNameUniqueAsync(string name, Guid? id = null)
    {
        var isUnique = !(await _dbContext.Threats.AnyAsync(a => a.Id != id && a.Name.Equals(name, StringComparison.CurrentCultureIgnoreCase)));

        return isUnique;
    }

    public async Task<Result<List<GetThreatResponse>>> GetListAsync()
    {
        var threats = await _dbContext.Threats
            .AsNoTracking()
            .ToListAsync();

        var responses = threats.Select(threat => _mapper.Map<GetThreatResponse>(threat)).ToList();

        return Result<List<GetThreatResponse>>.Success(responses);
    }

    public async Task<Result<GetThreatResponse>> GetByIdAsync(Guid id)
    {
        var threat = await _dbContext.Threats
            .AsNoTracking()
            .FirstOrDefaultAsync(threat => threat.Id == id);

        if (threat == null)
            return Result<GetThreatResponse>.Failure("Ameaça não encontrada.");

        return Result<GetThreatResponse>.Success(_mapper.Map<GetThreatResponse>(threat));
    }

    public async Task<Result<GetThreatResponse>> CreateAsync(UpsertThreatRequest request)
    {
        var isNameUnique = await IsNameUniqueAsync(request.Name);
        if (!isNameUnique)
            return Result<GetThreatResponse>.Failure("O nome informado já está em uso");

        var threat = new Threat(request.Name, request.Description, request.Category);

        await _dbContext.Threats.AddAsync(threat);
        await _dbContext.SaveChangesAsync();

        return await GetByIdAsync(threat.Id);
    }

    public async Task<Result<GetThreatResponse>> UpdateAsync(Guid id, UpsertThreatRequest request)
    {
        var threat = await _dbContext.Threats.FindAsync(id);
        if (threat == null)
            return Result<GetThreatResponse>.Failure("Ameaça não encontrada.");

        var isNameUnique = await IsNameUniqueAsync(request.Name, id);
        if (!isNameUnique)
            return Result<GetThreatResponse>.Failure("O nome informado já está em uso");

        threat.Update(request.Name, request.Description, request.Category);

        await _dbContext.SaveChangesAsync();

        return await GetByIdAsync(id);
    }

    public async Task<Result> DeleteAsync(Guid id)
    {
        var threat = await _dbContext.Threats.Include(x => x.Risks).FirstOrDefaultAsync(x => x.Id == id);
        if (threat == null)
            return Result.Failure("Ameaça não encontrada.");

        if (threat.Risks.Count > 0)
            return Result.Failure("Esta ameaça não pode ser excluída enquanto houver riscos vinculados.");

        _dbContext.Threats.Remove(threat);
        await _dbContext.SaveChangesAsync();

        return Result.Success();
    }
}
