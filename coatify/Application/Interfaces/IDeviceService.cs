namespace coatify.Application.Interfaces;
using Npgsql;

public interface IDeviceService
{
    Task<DeviceResponse> CreateDevice(Guid Id, string Name, string Status);
    Task<DeviceResponse> GetDevice(Guid id);
    Task<List<DeviceResponse>> GetDevices(List<Guid> ids);
    Task<DeviceResponse> UpdateDevice(Guid Id, string Name, string Status);
    Task<DeviceResponse> DeleteDevice(Guid Id);
}