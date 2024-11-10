using Microsoft.EntityFrameworkCore;
using ParkOnyx.Entities;

namespace ParkOnyx.Repositories.Contexts;

public class DataContext : DbContext
{
    public DataContext(DbContextOptions<DataContext> options) : base(options)
    {
    }

    public DbSet<UserEntity> UserEntities { get; init; }
    public DbSet<ReservationEntity> ReservationEntities { get; init; }
    public DbSet<ParkingLotEntity> ParkingLotEntities { get; init; }
    public DbSet<ParkingSpotEntity> ParkingSpotEntities { get; init; }
}