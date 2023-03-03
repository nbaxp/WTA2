using Microsoft.AspNetCore.Builder;
using WTA.Application.Abstractions;
using WTA.Application.Application;

[assembly: Module($"{nameof(WTA.Application.Identity)}", -2)]

namespace WTA.Application.Identity;

public class Startup : IStartup
{
    public void Configure(WebApplication app)
    {
    }

    public void ConfigureServices(WebApplicationBuilder builder)
    {
    }
}
