using coatify.Domain;
using Coatify.Shared;

namespace coatify.Application.Interfaces;
using Npgsql;

public interface IDeviceService
{
    Task<DeviceResponse> CreateDevice(CreateDeviceRequest request);
    Task<DeviceResponse> GetDevice(Guid id);
    Task<List<DeviceResponse>> GetDevices(List<Guid> ids);
    Task<DeviceResponse> UpdateDevice(Guid id, UpdateDeviceRequest request);
    Task<DeviceResponse> DeleteDevice(Guid Id);
    Task<List<DeviceResponse>> GetAllDevices();
    Task<DeviceDetailResponse> GetDeviceDetails(Guid id);
    Task<List<MeasurementResponse>> GetMeasurements(Guid deviceId);
    Task<List<DeviceTypeResponse>> GetDeviceTypes();
}