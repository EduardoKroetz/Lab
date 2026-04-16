namespace Lab.Api.Application.DTOs.Appointments;

public class GetAppointmentDto
{
    public GetAppointmentDto(Guid id, string name, string? description, Guid customerId, string customerName, Guid? serviceId, string? serviceName, DateTime startDate, DateTime endDate, Guid? createdBy, string? createdByName)
    {
        Id = id;
        Name = name;
        Description = description;
        CustomerId = customerId;
        CustomerName = customerName;
        ServiceId = serviceId;
        ServiceName = serviceName;
        StartDate = startDate;
        EndDate = endDate;
        CreatedBy = createdBy;
        CreatedByName = createdByName;
    }

    public Guid Id { get; set; }
    public string Name { get; set; }
    public string? Description { get; set; }

    public Guid CustomerId { get; set; }
    public string CustomerName { get; set; }

    public Guid? ServiceId { get; set; }
    public string? ServiceName { get; set; }

    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }

    public Guid? CreatedBy { get; set; }
    public string? CreatedByName { get; set; }
}
