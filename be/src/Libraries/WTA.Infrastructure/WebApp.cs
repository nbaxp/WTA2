using System.Globalization;
using System.IdentityModel.Tokens.Jwt;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Text.Unicode;
using Mapster;
using MapsterMapper;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.AspNetCore.Mvc.ApplicationModels;
using Microsoft.AspNetCore.Mvc.Razor;
using Microsoft.AspNetCore.Routing;
using Microsoft.AspNetCore.SignalR;
using Microsoft.AspNetCore.StaticFiles;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Serilog;
using Serilog.Debugging;
using Serilog.Settings.Configuration;
using Swashbuckle.AspNetCore.SwaggerGen;
using WTA.Application;
using WTA.Application.Abstractions;
using WTA.Application.Abstractions.Controllers;
using WTA.Application.Abstractions.Data;
using WTA.Application.Abstractions.SignalR;
using WTA.Application.Application;
using WTA.Application.Domain;
using WTA.Application.Extensions;
using WTA.Application.Resources;
using WTA.Infrastructure.Authentication;
using WTA.Infrastructure.Data;
using WTA.Infrastructure.DataAnnotations;
using WTA.Infrastructure.ExportImport;
using WTA.Infrastructure.Localization;
using WTA.Infrastructure.Options;
using WTA.Infrastructure.Routing;
using WTA.Infrastructure.Swagger;

namespace WTA.Infrastructure;

public class WebApp
{
    public WebApp()
    {
        this.OSPlatformName = OperatingSystem.IsWindows() ? nameof(OSPlatform.Windows) : (OperatingSystem.IsLinux() ? nameof(OSPlatform.Linux) : nameof(OSPlatform.OSX));
        this.Name = Assembly.GetEntryAssembly()?.GetName().Name!;

        // AppDomain.CurrentDomain.GetAssemblies() 未被调用过的程序集不会被加载
        var path = Path.GetDirectoryName(AppContext.BaseDirectory)!;
        var dlls = Directory.GetFiles(path, $"{nameof(WTA)}.*.dll");
        foreach (var item in dlls)
        {
            // Assembly.LoadFile 会导致 EFCore 报错：Cannot create DbSet for entity type
            App.Assemblies.Add(Assembly.LoadFrom(item));
        }
    }

    public string Name { get; }
    public string OSPlatformName { get; }

    public virtual void Configure(WebApplication app)
    {
        App.Configuration = app.Configuration;
        App.Logger = app.Logger;
        App.Services = app.Services;
        UseStaticFiles(app);
        UseRouting(app);
        UseLocalization(app);
        UseSwagger(app);
        UseAuthorization(app);
        UseDbContext(app);
        UseSignalR<PageHub>(app);
    }

    public virtual void ConfigureServices(WebApplicationBuilder builder)
    {
        AddHttp(builder);
        AddLocalization(builder);
        AddMvc(builder);
        AddSwagger(builder);
        AddAuthentication(builder);
        AddCache(builder);
        AddDbContext(builder);
        AddDefaultServices(builder);
        AddDefaultOptions(builder);
        builder.Services.ConfigureOptions(new EmbeddedConfigureOptions());
        builder.Services.AddTransient(typeof(IExportImportService<>), typeof(DefaultExportImportService<>));
        builder.Services.AddSingleton(new TypeAdapterConfig());
        builder.Services.AddScoped<IMapper, ServiceMapper>();
        AddSignalR(builder);
        //builder.Services.AddEventBus();
        //builder.Services.AddTransient(typeof(IApplicationService<>), typeof(ApplicationService<>));
    }

