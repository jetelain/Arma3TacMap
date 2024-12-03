using System;
using System.Threading.Tasks;
using PuppeteerSharp;

namespace Arma3TacMapWebApp.Services.Screenshots
{
    internal sealed class BrowserHolder : IAsyncDisposable
    {
        private readonly Task<IBrowser> browserTask;
        private int running = 0;
        private bool isDisposePending = false;
        private bool isDisposed = false;
        private readonly object locker = new object();

        public BrowserHolder(Task<IBrowser> browserTask)
        {
            this.browserTask = browserTask;
        }

        public async ValueTask DisposeAsync()
        {
            lock (locker)
            {
                isDisposePending = true;
                if (running > 0 || isDisposed)
                {
                    return;
                }
            }

            var browser = await browserTask;
            await browser.CloseAsync();
            await browser.DisposeAsync();
        }

        public async Task<byte[]?> MakeScreenshotAsync(string uri)
        {
            lock (locker)
            {
                if (isDisposed || isDisposePending)
                {
                    return null;
                }
                running++;
            }
            byte[] bytes;
            try
            {
                var browser = await browserTask;
                var page = await browser.NewPageAsync();
                await page.GoToAsync(uri);
                await page.WaitForExpressionAsync("arma3TacMapLoaded", new WaitForFunctionOptions() { PollingInterval = 500, Timeout = 7500 });
                bytes = await page.ScreenshotDataAsync();
                await page.CloseAsync();
            }
            finally
            {
                lock (locker)
                {
                    running--;
                }
                if (isDisposePending)
                {
                    await DisposeAsync();
                }
            }
            return bytes;
        }


    }
}
