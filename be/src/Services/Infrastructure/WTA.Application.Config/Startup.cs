using Microsoft.AspNetCore.Builder;
using WTA.Application.Abstractions;
using WTA.Application.Application;

[assembly: Module($"{nameof(WTA.Application.Config)}", -1)]

namespace WTA.Application.Config;

public class Startup : IStartup
{
    public void Configure(WebApplication app)
    {
    }

    public void ConfigureServices(WebApplicationBuilder builder)
    {
    }
}
