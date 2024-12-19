using Arma3TacMapWebApp.Entities;
using System.Threading.Tasks;

namespace Arma3TacMapWebApp.Maps
{
    public interface IMapPreviewService
    {
        int[] ValidSizes { get; }

        Task<byte[]> GetPreview(TacMapAccess access, int size, int? phase = null);
    }
}