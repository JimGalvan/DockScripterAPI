using ParkOnyx.Domain.Dtos.Requests;

namespace ParkOnyx.Services.Interfaces;

public interface IAuthService
{
    Task<bool> RegisterUser(RegisterUserRequestDto request, CancellationToken cancellationToken);
    Task<string?> LoginUser(LoginUserRequestDto request, CancellationToken cancellationToken);

    void LogoutUser(string token);
}