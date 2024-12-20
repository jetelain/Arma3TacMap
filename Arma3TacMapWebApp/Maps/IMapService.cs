using System.Security.Claims;
using System.Threading.Tasks;
using Arma3TacMapLibrary.Maps;

namespace Arma3TacMapWebApp.Maps
{
    public interface IMapService
    {
        Task<StoredMarker?> AddMarker(ClaimsPrincipal? user, MapId mapId, int? layerId, string markerData);

        Task<StoredMarker?> UpdateMarker(ClaimsPrincipal? user, MapId mapId, int mapMarkerID, int? layerId, string markerData);

        Task<StoredMarker?> RemoveMarker(ClaimsPrincipal? user, MapId mapId, int mapMarkerID);

        Task<MapUserInitialData?> GetInitialData(ClaimsPrincipal? user, MapId mapId);

        Task<bool> CanPointMap(ClaimsPrincipal? user, MapId mapId);

        Task<StoredLayer?> CreateLayer(ClaimsPrincipal? user, MapId mapId, string label, int? phase = null, int order = 0);

        Task<StoredLayer?> UpdateLayer(ClaimsPrincipal? user, MapId mapId, int layerId, string label, int? phase = null, int order = 0);

        Task<StoredLayerWithMarkers?> RemoveLayer(ClaimsPrincipal? user, MapId mapId, int layerId);
    }
}