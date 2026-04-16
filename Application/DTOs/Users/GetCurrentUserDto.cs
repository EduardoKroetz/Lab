namespace Lab.Api.Application.DTOs.Users;

public class GetCurrentUserDto
{
    public Guid Id { get; set; }
    public string? UserName { get; set; }
    public string? Email { get; set; }
    public string? Phone { get; set; }
}
