using AutoMapper;
using Lab.Application.Common.Interfaces;
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

    public async Task<GetCurrentUserResponse> GetCurrentUserAsync()
    {
        var userId = _userProvider.UserId;
        var user = await _identityService.GetUserByIdAsync(userId);

        return _mapper.Map<GetCurrentUserResponse>(user);
    }
}
