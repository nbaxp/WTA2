using System.Globalization;
using System.Reflection;
using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Cors.Infrastructure;
using Serilog;

// log
var logFile = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "logs", "log.txt");
var loggerConfiguration = new LoggerConfiguration()
    .Enrich.FromLogContext()
    .Enrich.WithMachineName()
    .Enrich.WithProcessName()
    .Enrich.WithProcessId()
    .Enrich.WithProperty("ApplicationName", Assembly.GetEntryAssembly().GetName().Name)
    .Enrich.WithProperty("Version", Assembly.GetEntryAssembly()?.GetName().Version)
    .WriteTo.Console(formatProvider: CultureInfo.InvariantCulture)
    .WriteTo.File(logFile, rollOnFileSizeLimit: true, rollingInterval: RollingInterval.Infinite, fileSizeLimitBytes: 1024 * 1024 * 100, formatProvider: CultureInfo.InvariantCulture);
if (Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") == "Development")
{
    loggerConfiguration.WriteTo.Http("http://localhost:5001", null);
}
Log.Logger = loggerConfiguration.CreateLogger();
try
{
    // builder
    var builder = WebApplication.CreateBuilder(args);
    // config
    builder.Host.UseSerilog();
    builder.Services.AddHttpClient();
    builder.Services.AddSignalR(o => o.EnableDetailedErrors = true)
        .AddJsonProtocol(o =>
        {
            o.PayloadSerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
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
    builder.Services.AddCors(options =>
    {
        options.AddPolicy("default", builder =>
        {
            builder.SetIsOriginAllowed(_ => true).AllowAnyMethod().AllowAnyHeader().AllowCredentials();
        });
        builder.Configuration.GetSection("CORS").GetChildren().ToList().ForEach(o =>
        {
            options.AddPolicy(o.Key, builder =>
            {
                //
                _ = o.GetValue(nameof(CorsPolicy.AllowAnyOrigin), true)
                ? builder.AllowAnyOrigin()
                : builder.WithOrigins(o.GetSection(nameof(CorsPolicy.Origins)).Get<string[]>());
                //
                _ = o.GetValue(nameof(CorsPolicy.AllowAnyHeader), true)
                ? builder.AllowAnyHeader()
                : builder.WithHeaders(o.GetSection(nameof(CorsPolicy.Headers)).Get<string[]>());
                //
                _ = o.GetValue(nameof(CorsPolicy.AllowAnyMethod), true) ?
                builder.AllowAnyMethod()
                : builder.WithMethods(o.GetSection(nameof(CorsPolicy.Methods)).Get<string[]>());
            });
        });
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
    app.UseCors("default");
    // run
    app.Run();
}
catch (Exception ex)
{
    Log.Fatal(ex, "Application terminated unexpectedly");
}
finally
{
    Log.CloseAndFlush();
}
