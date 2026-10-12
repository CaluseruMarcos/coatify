using System.Globalization;
using coatify.Application.Services;
using coatify.Domain;
using coatify.Infrastructure.Repositories;
using Microsoft.Extensions.Logging.Abstractions;

namespace Coatify.Tests;

public class GetDevice
{
    [Theory]
    [InlineData(
        "c477be02-183d-4b71-8adf-5e465d754cb0",
        "maschine 2",
        "4eff474d-a242-48a9-80ae-c12b63d876c2",
        "18c89e73-e4e0-45a3-93bc-10022817d7be",
        "working ",
        "2026-09-18 15:25:44.599394 +00:00")]
    [InlineData(
        "9d20d0bb-517c-433b-8ee8-aa65c51eb530",
        "maschine 5",
        "e2ad8f36-f9c9-4b51-8ec2-1a3cd6d4c26a",
        "65cd1857-2573-4fa2-a672-fdd4b0730ae5",
        "geht",
        "2026-09-18 14:07:50.000017 +00:00")]
    public async Task GetDeviceTest(
        string idText,
        string name,
        string serialNumber,
        string deviceTypeIdText,
        string status,
        string createdAtText)
    {
        var id = Guid.Parse(idText);
        var deviceTypeId = Guid.Parse(deviceTypeIdText);
        var createdAt = DateTimeOffset.Parse(createdAtText, CultureInfo.InvariantCulture).UtcDateTime;
        var database = new TestDatabase();
        await database.ConnectToDatabase();

        // Testdaten anlegen; bei einem weiteren Testlauf dieselben Datensätze aktualisieren.
        await using (var setupContext = database.CreateContext())
        {
            var deviceType = await setupContext.DeviceTypes.FindAsync(deviceTypeId);
            if (deviceType is null)
            {
                deviceType = new DeviceType
                {
                    Id = deviceTypeId,
                    Name = "Test device type",
                    Description = "Device type for GetDeviceTest"
                };
                setupContext.DeviceTypes.Add(deviceType);
            }

            var device = await setupContext.Devices.FindAsync(id);
            if (device is null)
            {
                device = new Device { Id = id };
                setupContext.Devices.Add(device);
            }

            device.Name = name;
            device.SerialNumber = serialNumber;
            device.DeviceType = deviceType;
            device.Status = status;
            device.CreatedAt = createdAt;
            await setupContext.SaveChangesAsync();
        }

        // Ein neuer Context liest die Daten so, wie es die Anwendung tun würde.
        await using var readContext = database.CreateContext();
        var service = new DeviceService(
            new DeviceRepository(),
            NullLogger<DeviceService>.Instance,
            readContext);

        var result = await service.GetDevice(id);

        Assert.Equal(id, result.Id);
        Assert.Equal(name, result.Name);
        Assert.Equal(status, result.Status);
    }
}
