using Microsoft.EntityFrameworkCore;
using coatify.Domain;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace coatify.Infrastructure.Configuration;

public class DeviceTypeConfiguration: IEntityTypeConfiguration<DeviceType>
{
    public void Configure(EntityTypeBuilder<DeviceType> builder)
    {
        builder.ToTable("DeviceTypes");
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Name).HasMaxLength(100);
        builder.Property(x => x.Description).HasMaxLength(500);
      
    }
}