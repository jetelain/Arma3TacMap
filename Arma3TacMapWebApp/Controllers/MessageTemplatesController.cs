﻿using System;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using Arma3TacMapLibrary.Arma3;
using Arma3TacMapWebApp.Entities;
using Arma3TacMapWebApp.Maps;
using Arma3TacMapWebApp.Models;
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
                .OrderBy(e => e.Visibility).ThenBy(e => e.Title);

            return View(await arma3TacMapContext.ToListAsync());
        }

        // GET: MessageTemplates/Details/5
        public async Task<IActionResult> Details(int? id, string? t = null, string? format = null)
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
            if (!await IsAccessAllowed(messageTemplate, t))
            {
                return Forbid();
            }

            ViewBag.CanEdit = await IsEditAllowed(messageTemplate);

            await LoadLines(messageTemplate);

            if (format == "json")
            {
                return Json(MessageTemplateExporter.ToJson(messageTemplate, GetUid(messageTemplate), GetHref(messageTemplate)));
            }
            return View(messageTemplate);
        }

        private async Task LoadLines(MessageTemplate messageTemplate)
        {
            messageTemplate.Lines = await _context.MessageLineTemplate
                .Where(l => l.MessageTemplateID == messageTemplate.MessageTemplateID)
                .OrderBy(l => l.SortNumber)
                .ToListAsync();

            var fields = await _context.MessageFieldTemplate
                .Where(f => f.MessageLineTemplate!.MessageTemplateID == messageTemplate.MessageTemplateID)
                .OrderBy(f => f.MessageLineTemplateID).ThenBy(l => l.SortNumber)
                .ToListAsync();

            foreach (var line in messageTemplate.Lines)
            {
                line.Fields = fields.Where(f => f.MessageLineTemplateID == line.MessageLineTemplateID).ToList();
            }
        }

        private string GetUid(MessageTemplate messageTemplate)
        {
            return HttpContext.Request.Host + "#" + messageTemplate.MessageTemplateID;
        }

        private string GetHref(MessageTemplate messageTemplate)
        {
            return Url.Action("Details", "MessageTemplates", new { id = messageTemplate.MessageTemplateID, t = messageTemplate.Token }, HttpContext.Request.Scheme) ?? string.Empty;
        }

        // GET: MessageTemplates/Clone
        [Authorize(Policy = "LoggedUser")]
        public async Task<IActionResult> Clone(int id, string? t = null)
        {
            var user = await _mapSvc.GetUser(User);
            if (user == null)
            {
                return Forbid();
            }

            var messageTemplate = await _context.MessageTemplate
                .FirstOrDefaultAsync(m => m.MessageTemplateID == id);
            if (messageTemplate == null)
            {
                return NotFound();
            }
            if (!await IsAccessAllowed(messageTemplate, t))
            {
                return Forbid();
            }
            return View(new MessageTemplate()
            {
                MessageTemplateID = messageTemplate.MessageTemplateID,
                CountryCode = messageTemplate.CountryCode,
                Title = messageTemplate.Title + " (copy)",
                Description = messageTemplate.Description,
                Visibility = messageTemplate.Visibility,
                Type = messageTemplate.Type,
                OwnerUserID = user.UserID,
                Owner = user,
                Created = DateTime.UtcNow,
            });
        }

        // POST: MessageTemplates/Clone
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Policy = "LoggedUser")]
        public async Task<IActionResult> Clone(int id, [Bind("MessageTemplateID,OwnerUserID,Created,Title,Description,Visibility,Type,CountryCode,Token")] MessageTemplate messageTemplate)
        {
            if (messageTemplate.Visibility != OrbatVisibility.Default && !await IsUserAdmin())
            {
                return Forbid();
            }
            messageTemplate.MessageTemplateID = 0;
            messageTemplate.Owner = await _mapSvc.GetUser(User);
            if (messageTemplate.Owner == null)
            {
                return Forbid();
            }
            messageTemplate.OwnerUserID = messageTemplate.Owner.UserID;
            messageTemplate.Created = DateTime.UtcNow;
            if (ModelState.IsValid)
            {
                var lines = await _context.MessageLineTemplate
                    .AsNoTracking()
                    .Where(l => l.MessageTemplateID == id)
                    .OrderBy(l => l.SortNumber)
                    .ToListAsync();

                var fields = await _context.MessageFieldTemplate
                    .AsNoTracking()
                    .Where(f => f.MessageLineTemplate!.MessageTemplateID == id)
                    .OrderBy(f => f.MessageLineTemplateID).ThenBy(l => l.SortNumber)
                    .ToListAsync();

                foreach (var line in lines)
                {
                    line.Fields = fields.Where(f => f.MessageLineTemplateID == line.MessageLineTemplateID).ToList();
                    line.MessageLineTemplateID = 0;
                    line.MessageTemplate = messageTemplate;
                    line.MessageTemplateID = 0;
                    foreach (var field in line.Fields)
                    {
                        field.MessageFieldTemplateID = 0;
                        field.MessageLineTemplateID = 0;
                        field.MessageLineTemplate = null;
                    }
                }

                messageTemplate.Token = MapService.GenerateToken();
                messageTemplate.Lines = lines;
                _context.Add(messageTemplate);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Details), new { id = messageTemplate.MessageTemplateID });
            }
            return View(messageTemplate);
        }

        // GET: MessageTemplates/Create
        [Authorize(Policy = "LoggedUser")]
        public async Task<IActionResult> Create()
        {
            var user = await _mapSvc.GetUser(User);
            if (user == null)
            {
                return Forbid();
            }
            return View(new MessageTemplate()
            {
                Title = string.Empty,
                OwnerUserID = user.UserID,
                Owner = user,
                Created = DateTime.UtcNow,
            });
        }

        // POST: MessageTemplates/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Policy = "LoggedUser")]
        public async Task<IActionResult> Create([Bind("MessageTemplateID,OwnerUserID,Created,Title,Description,Visibility,Type,CountryCode,Token")] MessageTemplate messageTemplate)
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
            messageTemplate.Created = DateTime.UtcNow;
            if (ModelState.IsValid)
            {
                messageTemplate.Token = MapService.GenerateToken();
                messageTemplate.Lines = [new MessageLineTemplate()
                {
                    Title = messageTemplate.Title,
                    SortNumber = 0
                }];
                _context.Add(messageTemplate);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Details), new { id = messageTemplate.MessageTemplateID });
            }
            return View(messageTemplate);
        }

        // GET: MessageTemplates/Create
        [Authorize(Policy = "LoggedUser")]
        public async Task<IActionResult> Import()
        {
            var user = await _mapSvc.GetUser(User);
            if (user == null)
            {
                return Forbid();
            }
            return View(new MessageTemplateImportViewModel());
        }

        // POST: MessageTemplates/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Policy = "LoggedUser")]
        public async Task<IActionResult> Import(MessageTemplateImportViewModel model)
        {
            if (model.Visibility != OrbatVisibility.Default && !await IsUserAdmin())
            {
                return Forbid();
            }

            if (ModelState.IsValid)
            {
                MessageTemplateJson? messageTemplateJson;
                try
                {
                    messageTemplateJson = JsonSerializer.Deserialize<MessageTemplateJson>(model.Json ?? "null", new JsonSerializerOptions() { PropertyNameCaseInsensitive = true });
                }
                catch (JsonException e)
                {
                    ModelState.AddModelError("Json", $"Invalid JSON: {e.Message}");
                    return View(model);
                }

                if (messageTemplateJson == null)
                {
                    ModelState.AddModelError("Json", "Invalid JSON");
                    return View(model);
                }

                if ( string.IsNullOrEmpty(messageTemplateJson.Title))
                {
                    ModelState.AddModelError("Json", "Title is required");
                    return View(model);
                }

                var messageTemplate = new MessageTemplate()
                {
                    Title = messageTemplateJson.Title,
                    Description = messageTemplateJson.Description,
                    Visibility = model.Visibility,
                    Type = messageTemplateJson.Type,
                    CountryCode = messageTemplateJson.CountryCode,
                    Owner = await _mapSvc.GetUser(User),
                    Created = DateTime.UtcNow,
                    Token = MapService.GenerateToken(),
                    Lines = messageTemplateJson.Lines
                };
                if (messageTemplate.Lines != null)
                {
                    var lineNumber = 0;
                    foreach (var line in messageTemplate.Lines)
                    {
                        line.SortNumber = lineNumber++;
                        if (line.Fields != null)
                        {
                            var fieldNumber = 0;
                            foreach (var field in line.Fields)
                            {
                                field.SortNumber = fieldNumber++;
                            }
                        }
                    }
                }
                _context.Add(messageTemplate);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Details), new { id = messageTemplate.MessageTemplateID });
            }
            return View(model);
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

        private async Task<bool> IsAccessAllowed(MessageTemplate messageTemplate, string? token)
        {
            if ( messageTemplate.Visibility == OrbatVisibility.Public)
            {
                return true;
            }
            if (!string.IsNullOrEmpty(token) && messageTemplate.Token == token)
            {
                return true;
            }
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
        public async Task<IActionResult> Edit(int id, [Bind("MessageTemplateID,OwnerUserID,Created,Title,Description,Visibility,Type,CountryCode,Token")] MessageTemplate messageTemplate)
        {
            if (id != messageTemplate.MessageTemplateID)
            {
                return NotFound();
            }
            var existing = await _context.MessageTemplate.AsNoTracking().FirstOrDefaultAsync(m => m.MessageTemplateID == id);
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
            messageTemplate.Created = existing.Created;
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
                return RedirectToAction(nameof(Details), new { id = messageTemplate.MessageTemplateID });
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

        public async Task<IActionResult> Export(int? id, string? t = null)
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
            if (!await IsAccessAllowed(messageTemplate, t))
            {
                return Forbid();
            }
            ViewBag.CanEdit = await IsEditAllowed(messageTemplate);

            await LoadLines(messageTemplate);

            var uid = GetUid(messageTemplate);

            var href = GetHref(messageTemplate);

            ViewBag.Script = $"{ArmaSerializer.ToSimpleArrayString(MessageTemplateExporter.ToArma3(messageTemplate, uid, href))} call ctab_fnc_registerMessageTemplate;";
            
            ViewBag.Json = MessageTemplateExporter.ToJson(messageTemplate, uid, href);

            return View(messageTemplate);
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
