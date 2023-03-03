using Microsoft.AspNetCore.Builder;
using WTA.Application.Abstractions;
using WTA.Application.Application;

[assembly: Module($"{nameof(WTA.Application.BaseData)}", -1)]

namespace WTA.Application.BaseData;

public class Startup : IStartup
{
    public void Configure(WebApplication app)
    {
    }

    public void ConfigureServices(WebApplicationBuilder builder)
    {
    }
}
