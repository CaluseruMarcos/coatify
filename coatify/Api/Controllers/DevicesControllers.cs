using System.Diagnostics.CodeAnalysis;
using Microsoft.AspNetCore.Mvc;
using Controller = coatify.Api.Controllers.AbstractClass.Controller;

namespace coatify.Api.Controllers;
using coatify.Application;
using coatify.Infrastructure.Repositories;

public class DevicesControllers : Controller
{
   
    
    public DevicesControllers( WebApplication app, DeviceService deviceService) : base()
    {
        
        App = app;
        this.deviceService = deviceService;
    }

    public void MapRoutes(WebApplication app)
    {
        app.MapGet("/api/devices", async () =>
        {
          
            List<Guid> randomGuid = new List<Guid>();
            randomGuid.Add(Guid.NewGuid());
            randomGuid.Add(Guid.NewGuid());
            randomGuid.Add(Guid.NewGuid());

            
            var devices = await deviceService.GetDevices(randomGuid);

            return Results.Ok(devices);
        });

        app.MapGet("/api/device/{id:guid}", async ( Guid id) =>
        {
            
           var devicesById= await deviceService.GetDevice(id);
            return Results.Ok(devicesById);
        });

        app.MapPost("/api/device", async (Guid id, string name, string status) =>
        {
            var devices = await deviceService.CreateDevice(id, name, status);
            return Results.Ok(devices);
        });
        
        app.MapPut("/api/device/{id:guid}", async (Guid id, string name, string status) =>
        {
            var devices = await deviceService.UpdateDevice(id, name, status);
            return Results.Ok(devices);
        });

        app.MapDelete("/api/device/{id:guid}", async (Guid id) =>
        {
            var devices = await deviceService.DeleteDevice(id);
            return Results.Ok(devices);
        });

       
    }
}
