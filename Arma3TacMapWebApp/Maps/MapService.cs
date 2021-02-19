using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Threading.Tasks;
using Arma3TacMapLibrary.Maps;
using Arma3TacMapWebApp.Entities;
using Arma3TacMapWebApp.Models;
using Microsoft.EntityFrameworkCore;

namespace Arma3TacMapWebApp.Maps
{
    public class MapService : IMapService<MapId>
    {
        private readonly Arma3TacMapContext _db;

        public MapService(Arma3TacMapContext db)
        {
            _db = db;
        }

        internal static string GetSteamId(ClaimsPrincipal user)
        {
            if (user?.Identity?.IsAuthenticated ?? false)
            {
                var nameClaim = user.FindFirst(ClaimTypes.NameIdentifier);
                if (nameClaim != null && nameClaim.Value.StartsWith("https://steamcommunity.com/openid/id/"))
                {
                    return nameClaim.Value.Substring("https://steamcommunity.com/openid/id/".Length);
                }
            }
            return null;
        }

        public async Task<TacMapAccess> CanWrite(ClaimsPrincipal user, MapId mapId)
        {
            if (mapId.IsReadOnly)
            {
                return null;
            }
            var steamId = GetSteamId(user);
            return await _db.TacMapAccesses.FirstOrDefaultAsync(a => a.User.SteamId == steamId && a.TacMapID == mapId.TacMapID && a.CanWrite);
        }

        internal async Task<MapId> CreateMap(ClaimsPrincipal user, string worldName, string label, Uri eventHref)
        {
            var dbUser = await GetOrCreateUser(user);
            if (dbUser == null)
            {
                throw new InvalidOperationException();
            }
            var map = new TacMap()
            {
                Created = DateTime.UtcNow,
                Label = label,
                EventHref = eventHref,
                Owner = dbUser,
                WorldName = worldName,
                ReadWriteToken = GenerateToken(),
                ReadOnlyToken = GenerateToken()
            };
            var access = new TacMapAccess()
            {
                TacMap = map,
                User = dbUser,
                CanWrite = true
            };
            await _db.AddAsync(map);
            await _db.AddAsync(access);
            await _db.SaveChangesAsync();
            return new MapId() { TacMapID = map.TacMapID };
        }

        internal async Task<TacMapAccess> GrantReadAccess(ClaimsPrincipal user, int id, string t)
        {
            var dbUser = await GetUser(user);
            if (dbUser != null)
            {
                var access = await _db.TacMapAccesses.Include(a => a.TacMap).FirstOrDefaultAsync(a => a.UserID == dbUser.UserID && a.TacMapID == id);
                if (access != null)
                {
                    return access;
                }
            }
            var map = await _db.TacMaps.FirstOrDefaultAsync(a => a.TacMapID == id && a.ReadOnlyToken == t);
            if (map != null)
            {
                var access = new TacMapAccess()
                {
                    TacMapID = map.TacMapID,
                    TacMap = map,
                    User = dbUser,
                    CanWrite = false
                };
                if (dbUser != null)
                {
                    await _db.AddAsync(access);
                    await _db.SaveChangesAsync();
                }
                return access;
            }
            return null;
        }

        internal async Task<TacMapAccess> GrantWriteAccess(ClaimsPrincipal user, int id, string t)
        {
            var dbUser = await GetOrCreateUser(user);
            if (dbUser == null)
            {
                throw new InvalidOperationException();
            }
            var access = await _db.TacMapAccesses.Include(a => a.TacMap).FirstOrDefaultAsync(a => a.UserID == dbUser.UserID && a.TacMapID == id);
            if (access != null)
            {
                if (t == access.TacMap.ReadWriteToken)
                {
                    access.CanWrite = true;
                    await _db.SaveChangesAsync();
                }
                if (access.CanWrite)
                {
                    return access;
                }
            }
            else
            {
                var map = await _db.TacMaps.FirstOrDefaultAsync(a => a.TacMapID == id && a.ReadWriteToken == t);
                if (map != null)
                {
                    access = new TacMapAccess() { CanWrite = true, TacMap = map, User = dbUser };
                    await _db.AddAsync(access);
                    await _db.SaveChangesAsync();
                    return access;
                }
            }
            return null;
        }

        public async Task<User> GetUser(ClaimsPrincipal user)
        {
            var steamId = GetSteamId(user);
            if (string.IsNullOrEmpty(steamId))
            {
                return await GetApiUser(user);
            }
            return await _db.Users.FirstOrDefaultAsync(u => u.SteamId == steamId);
        }

        private async Task<User> GetApiUser(ClaimsPrincipal user)
        {
            if (user?.Identity?.IsAuthenticated ?? false)
            {
                var userIDClaim = user.FindFirst(User.UserIDClaim);
                if (userIDClaim != null)
                {
                    var userID = int.Parse(userIDClaim.Value);
                    return await _db.Users.FirstOrDefaultAsync(u => u.UserID == userID);
                }
            }
            return null;
        }

        private async Task<User> GetOrCreateUser(ClaimsPrincipal user)
        {
            var steamId = GetSteamId(user);
            if (string.IsNullOrEmpty(steamId))
            {
                return await GetApiUser(user);
            }
            var dbUser = await _db.Users.FirstOrDefaultAsync(u => u.SteamId == steamId);
            if (dbUser == null)
            {
                dbUser = new User()
                {
                    SteamId = steamId,
                    UserLabel = user.Identity.Name
                };
                await _db.AddAsync(dbUser);
                await _db.SaveChangesAsync();
            }

            return dbUser;
        }

