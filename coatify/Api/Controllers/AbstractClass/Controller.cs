using coatify.Infrastructure.Repositories;

namespace coatify.Api.Controllers.AbstractClass;
using coatify.Application;
public abstract class Controller
{
  
   protected DeviceService deviceService;
    public WebApplication App { get; set; }
}