using DockScripter.Domain.Entities;
using DockScripter.Repositories;

namespace DockScripter.Services;

public class UserService : IUserService
{
    private readonly UserRepository _userRepository;

    public UserService(UserRepository userRepository)
    {
        _userRepository = userRepository;
    }

    public async Task<UserEntity?> GetUserByIdAsync(Guid userId, CancellationToken cancellationToken) =>
        await _userRepository.SelectById(userId, cancellationToken);

    public async Task<bool> CreateUserAsync(UserEntity user, CancellationToken cancellationToken)
    {
        await _userRepository.AddAsync(user, cancellationToken);
        await _userRepository.SaveChangesAsync(cancellationToken);
        return true;
    }

    public async Task<bool> DeleteUserAsync(Guid userId, CancellationToken cancellationToken) =>
        await _userRepository.DeleteById(userId, cancellationToken);
}