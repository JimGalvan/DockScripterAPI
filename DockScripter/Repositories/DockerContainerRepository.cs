using DockScripter.Domain.Entities;
using DockScripter.Repositories.Contexts;

namespace DockScripter.Repositories;

public class DockerContainerRepository : BaseRepository<DockerContainerEntity>
{
    public DockerContainerRepository(DataContext context) : base(context)
    {
    }
}