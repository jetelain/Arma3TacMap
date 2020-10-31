using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Microsoft.AspNetCore.Builder;
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

    }
}
