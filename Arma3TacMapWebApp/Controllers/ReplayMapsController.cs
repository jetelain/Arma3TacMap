using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Arma3TacMapWebApp.Entities;
using Arma3TacMapWebApp.Models;
using Arma3TacMapWebApp.Services;
using Arma3TacMapWebApp.Maps;
using System.IO;
using System.Text.Json;

namespace Arma3TacMapWebApp.Controllers
{
    public class ReplayMapsController : Controller
    {
        private readonly Arma3TacMapContext _context;
        private readonly ReplayImporter _importer;
        private readonly MapService _mapSvc;

        public ReplayMapsController(Arma3TacMapContext context, ReplayImporter importer, MapService mapSvc)
        {
            _context = context;
            _importer = importer;
            _mapSvc = mapSvc;
        }

        // GET: ReplayMaps
        public async Task<IActionResult> Index()
        {
            var arma3TacMapContext = _context.ReplayMap.Include(r => r.Owner).Include(r => r.TacMap);
            return View(await arma3TacMapContext.ToListAsync());
        }

        // GET: ReplayMaps/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var replayMap = await _context.ReplayMap
                .Include(r => r.Owner)
                .Include(r => r.TacMap)
                .FirstOrDefaultAsync(m => m.ReplayMapID == id);
            if (replayMap == null)
            {
                return NotFound();
            }

            return View(replayMap);
        }

        // GET: ReplayMaps/Create
        public IActionResult Create()
        {
            return View(new ImportReplayMapVM());
        }

        // POST: ReplayMaps/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(ImportReplayMapVM vm)
        {
            if (ModelState.IsValid)
            {
                var user = await _mapSvc.GetOrCreateUser(User);
                ReplayMap map;
                try
                {
                    if (vm.FileFormat == ImportFileFormat.AarTxt)
                    {
                        map = await _importer.ImportAarTxt(user, vm.File, vm.Label);
                    }
                    else if (vm.FileFormat == ImportFileFormat.AarLog)
                    {
                        map = await _importer.ImportAarLog(user, vm.File, vm.Label);
                    }
                    else if (vm.FileFormat == ImportFileFormat.GtdLog)
                    {
                        map = await _importer.ImportGtdLog(user, vm.File, vm.Label, TimeZoneInfo.Local);
                    }
                    else
                    {
                        return View(vm);
                    }
                    return RedirectToAction(nameof(Details), new { id = map.ReplayMapID });
                }
                catch (ApplicationException e)
                {
                    ModelState.AddModelError("File", e.Message);
                }
                catch (JsonException e)
                {
                    ModelState.AddModelError("File", e.Message);
                }
            }
            return View(vm);
        }

        // GET: ReplayMaps/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var replayMap = await _context.ReplayMap.FindAsync(id);
            if (replayMap == null)
            {
                return NotFound();
            }
            ViewData["OwnerUserID"] = new SelectList(_context.Users, "UserID", "UserID", replayMap.OwnerUserID);
            ViewData["TacMapID"] = new SelectList(_context.TacMaps, "TacMapID", "Label", replayMap.TacMapID);
            return View(replayMap);
        }

        // POST: ReplayMaps/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("ReplayMapID,OwnerUserID,Created,Label,ReadOnlyToken,WorldName,TacMapID")] ReplayMap replayMap)
        {
            if (id != replayMap.ReplayMapID)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(replayMap);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ReplayMapExists(replayMap.ReplayMapID))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            ViewData["OwnerUserID"] = new SelectList(_context.Users, "UserID", "UserID", replayMap.OwnerUserID);
            ViewData["TacMapID"] = new SelectList(_context.TacMaps, "TacMapID", "Label", replayMap.TacMapID);
            return View(replayMap);
        }

        // GET: ReplayMaps/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var replayMap = await _context.ReplayMap
                .Include(r => r.Owner)
                .Include(r => r.TacMap)
                .FirstOrDefaultAsync(m => m.ReplayMapID == id);
            if (replayMap == null)
            {
                return NotFound();
            }

            return View(replayMap);
        }

        // POST: ReplayMaps/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var replayMap = await _context.ReplayMap.FindAsync(id);
            _context.ReplayMap.Remove(replayMap);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ReplayMapExists(int id)
        {
            return _context.ReplayMap.Any(e => e.ReplayMapID == id);
        }
    }
}
