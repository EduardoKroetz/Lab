using Lab.Application.Common.Interfaces;
using Lab.Application.DTOs.Auth;
using Lab.Domain.Common.Models;
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

    public async Task<Result<LoginUserResponse>> LoginAsync(LoginUserRequest request)
    {
        var authResult = await _identityService.AuthenticateAsync(request.Email, request.Password);
        if (!authResult.Succeeded)
            return Result<LoginUserResponse>.Failure(authResult.Errors);

        var token = _tokenService.GenerateAccessToken(user: authResult.Value);

        var response = new LoginUserResponse
        {
            Token = token
        };

        return Result<LoginUserResponse>.Success(response);
    }

    public async Task<Result<CreateUserResponse>> RegisterAsync(RegisterUserRequest request)
    {
        if (!await _dbContext.Tenants.AnyAsync(t => t.Id == request.TenantId))
            return Result<CreateUserResponse>.Failure("Tenant não encontrado.");

        var result = await _identityService.CreateUserAsync(request.Email, request.Password, request.TenantId);
        if (!result.Succeeded)
            return Result<CreateUserResponse>.Failure(result.Errors);

        var response = new CreateUserResponse
        {
            UserId = result.Value.Id
        };

        return Result<CreateUserResponse>.Success(response);
    }
}
