using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.FileProviders;

namespace Arma3TacMapLibrary
{
    public static class ApplicationBuilderHelper
    {
        public static void UseArma3TacMapStaticFiles(this IApplicationBuilder app)
        {
            app.UseStaticFiles(new StaticFileOptions
            {
                FileProvider = new ManifestEmbeddedFileProvider(typeof(ApplicationBuilderHelper).Assembly, "wwwroot")
            });
        }

    }
}
