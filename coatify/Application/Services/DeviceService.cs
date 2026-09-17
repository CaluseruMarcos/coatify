using coatify.Infrastructure.Repositories;

namespace coatify.Application;
using coatify.Domain;
public class DeviceService : IDeviceService

{
    readonly DeviceResponse _response= new DeviceResponse();
    private readonly IDeviceRepository _deviceRepository;
    private readonly ILogger<DeviceService> _logger;
    
    public DeviceService(IDeviceRepository deviceRepository, ILogger<DeviceService> logger)
    {
        this._deviceRepository=deviceRepository;
        this._logger=logger;
    }
    public Task<DeviceResponse> CreateDevice(Guid Id, string Name, string Status)
    {
        
        _logger.LogInformation($"Creating device with id: {Id}, name: {Name}, status: {Status}");
        DeviceResponse response = this._response;
        response.Id = Id;
        response.Name = Name;
        response.Status = Status;
        return Task.FromResult(response);
      
    }

    public Task<DeviceResponse> GetDevice(Guid id)
    {
        DeviceResponse selectedDevice=this._response;
        selectedDevice.Id = id;
        _logger.LogInformation($"Searching for device with the id: {selectedDevice.Id}");
        return Task.FromResult(selectedDevice);
    }
    public Task<List<DeviceResponse>> GetDevices( List<Guid> ids)
    {
        List<DeviceResponse> resultListDevices= new List<DeviceResponse>(ids.Count);
         foreach (Guid id in ids)
        {
            DeviceResponse selectedDevice=this._response;
            selectedDevice.Id = id;
            resultListDevices.Add(selectedDevice);
            _logger.LogInformation($"Adding device to the list: {selectedDevice.Id}");
            
        }
        return Task.FromResult(resultListDevices);
    }

  

    public Task<DeviceResponse> UpdateDevice(Guid Id, string Name, string Status)
    {
        DeviceResponse selectedDevice=this._response;
        selectedDevice.Id = Id;
        selectedDevice.Name = Name;
        selectedDevice.Status = Status;
        _logger.LogInformation($"Updating device with id: {selectedDevice.Id}, name: {selectedDevice.Name}, status: {selectedDevice.Status}");
        return Task.FromResult(selectedDevice);
        
    }

    public Task<DeviceResponse> DeleteDevice(Guid Id)
    {
       DeviceResponse selectedDevice = this._response;
       if (Id == Guid.Empty)
       {
           _logger.LogCritical("Id cannot be empty");
           throw new ArgumentException("Id cannot be empty");
       }

       selectedDevice.Id = Id;
         _logger.LogInformation($"Deleting device with id: {selectedDevice.Id}");
       return Task.FromResult(selectedDevice);
       
    }
    
}