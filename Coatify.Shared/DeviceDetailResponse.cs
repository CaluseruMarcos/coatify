namespace Coatify.Shared;

public class DeviceDetailResponse
{
    public Guid Id { get; init; }
    public string Name { get; init; } = string.Empty;
    public string SerialNumber { get; init; } = string.Empty;
    public string Status { get; init; } = string.Empty;
    public Guid DeviceTypeId { get; init; }
    public string DeviceTypeName { get; init; } = string.Empty;
}
