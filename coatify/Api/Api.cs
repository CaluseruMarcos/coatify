using coatify.Api.Controllers;

namespace coatify.Api;

using coatify.Application;
using coatify.Infrastructure.Repositories;

 class Api{
    public static void run(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);
        
        builder.Services.AddOpenApi();
        builder.Services.AddScoped<IDeviceService, DeviceService>();
        builder.Services.AddScoped<IDeviceRepository, DeviceRepository>();
        
        builder.Services.AddLogging();
        var app = builder.Build();
        var logger = app.Services.GetRequiredService<ILogger<DeviceService>>();
        var deviceService = new DeviceService(
            new DeviceRepository(),
            logger
        );
        DevicesControllers devicesControllers = new DevicesControllers(app,deviceService);
        devicesControllers.MapRoutes(app);
        
        
        app.Run();
}


}
