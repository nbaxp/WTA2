using System.Globalization;
using System.Reflection;
using Serilog;

Log.Logger = new LoggerConfiguration()
    .Enrich.FromLogContext()
    .Enrich.WithMachineName()
    .Enrich.WithProcessName()
    .Enrich.WithProcessId()
    .Enrich.WithProperty("ApplicationName", Assembly.GetEntryAssembly()?.GetName().Name)
    .Enrich.WithProperty("Version", Assembly.GetEntryAssembly()?.GetName().Version)
    .WriteTo.Console(formatProvider: CultureInfo.InvariantCulture)
    .WriteTo.Http("http://localhost:5001", null)
    .CreateLogger();

var builder = WebApplication.CreateBuilder(args);
builder.Host.UseSerilog();
builder.Services.AddHttpClient();
builder.Services.AddSignalR(o => o.EnableDetailedErrors = true);
builder.Services.AddReverseProxy().LoadFromConfig(builder.Configuration.GetSection("ReverseProxy"));
builder.Services.AddMvc();

var app = builder.Build();
app.UseSerilogRequestLogging();
app.MapReverseProxy();
app.UseStaticFiles();
app.UseRouting();
app.UseAuthorization();
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");
app.MapHub<PageHub>("/hub");
app.Run();
