using System.Linq;
using System.Threading.Tasks;
using Arma3TacMapWebApp.Entities;
using Arma3TacMapWebApp.Maps;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace Arma3TacMapWebApp.Controllers
{
    public class MessageFieldTemplatesController : Controller
    {
        private readonly Arma3TacMapContext _context;
        private readonly MapService _mapSvc;

        public MessageFieldTemplatesController(Arma3TacMapContext context, MapService mapSvc)
        {
            _context = context;
            _mapSvc = mapSvc;
        }
        private async Task<bool> IsEditAllowed(MessageFieldTemplate messageFieldTemplate)
        {
            var user = await _mapSvc.GetUser(User);
            if (messageFieldTemplate.MessageLineTemplate == null)
            {
                messageFieldTemplate.MessageLineTemplate = await _context.MessageLineTemplate
                        .Include(l => l.MessageTemplate)
                        .FirstOrDefaultAsync(l => l.MessageLineTemplateID == messageFieldTemplate.MessageLineTemplateID);
            }
            if (user == null || messageFieldTemplate.MessageLineTemplate == null || messageFieldTemplate.MessageLineTemplate.MessageTemplate!.OwnerUserID != user.UserID)
            {
                return false;
            }
            return true;
        }

        // GET: MessageFieldTemplates/Create
        [Authorize(Policy = "LoggedUser")]
        public async Task<IActionResult> Create(int messageLineTemplateID)
        {
            var messageFieldTemplate = new MessageFieldTemplate()
            {
                MessageLineTemplateID = messageLineTemplateID,
                SortNumber = (await _context.MessageFieldTemplate
                    .Where(f => f.MessageLineTemplateID == messageLineTemplateID)
                    .Select(f => f.SortNumber)
                    .ToListAsync())
                    .DefaultIfEmpty(0)
                    .Max() + 1
            };
            if (!await IsEditAllowed(messageFieldTemplate))
            {
                return Forbid();
            }
            return View(messageFieldTemplate);
        }

        // POST: MessageFieldTemplates/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Policy = "LoggedUser")]
        public async Task<IActionResult> Create([Bind("MessageFieldTemplateID,MessageLineTemplateID,SortNumber,Title,Description,Type")] MessageFieldTemplate messageFieldTemplate)
        {
            if (!await IsEditAllowed(messageFieldTemplate))
            {
                return Forbid();
            }
            if (ModelState.IsValid)
            {
                _context.Add(messageFieldTemplate);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(MessageLineTemplatesController.Details), "MessageLineTemplates", new { id = messageFieldTemplate.MessageLineTemplateID });
            }
            return View(messageFieldTemplate);
        }

        // GET: MessageFieldTemplates/Edit/5
        [Authorize(Policy = "LoggedUser")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var messageFieldTemplate = await _context.MessageFieldTemplate.FindAsync(id);
            if (messageFieldTemplate == null)
            {
                return NotFound();
            }
            if (!await IsEditAllowed(messageFieldTemplate))
            {
                return Forbid();
            }
            return View(messageFieldTemplate);
        }

        // POST: MessageFieldTemplates/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Policy = "LoggedUser")]
        public async Task<IActionResult> Edit(int id, [Bind("MessageFieldTemplateID,MessageLineTemplateID,SortNumber,Title,Description,Type")] MessageFieldTemplate messageFieldTemplate)
        {
            if (id != messageFieldTemplate.MessageFieldTemplateID)
            {
                return NotFound();
            }
            var existing = _context.MessageFieldTemplate.AsNoTracking()
                .FirstOrDefault(m => m.MessageFieldTemplateID == id);
            if (existing == null)
            {
                return NotFound();
            }
            messageFieldTemplate.MessageLineTemplateID = existing.MessageLineTemplateID;
            if (!await IsEditAllowed(messageFieldTemplate))
            {
                return Forbid();
            }
            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(messageFieldTemplate);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!MessageFieldTemplateExists(messageFieldTemplate.MessageFieldTemplateID))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(MessageLineTemplatesController.Details), "MessageLineTemplates", new { id = messageFieldTemplate.MessageLineTemplateID });
            }
            return View(messageFieldTemplate);
        }

        // GET: MessageFieldTemplates/Delete/5
        [Authorize(Policy = "LoggedUser")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var messageFieldTemplate = await _context.MessageFieldTemplate
                .Include(m => m.MessageLineTemplate)
                .Include(m => m.MessageLineTemplate!.MessageTemplate)
                .FirstOrDefaultAsync(m => m.MessageFieldTemplateID == id);
            if (messageFieldTemplate == null)
            {
                return NotFound();
            }

            if (!await IsEditAllowed(messageFieldTemplate))
            {
                return Forbid();
            }

            return View(messageFieldTemplate);
        }

        // POST: MessageFieldTemplates/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Policy = "LoggedUser")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var messageFieldTemplate = await _context.MessageFieldTemplate
                .Include(m => m.MessageLineTemplate)
                .Include(m => m.MessageLineTemplate!.MessageTemplate)
                .FirstOrDefaultAsync(m => m.MessageFieldTemplateID == id);
            if (messageFieldTemplate != null)
            {
                if (!await IsEditAllowed(messageFieldTemplate))
                {
                    return Forbid();
                }
                _context.MessageFieldTemplate.Remove(messageFieldTemplate);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(MessageLineTemplatesController.Details), "MessageLineTemplates", new { id = messageFieldTemplate.MessageLineTemplateID });
            }
            return RedirectToAction(nameof(MessageTemplatesController.Index), "MessageTemplates");
        }

        private bool MessageFieldTemplateExists(int id)
        {
            return _context.MessageFieldTemplate.Any(e => e.MessageFieldTemplateID == id);
        }
    }
}
