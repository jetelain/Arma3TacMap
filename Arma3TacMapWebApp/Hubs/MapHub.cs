using System.Threading.Tasks;
using Arma3TacMapLibrary.Maps;
using Arma3TacMapWebApp.Maps;
using Microsoft.AspNetCore.SignalR;

namespace Arma3TacMapWebApp.Hubs
{
    public class MapHub : Hub 
    {
        private const string MapIdProperty = "MapId";
        private const string PseudoUserIdProperty = "PseudoUserId";

        private readonly IMapService _svc;

        public MapHub(IMapService service)
        {
            _svc = service;
        }

        public async Task Hello(MapId mapId)
        {
            var userAccess = await _svc.GetInitialData(Context.User, mapId);
            if (userAccess.CanRead)
            {
                await Groups.AddToGroupAsync(Context.ConnectionId, GetGroup(mapId));

                foreach (var marker in userAccess.InitialMarkers)
                {
                    await Clients.Caller.SendAsync("AddOrUpdateMarker", ToJsonMarker(mapId, marker), marker.IsReadOnly);
                }

                Context.Items[MapIdProperty] = mapId;
                Context.Items[PseudoUserIdProperty] = userAccess.PseudoUserId;
            }
        }

        private MapId GetContexMapId()
        {
            return (MapId)Context.Items[MapIdProperty];
        }

        private string GetPseudoUserId()
        {
            return (string)Context.Items[PseudoUserIdProperty];
        }

        public async Task AddMarker(MarkerData markerData)
        {
            var mapId = GetContexMapId();
            if (mapId == null)
            {
                return;
            }
            await AddMarkerToLayer(markerData, mapId, null);
        }

        public async Task AddMarkerToLayer(int layerId, MarkerData markerData)
        {
            var mapId = GetContexMapId();
            if (mapId == null)
            {
                return;
            }
            await AddMarkerToLayer(markerData, mapId, layerId);
        }

        private async Task AddMarkerToLayer(MarkerData markerData, MapId mapId, int? layerId)
        {
            var storedMarker = await _svc.AddMarker(Context.User, mapId, layerId, MarkerData.Serialize(markerData));
            if (storedMarker != null)
            {
                await Notify(mapId, "AddOrUpdateMarker", storedMarker);
            }
        }

        public async Task UpdateMarker(int mapMarkerID, MarkerData markerData)
        {
            await DoUpdateMarker(mapMarkerID, markerData, true, null);
        }

        public async Task UpdateMarkerToLayer(int mapMarkerID, int? layerId, MarkerData markerData)
        {
            await DoUpdateMarker(mapMarkerID, markerData, true, layerId);
        }

        public async Task MoveMarker(int mapMarkerID, MarkerData markerData)
        {
            await DoUpdateMarker(mapMarkerID, markerData, false, null);
        }

        private async Task DoUpdateMarker(int mapMarkerID, MarkerData markerData, bool notifyCaller, int? layerId)
        {
            var mapId = GetContexMapId();
            if (mapId == null)
            {
                return;
            }
            var marker = await _svc.UpdateMarker(Context.User, mapId, mapMarkerID, layerId, MarkerData.Serialize(markerData));
            if (marker != null)
            {
                await Notify(mapId, "AddOrUpdateMarker", marker, notifyCaller);
            }
        }

        public async Task RemoveMarker(int mapMarkerID)
        {
            var mapId = GetContexMapId();
            if (mapId == null)
            {
                return;
            }
            var marker = await _svc.RemoveMarker(Context.User, mapId, mapMarkerID);
            if (marker != null)
            {
                await Notify(mapId, "RemoveMarker", marker);
            }
        }

        public async Task PointMap(double[] pos)
        {
            var mapId = GetContexMapId();
            if (mapId == null)
            {
                return;
            }
            if (await _svc.CanPointMap(Context.User, mapId))
            {
                await Clients.Group(GetGroup(mapId)).SendAsync("PointMap", GetPseudoUserId(), pos);
            }
        }

        public async Task EndPointMap()
        {
            var mapId = GetContexMapId();
            if (mapId == null)
            {
                return;
            }
            if (await _svc.CanPointMap(Context.User, mapId))
            {
                await Clients.Group(GetGroup(mapId)).SendAsync("EndPointMap", GetPseudoUserId());
            }
        }

        private async Task Notify(MapId mapId, string method, StoredMarker marker, bool notifyCaller = true)
        {
            if (notifyCaller)
            {
                await Clients.Group(GetGroup(mapId)).SendAsync(method, ToJsonMarker(mapId, marker), marker.IsReadOnly);
            }
            else
            {
                await Clients.OthersInGroup(GetGroup(mapId)).SendAsync(method, ToJsonMarker(mapId, marker), marker.IsReadOnly);
            }
        }

        private static Marker ToJsonMarker(MapId mapId, StoredMarker marker)
        {
            return new Marker()
            {
                id = marker.Id,
                layerId = marker.LayerId,
                mapId = mapId,
                data = MarkerData.Deserialize(marker.MarkerData)
            };
        }

        private static string GetGroup(MapId id)
        {
            return $"TacMap-{id.TacMapID}";
        }
    }
}
