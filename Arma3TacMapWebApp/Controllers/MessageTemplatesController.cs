using System.Linq;
using System.Threading.Tasks;
using Arma3TacMapWebApp.Entities;
using Arma3TacMapWebApp.Maps;
using Arma3TacMapWebApp.Migrations;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Arma3TacMapWebApp.Controllers
{
    public class MessageTemplatesController : Controller
    {
        private readonly Arma3TacMapContext _context;
        private readonly MapService _mapSvc;
        private readonly IAuthorizationService _authorizationService;

        public MessageTemplatesController(Arma3TacMapContext context, MapService mapSvc, IAuthorizationService authorizationService)
        {
            _context = context;
            _mapSvc = mapSvc;
            _authorizationService = authorizationService;
        }

        // GET: MessageTemplates
        public async Task<IActionResult> Index()
        {
            var userId = (await _mapSvc.GetUser(User))?.UserID;
            
            ViewBag.UserId = userId;

            var arma3TacMapContext = _context.MessageTemplate
                .Include(m => m.Owner)
                .Where(m => m.OwnerUserID == userId || m.Visibility == OrbatVisibility.Public)
                .OrderBy(e => e.Visibility).ThenBy(e => e.Label);

            return View(await arma3TacMapContext.ToListAsync());
        }

        // GET: MessageTemplates/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var messageTemplate = await _context.MessageTemplate
                .Include(m => m.Owner)
                .FirstOrDefaultAsync(m => m.MessageTemplateID == id);
            if (messageTemplate == null)
            {
                return NotFound();
            }
            ViewBag.CanEdit = await IsEditAllowed(messageTemplate);

            messageTemplate.Lines = await _context.MessageLineTemplate
                .Where(l => l.MessageTemplateID == id)
                .OrderBy(l => l.SortNumber)
                .ToListAsync();

            var fields = await _context.MessageFieldTemplate
                .Where(f => f.MessageLineTemplate!.MessageTemplateID == id)
                .OrderBy(f => f.MessageLineTemplateID).ThenBy(l => l.SortNumber)
                .ToListAsync();

            foreach (var line in messageTemplate.Lines)
            {
                line.Fields = fields.Where(f => f.MessageLineTemplateID == line.MessageLineTemplateID).ToList();
            }

            return View(messageTemplate);
        }

        // GET: MessageTemplates/Create
        [Authorize(Policy = "LoggedUser")]
        public IActionResult Create()
        {
            return View();
        }

        // POST: MessageTemplates/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Policy = "LoggedUser")]
        public async Task<IActionResult> Create([Bind("MessageTemplateID,OwnerUserID,Created,Label,Description,Visibility,Type,CountryCode,Token")] MessageTemplate messageTemplate)
        {
            if (messageTemplate.Visibility != OrbatVisibility.Default && !await IsUserAdmin())
            {
                return Forbid();
            }
            messageTemplate.Owner = await _mapSvc.GetUser(User);
            if (messageTemplate.Owner == null)
            {
                return Forbid();
            }
            messageTemplate.OwnerUserID = messageTemplate.Owner.UserID;
            if (ModelState.IsValid)
            {
                messageTemplate.Token = MapService.GenerateToken();
                _context.Add(messageTemplate);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(messageTemplate);
        }

        private async Task<bool> IsEditAllowed(MessageTemplate messageTemplate)
        {
            var user = await _mapSvc.GetUser(User);
            if (user == null || messageTemplate.OwnerUserID != user.UserID)
            {
                return false;
            }
            return true;
        }

        // GET: MessageTemplates/Edit/5
        [Authorize(Policy = "LoggedUser")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var messageTemplate = await _context.MessageTemplate.FindAsync(id);
            if (messageTemplate == null)
            {
                return NotFound();
            }
            if (!await IsEditAllowed(messageTemplate))
            {
                return Forbid();
            }
            return View(messageTemplate);
        }

        // POST: MessageTemplates/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Policy = "LoggedUser")]
        public async Task<IActionResult> Edit(int id, [Bind("MessageTemplateID,OwnerUserID,Created,Label,Description,Visibility,Type,CountryCode,Token")] MessageTemplate messageTemplate)
        {
            if (id != messageTemplate.MessageTemplateID)
            {
                return NotFound();
            }
            var existing = await _context.MessageTemplate.FindAsync(id);
            if (existing == null) 
            {
                return NotFound();
            }
            if (!await IsEditAllowed(existing))
            {
                return Forbid();
            }
            messageTemplate.OwnerUserID = existing.OwnerUserID;
            messageTemplate.Owner = existing.Owner;
            messageTemplate.Token = existing.Token;
            if (messageTemplate.Visibility != OrbatVisibility.Default && !await IsUserAdmin())
            {
                return Forbid();
            }
            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(messageTemplate);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!MessageTemplateExists(messageTemplate.MessageTemplateID))
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
            return View(messageTemplate);
        }

        // GET: MessageTemplates/Delete/5
        [Authorize(Policy = "LoggedUser")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var messageTemplate = await _context.MessageTemplate
                .Include(m => m.Owner)
                .FirstOrDefaultAsync(m => m.MessageTemplateID == id);
            if (messageTemplate == null)
            {
                return NotFound();
            }
            if (!await IsEditAllowed(messageTemplate))
            {
                return Forbid();
            }

            return View(messageTemplate);
        }

        // POST: MessageTemplates/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Policy = "LoggedUser")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var messageTemplate = await _context.MessageTemplate.FindAsync(id);
            if (messageTemplate != null)
            {
                if (!await IsEditAllowed(messageTemplate))
                {
                    return Forbid();
                }
                _context.MessageTemplate.Remove(messageTemplate);
                await _context.SaveChangesAsync();
            }

            return RedirectToAction(nameof(Index));
        }

        private bool MessageTemplateExists(int id)
        {
            return _context.MessageTemplate.Any(e => e.MessageTemplateID == id);
        }
        private async Task<bool> IsUserAdmin()
        {
            return (await _authorizationService.AuthorizeAsync(User, "Admin")).Succeeded;
        }
    }
}
