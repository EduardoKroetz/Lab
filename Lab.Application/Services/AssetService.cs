using AutoMapper;
using Lab.Application.Common.Interfaces;
using Lab.Application.DTOs.Assets;
using Lab.Domain.Entities;
using Lab.Domain.Exceptions;
using Microsoft.EntityFrameworkCore;

namespace Lab.Application.Services;

public class AssetService
{
    private readonly IApplicationDbContext _dbContext;
    private readonly IMapper _mapper;

    public AssetService(IApplicationDbContext dbContext, IMapper mapper)
    {
        _dbContext = dbContext;
        _mapper = mapper;
    }

    private async Task<bool> IsNameUniqueAsync(string name, Guid? id = null)
    {
        var isUnique = !(await _dbContext.Assets.AnyAsync(a => a.Id != id && a.Name.Equals(name, StringComparison.CurrentCultureIgnoreCase)));

        return isUnique;
    }

    public async Task<List<GetAssetResponse>> GetListAsync()
    {
        var assets = await _dbContext.Assets
            .AsNoTracking()
            .ToListAsync();

        var responses = assets.Select(asset => _mapper.Map<GetAssetResponse>(asset)).ToList();
        return responses;
    }

    public async Task<GetAssetResponse> GetByIdAsync(Guid id)
    {
        var asset = await _dbContext.Assets
            .AsNoTracking()
            .FirstOrDefaultAsync(asset => asset.Id == id);

        if (asset == null)
            throw new NotFoundException("Ativo não encontrado.");

        return _mapper.Map<GetAssetResponse>(asset);
    }

    public async Task<GetAssetResponse> CreateAsync(UpsertAssetRequest request)
    {
        var isNameUnique = await IsNameUniqueAsync(request.Name);
        if (!isNameUnique)
            throw new ValidationException("O nome informado já está em uso");

        var asset = new Asset(request.Name, request.Description, request.Type, request.Criticality);

        await _dbContext.Assets.AddAsync(asset);
        await _dbContext.SaveChangesAsync();

        return await GetByIdAsync(asset.Id);
    }

    public async Task<GetAssetResponse> UpdateAsync(Guid id, UpsertAssetRequest request)
    {
        var asset = await _dbContext.Assets.FindAsync(id);
        if (asset == null)
            throw new NotFoundException("Ativo não encontrado.");

        var isNameUnique = await IsNameUniqueAsync(request.Name, id);
        if (!isNameUnique)
            throw new ValidationException("O nome informado já está em uso");

        asset.Update(request.Name, request.Description, request.Type, request.Criticality);

        await _dbContext.SaveChangesAsync();

        return await GetByIdAsync(id);
    }

    public async Task DeleteAsync(Guid id)
    {
        var asset = await _dbContext.Assets.Include(x => x.Risks).FirstOrDefaultAsync(x => x.Id == id);
        if (asset == null)
            throw new NotFoundException("Ativo não encontrado.");

        if (asset.Risks.Count > 0)
            throw new ValidationException("Este ativo não pode ser excluído enquanto houver riscos vinculados.");

        _dbContext.Assets.Remove(asset);
        await _dbContext.SaveChangesAsync();
    }
}
