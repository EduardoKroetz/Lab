namespace Lab.Application.Common.Interfaces;

public interface IIdentityService
{
    Task<IUser> GetUserIdByEmailAsync(string email);
    Task<IUser> AuthenticateAsync(string email, string password);
    Task<IUser> CreateUserAsync(string email, string password, Guid tenantId);
    Task<IUser> GetUserByIdAsync(Guid userId);
}
