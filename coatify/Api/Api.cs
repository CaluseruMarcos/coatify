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
        
        
        var app = builder.Build();
        DevicesControllers devicesControllers = new DevicesControllers(app, new DeviceService(new DeviceRepository()));
        devicesControllers.MapRoutes(app);
        
        
        app.Run();
}


}
