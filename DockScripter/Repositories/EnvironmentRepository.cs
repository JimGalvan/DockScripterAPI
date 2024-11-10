using DockScripter.Domain.Entities;
using DockScripter.Repositories.Contexts;

namespace DockScripter.Repositories;

public class EnvironmentRepository : BaseRepository<EnvironmentEntity>
{
    public EnvironmentRepository(DataContext context) : base(context)
    {
    }
}