        public async Task<StoredMarker> AddMarker(ClaimsPrincipal user, MapId mapId, string markerData)
        {
            var access = await CanWrite(user, mapId);
            if (access == null)
            {
                return null;
            }
            var marker = new TacMapMarker()
            {
                TacMapID = access.TacMapID,
                UserID = access.UserID,
                MarkerData = markerData,
                LastUpdate = DateTime.UtcNow
            };
            await _db.AddAsync(marker);
            await _db.SaveChangesAsync();
            return ToStored(marker);
        }

        public async Task<bool> CanPointMap(ClaimsPrincipal user, MapId mapId)
        {
            return await CanWrite(user, mapId) != null;
        }

        public async Task<MapUserInitialData> GetInitialData(ClaimsPrincipal user, MapId mapId)
        {
            var steamId = GetSteamId(user);
            if (!string.IsNullOrEmpty(steamId))
            {
                var userAccess = await _db.TacMapAccesses.FirstOrDefaultAsync(a => a.User.SteamId == steamId && a.TacMapID == mapId.TacMapID);
                if (userAccess != null)
                {
                    if (!mapId.IsReadOnly && !userAccess.CanWrite)
                    {
                        // Map has been requested as read+write, but user is not allowed
                        return MapUserInitialData.Denied;
                    }
                    return new MapUserInitialData()
                    {
                        CanRead = true,
                        PseudoUserId = $"user-{userAccess.UserID}",
                        InitialMarkers = await GetMarkers(mapId.TacMapID, mapId.IsReadOnly)
                    };
                }
            }
            if (mapId.IsReadOnly && await _db.TacMaps.AnyAsync(m => m.TacMapID == mapId.TacMapID && m.ReadOnlyToken == mapId.ReadToken))
            {
                return new MapUserInitialData()
                {
                    CanRead = true,
                    PseudoUserId = $"anonymous-{Guid.NewGuid()}",
                    InitialMarkers = await GetMarkers(mapId.TacMapID, mapId.IsReadOnly)
                };
            }
            return MapUserInitialData.Denied;
        }

        internal async Task<List<StoredMarker>> GetMarkers(int tacMapID, bool isReadOnly)
        {
            return (await _db.TacMapMarkers.Where(m => m.TacMapID == tacMapID).ToListAsync())
                .Select(m => new StoredMarker()
                {
                    Id = m.TacMapMarkerID,
                    IsReadOnly = isReadOnly,
                    MarkerData = m.MarkerData
                })
                .ToList();
        }

        public async Task<StoredMarker> RemoveMarker(ClaimsPrincipal user, MapId mapId, int mapMarkerID)
        {
            var access = await CanWrite(user, mapId);
            if (access == null)
            {
                return null;
            }
            var marker = await _db.TacMapMarkers.FirstOrDefaultAsync(m => m.TacMapID == access.TacMapID && m.TacMapMarkerID == mapMarkerID);
            if (marker != null)
            {
                _db.Remove(marker);
                await _db.SaveChangesAsync();
                return ToStored(marker);
            }
            return null;
        }

        private StoredMarker ToStored(TacMapMarker marker)
        {
            return new StoredMarker()
            {
                Id = marker.TacMapMarkerID,
                IsReadOnly = false,
                MarkerData = marker.MarkerData
            };
        }

        public async Task<StoredMarker> UpdateMarker(ClaimsPrincipal user, MapId mapId, int mapMarkerID, string markerData)
        {
            var access = await CanWrite(user, mapId);
            if (access == null)
            {
                return null;
            }
            var marker = await _db.TacMapMarkers.FirstOrDefaultAsync(m => m.TacMapID == access.TacMapID && m.TacMapMarkerID == mapMarkerID);
            if (marker != null)
            {
                marker.MarkerData = markerData;
                marker.UserID = access.UserID;
                marker.LastUpdate = DateTime.UtcNow;
                _db.Update(marker);
                await _db.SaveChangesAsync();
                return ToStored(marker);
            }
            return null;
        }
        internal static string GenerateToken()
        {
            var random = new byte[32];
            using (var rng = new RNGCryptoServiceProvider())
            {
                rng.GetBytes(random);
            }
            return Convert.ToBase64String(random).Replace("+", "-").Replace("/", "_").TrimEnd('=');
        }

        internal async Task<List<TacMapAccess>> GetUserMaps(ClaimsPrincipal user, int maxMaps = 1000)
        {
            var dbUser = await GetOrCreateUser(user);
            if (dbUser != null)
            {
                return await _db.TacMapAccesses.Where(m => m.UserID == dbUser.UserID).Include(t => t.TacMap).Include(t => t.TacMap.Owner).OrderByDescending(m => m.TacMap.Created).Take(maxMaps).ToListAsync();
            }
            return new List<TacMapAccess>();
        }


        public async Task<StaticMapData> GetStaticMapModel(int id, string t)
        {
            var map = await _db.TacMaps.FirstOrDefaultAsync(a => a.TacMapID == id && a.ReadOnlyToken == t);
            if (map == null)
            {
                return null;
            }
            return new StaticMapData()
            {
                Markers = await GetMarkers(map.TacMapID, true),
                WorldName = map.WorldName
            };
        }

    }
}
