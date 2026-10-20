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
        
        app.MapGet("/api/devices", async (IDeviceService deviceService) =>
        {
            var devices = await deviceService.GetAllDevices();
            return Results.Ok(devices);
        });

        app.MapGet("/api/devices/{id}", async (Guid id, IDeviceService deviceService) =>
        {
            return Results.Ok(await deviceService.GetDeviceDetails(id));
        });

        app.MapGet("/api/devices/{id}/measurements", async (Guid id, IDeviceService deviceService) =>
        {
            return Results.Ok(await deviceService.GetMeasurements(id));
        });

        app.MapGet("/api/device-types", async (IDeviceService deviceService) =>
        {
            return Results.Ok(await deviceService.GetDeviceTypes());
        });

        app.MapPost("/api/devices", async (
            [FromBody] CreateDeviceRequest request,
            IDeviceService deviceService) =>
        {
            var device = await deviceService.CreateDevice(request);
            return Results.Created($"/api/devices/{device.Id}", device);
        });

        app.MapPut("/api/devices/{id}", async (
            Guid id,
            [FromBody] UpdateDeviceRequest request,
            IDeviceService deviceService) =>
        {
            var device = await deviceService.UpdateDevice(id, request);
            return Results.Ok(device);
        });

        app.MapDelete("/api/devices/{id}", async (Guid id, IDeviceService deviceService) =>
        {
            await deviceService.DeleteDevice(id);
            return Results.NoContent();
        });

       
    }
}
