namespace Lab.Api.Application.DTOs.Customers;

public class GetCustomerDto
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    public string? CpfCnpj { get; set; }
    public string? Email { get; set; }
    public string? PhoneNumber { get; set; }
}
