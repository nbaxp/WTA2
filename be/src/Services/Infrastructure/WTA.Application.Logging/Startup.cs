using Microsoft.AspNetCore.Builder;
using Raven.Embedded;
using WTA.Application.Abstractions;
using WTA.Application.Application;

[assembly: Module($"{nameof(WTA.Application.Logging)}", -1)]

namespace WTA.Application.Logging;

public class Startup : IStartup
{
    public void Configure(WebApplication app)
    {
    }

    public void ConfigureServices(WebApplicationBuilder builder)
    {
        EmbeddedServer.Instance.StartServer();
    }
}
