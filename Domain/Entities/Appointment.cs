using Lab.Api.Domain.Entities.Base;

namespace Lab.Api.Domain.Entities;

public class Appointment : TenantEntity
{
    public string Name { get; set; }
    public string? Description { get; set; }
    public Guid CustomerId { get; set; }
    public Guid? ServiceId { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public Guid? CreatedBy { get; set; }

    public Customer Customer { get; set; }
    public Service? Service { get; set; }
    public ApplicationUser? CreatedByUser { get; set; }
}
