using DockScripter.Domain.Dtos.Requests;

namespace DockScripter.Services.Interfaces;

public interface IAuthService
{
    Task<bool> RegisterUser(RegisterUserRequestDto request, CancellationToken cancellationToken);
    Task<string?> LoginUser(LoginUserRequestDto request, CancellationToken cancellationToken);

    void LogoutUser(string token);
}