using AutoMapper;
using Lab.Application.Common.Interfaces;
using Lab.Application.Common.Models;
using Lab.Application.DTOs.Assets;
using Lab.Domain.Entities;
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

    public async Task<Result<List<GetAssetResponse>>> GetListAsync()
    {
        var assets = await _dbContext.Assets
            .AsNoTracking()
            .ToListAsync();

        var responses = assets.Select(asset => _mapper.Map<GetAssetResponse>(asset)).ToList();

        return Result<List<GetAssetResponse>>.Success(responses);
    }

    public async Task<Result<GetAssetResponse>> GetByIdAsync(Guid id)
    {
        var asset = await _dbContext.Assets
            .AsNoTracking()
            .FirstOrDefaultAsync(asset => asset.Id == id);

        if (asset == null)
            return Result<GetAssetResponse>.Failure("Ativo não encontrado.");

        return Result<GetAssetResponse>.Success(_mapper.Map<GetAssetResponse>(asset));
    }

    public async Task<Result<GetAssetResponse>> CreateAsync(UpsertAssetRequest request)
    {
        var isNameUnique = await IsNameUniqueAsync(request.Name);
        if (!isNameUnique)
            return Result<GetAssetResponse>.Failure("O nome informado já está em uso");

        var asset = new Asset(request.Name, request.Description, request.Type, request.Criticality);

        await _dbContext.Assets.AddAsync(asset);
        await _dbContext.SaveChangesAsync();

        return await GetByIdAsync(asset.Id);
    }

    public async Task<Result<GetAssetResponse>> UpdateAsync(Guid id, UpsertAssetRequest request)
    {
        var asset = await _dbContext.Assets.FindAsync(id);
        if (asset == null)
            return Result<GetAssetResponse>.Failure("Ativo não encontrado.");

        var isNameUnique = await IsNameUniqueAsync(request.Name, id);
        if (!isNameUnique)
            return Result<GetAssetResponse>.Failure("O nome informado já está em uso");

        asset.Update(request.Name, request.Description, request.Type, request.Criticality);

        await _dbContext.SaveChangesAsync();

        return await GetByIdAsync(id);
    }

    public async Task<Result> DeleteAsync(Guid id)
    {
        var asset = await _dbContext.Assets.Include(x => x.Risks).FirstOrDefaultAsync(x => x.Id == id);
        if (asset == null)
            return Result.Failure("Ativo não encontrado.");

        if (asset.Risks.Count > 0)
            return Result.Failure("Este ativo não pode ser excluído enquanto houver riscos vinculados.");

        _dbContext.Assets.Remove(asset);
        await _dbContext.SaveChangesAsync();

        return Result.Success();
    }
}
