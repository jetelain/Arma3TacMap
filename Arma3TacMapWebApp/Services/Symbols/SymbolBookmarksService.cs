using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Arma3TacMapWebApp.Controllers.Converters;
using Arma3TacMapWebApp.Entities;
using Arma3TacMapWebApp.Maps;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using Pmad.Milsymbol.AspNetCore.SymbolSelector.Bookmarks;

namespace Arma3TacMapWebApp.Services.Symbols
{
    public class SymbolBookmarksService : ISymbolBookmarksService
    {
        private readonly Arma3TacMapContext _context;
        private readonly IAuthorizationService _authorizationService;
        private readonly MapService _mapService;

        public SymbolBookmarksService(Arma3TacMapContext context, IAuthorizationService authorizationService, MapService mapService)
        {
            _context = context;
            _authorizationService = authorizationService;
            _mapService = mapService;
        }

        public async Task<bool> CanUseBookmarksAsync(ClaimsPrincipal user)
        {
            return (await _authorizationService.AuthorizeAsync(user, "LoggedUser")).Succeeded;
        }

        public async Task<SymbolBookmarks> GetBookmarksAsync(ClaimsPrincipal user)
        {
            var dbuser = await _mapService.GetUser(user);
            if (dbuser == null)
            {
                return new SymbolBookmarks() 
                { 
                    Bookmarks = new List<SymbolBookmark>(), 
                    LastModifiedUtc = DateTime.MinValue.ToUniversalTime() 
                };
            }
            var items = await _context.SymbolBookmarks.Where(s => s.UserID == dbuser.UserID).ToListAsync();
            return new SymbolBookmarks()
            {
                Bookmarks = items.Select(i => new SymbolBookmark() { Sidc = i.Sidc, Label = i.Label }).ToList(),
                LastModifiedUtc = DateTimeAssumeUniversal.AssumeUniversal(dbuser.LastSymbolBookmarksSaveUtc) ?? DateTime.MinValue.ToUniversalTime()
            };
        }

        public async Task SaveBookmarksAsync(ClaimsPrincipal user, List<string> bookmarks)
        {
            var dbuser = await _mapService.GetOrCreateUser(user);
            if (dbuser == null)
            {
                return;
            }
            var existing = await _context.SymbolBookmarks.Where(s => s.UserID == dbuser.UserID).ToListAsync();

            // Remove existing bookmarks that are not in the new list
            _context.SymbolBookmarks.RemoveRange(existing.Where(e => !bookmarks.Contains(e.Sidc)));

            // Add new bookmarks that are not in the existing list
            foreach (var bookmark in bookmarks)
            {
                if (!existing.Any(e => e.Sidc == bookmark))
                {
                    _context.SymbolBookmarks.Add(new UserSymbolBookmark
                    {
                        UserID = dbuser.UserID,
                        Sidc = bookmark
                    });
                }
            }

            dbuser.LastSymbolBookmarksSaveUtc = DateTime.UtcNow;
            _context.Update(dbuser);
            await _context.SaveChangesAsync();
        }
    }
}
