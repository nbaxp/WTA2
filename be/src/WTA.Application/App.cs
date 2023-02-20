using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace WTA.Core;

public class App
{
    public static IServiceProvider? Services { get; set; }
    public static IConfiguration? Configuration { get; set; }
    public static ILogger? Logger { get; set; }
}
