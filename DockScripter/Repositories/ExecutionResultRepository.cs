using DockScripter.Domain.Entities;
using DockScripter.Repositories.Contexts;

namespace DockScripter.Repositories;

public class ExecutionResultRepository : BaseRepository<ExecutionResultEntity>
{
    public ExecutionResultRepository(DataContext context) : base(context)
    {
    }
}