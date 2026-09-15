namespace coatify.Domain;

public class Measurement
{
    public Guid Id { get; private set; }
    public Guid DeviceId { get; private set; }

    public DateTime Timestamp { get; private set; }

    public float Temperature { get; private set; }
    public float Pressure { get; private set; }
    public float PowerConsumption { get; private set; }
}