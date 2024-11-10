using DockScripter.Domain.Entities;
using DockScripter.Repositories.Contexts;
using Microsoft.EntityFrameworkCore;

namespace DockScripter.Repositories;

public class ScriptRepository : BaseRepository<ScriptEntity>
{
    public ScriptRepository(DataContext context) : base(context)
    {
    }
}