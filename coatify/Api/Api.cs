using coatify.Api.Controllers;

namespace coatify.Api;
using coatify.Api.ErrorHandling;
using coatify.Application.Interfaces;
using coatify.Application.Services;
using coatify.Infrastructure.Repositories;
using coatify.Application.Interfaces;


 class Api{
    public static void run(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);
        
        builder.Services.AddOpenApi();
        builder.Services.AddScoped<IDeviceService, DeviceService>();
        builder.Services.AddScoped<IDeviceRepository, DeviceRepository>();
        
        builder.Services.AddLogging();
        var app = builder.Build();
        app.UseExceptionHandler(errorApp =>
        {
            errorApp.Run(async context =>
            {
                var exception = context.Features
                    .Get<Microsoft.AspNetCore.Diagnostics.IExceptionHandlerFeature>()
                    ?.Error;

                if (exception is null)
                    return;

                var error = GlobalExceptionHandler.HandleException(exception);

                context.Response.StatusCode = (int)error.StatusCode;

                await context.Response.WriteAsJsonAsync(new
                {
                    status = (int)error.StatusCode,
                    message = error.Message
                });
            });
        });
        var logger = app.Services.GetRequiredService<ILogger<DeviceService>>();
        var deviceService = new DeviceService(
            new DeviceRepository(),
            logger
        );
        DevicesControllers devicesControllers = new DevicesControllers(app, deviceService);
        devicesControllers.MapRoutes(app);
        
        
        app.Run();
}


}
