using coatify.Application.Interfaces;
using coatify.Infrastructure.DbContext;
using coatify.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
using Coatify.Shared;

namespace coatify.Application.Services;
using coatify.Domain;

public class DeviceService : IDeviceService

{
    readonly DeviceResponse _response = new DeviceResponse();
    private readonly IDeviceRepository _deviceRepository;
    private readonly ILogger<DeviceService> _logger;
    private readonly CoatifyContext _dbContext;

    public DeviceService(IDeviceRepository deviceRepository, ILogger<DeviceService> logger, CoatifyContext dbContext)
    {
        this._deviceRepository = deviceRepository;
        this._logger = logger;
        _dbContext = dbContext;
    }

    public async Task<DeviceResponse> CreateDevice(CreateDeviceRequest request)
    {
        await ValidateDeviceInput(request.Name, request.SerialNumber, request.Status, request.DeviceTypeId);

        var device = new Device
        {
            Id = Guid.NewGuid(),
            Name = request.Name,
            SerialNumber = request.SerialNumber,
            Status = request.Status,
            DeviceTypeId = request.DeviceTypeId,
            CreatedAt = DateTime.UtcNow
        };

        _dbContext.Devices.Add(device);
        await _dbContext.SaveChangesAsync();
        _logger.LogInformation("Created device {DeviceId}", device.Id);

        return new DeviceResponse { Id = device.Id, Name = device.Name, Status = device.Status };
    }

    public Task<DeviceResponse> GetDevice(Guid id)
    {
        DeviceResponse selectedDevice = this._response;
        selectedDevice.Id = id;

        var response = _dbContext.Devices.FirstOrDefault(d => d.Id == id);
        if (response is null)
        {
            throw new Exception($"Device with id: {response.Id}  was not found");
        }

        _logger.LogInformation($"Searching for device with the id: {selectedDevice.Id}");
        selectedDevice.Name = response.Name;
        selectedDevice.Status = response.Status;
        return Task.FromResult(selectedDevice);
    }

    public Task<List<DeviceResponse>> GetDevices(List<Guid> ids)
    {
        List<DeviceResponse> resultListDevices = new List<DeviceResponse>(ids.Count);
        foreach (Guid id in ids)
        {
            var response = _dbContext.Devices.FirstOrDefault(d => d.Id == id);
            if (response is null)
            {
                _logger.LogWarning($"Device with id: {id} was not found");
                continue;
            }

            DeviceResponse selectedDevice = new DeviceResponse
            {
                Id = response.Id,
                Name = response.Name,
                Status = response.Status
            };
            resultListDevices.Add(selectedDevice);

        }

        return Task.FromResult(resultListDevices);
    }



    public async Task<DeviceResponse> UpdateDevice(Guid id, UpdateDeviceRequest request)
    {
        var device = await _dbContext.Devices.FindAsync(id)
                     ?? throw new KeyNotFoundException($"Device {id} was not found.");

        await ValidateDeviceInput(request.Name, request.SerialNumber, request.Status, request.DeviceTypeId, id);

        device.Name = request.Name;
        device.SerialNumber = request.SerialNumber;
        device.Status = request.Status;
        device.DeviceTypeId = request.DeviceTypeId;


        await _dbContext.SaveChangesAsync();
        _logger.LogInformation("Updated device {DeviceId}", device.Id);

        return new DeviceResponse { Id = device.Id, Name = device.Name, Status = device.Status };
    }

    private async Task ValidateDeviceInput(
        string name,
        string serialNumber,
        string status,
        Guid deviceTypeId,
        Guid? deviceId = null)
    {
        if (string.IsNullOrWhiteSpace(name) ||
            string.IsNullOrWhiteSpace(serialNumber) ||
            string.IsNullOrWhiteSpace(status))
        {
            throw new ArgumentException("Name, SerialNumber and Status must not be empty.");
        }

        if (deviceTypeId == Guid.Empty ||
            !await _dbContext.DeviceTypes.AnyAsync(type => type.Id == deviceTypeId))
        {
            throw new ArgumentException($"Device type {deviceTypeId} does not exist.");
        }

        if (await _dbContext.Devices.AnyAsync(device =>
                device.SerialNumber == serialNumber && device.Id != deviceId))
        {
            throw new ArgumentException($"Serial number {serialNumber} is already in use.");
        }
    }

    public async Task<DeviceResponse> DeleteDevice(Guid Id)
    {
        DeviceResponse selectedDevice = this._response;
        if (Id == Guid.Empty)
        {
            _logger.LogCritical("Id cannot be empty");
            throw new ArgumentException("Id cannot be empty");
        }

        selectedDevice.Id = Id;
        int deletedRows = await _dbContext.Devices
            .Where(device => device.Id == Id)
            .ExecuteDeleteAsync();

        if (deletedRows == 0)
        {
            throw new KeyNotFoundException(
                $"Device {Id} wurde nicht gefunden.");
        }

        _logger.LogInformation($"Deleting device with id: {selectedDevice.Id}");
        return await Task.FromResult(selectedDevice);

    }

    public Task<List<DeviceResponse>> GetAllDevices()
    {
        var devices = _dbContext.Devices.Select(d => new DeviceResponse
        {
            Id = d.Id,
            Name = d.Name,
            Status = d.Status
        }).ToList();
        return Task.FromResult(devices);

    }

    public async Task<DeviceDetailResponse> GetDeviceDetails(Guid id)
    {
        var device = await _dbContext.Devices
            .Include(d => d.DeviceType)
            .FirstOrDefaultAsync(d => d.Id == id)
            ?? throw new KeyNotFoundException($"Device {id} was not found.");

        return new DeviceDetailResponse
        {
            Id = device.Id,
            Name = device.Name,
            SerialNumber = device.SerialNumber,
            Status = device.Status,
            DeviceTypeId = device.DeviceTypeId,
            DeviceTypeName = device.DeviceType.Name
        };
    }

    public async Task<List<MeasurementResponse>> GetMeasurements(Guid deviceId)
    {
        if (!await _dbContext.Devices.AnyAsync(d => d.Id == deviceId))
            throw new KeyNotFoundException($"Device {deviceId} was not found.");

        return await _dbContext.Measurements
            .Where(m => m.DeviceId == deviceId)
            .OrderByDescending(m => m.Timestamp)
            .Select(m => new MeasurementResponse
            {
                Id = m.Id,
                Timestamp = m.Timestamp,
                Temperature = m.Temperature,
                Pressure = m.Pressure,
                PowerConsumption = m.PowerConsumption
            })
            .ToListAsync();
    }

    public Task<List<DeviceTypeResponse>> GetDeviceTypes()
    {
        return _dbContext.DeviceTypes
            .OrderBy(type => type.Name)
            .Select(type => new DeviceTypeResponse { Id = type.Id, Name = type.Name })
            .ToListAsync();
    }
}