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
    [Authorize(Policy = "Admin")]
    public class AdminUsersController : Controller
    {
        private readonly Arma3TacMapContext _context;

        public AdminUsersController(Arma3TacMapContext context)
        {
            _context = context;
        }

        // GET: AdminUsers
        public async Task<IActionResult> Index()
        {
            return View(await _context.Users.ToListAsync());
        }

        // GET: AdminUsers/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var user = await _context.Users
                .Include(u => u.ApiKeys)
                .FirstOrDefaultAsync(m => m.UserID == id);
            if (user == null)
            {
                return NotFound();
            }

            return View(user);
        }

        // GET: AdminUsers/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: AdminUsers/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("UserLabel")] User user)
        {
            if (ModelState.IsValid)
            {
                user.IsService = true;
                _context.Add(user);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(user);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateApiKey(int id)
        {
            var user = await _context.Users
                .Include(u => u.ApiKeys)
                .FirstOrDefaultAsync(m => m.UserID == id);
            if (user == null)
            {
                return NotFound();
            }
            var token = MapService.GenerateToken();
            var key = new UserApiKey();
            key.User = user;
            key.ValidUntil = DateTime.UtcNow.AddYears(1);
            key.SetToken(token);
            _context.Add(key);
            await _context.SaveChangesAsync();
            ViewData["CreatedKey"] = $"{key.UserApiKeyID:X}:{token}";
            return View("Details", user);
        }
        
        // GET: AdminUsers/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var user = await _context.Users.FindAsync(id);
            if (user == null)
            {
                return NotFound();
            }
            return View(user);
        }

        // POST: AdminUsers/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("UserID,UserLabel,SteamId,IsService")] User user)
        {
            if (id != user.UserID)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(user);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!UserExists(user.UserID))
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
            return View(user);
        }

        // GET: AdminUsers/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var user = await _context.Users
                .FirstOrDefaultAsync(m => m.UserID == id);
            if (user == null)
            {
                return NotFound();
            }

            return View(user);
        }

        // POST: AdminUsers/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var user = await _context.Users.FindAsync(id);
            _context.Users.Remove(user);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }


        public async Task<IActionResult> DeleteApiKey(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            var apiKey = await _context.UserApiKeys
                .Include(u => u.User)
                .FirstOrDefaultAsync(m => m.UserApiKeyID == id);
            if (apiKey == null)
            {
                return NotFound();
            }
            return View(apiKey);
        }

        [HttpPost, ActionName("DeleteApiKey")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteApiKeyConfirmed(int id)
        {
            var apiKey = await _context.UserApiKeys.FindAsync(id);
            _context.UserApiKeys.Remove(apiKey);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Details), new { id = apiKey.UserID });
        }
        

        private bool UserExists(int id)
        {
            return _context.Users.Any(e => e.UserID == id);
        }
    }
}
