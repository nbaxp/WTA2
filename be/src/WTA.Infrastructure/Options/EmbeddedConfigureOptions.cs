using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Options;
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
        WebApp.ModuleAssemblies?
            .ForEach(o =>
            {
                try
                {
                    providers.Add(new ManifestEmbeddedFileProvider(o, "wwwroot"));
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex);
                }
            });
        options.FileProvider = new CompositeFileProvider(providers.ToArray());
    }
}
