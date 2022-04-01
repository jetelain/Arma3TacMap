using System.Security.Claims;
using System.Threading.Tasks;
using Arma3TacMapLibrary.Maps;

namespace Arma3TacMapWebApp.Maps
{
    public interface IMapService
    {
        Task<StoredMarker> AddMarker(ClaimsPrincipal user, MapId mapId, string markerData);

        Task<StoredMarker> UpdateMarker(ClaimsPrincipal user, MapId mapId, int mapMarkerID, string markerData);

        Task<StoredMarker> RemoveMarker(ClaimsPrincipal user, MapId mapId, int mapMarkerID);

        Task<MapUserInitialData> GetInitialData(ClaimsPrincipal user, MapId mapId);

        Task<bool> CanPointMap(ClaimsPrincipal user, MapId mapId);
    }
}