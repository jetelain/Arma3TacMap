using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Arma3TacMapWebApp.Entities;
using Arma3TacMapWebApp.Maps;
using Microsoft.AspNetCore.Authorization;

namespace Arma3TacMapWebApp.Controllers
{
    [Authorize(Policy = "Admin")]
    //[Authorize(Policy = "LoggedUser")]
    public class OrbatUnitsController : Controller
    {
        private readonly Arma3TacMapContext _context;
        private readonly MapService _mapSvc;
        private readonly IAuthorizationService _authorizationService;

        public OrbatUnitsController(Arma3TacMapContext context, MapService mapSvc, IAuthorizationService authorizationService)
        {
            _context = context;
            _mapSvc = mapSvc;
            _authorizationService = authorizationService;
        }

        private async Task<bool> IsEditAllowed(Orbat orbat)
        {
            return await IsEditAllowed((await _mapSvc.GetUser(User)), orbat);
        }

        private async Task<bool> IsEditAllowed(User user, Orbat orbat)
        {
            if (user.UserID == orbat.OwnerUserID)
            {
                return true;
            }
            return orbat.Visibility != OrbatVisibility.Default && await IsUserAdmin();
        }

        private async Task<bool> IsUserAdmin()
        {
            return (await _authorizationService.AuthorizeAsync(User, "Admin")).Succeeded;
        }

        // GET: OrbatUnits/Create
        public async Task<IActionResult> Create(int orbatID)
        {
            var orbatUnit = new OrbatUnit();
            orbatUnit.OrbatID = orbatID;
            orbatUnit.Orbat = await _context.Orbats.FindAsync(orbatUnit.OrbatID);
            if (orbatUnit.Orbat == null)
            {
                return NotFound();
            }
            if (!await IsEditAllowed(orbatUnit.Orbat))
            {
                return Forbid();
            }
            await PrepareDropdownList(orbatUnit); 
            return View(orbatUnit);
        }

        // POST: OrbatUnits/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("OrbatUnitID,OrbatID,ParentOrbatUnitID,Name,UniqueDesignation,NatoSymbolIcon,NatoSymbolMod1,NatoSymbolMod2,NatoSymbolSize,NatoSymbolHQ,NatoSymbolFriendlyImageBase64,NatoSymbolHostileImageBase64,NatoSymbolHostileAssumedImageBase64,Position")] OrbatUnit orbatUnit)
        {
            orbatUnit.Orbat = await _context.Orbats.FindAsync(orbatUnit.OrbatID);
            if (orbatUnit.Orbat == null)
            {
                return NotFound();
            }
            if (!await IsEditAllowed(orbatUnit.Orbat))
            {
                return Forbid();
            }
            if (ModelState.IsValid)
            {
                orbatUnit.Trigram = await GetTrigam(orbatUnit.OrbatID);
                _context.Add(orbatUnit);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(OrbatsController.Details), "Orbats", new { id = orbatUnit.OrbatID });
            }
            await PrepareDropdownList(orbatUnit);
            return View(orbatUnit);
        }

        private async Task<string> GetTrigam(int orbatID)
        {
            var rnd = new Random();
            string trigram;
            do
            {
                trigram = new string(Enumerable.Range(0, 3).Select(_ => (char)('A' + rnd.Next(0, 'Z' - 'A'))).ToArray());
            } while (await _context.OrbatUnits.AnyAsync(u => u.OrbatID == orbatID && u.Trigram == trigram));
            return trigram;
        }

        private async Task PrepareDropdownList(OrbatUnit orbatUnit)
        {
            var units = MapService.SortAndSetLevel(await _context.OrbatUnits.Where(o => o.OrbatID == orbatUnit.OrbatID).ToListAsync());
            ViewData["ParentOrbatUnitID"] = new SelectList(units.Where(u => !u.IsSelfOrParent(orbatUnit)), "OrbatUnitID", "UniqueDesignation", orbatUnit.ParentOrbatUnitID);
        }

        // GET: OrbatUnits/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var orbatUnit = await _context.OrbatUnits
                .Include(o => o.Orbat)
                .Include(o => o.Parent)
                .FirstOrDefaultAsync(m => m.OrbatUnitID == id);
            if (orbatUnit == null)
            {
                return NotFound();
            }
            if (!await IsEditAllowed(orbatUnit.Orbat))
            {
                return Forbid();
            }
            await PrepareDropdownList(orbatUnit);
            return View(orbatUnit);
        }

        // POST: OrbatUnits/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("OrbatUnitID,OrbatID,ParentOrbatUnitID,Name,UniqueDesignation,NatoSymbolIcon,NatoSymbolMod1,NatoSymbolMod2,NatoSymbolSize,NatoSymbolHQ,NatoSymbolFriendlyImageBase64,NatoSymbolHostileImageBase64,NatoSymbolHostileAssumedImageBase64,Position,Trigram")] OrbatUnit orbatUnit)
        {
            if (id != orbatUnit.OrbatUnitID)
            {
                return NotFound();
            }
            orbatUnit.Orbat = await _context.Orbats.FindAsync(orbatUnit.OrbatID);
            if (orbatUnit.Orbat == null)
            {
                return NotFound();
            }
            if (!await IsEditAllowed(orbatUnit.Orbat))
            {
                return Forbid();
            }

            if (orbatUnit.ParentOrbatUnitID != null)
            {
                await LoadParents(orbatUnit);
                if (orbatUnit.Parent.IsSelfOrParent(orbatUnit))
                {
                    return Forbid();
                }
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(orbatUnit);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!OrbatUnitExists(orbatUnit.OrbatUnitID))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(OrbatsController.Details), "Orbats", new { id = orbatUnit.OrbatID } );
            }
            await PrepareDropdownList(orbatUnit);
            return View(orbatUnit);
        }

        private async Task LoadParents(OrbatUnit unit)
        {
            await LoadParents(unit, new HashSet<int>());
        }

        private async Task LoadParents(OrbatUnit unit, HashSet<int> seen)
        {
            seen.Add(unit.OrbatUnitID);
            if (unit.ParentOrbatUnitID != null && !seen.Contains(unit.ParentOrbatUnitID.Value))
            {
                unit.Parent = await _context.OrbatUnits.FindAsync(unit.ParentOrbatUnitID);
                await LoadParents(unit.Parent, seen);
            }
        }

        // GET: OrbatUnits/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var orbatUnit = await _context.OrbatUnits
                .Include(o => o.Orbat)
                .Include(o => o.Parent)
                .FirstOrDefaultAsync(m => m.OrbatUnitID == id);
            if (orbatUnit == null)
            {
                return NotFound();
            }
            if (!await IsEditAllowed(orbatUnit.Orbat))
            {
                return Forbid();
            }
            return View(orbatUnit);
        }

        // POST: OrbatUnits/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var orbatUnit = await _context.OrbatUnits
                .Include(o => o.Orbat)
                .FirstOrDefaultAsync(m => m.OrbatUnitID == id); 
            if (!await IsEditAllowed(orbatUnit.Orbat))
            {
                return Forbid();
            }
            _context.OrbatUnits.Remove(orbatUnit);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(OrbatsController.Details), "Orbats", new { id = orbatUnit.OrbatID });
        }

        private bool OrbatUnitExists(int id)
        {
            return _context.OrbatUnits.Any(e => e.OrbatUnitID == id);
        }
    }
}
