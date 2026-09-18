using System.Diagnostics.CodeAnalysis;
using coatify.Domain;
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
        app.MapGet("/api/devices", async (
            [FromQuery] Guid[] ids,
            IDeviceService deviceService) =>
        {
            if (ids.Length == 0)
            {
                return Results.BadRequest("Mindestens eine ID angeben.");
            }

            var devices = await deviceService.GetDevices(ids.ToList());

            return Results.Ok(devices);
        });

        app.MapGet("/api/device/{id}", async (Guid id, IDeviceService deviceService) =>
        {
            
           var devicesById= await deviceService.GetDevice(id);
           
            return Results.Ok(devicesById);
        });

        app.MapPost("/api/device", async (
            [FromBody] CreateDeviceRequest request,
            IDeviceService deviceService) =>
        {
            var device = await deviceService.CreateDevice(request);
            return Results.Created($"/api/device/{device.Id}", device);
        });

        app.MapPut("/api/devices/{id}", async (
            Guid id,
            [FromBody] UpdateDeviceRequest request,
            IDeviceService deviceService) =>
        {
            var device = await deviceService.UpdateDevice(id, request);
            return Results.Ok(device);
        });

        app.MapDelete("/api/device/{id}", async (Guid id, IDeviceService deviceService) =>
        {
            var devices = await deviceService.DeleteDevice(id);
            return Results.Ok(devices);
        });

       
    }
}
