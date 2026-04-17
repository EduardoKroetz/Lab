namespace Lab.Api.Application.DTOs.Offerings;

public class GetOfferingDto
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    public string? Description { get; set; }
    public decimal? Price { get; set; }
}
