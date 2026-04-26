using Lab.Application.Common.Models;

namespace Lab.Application.Common.Interfaces;

public interface IIdentityService
{
    Task<Result<IUser>> GetUserIdByEmailAsync(string email);
    Task<Result<IUser>> AuthenticateAsync(string email, string password);
    Task<Result<IUser>> CreateUserAsync(string email, string password, Guid tenantId);
    Task<Result<IUser>> GetUserByIdAsync(Guid userId);
}
