using System.Diagnostics.CodeAnalysis;
using Microsoft.AspNetCore.Mvc;
using Controller = coatify.Api.Controllers.AbstractClass.Controller;

namespace coatify.Api.Controllers;
using coatify.Application;
using coatify.Application.Interfaces;
using coatify.Infrastructure.Repositories;
using coatify.Application.Services;

public class DevicesControllers : Controller
{
   
    
    public DevicesControllers( WebApplication app) : base()
    {
        
        App = app;

    }

    public void MapRoutes(WebApplication app)
    {
        app.MapGet("/api/devices", async (IDeviceService deviceService) =>
        {
          
            List<Guid> randomGuid = new List<Guid>();
            randomGuid.Add(Guid.NewGuid());
            randomGuid.Add(Guid.NewGuid());
            randomGuid.Add(Guid.NewGuid());

            
            var devices = await deviceService.GetDevices(randomGuid);

            return Results.Ok(devices);
        });

        app.MapGet("/api/device/{id}", async (Guid id, IDeviceService deviceService) =>
        {
            
           var devicesById= await deviceService.GetDevice(id);
           
            return Results.Ok(devicesById);
        });

        app.MapPost("/api/device", async (Guid id, string name, string status, IDeviceService deviceService) =>
        {
            var devices = await deviceService.CreateDevice(id, name, status);
            return Results.Ok(devices);
        });
        
        app.MapPut("/api/device/{id}", async (Guid id, string name, string status, IDeviceService deviceService) =>
        {
            var devices = await deviceService.UpdateDevice(id, name, status);
            return Results.Ok(devices);
        });

        app.MapDelete("/api/device/{id}", async (Guid id, IDeviceService deviceService) =>
        {
            var devices = await deviceService.DeleteDevice(id);
            return Results.Ok(devices);
        });

       
    }
}
