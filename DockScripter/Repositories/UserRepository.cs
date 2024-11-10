using ParkOnyx.Entities;
using ParkOnyx.Repositories.Contexts;
using ParkOnyx.Repositories.Interfaces;

namespace ParkOnyx.Repositories
{
    public class UserRepository : BaseRepository<UserEntity>, IUserRepository
    {
        public UserRepository(DataContext context) : base(context)
        {
        }
    }
}