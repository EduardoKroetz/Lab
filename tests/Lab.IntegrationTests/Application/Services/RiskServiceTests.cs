using AutoMapper;
using Lab.Application.DTOs.RiskControls;
using Lab.Application.DTOs.Risks;
using Lab.Application.Services;
using Lab.Domain.Enums;
using Lab.Domain.Exceptions;
using Lab.IntegrationTests.Common;
using Lab.IntegrationTests.Seeds;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging.Abstractions;

namespace Lab.IntegrationTests.Application.Services;

public class RiskServiceTests : IntegrationTestsBase
{
    private readonly RiskService _service;

    public RiskServiceTests()
    {
        var mapperConfig = new MapperConfiguration(cfg => cfg.AddMaps(typeof(RiskService).Assembly), NullLoggerFactory.Instance);

        _service = new RiskService(DbContext, mapperConfig.CreateMapper(), Clock);
    }

    // -------------------------------------------------------
    // CreateAsync
    // -------------------------------------------------------

    [Fact]
    public async Task CreateAsync_ValidRequest_MustPersistRisk()
    {
        var (asset, threat, vulnerability) = await RiskSeeds.SeedBaseEntitiesAsync(DbContext);

        var result = await _service.CreateAsync(NewInsertRequest(asset.Id, threat.Id, vulnerability.Id));

        var persisted = await DbContext.Risks.FindAsync(result.Id);
        Assert.NotNull(persisted);
        Assert.Equal(ERiskStatus.Identified, persisted.Status);
        Assert.Equal(Clock.UtcNow.AddMonths(3), persisted.ReviewFixedDate);
    }

    [Fact]
    public async Task CreateAsync_DuplicateCombination_MustThrowValidationException()
    {
        var (asset, threat, vulnerability) = await RiskSeeds.SeedBaseEntitiesAsync(DbContext);
        await _service.CreateAsync(NewInsertRequest(asset.Id, threat.Id, vulnerability.Id));

        await Assert.ThrowsAsync<ValidationException>(() => _service.CreateAsync(NewInsertRequest(asset.Id, threat.Id, vulnerability.Id)));
    }

    [Fact]
    public async Task CreateAsync_AssetNotFound_MustThrowNotFoundException()
    {
        var (_, threat, vulnerability) = await RiskSeeds.SeedBaseEntitiesAsync(DbContext);

        await Assert.ThrowsAsync<NotFoundException>(() => _service.CreateAsync(NewInsertRequest(Guid.NewGuid(), threat.Id, vulnerability.Id)));
    }

    [Fact]
    public async Task CreateAsync_ThreatNotFound_MustThrowNotFoundException()
    {
        var (asset, _, vulnerability) = await RiskSeeds.SeedBaseEntitiesAsync(DbContext);

        await Assert.ThrowsAsync<NotFoundException>(() => _service.CreateAsync(NewInsertRequest(asset.Id, Guid.NewGuid(), vulnerability.Id)));
    }

    [Fact]
    public async Task CreateAsync_VulnerabilityNotFound_MustThrowNotFoundException()
    {
        var (asset, threat, _) = await RiskSeeds.SeedBaseEntitiesAsync(DbContext);

        await Assert.ThrowsAsync<NotFoundException>(() => _service.CreateAsync(NewInsertRequest(asset.Id, threat.Id, Guid.NewGuid())));
    }

    // -------------------------------------------------------
    // GetByIdAsync
    // -------------------------------------------------------

    [Fact]
    public async Task GetByIdAsync_ExistingRisk_MustReturnRisk()
    {
        var risk = await RiskSeeds.SeedRiskAsync(DbContext, Clock);

        var result = await _service.GetByIdAsync(risk.Id);

        Assert.NotNull(result);
        Assert.Equal(risk.Id, result.Id);
    }

    [Fact]
    public async Task GetByIdAsync_RiskNotFound_MustThrowNotFoundException()
    {
        await Assert.ThrowsAsync<NotFoundException>(() => _service.GetByIdAsync(Guid.NewGuid()));
    }

    // -------------------------------------------------------
    // UpdateAsync
    // -------------------------------------------------------

