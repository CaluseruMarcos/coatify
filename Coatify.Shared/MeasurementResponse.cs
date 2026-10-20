namespace Coatify.Shared;

public class MeasurementResponse
{
    public Guid Id { get; init; }
    public DateTime Timestamp { get; init; }
    public float Temperature { get; init; }
    public float Pressure { get; init; }
    public float PowerConsumption { get; init; }
}
