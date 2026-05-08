using AutoMapper;
using Lab.Application.DTOs.IncidentImpacts;
using Lab.Application.DTOs.Incidents;
using Lab.Application.Services;
using Lab.Domain.Enums;
using Lab.Domain.Exceptions;
using Lab.IntegrationTests.Common;
using Lab.IntegrationTests.Seeds;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging.Abstractions;

namespace Lab.IntegrationTests.Application.Services;

public class IncidentServiceTests : IntegrationTestsBase
{
    private readonly IncidentService _service;

    public IncidentServiceTests()
    {
        var mapperConfig = new MapperConfiguration(cfg => cfg.AddMaps(typeof(IncidentService).Assembly), NullLoggerFactory.Instance);

        _service = new IncidentService(DbContext, mapperConfig.CreateMapper());
    }

    // ----------------
    // CreateAsync
    // ----------------

    [Fact]
    public async Task CreateAsync_ValidRequest_MustPersistIncident()
    {
        var risk = await RiskSeeds.SeedRiskAsync(DbContext, Clock);

        var result = await _service.CreateAsync(NewUpsertRequest(risk.Id));

        var persisted = await DbContext.Incidents.FindAsync(result.Id);
        Assert.NotNull(persisted);
        Assert.Equal(result.Description, persisted.Description);
        Assert.Equal(result.DateOccurred, persisted.DateOccurred);
        Assert.Equal(result.Status, persisted.Status);
        Assert.Equal(result.RiskId, persisted.RiskId);
    }

    // ---------------
    // UpdateAsync
    // ---------------
    [Fact]
    public async Task UpdateAsync_ValidRequest_MustPersistChanges()
    {
        var created = await IncidentSeeds.SeedIncidentAsync(DbContext, Clock);

        var result = await _service.UpdateAsync(created.Id, NewUpsertRequest(created.RiskId, "Updated Name", DateTime.UtcNow.AddMonths(-2), EIncidentStatus.Resolved));

        var persisted = await DbContext.Incidents.FindAsync(result.Id);
        Assert.NotNull(persisted);
        Assert.Equal(result.Description, persisted.Description);
        Assert.Equal(result.DateOccurred, persisted.DateOccurred);
        Assert.Equal(result.Status, persisted.Status);
        Assert.Equal(result.RiskId, persisted.RiskId);
    }

    // ---------------
    // DeleteAsync
    // ---------------

    [Fact]
    public async Task DeleteAsync_ExistingIncident_MustRemoveFromDatabase()
    {
        var created = await IncidentSeeds.SeedIncidentAsync(DbContext, Clock);

        await _service.DeleteAsync(created.Id);

        var persisted = await DbContext.Incidents.FindAsync(created.Id);
        Assert.Null(persisted);
    }

    [Fact]
    public async Task DeleteAsync_IncidentNotFound_MustThrowNotFoundException()
    {
        await Assert.ThrowsAsync<NotFoundException>(() => _service.DeleteAsync(Guid.NewGuid()));
    }

    [Fact]
    public async Task DeleteAsync_IncidentWithLinkedImpacts_MustRemoveFromDatabase()
    {
        var created = await IncidentSeeds.SeedIncidentAsync(DbContext, Clock);

        created.AddImpact(EIncidentImpactType.Reputation, 7, "desc");

        await _service.DeleteAsync(created.Id);

        var persistedIncident = await DbContext.Incidents.FindAsync(created.Id);
        var persistedImpacts = await DbContext.IncidentImpacts.Where(ii => ii.IncidentId == created.Id).ToListAsync();
        Assert.Null(persistedIncident);
        Assert.Empty(persistedImpacts);
    }

    // ---------------
    // GetByIdAsync
    // ---------------
    [Fact]
    public async Task GetByIdAsync_ExistingIncident_MustReturnIncident()
    {
        var created = await IncidentSeeds.SeedIncidentAsync(DbContext, Clock);

        var result = await _service.GetByIdAsync(created.Id);

        Assert.NotNull(result);
        Assert.Equal(created.Id, result.Id);
    }

    [Fact]
    public async Task GetByIdAsync_IncidentNotFound_MustThrowNotFoundException()
    {
        await Assert.ThrowsAsync<NotFoundException>(() => _service.GetByIdAsync(Guid.NewGuid()));
    }

    // ---------------
    // AddImpactAsync / RemoveImpactAsync
    // ---------------

    [Fact]
    public async Task AddImpactAsync_ValidRequest_MustLinkImpactToRisk()
    {
        var incident = await IncidentSeeds.SeedIncidentAsync(DbContext, Clock);

        await _service.AddImpactAsync(incident.Id, NewUpsertIncidentImpactRequest());

        ClearTracking();
        var impacts = await _service.GetListImpactsAsync(incident.Id);
        Assert.Single(impacts);
    }

    [Fact]
    public async Task AddImpactAsync_IncidentNotFound_MustThrowNotFoundException()
    {
        await Assert.ThrowsAsync<NotFoundException>(() => _service.AddImpactAsync(Guid.NewGuid(), NewUpsertIncidentImpactRequest()));
    }

    [Fact]
    public async Task UpdateImpactAsync_ValidRequest_MustPersistChanges()
    {
        var incident = await IncidentSeeds.SeedIncidentAsync(DbContext, Clock);
        var created = await _service.AddImpactAsync(incident.Id, NewUpsertIncidentImpactRequest());

        ClearTracking();
        var updated = await _service.UpdateImpactAsync(incident.Id, created.Id, NewUpsertIncidentImpactRequest());

        var persisted = await DbContext.IncidentImpacts.FirstAsync(ii => ii.Id == created.Id);
        Assert.NotNull(persisted);
        Assert.Equal(updated.Type, persisted.Type);
        Assert.Equal(updated.SeverityScore, persisted.SeverityScore);
        Assert.Equal(updated.Description, persisted.Description);
        Assert.Equal(updated.SeverityLevel, persisted.SeverityLevel);
    }

    [Fact]
    public async Task RemoveImpactAsync_ExistingImpact_MustUnlinkFromRisk()
    {
        var incident = await IncidentSeeds.SeedIncidentAsync(DbContext, Clock);

        ClearTracking();
        var impact = await _service.AddImpactAsync(incident.Id, NewUpsertIncidentImpactRequest());

        ClearTracking();
        await _service.RemoveImpactAsync(incident.Id, impact.Id);

        var persisted = await DbContext.Incidents.Include(i => i.IncidentImpacts).FirstAsync(i => i.Id == incident.Id);
        Assert.Empty(persisted.IncidentImpacts);
    }

    // Helpers
    private static UpsertIncidentRequest NewUpsertRequest(Guid riskId, string description = "Desc", DateTime? dateOccurred = null, EIncidentStatus status = EIncidentStatus.Investigating)
    {
        return new UpsertIncidentRequest
        {
            Description = description,
            DateOccurred = dateOccurred ?? DateTime.UtcNow.AddDays(-7),
            Status = status,
            RiskId = riskId
        };
    }

    private static UpsertIncidentImpactRequest NewUpsertIncidentImpactRequest()
    {
        return new UpsertIncidentImpactRequest
        {
            Description = "desc",
            SeverityScore = 6,
            Type = EIncidentImpactType.Reputation
        };
    }
}

