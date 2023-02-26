using System.Reflection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using WTA.Application.Abstractions;

namespace WTA.Application;

public class App
{
    public static IServiceProvider? Services { get; set; }
    public static IConfiguration? Configuration { get; set; }
    public static ILogger? Logger { get; set; }
    public static List<Assembly>? ModuleAssemblies { get; set; }
    public static List<IDbContext>? DbContextList { get; set; }
    public static List<IStartup>? StartupList { get; set; }
}
