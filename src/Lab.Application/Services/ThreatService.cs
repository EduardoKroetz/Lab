using AutoMapper;
using Lab.Application.Common.Interfaces;
using Lab.Application.DTOs.Threats;
using Lab.Domain.Entities;
using Lab.Domain.Exceptions;
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
        return !(await _dbContext.Threats.AnyAsync(a => a.Id != id && a.Name.Equals(name, StringComparison.CurrentCultureIgnoreCase)));
    }

    public async Task<List<GetThreatResponse>> GetListAsync()
    {
        var threats = await _dbContext.Threats
            .AsNoTracking()
            .ToListAsync();

        return threats.Select(threat => _mapper.Map<GetThreatResponse>(threat)).ToList();
    }

    public async Task<GetThreatResponse> GetByIdAsync(Guid id)
    {
        var threat = await _dbContext.Threats
            .AsNoTracking()
            .FirstOrDefaultAsync(threat => threat.Id == id);

        if (threat == null)
            throw new NotFoundException("Ameaça não encontrada.");

        return _mapper.Map<GetThreatResponse>(threat);
    }

    public async Task<GetThreatResponse> CreateAsync(UpsertThreatRequest request)
    {
        var isNameUnique = await IsNameUniqueAsync(request.Name);
        if (!isNameUnique)
            throw new ValidationException("O nome informado já está em uso");

        var threat = new Threat(request.Name, request.Description, request.Category);

        await _dbContext.Threats.AddAsync(threat);
        await _dbContext.SaveChangesAsync();

        return await GetByIdAsync(threat.Id);
    }

    public async Task<GetThreatResponse> UpdateAsync(Guid id, UpsertThreatRequest request)
    {
        var threat = await _dbContext.Threats.FindAsync(id);
        if (threat == null)
            throw new NotFoundException("Ameaça não encontrada.");

        var isNameUnique = await IsNameUniqueAsync(request.Name, id);
        if (!isNameUnique)
            throw new ValidationException("O nome informado já está em uso");

        threat.Update(request.Name, request.Description, request.Category);

        await _dbContext.SaveChangesAsync();

        return await GetByIdAsync(id);
    }

    public async Task DeleteAsync(Guid id)
    {
        var threat = await _dbContext.Threats.Include(x => x.Risks).FirstOrDefaultAsync(x => x.Id == id);
        if (threat == null)
            throw new NotFoundException("Ameaça não encontrada.");

        if (threat.Risks.Count > 0)
            throw new ValidationException("Esta ameaça não pode ser excluída enquanto houver riscos vinculados.");

        _dbContext.Threats.Remove(threat);
        await _dbContext.SaveChangesAsync();
    }
}