    /// <summary>
    /// 启动应用
    /// </summary>
    /// <param name="args">命令行参数</param>
    /// <param name="builder">WebApplicationBuilder 配置服务</param>
    /// <param name="app">WebApplication 配置应用</param>
    /// <param name="configureAssembly">程序集扫描约束</param>
    public void Start(string[] args,
        Action<WebApplicationBuilder>? configureBuilder = null,
        Action<WebApplication>? configureApp = null,
        Func<Assembly, bool>? configureAssembly = null)
    {
        var aspNetCoreEnvironment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");
        if (aspNetCoreEnvironment == "Development")
        {
            SelfLog.Enable(Console.WriteLine);
        }
        Log.Logger = new LoggerConfiguration()
            .Enrich.FromLogContext()
            .WriteTo.Console(formatProvider: CultureInfo.InvariantCulture)
            .CreateBootstrapLogger();
        try
        {
            Log.Information($"{this.Name} start");
            var startupList = App.Assemblies
                .Where(o => o.GetCustomAttributes<ModuleAttribute>().Any())
                .SelectMany(o => o.GetTypes())
                .Where(o => typeof(IStartup).IsAssignableFrom(o))
                .Select(o => (Activator.CreateInstance(o) as IStartup)!)
                .ToList();
            var builder = WebApplication.CreateBuilder(args);
            builder.Host.UseSerilog((hostingContext, services, configBuilder) =>
            {
                configBuilder
                .ReadFrom.Configuration(hostingContext.Configuration, ConfigurationAssemblySource.AlwaysScanDllFiles)
                .Enrich.FromLogContext()
                .WriteTo.Console(formatProvider: CultureInfo.InvariantCulture);
            }, writeToProviders: true);
            this.ConfigureServices(builder);
            configureBuilder?.Invoke(builder);
            startupList?.ForEach(o => o.ConfigureServices(builder));
            var app = builder.Build();
            app.UseSerilogRequestLogging();
            this.Configure(app);
            configureApp?.Invoke(app);
            startupList?.ForEach(o => o.Configure(app));
            app.Run();
        }
        catch (Exception ex)
        {
            Log.Fatal(ex, ex.Message);
        }
        finally
        {
            Log.Information($"{this.Name} stop");
            Log.CloseAndFlush();
        }
    }

    protected virtual void AddSignalR(WebApplicationBuilder builder)
    {
        var signalRServerBuilder = builder.Services.AddSignalR(o => o.EnableDetailedErrors = true);
        var cacheConnectionName = builder.Configuration.GetConnectionString("DefaultCache");
        if (cacheConnectionName!.ToLowerInvariant() == "redis")
        {
            var redisConnectionString = builder.Configuration.GetConnectionString("Redis")!;
            signalRServerBuilder.AddStackExchangeRedis(redisConnectionString, o => o.Configuration.ChannelPrefix = nameof(WebApp));
        }
    }

    protected virtual void UseSignalR<T>(WebApplication app, string pattern = "/hub") where T : Hub
    {
        app.MapHub<T>(pattern);
    }

    #region add services

    /// <summary>
    /// 根据 OptionsAttribute 自动配置 Options
    /// </summary>
    public void AddDefaultOptions(WebApplicationBuilder builder)
    {
        var configureMethod = typeof(OptionsConfigurationServiceCollectionExtensions)
            .GetMethod(nameof(OptionsConfigurationServiceCollectionExtensions.Configure),
            new[] { typeof(IServiceCollection), typeof(IConfiguration) });

        AppDomain.CurrentDomain.GetAssemblies()
            .Where(o => o.FullName!.StartsWith(nameof(WTA)))
            .Where(o => o.GetTypes()
            .Any(o => o.GetCustomAttributes(typeof(OptionsAttribute)).Any()))
            .SelectMany(o => o.GetTypes())
            .Where(type => type.GetCustomAttributes<OptionsAttribute>().Any())
            .ForEach(type =>
            {
                var attribute = type.GetCustomAttribute<OptionsAttribute>()!;
                var configurationSection = builder.Configuration.GetSection(attribute.Section ?? type.Name.TrimEndOptions());
                configureMethod?.MakeGenericMethod(type).Invoke(null, new object[] { builder.Services, configurationSection });
            });
    }

