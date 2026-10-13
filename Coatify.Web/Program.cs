using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Coatify.Web;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");
var baseUrl=builder.Configuration["Api:BaseUrl"] ?? throw new InvalidOperationException("BaseUrl is missing in appsettings.json");
builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(baseUrl) });


await builder.Build().RunAsync();
