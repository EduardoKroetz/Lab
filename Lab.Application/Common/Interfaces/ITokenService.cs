namespace Lab.Application.Common.Interfaces;

public interface ITokenService
{
    string GenerateAccessToken(IUser user);
}
