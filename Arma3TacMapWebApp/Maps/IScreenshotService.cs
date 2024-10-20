using System.Threading.Tasks;

namespace Arma3TacMapWebApp.Maps
{
    public interface IScreenshotService
    {
        Task<byte[]?> MakeScreenshotAsync(string uri);
    }
}