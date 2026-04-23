using AutoMapper;
using Lab.Application.Common.Interfaces;
using Lab.Application.DTOs.Assets;
using Lab.Domain.Common.Models;
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

        asset.Update(request.Name, request.Description, request.Type, request.Criticality);

        await _dbContext.SaveChangesAsync();

        return await GetByIdAsync(id);
    }

    public async Task<Result> DeleteAsync(Guid id)
    {
        var asset = await _dbContext.Assets.FindAsync(id);
        if (asset == null)
            return Result.Failure("Ativo não encontrado.");

        _dbContext.Assets.Remove(asset);
        await _dbContext.SaveChangesAsync();

        return Result.Success();
    }
}
