﻿using DockScripter.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using DockScripter.Entities;

namespace DockScripter.Repositories.Contexts;

public class DataContext : DbContext
{
    public DataContext(DbContextOptions<DataContext> options) : base(options)
    {
    }

    public DbSet<UserEntity> UserEntities { get; init; }
}