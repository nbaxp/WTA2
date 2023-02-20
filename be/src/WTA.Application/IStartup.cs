using Microsoft.AspNetCore.Builder;

namespace WTA.Application;

public interface IStartup
{
    void ConfigureServices(WebApplicationBuilder builder);

    void Configure(WebApplication app);
}
