using coatify.Domain;

namespace Coatify.Tests;

public class CreateDevice
{
    [Fact]
    public async Task CreateDeviceTest()
    {
        var context= new TestDatabase().CreateContext();
        Guid deviceTypeId = Guid.NewGuid();
        var deviceType = new DeviceType
        {
            Id= deviceTypeId,
            Name = "TestDeviceType",
            Description = "Test description",
            

        };
        var device = new Device
        {
            Name = "TestDevice",
            CreatedAt = DateTime.UtcNow,
            
            DeviceType = deviceType,
            SerialNumber = Guid.NewGuid().ToString(),
            Status = "Active"
            
        };
        
        context.Devices.Add(device);
        var result = await context.SaveChangesAsync();
        
        Assert.Equal(2, result);


    }
}
