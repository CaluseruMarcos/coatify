using coatify.Application;

namespace coatify.Infrastructure.Repositories;
using coatify.Application.Interfaces;
using Npgsql;
public class DeviceRepository : IDeviceRepository

{
    public string Host { get; set; }
    public string Username { get; set; }
    public string Password { get; set; }
    public string Database { get; set; }
    
   
    
    public async Task<NpgsqlConnection>  CreatePGDataPool()
    {
        string connectionString = $"Host={Host};Username={Username};Password={Password};Database={Database}";
        await using var dataSource = NpgsqlDataSource.Create(connectionString);
        await using var connection = await dataSource.OpenConnectionAsync();

        
        
        return connection;
    }
    

    public Task<DeviceResponse> CreateDevice(Guid Id, string Name, string Status, NpgsqlConnection dataSource)
    {
        new NpgsqlCommand("INSERT INTO devices (id, name, status) VALUES (@id, @name, @status)", dataSource)
        {
            Parameters =
            {
                new NpgsqlParameter("@id", Id),
                new NpgsqlParameter("@name", Name),
                new NpgsqlParameter("@status", Status)
            }
        }.ExecuteNonQuery();
        DeviceResponse response = new DeviceResponse
        {
            Id = Id,
            Name = Name,
            Status = Status
        };
        return Task.FromResult(response);
    }

    public Task<DeviceResponse> GetDevice(Guid Id,  NpgsqlConnection dataSource)
    {
        throw new NotImplementedException();
    }

    public Task<DeviceResponse> UpdateDevice(Guid Id, string Name, string Status,  NpgsqlConnection dataSource)
    {
        throw new NotImplementedException();
    }

    public Task<DeviceResponse> DeleteDevice(Guid Id, NpgsqlConnection dataSource)
    {
        throw new NotImplementedException();
    }

    public Task<List<DeviceResponse>> GetDevices(List<Guid> Ids, NpgsqlConnection dataSource)
    {
        throw new NotImplementedException();
    }
    
    public void ClearConnectionPool(NpgsqlConnection dataSource)
    {
        dataSource.Close();
    }

 
    
    
}