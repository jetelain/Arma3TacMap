using System;
using System.Linq;
using System.Threading.Tasks;
using Arma3TacMapWebApp.Entities;
using Arma3TacMapWebApp.Maps;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Arma3TacMapWebApp.Controllers
{
    public class OrbatsController : Controller
    {
        private readonly Arma3TacMapContext _context;
        private readonly MapService _mapSvc;
        private readonly IAuthorizationService _authorizationService;

        public OrbatsController(Arma3TacMapContext context, MapService mapSvc, IAuthorizationService authorizationService)
        {
            _context = context;
            _mapSvc = mapSvc;
            _authorizationService = authorizationService;
        }

        // GET: Orbats
        public async Task<IActionResult> Index()
        {
            var user = await _mapSvc.GetUser(User);
            var userId = user?.UserID;
            ViewBag.UserId = userId;
            return View(await _context.Orbats
                .Where(o => o.OwnerUserID == userId || o.Visibility != OrbatVisibility.Default)
                .Include(o => o.Owner)
                .OrderBy(o => o.Visibility)
                .ThenByDescending(o => o.Created).ToListAsync());
        }

        // GET: Orbats/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            var user = await _mapSvc.GetUser(User);
            var orbat = await _context.Orbats
                .Include(o => o.Owner)
                .FirstOrDefaultAsync(m => m.OrbatID == id);
            if (orbat == null)
            {
                return NotFound();
            }
            if (orbat.Visibility == OrbatVisibility.Default && user?.UserID != orbat.OwnerUserID)
            {
                return Forbid();
            }
            ViewBag.CanEdit = await IsEditAllowed(user, orbat);

            orbat.Units = await _context.OrbatUnits
                .Include(u => u.Parent)
                .Where(u => u.OrbatID == id)
                .ToListAsync();

            orbat.Units = MapService.SortAndSetLevel(orbat.Units);

            return View(orbat);
        }



        // GET: Orbats/Create
        [Authorize(Policy = "LoggedUser")]
        public IActionResult Create()
        {
            return View();
        }

        // POST: Orbats/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [Authorize(Policy = "LoggedUser")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Label,Visibility")] Orbat orbat)
        {
            if (orbat.Visibility != OrbatVisibility.Default && !await IsUserAdmin())
            {
                return Forbid();
            }
            if (ModelState.IsValid)
            {
                orbat.OwnerUserID = (await _mapSvc.GetUser(User)).UserID;
                orbat.Created = DateTime.UtcNow;
                orbat.Token = MapService.GenerateToken();
                _context.Add(orbat);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(orbat);
        }
        private async Task<bool> IsEditAllowed(Orbat orbat)
        {
            return await IsEditAllowed((await _mapSvc.GetUser(User)), orbat);
        }

        private async Task<bool> IsEditAllowed(User? user, Orbat orbat)
        {
            if (user == null)
            {
                return false;
            }
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

        // GET: Orbats/Edit/5
        [Authorize(Policy = "LoggedUser")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            var orbat = await _context.Orbats.FindAsync(id);
            if (orbat == null)
            {
                return NotFound();
            }
            if (!await IsEditAllowed(orbat))
            {
                return Forbid();
            }
            return View(orbat);
        }

        // POST: Orbats/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [Authorize(Policy = "LoggedUser")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("OrbatID,Label,Visibility")] Orbat orbat)
        {
            if (id != orbat.OrbatID)
            {
                return NotFound();
            }
            var previous = await _context.Orbats.FindAsync(id);
            if (previous == null)
            {
                return NotFound();
            }
            if (!await IsEditAllowed(previous))
            {
                return Forbid();
            }
            if (orbat.Visibility != OrbatVisibility.Default && !await IsUserAdmin())
            {
                return Forbid();
            }
            if (ModelState.IsValid)
            {
                try
                {
                    previous.Label = orbat.Label;
                    previous.Visibility = orbat.Visibility;
                    _context.Update(previous);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!OrbatExists(orbat.OrbatID))
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
            return View(orbat);
        }

        [Authorize(Policy = "LoggedUser")]
        // GET: Orbats/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var orbat = await _context.Orbats
                .Include(o => o.Owner)
                .FirstOrDefaultAsync(m => m.OrbatID == id);
            if (orbat == null)
            {
                return NotFound();
            }
            if (!await IsEditAllowed(orbat))
            {
                return Forbid();
            }
            return View(orbat);
        }

        // POST: Orbats/Delete/5
        [Authorize(Policy = "LoggedUser")]
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var orbat = await _context.Orbats.FindAsync(id);
            if (orbat == null)
            {
                return NotFound();
            }
            if (!await IsEditAllowed(orbat))
            {
                return Forbid();
            }
            _context.Orbats.Remove(orbat);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool OrbatExists(int id)
        {
            return _context.Orbats.Any(e => e.OrbatID == id);
        }
    }
}
