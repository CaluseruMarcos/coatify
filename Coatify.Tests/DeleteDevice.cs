namespace Coatify.Tests;

public class DeleteDevice
{
    [Fact]
    public async Task DeleteDeviceTest()
    {
        var context= new TestDatabase().CreateContext();
        var device = context.Devices.First();
        context.Devices.Remove(device);
        await context.SaveChangesAsync();
        Assert.True(context.Devices.All(d => d.Id != device.Id));
        
    }
}