    /// <summary>
    /// 根据 ImplementationAttribute 自动配置依赖注入
    /// </summary>
    public virtual void AddDefaultServices(WebApplicationBuilder builder)
    {
        AppDomain.CurrentDomain.GetAssemblies()
            .Where(o => o.FullName!.StartsWith(nameof(WTA)))
            .Where(o => o.GetTypes()
            .Any(o => o.GetCustomAttributes(typeof(ServiceAttribute<>)).Any()))
            .SelectMany(o => o.GetTypes())
            .Where(type => type.GetCustomAttributes(typeof(ServiceAttribute<>)).Any())
            .ForEach(type =>
            {
                foreach (var implementation in type.GetCustomAttributes(typeof(ServiceAttribute<>)).Select(o => (o as BaseServiceAttribute)!))
                {
                    var currentPlatformType = (PlatformType)Enum.Parse(typeof(PlatformType), this.OSPlatformName);
                    if (implementation.PlatformType.HasFlag(currentPlatformType))
                    {
                        if (implementation.ServiceType.IsAssignableTo(typeof(IHostedService)))
                        {
                            var method = typeof(ServiceCollectionHostedServiceExtensions)
                            .GetMethod(nameof(ServiceCollectionHostedServiceExtensions.AddHostedService),
                            new[] { typeof(IServiceCollection) });
                            method?.MakeGenericMethod(type).Invoke(null, new object[] { builder.Services });
                        }
                        else
                        {
                            var descriptor = new ServiceDescriptor(implementation.ServiceType, type, implementation.Lifetime);
                            builder.Services.Add(descriptor);
                        }
                    }
                }
            });
    }