    [Fact]
    public async Task UpdateAsync_ValidRequest_MustPersistChanges()
    {
        var (asset, threat, vulnerability) = await RiskSeeds.SeedBaseEntitiesAsync(DbContext);
        var created = await _service.CreateAsync(NewInsertRequest(asset.Id, threat.Id, vulnerability.Id));

        ClearTracking();

        var updateRequest = new UpdateRiskRequest
        {
            Probability = 5,
            Impact = 5,
            Status = ERiskStatus.Identified,
            Treatment = null,
            TreatmentDescription = null,
            ReviewFixedDate = Clock.UtcNow.AddMonths(6),
            ReviewInterval = null
        };

        await _service.UpdateAsync(created.Id, updateRequest);

        var persisted = await DbContext.Risks.FindAsync(created.Id);
        Assert.Equal(5, persisted!.Probability);
        Assert.Equal(5, persisted.Impact);
    }

    [Fact]
    public async Task UpdateAsync_RiskNotFound_MustThrowNotFoundException()
    {
        var request = new UpdateRiskRequest
        {
            Probability = 3,
            Impact = 3,
            Status = ERiskStatus.Identified,
            ReviewFixedDate = Clock.UtcNow.AddMonths(3),
            ReviewInterval = null
        };

        await Assert.ThrowsAsync<NotFoundException>(() => _service.UpdateAsync(Guid.NewGuid(), request));
    }

    // -------------------------------------------------------
    // DeleteAsync
    // -------------------------------------------------------

    [Fact]
    public async Task DeleteAsync_ExistingRisk_MustRemoveFromDatabase()
    {
        var risk = await RiskSeeds.SeedRiskAsync(DbContext, Clock);

        await _service.DeleteAsync(risk.Id);

        var persisted = await DbContext.Risks.FindAsync(risk.Id);
        Assert.Null(persisted);
    }

    [Fact]
    public async Task DeleteAsync_RiskNotFound_MustThrowNotFoundException()
    {
        await Assert.ThrowsAsync<NotFoundException>(() => _service.DeleteAsync(Guid.NewGuid()));
    }

    // -------------------------------------------------------
    // AddControlAsync / RemoveControlAsync
    // -------------------------------------------------------

    [Fact]
    public async Task AddControlAsync_ValidRequest_MustLinkControlToRisk()
    {
        var risk = await RiskSeeds.SeedRiskAsync(DbContext, Clock);
        var control = await RiskSeeds.SeedControlAsync(DbContext);

        ClearTracking();
        await _service.AddControlAsync(risk.Id, new InsertRiskControlRequest { ControlId = control.Id });

        var persisted = await DbContext.Risks.Include(r => r.RiskControls).FirstAsync(r => r.Id == risk.Id);
        Assert.Single(persisted.RiskControls);
    }

    [Fact]
    public async Task AddControlAsync_RiskNotFound_MustThrowNotFoundException()
    {
        var control = await RiskSeeds.SeedControlAsync(DbContext);

        await Assert.ThrowsAsync<NotFoundException>(() => _service.AddControlAsync(Guid.NewGuid(), new InsertRiskControlRequest { ControlId = control.Id }));
    }

    [Fact]
    public async Task AddControlAsync_ControlNotFound_MustThrowNotFoundException()
    {
        var risk = await RiskSeeds.SeedRiskAsync(DbContext, Clock);

        await Assert.ThrowsAsync<NotFoundException>(() => _service.AddControlAsync(risk.Id, new InsertRiskControlRequest { ControlId = Guid.NewGuid() }));
    }

    [Fact]
    public async Task RemoveControlAsync_ExistingControl_MustUnlinkFromRisk()
    {
        var risk = await RiskSeeds.SeedRiskAsync(DbContext, Clock);
        var control = await RiskSeeds.SeedControlAsync(DbContext);
        ClearTracking();
        await _service.AddControlAsync(risk.Id, new InsertRiskControlRequest { ControlId = control.Id });

        ClearTracking();
        await _service.RemoveControlAsync(risk.Id, control.Id);

        var persisted = await DbContext.Risks.Include(r => r.RiskControls).FirstAsync(r => r.Id == risk.Id);
        Assert.Empty(persisted.RiskControls);
    }

    // -------------------------------------------------------
    // Helpers
    // -------------------------------------------------------

    private InsertRiskRequest NewInsertRequest(Guid assetId, Guid threatId, Guid vulnerabilityId)
    {
        return new InsertRiskRequest
        {
            AssetId = assetId,
            ThreatId = threatId,
            VulnerabilityId = vulnerabilityId,
            Probability = 3,
            Impact = 4,
            ReviewFixedDate = Clock.UtcNow.AddMonths(3),
            ReviewInterval = null
        };
    }
}