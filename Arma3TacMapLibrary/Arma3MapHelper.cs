using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Extensions.Configuration;

namespace Arma3TacMapLibrary
{
    internal static class Arma3MapHelper
    {
        public static string GetEndpoint(IConfiguration configuration)
        {
            return configuration.GetValue<string>("Arma3MapEndpoint") ?? "https://jetelain.github.io/Arma3Map";
        }
    }
}
