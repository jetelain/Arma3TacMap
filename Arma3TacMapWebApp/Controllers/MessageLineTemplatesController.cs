﻿using System.Linq;
using System.Threading.Tasks;
using Arma3TacMapWebApp.Entities;
using Arma3TacMapWebApp.Maps;
using Arma3TacMapWebApp.Migrations;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.Identity.Client;

namespace Arma3TacMapWebApp.Controllers
{
    public class MessageLineTemplatesController : Controller
    {
        private readonly Arma3TacMapContext _context;
        private readonly MapService _mapSvc;

        public MessageLineTemplatesController(Arma3TacMapContext context, MapService mapSvc)
        {
            _context = context;
            _mapSvc = mapSvc;
        }

        // GET: MessageLineTemplates/Details/5
        public async Task<IActionResult> Details(int? id, string? t = null)
        {
            if (id == null)
            {
                return NotFound();
            }

            var messageLineTemplate = await _context.MessageLineTemplate
                .Include(m => m.MessageTemplate)
                .FirstOrDefaultAsync(m => m.MessageLineTemplateID == id);
            if (messageLineTemplate == null)
            {
                return NotFound();
            }
            if (!await IsAccessAllowed(messageLineTemplate, t))
            {
                return Forbid();
            }

            ViewBag.CanEdit = await IsEditAllowed(messageLineTemplate);

            messageLineTemplate.Fields = await _context.MessageFieldTemplate
                .Where(f => f.MessageLineTemplateID == id)
                .OrderBy(f => f.SortNumber)
                .ToListAsync();

            return View(messageLineTemplate);
        }
        private async Task<bool> IsEditAllowed(MessageLineTemplate messageLineTemplate)
        {
            var user = await _mapSvc.GetUser(User);
            if (messageLineTemplate.MessageTemplate == null)
            {
                messageLineTemplate.MessageTemplate = await _context.MessageTemplate
                        .FirstOrDefaultAsync(l => l.MessageTemplateID == messageLineTemplate.MessageTemplateID);
            }
            if (user == null || messageLineTemplate.MessageTemplate == null || messageLineTemplate.MessageTemplate.OwnerUserID != user.UserID)
            {
                return false;
            }
            return true;
        }

        private async Task<bool> IsAccessAllowed(MessageLineTemplate messageLineTemplate, string? token)
        {
            if (messageLineTemplate.MessageTemplate == null)
            {
                messageLineTemplate.MessageTemplate = await _context.MessageTemplate
                        .FirstOrDefaultAsync(l => l.MessageTemplateID == messageLineTemplate.MessageTemplateID);
            }
            if (messageLineTemplate.MessageTemplate!.Visibility == OrbatVisibility.Public)
            {
                return true;
            }
            if (!string.IsNullOrEmpty(token) && messageLineTemplate.MessageTemplate!.Token == token)
            {
                return true;
            }
            var user = await _mapSvc.GetUser(User);
            if (user == null || messageLineTemplate.MessageTemplate!.OwnerUserID != user.UserID)
            {
                return false;
            }
            return true;
        }

        // GET: MessageLineTemplates/Create
        [Authorize(Policy = "LoggedUser")]
        public async Task<IActionResult> Create(int messageTemplateID)
        {
            var sortNumber = (await _context.MessageLineTemplate
                    .Where(l => l.MessageTemplateID == messageTemplateID)
                    .Select(l => l.SortNumber)
                    .ToListAsync())
                    .DefaultIfEmpty(0)
                    .Max() + 1;
            var messageLineTemplate = new MessageLineTemplate()
            {
                MessageTemplateID = messageTemplateID,
                SortNumber = sortNumber,
                Title = $"Line {sortNumber}"
            };
            if (!await IsEditAllowed(messageLineTemplate)) 
            {
                return Forbid(); 
            }
            return View(messageLineTemplate);
        }

        // POST: MessageLineTemplates/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Policy = "LoggedUser")]
        public async Task<IActionResult> Create([Bind("MessageLineTemplateID,MessageTemplateID,SortNumber,Title,Description")] MessageLineTemplate messageLineTemplate)
        {
            if (!await IsEditAllowed(messageLineTemplate))
            {
                return Forbid();
            }
            if (ModelState.IsValid)
            {
                _context.Add(messageLineTemplate);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Details), new { id = messageLineTemplate.MessageLineTemplateID });
            }
            return View(messageLineTemplate);
        }

        // GET: MessageLineTemplates/Edit/5
        [Authorize(Policy = "LoggedUser")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var messageLineTemplate = await _context.MessageLineTemplate.FindAsync(id);
            if (messageLineTemplate == null)
            {
                return NotFound();
            }
            if (!await IsEditAllowed(messageLineTemplate))
            {
                return Forbid();
            }
            return View(messageLineTemplate);
        }

        // POST: MessageLineTemplates/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Policy = "LoggedUser")]
        public async Task<IActionResult> Edit(int id, [Bind("MessageLineTemplateID,MessageTemplateID,SortNumber,Title,Description")] MessageLineTemplate messageLineTemplate)
        {
            if (id != messageLineTemplate.MessageLineTemplateID)
            {
                return NotFound();
            }

            var existing = _context.MessageLineTemplate.AsNoTracking().FirstOrDefault(l => l.MessageLineTemplateID == id);
            if (existing == null)
            {
                return NotFound();
            }
            messageLineTemplate.MessageTemplateID = existing.MessageTemplateID;
            if (!await IsEditAllowed(messageLineTemplate))
            {
                return Forbid();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(messageLineTemplate);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!MessageLineTemplateExists(messageLineTemplate.MessageLineTemplateID))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Details), new { id = messageLineTemplate.MessageLineTemplateID });
            }
            return View(messageLineTemplate);
        }

        // GET: MessageLineTemplates/Delete/5
        [Authorize(Policy = "LoggedUser")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var messageLineTemplate = await _context.MessageLineTemplate
                .Include(m => m.MessageTemplate)
                .FirstOrDefaultAsync(m => m.MessageLineTemplateID == id);
            if (messageLineTemplate == null)
            {
                return NotFound();
            }
            if (!await IsEditAllowed(messageLineTemplate))
            {
                return Forbid();
            }

            return View(messageLineTemplate);
        }

        // POST: MessageLineTemplates/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Policy = "LoggedUser")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var messageLineTemplate = await _context.MessageLineTemplate
                           .Include(m => m.MessageTemplate)
                           .FirstOrDefaultAsync(m => m.MessageLineTemplateID == id);
            if (messageLineTemplate != null)
            {
                if (!await IsEditAllowed(messageLineTemplate))
                {
                    return Forbid();
                }
                _context.MessageLineTemplate.Remove(messageLineTemplate);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(MessageTemplatesController.Details), "MessageTemplates", new { id = messageLineTemplate.MessageTemplateID });
            }

            return RedirectToAction(nameof(MessageTemplatesController.Index), "MessageTemplates");
        }

        private bool MessageLineTemplateExists(int id)
        {
            return _context.MessageLineTemplate.Any(e => e.MessageLineTemplateID == id);
        }
    }
}
