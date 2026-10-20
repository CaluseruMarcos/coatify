using coatify.Infrastructure.DbContext;
using Microsoft.EntityFrameworkCore;
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
        builder.Services.AddHealthChecks();
        // Cors settings for the web app
        builder.Services.AddCors(options =>
        {
            options.AddPolicy("CoatifyWeb", policy =>
                policy.WithOrigins(
                        "http://localhost:5001",
                        "http://localhost:5217",
                        "https://localhost:7152")
                    .AllowAnyHeader()
                    .AllowAnyMethod());
        });
        builder.Services.AddDbContext<CoatifyContext>(options =>
            options.UseNpgsql(builder.Configuration.GetConnectionString("Coatify")
                ?? throw new InvalidOperationException("Connection string Coatify is missing.")));
        builder.Services.AddScoped<IDeviceService, DeviceService>();
        builder.Services.AddScoped<IDeviceRepository, DeviceRepository>();
        
        builder.Services.AddLogging();
        var app = builder.Build();
        app.UseCors("CoatifyWeb");
        app.MapHealthChecks("/health");
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
        DevicesControllers devicesControllers = new DevicesControllers(app);
        devicesControllers.MapRoutes(app);
        
        
        app.Run();
}


}
