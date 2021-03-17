using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Arma3TacMapLibrary.Arma3;
using Arma3TacMapWebApp.Entities;
using Arma3TacMapWebApp.Maps;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Arma3TacMapLibrary.TacMaps;
using Microsoft.AspNetCore.Cors;
// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Arma3TacMapWebApp.Controllers
{
    [Authorize(Policy = "ApiAny")]
    [Route("api/TacMaps")]
    [ApiController]
    public class ApiTacMapsController : ControllerBase
    {
        private readonly Arma3TacMapContext _context;
        private readonly MapInfosService _mapInfos;
        private readonly MapService _mapSvc;

        public ApiTacMapsController(Arma3TacMapContext context, MapInfosService mapInfos, MapService mapSvc)
        {
            _context = context;
            _mapInfos = mapInfos;
            _mapSvc = mapSvc;
        }

        [HttpGet]
        [EnableCors("Web")]
        public async Task<IEnumerable<ApiTacMap>> Get(string worldName)
        {
            var list = new List<ApiTacMap>();
            var user = await _mapSvc.GetUser(User);
            foreach (var access in await _mapSvc.GetUserMaps(User))
            {
                if (worldName == null || string.Equals(access.TacMap.WorldName, worldName, StringComparison.OrdinalIgnoreCase))
                {
                    list.Add(await ToApiTacMap(access, user, false));
                }
            }
            return list;
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> Get(int id)
        {
            var access = await _mapSvc.GrantReadAccess(User, id, null);
            if (access == null)
            {
                return NotFound();
            }
            var user = await _mapSvc.GetUser(User);
            return Ok(await ToApiTacMap(access, user, true));
        }

        private async Task<ApiTacMap> ToApiTacMap(TacMapAccess access, User user, bool includeMarkers)
        {
            var map = new ApiTacMap()
            {
                Id = access.TacMap.TacMapID,
                WorldName = access.TacMap.WorldName,
                Label = access.TacMap.Label,
                EventHref = access.TacMap.EventHref,
                Markers = includeMarkers ? await _mapSvc.GetMarkers(access.TacMap.TacMapID, true) : null,
                ReadOnlyToken = access.TacMap.ReadOnlyToken,
                ReadOnlyHref = Url.Action(nameof(HomeController.ViewMap), "Home", new { id = access.TacMap.TacMapID, t = access.TacMap.ReadOnlyToken }, Request.Scheme)
            };
            if (access.TacMap.OwnerUserID == user.UserID)
            {
                map.ReadWriteToken = access.TacMap.ReadWriteToken;
                map.ReadWriteHref = Url.Action(nameof(HomeController.EditMap), "Home", new { id = access.TacMap.TacMapID, t = access.TacMap.ReadWriteToken }, Request.Scheme);
            }
            map.PreviewHref = new Dictionary<int, string>();
            foreach(var size in MapPreviewService.ValidSizes)
            {
                map.PreviewHref.Add(size, Url.Action(nameof(HomeController.ViewMapScreenShot), "Home", new { id = access.TacMap.TacMapID, t = access.TacMap.ReadOnlyToken, size }, Request.Scheme));
            }
            return map;
        }

        [Authorize(Policy = "ApiClient")]
        [HttpPost]
        public async Task<IActionResult> Post([FromBody] ApiTacMapCreate value)
        {
            var id = await _mapSvc.CreateMap(User, value.WorldName, value.Label, value.EventHref);
            if (value.Markers != null && value.Markers.Count > 0 )
            {
                var dbUser = await _mapSvc.GetUser(User);
                foreach (var marker in value.Markers)
                {
                    await _context.AddAsync(new TacMapMarker()
                    {
                        TacMapID = id.TacMapID,
                        UserID = dbUser.UserID,
                        MarkerData = marker.MarkerData,
                        LastUpdate = DateTime.UtcNow
                    });
                }
                await _context.SaveChangesAsync();
            }
            return await Get(id.TacMapID);
        }

        [Authorize(Policy = "ApiClient")]
        [HttpPatch("{id}")]
        public async Task<IActionResult> Patch(int id, [FromBody] ApiTacMapPatch value)
        {
            var tacMap = await _context.TacMaps.FindAsync(id);
            if (tacMap == null)
            {
                return NotFound();
            }
            var user = await _mapSvc.GetUser(User);
            if (user == null || tacMap.OwnerUserID != user.UserID)
            {
                return Forbid();
            }
            tacMap.Label = value.Label ?? tacMap.Label;
            tacMap.EventHref = value.EventHref ?? tacMap.EventHref;
            _context.TacMaps.Update(tacMap);
            await _context.SaveChangesAsync();
            return await Get(id);
        }

        [Authorize(Policy = "ApiClient")]
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var tacMap = await _context.TacMaps.FindAsync(id);
            var user = await _mapSvc.GetUser(User);
            if (tacMap == null || user == null || tacMap.OwnerUserID != user.UserID)
            {
                return NotFound();
            }
            _context.TacMaps.Remove(tacMap);
            await _context.SaveChangesAsync();
            return Ok();
        }
    }
}
