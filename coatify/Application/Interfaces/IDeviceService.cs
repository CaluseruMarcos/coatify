using coatify.Domain;

namespace coatify.Application.Interfaces;
using Npgsql;

public interface IDeviceService
{
    Task<DeviceResponse> CreateDevice(CreateDeviceRequest request);
    Task<DeviceResponse> GetDevice(Guid id);
    Task<List<DeviceResponse>> GetDevices(List<Guid> ids);
    Task<DeviceResponse> UpdateDevice(Guid id, UpdateDeviceRequest request);
    Task<DeviceResponse> DeleteDevice(Guid Id);
}