using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Arma3TacMapWebApp.Entities;
using Microsoft.AspNetCore.Authorization;
using Arma3TacMapWebApp.Maps;
using Arma3TacMapWebApp.Models;
using Arma3TacMapLibrary.Maps;
using Arma3TacMapLibrary.Arma3;

namespace Arma3TacMapWebApp.Controllers
{

    [Authorize(Policy = "LoggedUser")]
    public class TacMapsController : Controller
    {
        private readonly Arma3TacMapContext _context;
        private readonly MapInfosService _mapInfos;
        private readonly MapService _mapSvc;

        public TacMapsController(Arma3TacMapContext context, MapInfosService mapInfos, MapService mapSvc)
        {
            _context = context;
            _mapInfos = mapInfos;
            _mapSvc = mapSvc;
        }

        // GET: TacMaps
        public async Task<IActionResult> Index()
        {
            var maps = await _mapInfos.GetMapsInfos();
            var list = await _mapSvc.GetUserMaps(User);
            foreach (var map in list)
            {
                map.TacMap.MapInfos = maps.FirstOrDefault(m => m.worldName == map.TacMap.WorldName);
            }
            return View(list);
        }

        // GET: TacMaps/Create
        public async Task<IActionResult> Create(string worldName)
        {
            ViewBag.Maps = await _mapInfos.GetMapsInfos(); 
            return View(new TacMap()
            {
                WorldName = worldName,
                Label = "Carte tactique sans nom"
            });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Label,WorldName")] TacMap tacMap)
        {
            if (ModelState.IsValid)
            {
                var map = await _mapSvc.CreateMap(User, tacMap.WorldName, tacMap.Label);
                return RedirectToAction(nameof(HomeController.EditMap), "Home", new { id = map.TacMapID });
            }
            ViewBag.Maps = await _mapInfos.GetMapsInfos();
            return View(tacMap);
        }

        // GET: TacMaps/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var tacMap = await _context.TacMaps
                .Include(t => t.Owner)
                .FirstOrDefaultAsync(m => m.TacMapID == id);
            if (tacMap == null)
            {
                return NotFound();
            }
            if (tacMap.OwnerUserID != (await _mapSvc.GetUser(User)).UserID)
            {
                return Forbid();
            }
            return View(tacMap);
        }

        // POST: TacMaps/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("TacMapID,Label,WorldName")] TacMap edited)
        {
            if (id != edited.TacMapID)
            {
                return NotFound();
            }
            var tacMap = await _context.TacMaps
                .Include(t => t.Owner)
                .FirstOrDefaultAsync(m => m.TacMapID == id);

            if (tacMap.OwnerUserID != (await _mapSvc.GetUser(User)).UserID)
            {
                return Forbid();
            }

            tacMap.Label = edited.Label;
            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(tacMap);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!TacMapExists(edited.TacMapID))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(HomeController.EditMap), "Home", new { id = edited.TacMapID });
            }
            return View(tacMap);
        }

        // GET: TacMaps/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var tacMap = await _context.TacMaps
                .Include(t => t.Owner)
                .FirstOrDefaultAsync(m => m.TacMapID == id);
            if (tacMap == null)
            {
                return NotFound();
            }

            if (tacMap.OwnerUserID != (await _mapSvc.GetUser(User)).UserID)
            {
                return Forbid();
            }
            return View(tacMap);
        }

        // POST: TacMaps/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var tacMap = await _context.TacMaps.FindAsync(id);
            if (tacMap.OwnerUserID != (await _mapSvc.GetUser(User)).UserID)
            {
                return Forbid();
            }
            _context.TacMaps.Remove(tacMap);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Forget(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            var user = await _mapSvc.GetUser(User);
            var tacMapAccess = await _context.TacMapAccesses
                .Include(t => t.TacMap).ThenInclude(m => m.Owner)
                .FirstOrDefaultAsync(m => m.TacMapID == id && m.UserID == user.UserID);
            if (tacMapAccess == null)
            {
                return NotFound();
            }
            if (tacMapAccess.TacMap.OwnerUserID == user.UserID)
            {
                return RedirectToAction(nameof(Delete), new { id });
            }
            return View(tacMapAccess.TacMap);
        }

        [HttpPost, ActionName("Forget")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ForgetConfirmed(int id)
        {
            var user = await _mapSvc.GetUser(User);
            var tacMapAccess = await _context.TacMapAccesses.FirstOrDefaultAsync(m => m.TacMapID == id && m.UserID == user.UserID);
            if (tacMapAccess == null)
            {
                return NotFound();
            }
            _context.TacMapAccesses.Remove(tacMapAccess);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }


        public async Task<IActionResult> Export(int id)
        {
            var mapAccess = await _mapSvc.GrantWriteAccess(User, id, null);
            if (mapAccess == null)
            {
                return Forbid();
            }

            return View(new ExportViewModel()
            {
                TacMap = mapAccess.TacMap,
                Access = mapAccess,
                Script = MapExporter.GetSqf(await _mapSvc.GetMarkers(id, true))
            });
        }
        private bool TacMapExists(int id)
        {
            return _context.TacMaps.Any(e => e.TacMapID == id);
        }
    }
}
