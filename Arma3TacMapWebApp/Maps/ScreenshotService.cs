using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Configuration;
using PuppeteerSharp;
using System.Threading;

namespace Arma3TacMapWebApp.Maps
{
    public class ScreenshotService : IDisposable
    {
        private readonly string chromePath;
        private readonly SemaphoreSlim semaphoreSlim = new SemaphoreSlim(1, 1);
        private Task<Browser> browserTask;

        public ScreenshotService(IConfiguration config, IHostApplicationLifetime lifetime)
        {
            chromePath = config.GetValue<string>("Chrome");
            lifetime.ApplicationStopping.Register(Dispose);
        }

        public void Dispose()
        {
            if (browserTask != null)
            {
                browserTask.Result.CloseAsync().Wait();
                browserTask.Result.Dispose();
            }
        }

        public async Task<byte[]> MakeScreenshotAsync(string uri)
        {
            var browser = await GetBrowser();
            var page = await browser.NewPageAsync();
            await page.GoToAsync(uri);
            await page.WaitForExpressionAsync("arma3TacMapLoaded", new WaitForFunctionOptions() { PollingInterval = 500, Timeout = 7500 });
            var bytes = await page.ScreenshotDataAsync();
            await page.CloseAsync();
            return bytes;
        }

        private async Task<Browser> GetBrowser()
        {
            if (browserTask == null)
            {
                await semaphoreSlim.WaitAsync();
                try
                {
                    if (browserTask == null)
                    {
                        browserTask = GetBrowserTask();
                    }
                }
                finally
                {
                    semaphoreSlim.Release();
                }
            }
            return await browserTask;
        }

        private async Task<Browser> GetBrowserTask()
        {
            return await Puppeteer.LaunchAsync(new LaunchOptions
            {
                Headless = true,
                ExecutablePath = chromePath,
                IgnoreHTTPSErrors = true,
                DefaultViewport = new ViewPortOptions() { Width = 2048, Height = 2048 }
            });
        }
    }
}
