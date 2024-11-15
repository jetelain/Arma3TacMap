using System.Threading.Tasks;

namespace Arma3TacMapWebApp.Services.Screenshots
{
    public interface IScreenshotService
    {
        Task<byte[]?> MakeScreenshotAsync(string uri);
    }
}
