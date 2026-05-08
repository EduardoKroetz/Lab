using AutoMapper;
using Lab.Application.DTOs.Threats;
using Lab.Application.Services;
using Lab.Domain.Enums;
using Lab.Domain.Exceptions;
using Lab.IntegrationTests.Common;
using Lab.IntegrationTests.Seeds;
using Microsoft.Extensions.Logging.Abstractions;

namespace Lab.IntegrationTests.Application.Services;

public class ThreatServiceTests : IntegrationTestsBase
{
    private readonly ThreatService _service;

    public ThreatServiceTests()
    {
        var mapperConfig = new MapperConfiguration(cfg => cfg.AddMaps(typeof(ThreatService).Assembly), NullLoggerFactory.Instance);

        _service = new ThreatService(DbContext, mapperConfig.CreateMapper());
    }

    // ----------------
    // CreateAsync
    // ----------------

    [Fact]
    public async Task CreateAsync_ValidRequest_MustPersistThreat()
    {
        var result = await _service.CreateAsync(NewUpsertRequest());

        var persisted = await DbContext.Threats.FindAsync(result.Id);
        Assert.NotNull(persisted);
        Assert.Equal(result.Name, persisted.Name);
        Assert.Equal(result.Description, persisted.Description);
        Assert.Equal(result.Category, persisted.Category);
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
        var created = await ThreatSeeds.SeedThreatAsync(DbContext);

        var result = await _service.UpdateAsync(created.Id, NewUpsertRequest("Updated Name", "Updated Description", EThreatCategory.UnauthorizedAccess));

        var persisted = await DbContext.Threats.FindAsync(result.Id);
        Assert.NotNull(persisted);
        Assert.Equal(result.Name, persisted.Name);
        Assert.Equal(result.Description, persisted.Description);
        Assert.Equal(result.Category, persisted.Category);
    }

    [Fact]
    public async Task UpdateAsync_NameAlreadyUsed_MustThrowValidationException()
    {
        await _service.CreateAsync(NewUpsertRequest());
        var created = await _service.CreateAsync(NewUpsertRequest("Another Name", "Another Description", EThreatCategory.DataBreach));

        await Assert.ThrowsAsync<ValidationException>(() => _service.UpdateAsync(created.Id, NewUpsertRequest()));
    }

    // ---------------
    // DeleteAsync
    // ---------------

    [Fact]
    public async Task DeleteAsync_ExistingThreat_MustRemoveFromDatabase()
    {
        var created = await ThreatSeeds.SeedThreatAsync(DbContext);

        await _service.DeleteAsync(created.Id);

        var persisted = await DbContext.Threats.FindAsync(created.Id);
        Assert.Null(persisted);
    }

    [Fact]
    public async Task DeleteAsync_ThreatNotFound_MustThrowNotFoundException()
    {
        await Assert.ThrowsAsync<NotFoundException>(() => _service.DeleteAsync(Guid.NewGuid()));
    }

    [Fact]
    public async Task DeleteAsync_ThreatWithLinkedRisk_MustThrowValidationException()
    {
        var risk = await RiskSeeds.SeedRiskAsync(DbContext, Clock);

        await Assert.ThrowsAsync<ValidationException>(() => _service.DeleteAsync(risk.ThreatId));
    }


    // ---------------
    // GetByIdAsync
    // ---------------
    [Fact]
    public async Task GetByIdAsync_ExistingThreat_MustReturnThreat()
    {
        var created = await ThreatSeeds.SeedThreatAsync(DbContext);

        var result = await _service.GetByIdAsync(created.Id);

        Assert.NotNull(result);
        Assert.Equal(created.Id, result.Id);
    }

    [Fact]
    public async Task GetByIdAsync_ThreatNotFound_MustThrowNotFoundException()
    {
        await Assert.ThrowsAsync<NotFoundException>(() => _service.GetByIdAsync(Guid.NewGuid()));
    }

    // Helpers
    private static UpsertThreatRequest NewUpsertRequest(string name = "Crash", string description = "desc", EThreatCategory category = EThreatCategory.Malware)
    {
        return new UpsertThreatRequest
        {
            Name = name,
            Description = description,
            Category = category
        };
    }
}
