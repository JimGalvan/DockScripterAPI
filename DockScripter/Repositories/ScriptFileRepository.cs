using DockScripter.Domain.Entities;
using DockScripter.Repositories.Contexts;

namespace DockScripter.Repositories;

public class ScriptFileRepository : BaseRepository<ScriptFile>
{
    public ScriptFileRepository(DataContext context) : base(context) { }
}