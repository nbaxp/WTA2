using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Options;
using WTA.Application;
using WTA.Application.Resources;

namespace WTA.Infrastructure.Options;

public class EmbeddedConfigureOptions : IPostConfigureOptions<StaticFileOptions>
{
    public void PostConfigure(string? name, StaticFileOptions options)
    {
        var providers = new List<IFileProvider>
        {
            new ManifestEmbeddedFileProvider(typeof(Resource).Assembly, "wwwroot")
        };
        App.ModuleAssemblies?
            .ForEach(o =>
            {
                try
                {
                    providers.Add(new ManifestEmbeddedFileProvider(o, "wwwroot"));
                }
                catch (Exception ex)
                {
                    Console.WriteLine(o.FullName);
                    Console.WriteLine(ex);
                }
            });
        providers.Add(new PhysicalFileProvider(Path.Combine(Directory.GetCurrentDirectory(), "wwwroot")));
        options.FileProvider = new CompositeFileProvider(providers.ToArray());
    }
}
