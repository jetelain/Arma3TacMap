using System.Threading.Tasks;
using Arma3TacMapLibrary.Maps;
using Microsoft.AspNetCore.SignalR;

namespace Arma3TacMapLibrary.Hubs
{
    public class MapHub<TMapId> : Hub 
        where TMapId: IMapId
    {
        private const string MapIdProperty = "MapId";
        private const string PseudoUserIdProperty = "PseudoUserId";

        private readonly IMapService<TMapId> _svc;

        public MapHub(IMapService<TMapId> service)
        {
            _svc = service;
        }

        public async Task Hello(TMapId mapId)
        {
            var userAccess = await _svc.GetInitialData(Context.User, mapId);
            if (userAccess.CanRead)
            {
                await Groups.AddToGroupAsync(Context.ConnectionId, mapId.GetGroup());

                foreach (var marker in userAccess.InitialMarkers)
                {
                    await Clients.Caller.SendAsync("AddOrUpdateMarker", ToJsonMarker(mapId, marker), marker.IsReadOnly);
                }

                Context.Items[MapIdProperty] = mapId;
                Context.Items[PseudoUserIdProperty] = userAccess.PseudoUserId;
            }
        }

        private TMapId GetContextMapId()
        {
            return (TMapId)Context.Items[MapIdProperty];
        }

        private string GetPseudoUserId()
        {
            return (string)Context.Items[PseudoUserIdProperty];
        }

        public async Task AddMarker(MarkerData markerData)
        {
            var mapId = GetContextMapId();
            if (mapId == null)
            {
                return;
            }
            var storedMarker = await _svc.AddMarker(Context.User, mapId, MarkerData.Serialize(markerData));
            if (storedMarker != null)
            {
                await Notify(mapId, "AddOrUpdateMarker", storedMarker);
            }
        }

        public async Task UpdateMarker(int mapMarkerID, MarkerData markerData)
        {
            await DoUpdateMarker(mapMarkerID, markerData, true);
        }

        public async Task MoveMarker(int mapMarkerID, MarkerData markerData)
        {
            await DoUpdateMarker(mapMarkerID, markerData, false);
        }

        private async Task DoUpdateMarker(int mapMarkerID, MarkerData markerData, bool notifyCaller)
        {
            var mapId = GetContextMapId();
            if (mapId == null)
            {
                return;
            }
            var marker = await _svc.UpdateMarker(Context.User, mapId, mapMarkerID, MarkerData.Serialize(markerData));
            if (marker != null)
            {
                await Notify(mapId, "AddOrUpdateMarker", marker, notifyCaller);
            }
        }

        public async Task RemoveMarker(int mapMarkerID)
        {
            var mapId = GetContextMapId();
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
            var mapId = GetContextMapId();
            if (mapId == null)
            {
                return;
            }
            if (await _svc.CanPointMap(Context.User, mapId))
            {
                await Clients.Group(mapId.GetGroup()).SendAsync("PointMap", GetPseudoUserId(), pos);
            }
        }

        public async Task EndPointMap()
        {
            var mapId = GetContextMapId();
            if (mapId == null)
            {
                return;
            }
            if (await _svc.CanPointMap(Context.User, mapId))
            {
                await Clients.Group(mapId.GetGroup()).SendAsync("EndPointMap", GetPseudoUserId());
            }
        }

        private async Task Notify(TMapId mapId, string method, StoredMarker marker, bool notifyCaller = true)
        {
            if (notifyCaller)
            {
                await Clients.Group(mapId.GetGroup()).SendAsync(method, ToJsonMarker(mapId, marker), marker.IsReadOnly);
            }
            else
            {
                await Clients.OthersInGroup(mapId.GetGroup()).SendAsync(method, ToJsonMarker(mapId, marker), marker.IsReadOnly);
            }
        }

        private static Marker<TMapId> ToJsonMarker(TMapId mapId, StoredMarker marker)
        {
            return new Marker<TMapId>()
            {
                id = marker.Id,
                mapId = mapId,
                data = MarkerData.Deserialize(marker.MarkerData)
            };
        }
    }
}
