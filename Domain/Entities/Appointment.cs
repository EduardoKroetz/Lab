using Lab.Api.Domain.Entities.Base;

namespace Lab.Api.Domain.Entities;

public class Appointment : TenantEntity
{
    public Appointment(string name, string? description, DateTime startDate, DateTime endDate, Customer customer, Offering? offering, ApplicationUser? createdByUser)
    {
        Name = name;
        Description = description;
        StartDate = startDate;
        EndDate = endDate;
        Customer = customer;
        CustomerId = customer.Id;
        Offering = offering;
        OfferingId = offering?.Id;
        CreatedByUser = createdByUser;
        CreatedBy = createdByUser?.Id;

        Validate();
    }

    public string Name { get; private set; }
    public string? Description { get; private set; }
    public Guid CustomerId { get; private set; }
    public Guid? OfferingId { get; private set; }
    public DateTime StartDate { get; private set; }
    public DateTime EndDate { get; private set; }
    public Guid? CreatedBy { get; private set; }

    public Customer Customer { get; private set; }
    public Offering? Offering { get; private set; }
    public ApplicationUser? CreatedByUser { get; private set; }

    private void Validate()
    {
        if (string.IsNullOrWhiteSpace(Name))
            throw new ArgumentException("Nome é obrigatório.");

        if (StartDate >= EndDate)
            throw new ArgumentException("A data de início deve ser anterior à data de término.");
    }

    public void Update(string name, string? description, DateTime startDate, DateTime endDate, Customer customer, Offering? offering)
    {
        Name = name;
        Description = description;
        StartDate = startDate;
        EndDate = endDate;
        Customer = customer;
        CustomerId = customer.Id;
        Offering = offering;
        OfferingId = offering?.Id;

        Validate();
    }
}
