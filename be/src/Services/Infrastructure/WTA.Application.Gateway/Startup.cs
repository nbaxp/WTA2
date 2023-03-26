using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using WTA.Application.Abstractions;
using WTA.Application.Application;

[assembly: Module($"{nameof(WTA.Application.Gateway)}", -1)]

namespace WTA.Application.Gateway;

public class Startup : IStartup
{
    public void Configure(WebApplication app)
    {
        app.MapReverseProxy();
    }

    public void ConfigureServices(WebApplicationBuilder builder)
    {
        builder.Services.AddReverseProxy().LoadFromConfig(builder.Configuration.GetSection("ReverseProxy"));
    }
}
