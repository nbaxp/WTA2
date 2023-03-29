using System.Globalization;
using System.Reflection;
using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Authentication.Cookies;
using Serilog;
// log
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
// builder
var builder = WebApplication.CreateBuilder(args);
// config 
builder.Host.UseSerilog();
builder.Services.AddHttpClient();
builder.Services.AddSignalR(o => o.EnableDetailedErrors = true)
    .AddJsonProtocol(o=> {
        o.PayloadSerializerOptions.PropertyNamingPolicy= JsonNamingPolicy.CamelCase;
        o.PayloadSerializerOptions.DictionaryKeyPolicy = JsonNamingPolicy.CamelCase;
        o.PayloadSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
    });
builder.Services.AddReverseProxy().LoadFromConfig(builder.Configuration.GetSection("ReverseProxy"));
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme).AddCookie();
builder.Services.AddMvc().AddJsonOptions(options =>
{
    options.JsonSerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
    options.JsonSerializerOptions.DictionaryKeyPolicy = JsonNamingPolicy.CamelCase;
    options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
});
// build
var app = builder.Build();
// config
app.UseSerilogRequestLogging();
app.MapReverseProxy();
app.UseStaticFiles();
app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");
app.MapHub<PageHub>("/hub");
// run
app.Run();
