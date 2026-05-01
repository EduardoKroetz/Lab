namespace Lab.Application.DTOs.Users;

public class GetCurrentUserResponse
{
    public Guid Id { get; set; }
    public string? Email { get; set; }
    public Guid TenantId { get; set; }
}
