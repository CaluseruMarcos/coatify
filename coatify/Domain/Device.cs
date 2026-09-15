namespace coatify.Domain;

public class Device
{
    public Guid Id { get; private set; }

    public string Name { get; private set; }

    public string SerialNumber { get; private set; }

    public DeviceType DeviceType { get; private set; }

    public string Status { get; private set; }

    public DateTime CreatedAt { get; private set; }
}