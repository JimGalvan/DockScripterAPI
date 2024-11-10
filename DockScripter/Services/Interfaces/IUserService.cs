using DockScripter.Domain.Entities;

namespace DockScripter.Services;

public interface IUserService
{
    Task<UserEntity?> GetUserByIdAsync(Guid userId, CancellationToken cancellationToken);
    Task<bool> CreateUserAsync(UserEntity user, CancellationToken cancellationToken);
    Task<bool> DeleteUserAsync(Guid userId, CancellationToken cancellationToken);
}