using DockScripter.Domain.Entities;
using DockScripter.Repositories.Contexts;
using DockScripter.Repositories.Interfaces;

namespace DockScripter.Repositories
{
    public class UserRepository : BaseRepository<UserEntity>, IUserRepository
    {
        public UserRepository(DataContext context) : base(context)
        {
        }
    }
}