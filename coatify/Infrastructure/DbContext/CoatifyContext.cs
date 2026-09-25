using coatify.Domain;
using coatify.Infrastructure.Configuration;

namespace coatify.Infrastructure.DbContext;
using Microsoft.EntityFrameworkCore;
public class CoatifyContext:DbContext
{
    public CoatifyContext(DbContextOptions<CoatifyContext> options) : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
     base.OnModelCreating(modelBuilder);
        modelBuilder.ApplyConfiguration(new DeviceConfiguration());
        modelBuilder.ApplyConfiguration(new DeviceTypeConfiguration());
        modelBuilder.ApplyConfiguration(new UserConfiguration());
        modelBuilder.ApplyConfiguration(new MeasurementConfiguration());
        
    }
    public DbSet<Device> Devices { get; set; }
    public DbSet<User> Users { get; set; }
    public DbSet<DeviceType> DeviceTypes { get; set; }
    public DbSet<Measurement> Measurements { get; set; }
    
    
}