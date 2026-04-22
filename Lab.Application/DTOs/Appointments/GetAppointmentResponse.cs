namespace Lab.Application.DTOs.Appointments;

public class GetAppointmentResponse
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    public string? Description { get; set; }
    public Guid CustomerId { get; set; }
    public string CustomerName { get; set; }
    public Guid? OfferingId { get; set; }
    public string? OfferingName { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public Guid? CreatedBy { get; set; }
    public string? CreatedByName { get; set; }
}
