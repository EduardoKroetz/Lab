using Lab.Domain.Entities;
using Lab.Domain.Enums;
using Lab.Infrastructure.Data;

namespace Lab.IntegrationTests.Seeds;

public static class ControlSeeds
{

    public static async Task<Control> SeedControlAsync(ApplicationDbContext dbContext)
    {
        var control = new Control("Audit log record", "It stores records of actions performed in the system to allow for traceability and detection of suspicious activity.", EControlType.Preventive, EControlCategory.Organizational);

        dbContext.Controls.Add(control);
        await dbContext.SaveChangesAsync();

        return control;
    }
}
