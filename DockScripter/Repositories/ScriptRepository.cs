using DockScripter.Domain.Entities;
using DockScripter.Repositories.Contexts;
using Microsoft.EntityFrameworkCore;

namespace DockScripter.Repositories;

public class ScriptRepository : BaseRepository<ScriptEntity>
{
    public ScriptRepository(DataContext context) : base(context)
    {
    }

    public async Task<IEnumerable<ScriptEntity>> GetScriptsByUserId(Guid userId, CancellationToken cancellationToken)
    {
        return await _context.Set<ScriptEntity>()
            .Where(x => x.UserId == userId)
            .OrderByDescending(x => x.CreationDateTimeUtc)
            .ToListAsync(cancellationToken);
    }
}