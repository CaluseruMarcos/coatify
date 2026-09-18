namespace coatify.Domain;

public class Device
{
    public Guid Id { get;  set; }

    public string Name { get; set; }

    public string SerialNumber { get;  set; }

    public Guid DeviceTypeId { get; set; }

    public DeviceType DeviceType { get;  set; }

    public string Status { get;  set; }

    public DateTime CreatedAt { get;  set; }
}