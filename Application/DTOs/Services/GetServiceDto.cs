namespace Lab.Api.Application.DTOs.Services;

public class GetServiceDto
{
    public GetServiceDto(Guid id, string name, string? description, decimal? price)
    {
        Id = id;
        Name = name;
        Description = description;
        Price = price;
    }

    public Guid Id { get; set; }
    public string Name { get; set; }
    public string? Description { get; set; }
    public decimal? Price { get; set; }
}
