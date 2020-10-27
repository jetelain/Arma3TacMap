using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Arma3TacMapLibrary.Maps
{
    public interface IMapService<TMapId> where TMapId : IMapId
    {
        Task<StoredMarker> AddMarker(ClaimsPrincipal user, TMapId mapId, string markerData);

        Task<StoredMarker> UpdateMarker(ClaimsPrincipal user, TMapId mapId, int mapMarkerID, string markerData);

        Task<StoredMarker> RemoveMarker(ClaimsPrincipal user, TMapId mapId, int mapMarkerID);

        Task<MapUserInitialData> GetInitialData(ClaimsPrincipal user, TMapId mapId);

        Task<bool> CanPointMap(ClaimsPrincipal user, TMapId mapId);
    }
}