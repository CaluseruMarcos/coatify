using coatify.Infrastructure.Repositories;

namespace coatify.Application;
using coatify.Domain;
public class DeviceService : IDeviceService



    
{
    readonly DeviceResponse _response= new DeviceResponse();
    private DeviceRepository _deviceRepository;
    
    DeviceService(DeviceRepository deviceRepository)
    {
        this._deviceRepository=deviceRepository;
    }
    public Task<DeviceResponse> CreateDevice(Guid Id, string Name, string Status)
    {
        
        Console.WriteLine("Creating a Device :)");
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
        Console.WriteLine("Searching for device :) with the id: "+ selectedDevice.Id);
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
            
        }
        return Task.FromResult(resultListDevices);
    }

  

    public Task<DeviceResponse> UpdateDevice(Guid Id, string Name, string Status)
    {
        DeviceResponse selectedDevice=this._response;
        selectedDevice.Id = Id;
        selectedDevice.Name = Name;
        selectedDevice.Status = Status;
        return Task.FromResult(selectedDevice);
        
    }

    public Task<DeviceResponse> DeleteDevice(Guid Id)
    {
       DeviceResponse selectedDevice = this._response;
       if (Id == Guid.Empty)
       {
           throw new ArgumentException("Id cannot be empty");
       }

       selectedDevice.Id = Id;
       return Task.FromResult(selectedDevice);
       
    }
}