    protected virtual void AddAuthentication(WebApplicationBuilder builder)
    {
        builder.Services.AddSingleton<CustomJwtSecurityTokenHandler>();
        builder.Services.AddSingleton<JwtSecurityTokenHandler, CustomJwtSecurityTokenHandler>();
        builder.Services.AddSingleton<IPostConfigureOptions<JwtBearerOptions>, CustomJwtBearerPostConfigureOptions>();
        builder.Services.Configure<JwtOptions>(builder.Configuration.GetSection(JwtOptions.Position));
        var jwtOptions = new JwtOptions();
        builder.Configuration.GetSection(JwtOptions.Position).Bind(jwtOptions);
        var issuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtOptions.Key));
        builder.Services.AddSingleton(new SigningCredentials(issuerSigningKey, SecurityAlgorithms.HmacSha256Signature));
        var tokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,
            ValidIssuer = jwtOptions.Issuer,
            ValidAudience = jwtOptions.Audience,
            IssuerSigningKey = issuerSigningKey,
            NameClaimType = "Name",
            RoleClaimType = "Role",
            ClockSkew = TimeSpan.Zero,//default 300
        };
        builder.Services.AddSingleton(tokenValidationParameters);
        builder.Services.AddAuthentication(options =>
        {
            options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
            options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
        }).AddJwtBearer(o =>
        {
            o.TokenValidationParameters = tokenValidationParameters;
            o.Events = new JwtBearerEvents
            {
                OnMessageReceived = context =>
                {
                    if (!context.Request.IsJsonRequest())
                    {
                        if (context.Request.Cookies.TryGetValue("access_token", out var token))
                        {
                            context.Token = token;
                        }
                        else if (context.Request.Query.ToString()!.Contains("access_token"))
                        {
                            context.Token = context.Request.Query["access_token"];
                        }
                    }
                    return Task.CompletedTask;
                },
                OnChallenge = context =>
                {
                    if (!context.Request.IsJsonRequest())
                    {
                        var linkGenerator = context.HttpContext.RequestServices.GetRequiredService<LinkGenerator>();
                        var url = linkGenerator.GetPathByAction("Login", "Account", new { returnUrl = context.Request.GetDisplayUrl() }, pathBase: context.HttpContext.Request.PathBase);
                        context.Response.Redirect(url!);
                        context.HandleResponse();
                    }
                    return Task.CompletedTask;
                },
            };
        });
        builder.Services.AddAuthorization();
    }

    protected virtual void AddCache(WebApplicationBuilder builder)
    {
        builder.Services.AddMemoryCache();
        var cacheConnectionName = builder.Configuration.GetConnectionString("DefaultCache");
        if (cacheConnectionName!.ToLowerInvariant() == "redis")
        {
            builder.Services.AddStackExchangeRedisCache(options =>
            {
                options.Configuration = builder.Configuration.GetConnectionString(cacheConnectionName);
                // options.InstanceName = "DefaultInstance";
            });
        }
        else
        {
            builder.Services.AddDistributedMemoryCache();
        }
    }

    protected virtual void AddDbContext(WebApplicationBuilder builder)
    {
        var dbConnectionName = builder.Configuration.GetConnectionString("DefaultDatabase");
        var connectionString = builder.Configuration.GetConnectionString(dbConnectionName!);
        App.Assemblies!.ForEach(assembly =>
        {
            assembly.GetTypes().Where(type => !type.IsAbstract && type.IsAssignableTo(typeof(DbContext))).ForEach(contextType =>
            {
                var optionsType = typeof(DbContextOptions<>).MakeGenericType(contextType);
                builder.Services.AddScoped(optionsType, o =>
                {
                    var optionsBuilderType = typeof(DbContextOptionsBuilder<>).MakeGenericType(contextType);
                    var optionsBuilder = Activator.CreateInstance(optionsBuilderType) as DbContextOptionsBuilder;
                    var values = builder.Configuration.GetConnectionString($"{contextType.Name}")?.Split(':');
                    if (values != null)
                    {
                        var database = values[0];
                        var connectionString = values[1];
                        if (database == "mysql")
                        {
                            optionsBuilder?.UseMySql(values[1], ServerVersion.AutoDetect(connectionString));
                        }
                        else if (database == "sqlite")
                        {
                            optionsBuilder?.UseSqlite(connectionString);
                        }
                    }
                    return optionsBuilder?.Options!;
                });
                builder.Services.AddScoped(contextType);
                builder.Services.AddScoped(typeof(DbContext), contextType);
            });
        });
        //builder.Services.AddScoped<DbContext, BaseDbContext>();
        builder.Services.AddScoped(typeof(IRepository<>), typeof(EfRepository<>));
    }

    protected virtual void AddHttp(WebApplicationBuilder builder)
    {
        builder.Services.AddHttpClient();
        builder.Services.AddHttpContextAccessor();
        builder.Services.AddCors(options =>
        {
            options.AddPolicy(this.Name!, builder =>
            {
                builder.SetIsOriginAllowed(isOriginAllowed => true)
                .AllowAnyMethod()
                .AllowAnyHeader()
                .AllowCredentials();
            });
        });
        builder.Services.AddWebEncoders(options => options.TextEncoderSettings = new TextEncoderSettings(UnicodeRanges.All));
        builder.Services.Configure<Microsoft.AspNetCore.Http.Json.JsonOptions>(options =>
        {
            options.SerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
            options.SerializerOptions.Encoder = JavaScriptEncoder.Create(UnicodeRanges.All);
            options.SerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
            options.SerializerOptions.DictionaryKeyPolicy = JsonNamingPolicy.CamelCase;
        });
        builder.Services.Configure<FormOptions>(options =>
        {
            options.ValueCountLimit = int.MaxValue;
            options.MultipartBodyLengthLimit = long.MaxValue;
        });
    }

    protected virtual void AddLocalization(WebApplicationBuilder builder)
    {
        builder.Services.AddLocalization();
        builder.Services.AddSingleton<IStringLocalizerFactory, JsonStringLocalizerFactory>();
        builder.Services.AddSingleton<IStringLocalizer>(o => o.GetRequiredService<IStringLocalizer<Resource>>());
        builder.Services.Configure<RequestLocalizationOptions>(options =>
        {
            var supportedCultures = new List<CultureInfo>
            {
                new CultureInfo("zh"),
                new CultureInfo("en")
            };

            options.DefaultRequestCulture = new RequestCulture(supportedCultures.First());
            options.SupportedCultures = supportedCultures;
            options.SupportedUICultures = supportedCultures;
            //options.RequestCultureProviders.Insert(0, new RouteDataRequestCultureProvider());
        });
    }

    protected virtual void AddMvc(WebApplicationBuilder builder)
    {
        builder.Services.Configure<RazorViewEngineOptions>(options =>
        {
            options.ViewLocationFormats.Add("/Views/{1}/{0}" + RazorViewEngine.ViewExtension);
            options.ViewLocationFormats.Add("/Views/Shared/{0}" + RazorViewEngine.ViewExtension);
            options.ViewLocationFormats.Add("/Views/Shared/Default" + RazorViewEngine.ViewExtension);// add for default

            options.AreaViewLocationFormats.Add("/Areas/{2}/Views/{1}/{0}" + RazorViewEngine.ViewExtension);
            options.AreaViewLocationFormats.Add("/Areas/{2}/Views/Shared/{0}" + RazorViewEngine.ViewExtension);
            options.AreaViewLocationFormats.Add("/Areas/{2}/Views/Shared/Default" + RazorViewEngine.ViewExtension);// add for default
            options.AreaViewLocationFormats.Add("/Views/Shared/{0}" + RazorViewEngine.ViewExtension);
        });
        builder.Services.AddRouting(options => options.ConstraintMap["slugify"] = typeof(SlugifyParameterTransformer));

        // MVC
        builder.Services.AddMvc(options =>
        {
            options.ModelMetadataDetailsProviders.Insert(0, new DefaultDisplayMetadataProvider());
            options.Conventions.Add(new RouteTokenTransformerConvention(new SlugifyParameterTransformer()));
            options.Conventions.Add(new GenericControllerRouteConvention());
            //options.ModelBindingMessageProvider
        })
        .AddJsonOptions(options =>
        {
            options.JsonSerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
            options.JsonSerializerOptions.DictionaryKeyPolicy = JsonNamingPolicy.CamelCase;
            options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
            options.JsonSerializerOptions.Encoder = JavaScriptEncoder.Create(UnicodeRanges.All);
            options.JsonSerializerOptions.NumberHandling = JsonNumberHandling.AllowNamedFloatingPointLiterals;
            if (builder.Environment.IsDevelopment())
            {
                options.JsonSerializerOptions.WriteIndented = true;
            }
        })
        .AddViewLocalization(LanguageViewLocationExpanderFormat.Suffix)
        .AddDataAnnotationsLocalization(options =>
        {
            options.DataAnnotationLocalizerProvider = (type, factory) =>
            {
                var localizer = factory.Create(typeof(Resource));
                return localizer;
            };
        })
        .ConfigureApiBehaviorOptions(options =>
        {
            options.SuppressModelStateInvalidFilter = true;
            //options.InvalidModelStateResponseFactory = context =>
            //{
            //    context.ActionDescriptor.Parameters[0].ParameterType.GetMetadataForType(context.HttpContext.RequestServices);
            //    if (!context.ModelState.IsValid)
            //    {
            //        var errors = context.ModelState.ToErrors();
            //    }
            //    return new BadRequestObjectResult(context.ModelState);
            //};
        })
        .ConfigureApplicationPartManager(o => o.FeatureProviders.Add(new GenericControllerFeatureProvider()))
        .AddControllersAsServices();// must after ConfigureApplicationPartManager
    }

    protected virtual void AddSwagger(WebApplicationBuilder builder)
    {
        builder.Services.AddTransient<IConfigureOptions<SwaggerGenOptions>, CustomSwaggerGenOptions>();
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen(options =>
        {
            options.DocumentFilter<SwaggerFilter>();
            options.OperationFilter<SwaggerFilter>();
            options.DocInclusionPredicate((docName, api) =>
            {
                if (docName == "Default" && api.GroupName == null)
                {
                    return true;
                }
                return api.GroupName == docName;
            });
            options.AddSecurityDefinition("bearerAuth", new OpenApiSecurityScheme
            {
                Type = SecuritySchemeType.Http,
                Scheme = "bearer",
                BearerFormat = "JWT",
            });
            options.AddSecurityRequirement(new OpenApiSecurityRequirement
            {
                {
                    new OpenApiSecurityScheme
                    {
                        Reference = new OpenApiReference { Type = ReferenceType.SecurityScheme, Id = "bearerAuth" }
                    },
                    Array.Empty<string>()
                }
            });
        });
    }

    #endregion add services

    #region configure

    protected virtual void UseAuthorization(WebApplication app)
    {
        app.UseCors(this.Name);
        app.UseAuthentication();
        app.UseAuthorization();
    }

    protected virtual void UseDbContext(WebApplication app)
    {
        using var scope = app.Services.CreateScope();
        using var metaContext = scope.ServiceProvider.GetRequiredService<MetaDbContext>();
        if (metaContext.Database.EnsureCreated())
        {
            metaContext.Seed(metaContext);
            //metaContext.SaveChanges();
        }
        var contextList = scope.ServiceProvider.GetServices<DbContext>().Where(o => o.GetType().Name != nameof(MetaDbContext));
        foreach (var context in contextList)
        {
            var name = context.GetType().FullName!;
            var dbCreator = (context.GetService<IRelationalDatabaseCreator>() as RelationalDatabaseCreator)!;
            var sql = dbCreator.GenerateCreateScript();
            var dbExists = dbCreator.Exists();
            if (!dbExists)
            {
                dbCreator.Create();
            }
            //using var transaction = context.Database.BeginTransaction();
            try
            {
                var history = metaContext.Set<DbContextHistory>().FirstOrDefault(o => o.Name == name);
                var hash = sql.Md5()!;
                if (history == null)
                {
                    context.Database.ExecuteSqlRaw(sql);
                    (context as IDbSeed)?.Seed(context);
                    metaContext.Set<DbContextHistory>().Add(new DbContextHistory { Provider = context?.Database.ProviderName!, Name = name, Hash = hash, Sql = sql! });
                    metaContext.SaveChanges();
                    context?.SaveChanges();
                }
                else
                {
                    if (history.Hash != hash)
                    {
                        Console.WriteLine($"error:{name},{hash},{history.Hash}");
                    }
                }
                //transaction.Commit();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                //transaction.Rollback();
            }
        }
    }

    protected virtual void UseLocalization(WebApplication app)
    {
        var localizationOptions = app.Services.GetService<IOptions<RequestLocalizationOptions>>()!.Value;
        app.UseRequestLocalization(localizationOptions);
        Thread.CurrentThread.CurrentCulture = localizationOptions.DefaultRequestCulture.Culture;
        Thread.CurrentThread.CurrentUICulture = localizationOptions.DefaultRequestCulture.UICulture;
    }

    protected virtual void UseRouting(WebApplication app)
    {
        var requestLocalizationOptions = app.Services.GetRequiredService<IOptions<RequestLocalizationOptions>>().Value;
        var defaults = new { culture = requestLocalizationOptions.DefaultRequestCulture.Culture.Name };
        app.UseRouting();
        app.MapControllerRoute(name: "area", pattern: "{area:exists:slugify}/{controller:slugify=Home}/{action:slugify=Index}/{id?}", defaults: defaults);
        app.MapControllerRoute(name: "default", pattern: "{controller:slugify=Home}/{action:slugify=Index}/{id?}", defaults: defaults);
    }

    protected virtual void UseStaticFiles(WebApplication app)
    {
        // app 下载配置
        var provider = new FileExtensionContentTypeProvider();
        provider.Mappings.Add(".apk", "application/vnd.android.package-archive");
        provider.Mappings.Add(".plist", "text/xml");
        provider.Mappings.Add(".ipa", "application/iphone");
        app.UseStaticFiles();
    }

    protected virtual void UseSwagger(WebApplication app)
    {
        app.UseSwagger();
        app.UseSwaggerUI(options =>
        {
            var apiDescriptionGroups = app.Services.GetRequiredService<IApiDescriptionGroupCollectionProvider>().ApiDescriptionGroups.Items;
            foreach (var description in apiDescriptionGroups)
            {
                if (description.GroupName is not null)
                {
                    options.SwaggerEndpoint($"/swagger/{description.GroupName}/swagger.json", description.GroupName);
                }
                else
                {
                    options.SwaggerEndpoint($"/swagger/Default/swagger.json", "Default");
                }
            }
        });
    }

    #endregion configure
}
