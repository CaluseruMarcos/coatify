namespace coatify.Application.Interfaces;
using Npgsql;
public interface IDeviceRepository
{
    public Task<NpgsqlConnection> CreatePGDataPool();
    public Task<DeviceResponse> CreateDevice(Guid Id, string Name, string Status, NpgsqlConnection dataSource);
    public Task<DeviceResponse> GetDevice(Guid Id, NpgsqlConnection dataSource);
    public Task<DeviceResponse> UpdateDevice(Guid Id, string Name, string Status, NpgsqlConnection dataSource);
    public Task<DeviceResponse> DeleteDevice(Guid Id, NpgsqlConnection dataSource);
    public Task<List<DeviceResponse>> GetDevices(List<Guid> Ids, NpgsqlConnection dataSource);
    
    public void ClearConnectionPool(NpgsqlConnection dataSource);
    
    
    
}