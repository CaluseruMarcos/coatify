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
        app.MapGet("/api/devices", async (List<Guid> ids) =>
        {
          
          
            var devices = await deviceService.GetDevices(ids);

            return Results.Ok(devices);
        });

        app.MapGet("/api/devices/{id}", async ( Guid id) =>
        {
            
           var devicesById= await deviceService.GetDevice(id);
            return Results.Ok(devicesById);
        });

        app.MapPost("/api/devices", async (Guid id, string name, string status) =>
        {
            var devices = await deviceService.CreateDevice(id, name, status);
            return Results.Ok(devices);
        });
        
        app.MapPut("/api/devices/{id}", async (Guid id, string name, string status) =>
        {
            var devices = await deviceService.UpdateDevice(id, name, status);
            return Results.Ok(devices);
        });

        app.MapDelete("/api/devices/{id}", async (Guid id) =>
        {
            var devices = await deviceService.DeleteDevice(id);
            return Results.Ok(devices);
        });

       
    }
}