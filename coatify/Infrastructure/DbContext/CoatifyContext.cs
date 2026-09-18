using coatify.Domain;

namespace coatify.Infrastructure.DbContext;
using Microsoft.EntityFrameworkCore;
public class CoatifyContext:DbContext
{
    public CoatifyContext(DbContextOptions<CoatifyContext> options) : base(options)
    {
    }

    
    public DbSet<Device> Devices { get; set; }
    public DbSet<User> Users { get; set; }
    public DbSet<DeviceType> DeviceTypes { get; set; }
    public DbSet<Measurement> Measurements { get; set; }
    
    
}