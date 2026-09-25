using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using coatify.Domain;

namespace coatify.Infrastructure.Configuration;









public class DeviceConfiguration : IEntityTypeConfiguration<Device>
{
    public void Configure(EntityTypeBuilder<Device> builder)
    {
        builder.ToTable("Devices");
        builder.HasKey(x => x.Id);
        builder.HasIndex(device => device.SerialNumber)
            .IsUnique();
        builder.Property(x => x.Name).IsRequired().HasMaxLength(100);
        builder.Property(x => x.DeviceTypeId).IsRequired();
        builder.Property(x => x.Status).HasMaxLength(50);
        
    }
}