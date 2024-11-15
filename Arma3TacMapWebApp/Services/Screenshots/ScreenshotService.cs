using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using PuppeteerSharp;

namespace Arma3TacMapWebApp.Services.Screenshots
{
    public sealed class ScreenshotService : IScreenshotService, IDisposable
    {
        private readonly string? chromePath;
        private readonly SemaphoreSlim semaphoreSlim = new SemaphoreSlim(1, 1);
        private BrowserHolder? browserHolder;

        public ScreenshotService(IConfiguration config, IHostApplicationLifetime lifetime)
        {
            chromePath = config.GetValue<string>("Chrome");
            lifetime.ApplicationStopping.Register(Dispose);
        }

        public void Dispose()
        {
            if (browserHolder != null)
            {
                browserHolder.DisposeAsync().GetAwaiter().GetResult();
            }
        }


        public async Task<byte[]?> MakeScreenshotAsync(string uri)
        {
            if (string.IsNullOrEmpty(chromePath))
            {
                return null;
            }
            return await (await GetBrowser()).MakeScreenshotAsync(uri);
        }

        private async Task<BrowserHolder> GetBrowser()
        {
            if (browserHolder == null)
            {
                await semaphoreSlim.WaitAsync();
                try
                {
                    if (browserHolder == null)
                    {
                        browserHolder = new BrowserHolder(GetBrowserTask());
                        _ = Task.Run(CloseBrowserLater);
                    }
                }
                finally
                {
                    semaphoreSlim.Release();
                }
            }
            return browserHolder;
        }

        private async Task CloseBrowserLater()
        {
            await Task.Delay(TimeSpan.FromMinutes(15));
            await CloseBrowser();
        }

        public async Task CloseBrowser()
        {
            await semaphoreSlim.WaitAsync();
            try
            {
                if (browserHolder != null)
                {
                    var previousHolder = browserHolder;
                    browserHolder = null;
                    _ = Task.Run(previousHolder.DisposeAsync);
                }
            }
            finally
            {
                semaphoreSlim.Release();
            }
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
