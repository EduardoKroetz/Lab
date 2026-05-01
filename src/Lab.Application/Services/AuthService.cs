using Lab.Application.Common.Interfaces;
using Lab.Application.DTOs.Auth;
using Lab.Domain.Exceptions;
using Microsoft.EntityFrameworkCore;

namespace Lab.Application.Services;

public class AuthService
{
    private readonly IIdentityService _identityService;
    private readonly ITokenService _tokenService;
    private readonly IApplicationDbContext _dbContext;

    public AuthService(IIdentityService identityService, ITokenService tokenService, IApplicationDbContext dbContext)
    {
        _identityService = identityService;
        _tokenService = tokenService;
        _dbContext = dbContext;
    }

    public async Task<LoginUserResponse> LoginAsync(LoginUserRequest request)
    {
        var user = await _identityService.AuthenticateAsync(request.Email, request.Password);
        var token = _tokenService.GenerateAccessToken(user);

        return new LoginUserResponse
        {
            Token = token
        };
    }

    public async Task<RegisterUserResponse> RegisterAsync(RegisterUserRequest request)
    {
        if (!await _dbContext.Tenants.AnyAsync(t => t.Id == request.TenantId))
            throw new NotFoundException("Tenant não encontrado.");

        var user = await _identityService.CreateUserAsync(request.Email, request.Password, request.TenantId);

        return new RegisterUserResponse
        {
            UserId = user.Id
        };
    }
}
