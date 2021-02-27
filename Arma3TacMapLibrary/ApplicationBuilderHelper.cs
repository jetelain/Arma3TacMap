using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Arma3TacMapLibrary.TacMaps;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;

namespace Arma3TacMapLibrary
{
    public static class ApplicationBuilderHelper
    {
        public static void UseArma3TacMapStaticFiles(this IApplicationBuilder app)
        {
            var assembly = typeof(ApplicationBuilderHelper).Assembly;
            app.UseStaticFiles(new StaticFileOptions
            {
                FileProvider = new ManifestEmbeddedFileProvider(assembly, "wwwroot", "Arma3TacMapLibrary.Manifest.xml", File.GetLastWriteTimeUtc(assembly.Location))
            });
        }
        public static void AddArma3TacMapApi(this IServiceCollection svc, IConfiguration config)
        {
            svc.AddHttpClient<IApiTacMaps, ApiTacMaps>(c =>
            {
                c.BaseAddress = new Uri(config.GetValue<string>("ApiTacMaps:Url") ?? "https://maps.plan-ops.fr/", UriKind.Absolute);
                c.DefaultRequestHeaders.Add("ApiKey", config.GetValue<string>("ApiTacMaps:Key"));
            });
        }
    }
}
