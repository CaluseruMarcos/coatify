namespace coatify.Application;

public class UpdateDeviceRequest
{
    public required string Name { get; init; }
    public required string SerialNumber { get; init; }
    public required string Status { get; init; }
    public required Guid DeviceTypeId { get; init; }
}
