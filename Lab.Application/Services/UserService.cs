using AutoMapper;
using Lab.Application.Common.Interfaces;
using Lab.Application.Common.Models;
using Lab.Application.DTOs.Users;

namespace Lab.Application.Services;

public class UserService
{
    private readonly IIdentityService _identityService;
    private readonly IUserProvider _userProvider;
    private readonly IMapper _mapper;

    public UserService(IIdentityService identityService, IUserProvider userProvider, IMapper mapper)
    {
        _identityService = identityService;
        _userProvider = userProvider;
        _mapper = mapper;
    }

    public async Task<Result<GetCurrentUserResponse>> GetCurrentUserAsync()
    {
        var userId = _userProvider.UserId;

        var userResult = await _identityService.GetUserByIdAsync(userId);
        if (!userResult.Succeeded)
            return Result<GetCurrentUserResponse>.Failure(userResult.Errors);

        var response = _mapper.Map<GetCurrentUserResponse>(userResult.Value);

        return Result<GetCurrentUserResponse>.Success(response);
    }
}
