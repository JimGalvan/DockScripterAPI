using DockScripter.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace DockScripter.Repositories.Contexts;

public class DataContext : DbContext
{
    public DataContext(DbContextOptions<DataContext> options) : base(options)
    {
    }

    public DbSet<UserEntity> UserEntities { get; init; }
    public DbSet<EnvironmentEntity> EnvironmentEntities { get; init; }
    public DbSet<ExecutionResultEntity> ExecutionResultEntities { get; init; }
    public DbSet<ScriptEntity> ScriptEntities { get; init; }
}