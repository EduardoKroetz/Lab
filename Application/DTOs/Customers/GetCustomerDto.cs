namespace Lab.Api.Application.DTOs.Customers;

public class GetCustomerDto
{
    public GetCustomerDto(Guid id, string name, string? cpfCnpj, string? email, string? phoneNumber)
    {
        Id = id;
        Name = name;
        CpfCnpj = cpfCnpj;
        Email = email;
        PhoneNumber = phoneNumber;
    }

    public Guid Id { get; set; }
    public string Name { get; set; }
    public string? CpfCnpj { get; set; }
    public string? Email { get; set; }
    public string? PhoneNumber { get; set; }
}
