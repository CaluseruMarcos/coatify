namespace Coatify.Tests;

public class UpdateDevice
{
    [Fact]
    public void UpdateDeviceTest()
    {
        var context= new TestDatabase().CreateContext();
        var device = context.Devices.First();
        device.Name = "UpdatedDeviceName";
        context.Devices.Update(device);
        context.SaveChanges();
        var updatedDevice = context.Devices.First(d => d.Id == device.Id);
        Assert.Equal("UpdatedDeviceName", updatedDevice.Name);
    }
}