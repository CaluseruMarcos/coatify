using System.Diagnostics.Contracts;
using coatify.Infrastructure.Repositories;

namespace coatify.Api.Controllers.AbstractClass;
using coatify.Application;
using coatify.Application.Services;
public abstract class Controller
{
  
    public WebApplication App { get; set; }
}