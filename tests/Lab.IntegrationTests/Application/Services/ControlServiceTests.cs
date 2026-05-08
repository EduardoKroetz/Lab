using AutoMapper;
using Lab.Application.DTOs.Controls;
using Lab.Application.Services;
using Lab.Domain.Enums;
using Lab.Domain.Exceptions;
using Lab.IntegrationTests.Common;
using Lab.IntegrationTests.Seeds;
using Microsoft.Extensions.Logging.Abstractions;

namespace Lab.IntegrationTests.Application.Services;

public class ControlServiceTests : IntegrationTestsBase
{
    private readonly ControlService _service;

    public ControlServiceTests()
    {
        var mapperConfig = new MapperConfiguration(cfg => cfg.AddMaps(typeof(ControlService).Assembly), NullLoggerFactory.Instance);

        _service = new ControlService(DbContext, mapperConfig.CreateMapper());
    }

    // ----------------
    // CreateAsync
    // ----------------

    [Fact]
    public async Task CreateAsync_ValidRequest_MustPersistControl()
    {
        var result = await _service.CreateAsync(NewUpsertRequest());

        var persisted = await DbContext.Controls.FindAsync(result.Id);
        Assert.NotNull(persisted);
        Assert.Equal(result.Name, persisted.Name);
        Assert.Equal(result.Description, persisted.Description);
        Assert.Equal(result.Type, persisted.Type);
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
        var created = await ControlSeeds.SeedControlAsync(DbContext);

        var result = await _service.UpdateAsync(created.Id, NewUpsertRequest("Updated Name", "Updated Description", EControlType.Detective, EControlCategory.Physical));

        var persisted = await DbContext.Controls.FindAsync(result.Id);
        Assert.NotNull(persisted);
        Assert.Equal(result.Name, persisted.Name);
        Assert.Equal(result.Description, persisted.Description);
        Assert.Equal(result.Type, persisted.Type);
    }

    [Fact]
    public async Task UpdateAsync_NameAlreadyUsed_MustThrowValidationException()
    {
        await _service.CreateAsync(NewUpsertRequest());
        var created = await _service.CreateAsync(NewUpsertRequest("Another Name", "Another Description", EControlType.Corrective, EControlCategory.Regulatory));

        await Assert.ThrowsAsync<ValidationException>(() => _service.UpdateAsync(created.Id, NewUpsertRequest()));
    }

    // ---------------
    // DeleteAsync
    // ---------------

    [Fact]
    public async Task DeleteAsync_ExistingControl_MustRemoveFromDatabase()
    {
        var created = await ControlSeeds.SeedControlAsync(DbContext);

        await _service.DeleteAsync(created.Id);

        var persisted = await DbContext.Vulnerabilities.FindAsync(created.Id);
        Assert.Null(persisted);
    }

    [Fact]
    public async Task DeleteAsync_ControlNotFound_MustThrowNotFoundException()
    {
        await Assert.ThrowsAsync<NotFoundException>(() => _service.DeleteAsync(Guid.NewGuid()));
    }

    [Fact]
    public async Task DeleteAsync_ControlWithLinkedRiskControls_MustThrowValidationException()
    {
        var risk = await RiskSeeds.SeedRiskAsync(DbContext, Clock);
        var control = await ControlSeeds.SeedControlAsync(DbContext);

        risk.AddControl(control.Id, control.Type, 60);

        DbContext.Risks.Update(risk);
        await DbContext.SaveChangesAsync();

        await Assert.ThrowsAsync<ValidationException>(() => _service.DeleteAsync(control.Id));
    }

    // ---------------
    // GetByIdAsync
    // ---------------
    [Fact]
    public async Task GetByIdAsync_ExistingControl_MustReturnControl()
    {
        var created = await ControlSeeds.SeedControlAsync(DbContext);

        var result = await _service.GetByIdAsync(created.Id);

        Assert.NotNull(result);
        Assert.Equal(created.Id, result.Id);
    }

    [Fact]
    public async Task GetByIdAsync_ControlNotFound_MustThrowNotFoundException()
    {
        await Assert.ThrowsAsync<NotFoundException>(() => _service.GetByIdAsync(Guid.NewGuid()));
    }

    // Helpers
    private static UpsertControlRequest NewUpsertRequest(string name = "Network Firewall", string description = "It controls inbound and outbound network traffic to block unauthorized access.", EControlType type = EControlType.Preventive, EControlCategory category = EControlCategory.Technical)
    {
        return new UpsertControlRequest
        {
            Name = name,
            Description = description,
            Type = type,
            Category = category
        };
    }
}

