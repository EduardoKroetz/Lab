using AutoMapper;
using Lab.Application.DTOs.Assets;
using Lab.Application.Services;
using Lab.Domain.Enums;
using Lab.Domain.Exceptions;
using Lab.IntegrationTests.Common;
using Lab.IntegrationTests.Seeds;
using Microsoft.Extensions.Logging.Abstractions;

namespace Lab.IntegrationTests.Application.Services;

public class AssetServiceTests : IntegrationTestsBase
{
    private readonly AssetService _service;

    public AssetServiceTests()
    {
        var mapperConfig = new MapperConfiguration(cfg => cfg.AddMaps(typeof(AssetService).Assembly), NullLoggerFactory.Instance);

        _service = new AssetService(DbContext, mapperConfig.CreateMapper());
    }

    // ----------------
    // CreateAsync
    // ----------------

    [Fact]
    public async Task CreateAsync_ValidRequest_MustPersistAsset()
    {
        var result = await _service.CreateAsync(NewUpsertRequest());

        var persisted = await DbContext.Assets.FindAsync(result.Id);
        Assert.NotNull(persisted);
        Assert.Equal(result.Name, persisted.Name);
        Assert.Equal(result.Description, persisted.Description);
        Assert.Equal(result.Criticality, persisted.Criticality);
        Assert.Equal(result.Type, persisted.Type);
    }

    [Fact]
    public async Task CreateAsync_NameAlreadyUsed_MustThrowValidationException()
    {
        await _service.CreateAsync(NewUpsertRequest());

        await Assert.ThrowsAsync<ValidationException>(() => _service.CreateAsync(NewUpsertRequest()));
    }

    // ---------------
    // UpdateAsync
    // ---------------
    [Fact]
    public async Task UpdateAsync_ValidRequest_MustPersistChanges()
    {
        var created = await AssetSeeds.SeedAssetAsync(DbContext);

        var result = await _service.UpdateAsync(created.Id, NewUpsertRequest("Updated Name", "Updated Description", EAssetCriticality.Medium, EAssetType.Data));

        var persisted = await DbContext.Assets.FindAsync(result.Id);
        Assert.NotNull(persisted);
        Assert.Equal(result.Name, persisted.Name);
        Assert.Equal(result.Description, persisted.Description);
        Assert.Equal(result.Criticality, persisted.Criticality);
        Assert.Equal(result.Type, persisted.Type);
    }

    [Fact]
    public async Task UpdateAsync_NameAlreadyUsed_MustThrowValidationException()
    {
        await _service.CreateAsync(NewUpsertRequest());
        var created = await _service.CreateAsync(NewUpsertRequest("Another Name", "Another Description", EAssetCriticality.Medium, EAssetType.Data));

        await Assert.ThrowsAsync<ValidationException>(() => _service.UpdateAsync(created.Id, NewUpsertRequest()));
    }

    // ---------------
    // DeleteAsync
    // ---------------

    [Fact]
    public async Task DeleteAsync_ExistingAsset_MustRemoveFromDatabase()
    {
        var created = await AssetSeeds.SeedAssetAsync(DbContext);

        await _service.DeleteAsync(created.Id);

        var persisted = await DbContext.Assets.FindAsync(created.Id);
        Assert.Null(persisted);
    }

    [Fact]
    public async Task DeleteAsync_AssetNotFound_MustThrowNotFoundException()
    {
        await Assert.ThrowsAsync<NotFoundException>(() => _service.DeleteAsync(Guid.NewGuid()));
    }

    [Fact]
    public async Task DeleteAsync_AssetWithLinkedRisk_MustThrowValidationException()
    {
        var risk = await RiskSeeds.SeedRiskAsync(DbContext, Clock);

        await Assert.ThrowsAsync<ValidationException>(() => _service.DeleteAsync(risk.AssetId));
    }


    // ---------------
    // GetByIdAsync
    // ---------------
    [Fact]
    public async Task GetByIdAsync_ExistingAsset_MustReturnAsset()
    {
        var created = await AssetSeeds.SeedAssetAsync(DbContext);

        var result = await _service.GetByIdAsync(created.Id);

        Assert.NotNull(result);
        Assert.Equal(created.Id, result.Id);
    }

    [Fact]
    public async Task GetByIdAsync_AssetNotFound_MustThrowNotFoundException()
    {
        await Assert.ThrowsAsync<NotFoundException>(() => _service.GetByIdAsync(Guid.NewGuid()));
    }

    // Helpers
    private static UpsertAssetRequest NewUpsertRequest(string name = "Enterprise application", string description = "Enterprise application used for managing business processes.", EAssetCriticality criticality = EAssetCriticality.High, EAssetType type = EAssetType.System)
    {
        return new UpsertAssetRequest
        {
            Name = name,
            Description = description,
            Criticality = criticality,
            Type = type
        };
    }
}
