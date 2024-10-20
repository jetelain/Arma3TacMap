using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using PuppeteerSharp;

namespace Arma3TacMapWebApp.Maps
{
    public class ScreenshotService : IScreenshotService, IDisposable
    {
        private readonly string? chromePath;
        private readonly SemaphoreSlim semaphoreSlim = new SemaphoreSlim(1, 1);
        private Task<IBrowser>? browserTask;

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

        public async Task<byte[]?> MakeScreenshotAsync(string uri)
        {
            if (string.IsNullOrEmpty(chromePath))
            {
                return null;
            }
            var browser = await GetBrowser();
            var page = await browser.NewPageAsync();
            await page.GoToAsync(uri);
            await page.WaitForExpressionAsync("arma3TacMapLoaded", new WaitForFunctionOptions() { PollingInterval = 500, Timeout = 7500 });
            var bytes = await page.ScreenshotDataAsync();
            await page.CloseAsync();
            return bytes;
        }

        private async Task<IBrowser> GetBrowser()
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

        private async Task<IBrowser> GetBrowserTask()
        {
            return await Puppeteer.LaunchAsync(new LaunchOptions
            {
                Headless = true,
                ExecutablePath = chromePath,
                DefaultViewport = new ViewPortOptions() { Width = 2048, Height = 2048 }
            });
        }
    }
}
