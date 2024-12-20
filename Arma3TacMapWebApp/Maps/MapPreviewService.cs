﻿using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Arma3TacMapWebApp.Controllers;
using Arma3TacMapWebApp.Entities;
using Arma3TacMapWebApp.Services.Screenshots;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.EntityFrameworkCore;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Processing;

namespace Arma3TacMapWebApp.Maps
{
    public class MapPreviewService : IMapPreviewService
    {
        private readonly Arma3TacMapContext _db;
        private readonly Arma3TacMapPreviewContext _pdb;
        private readonly LinkGenerator _linkGenerator;
        private readonly IHttpContextAccessor _accessor;
        private readonly IScreenshotService _screenshot;

        public static int[] ValidSizesStatic = new[] { 256, 512, 1024, 2048 };

        public MapPreviewService(Arma3TacMapContext db, Arma3TacMapPreviewContext pdb, IHttpContextAccessor accessor, LinkGenerator linkGenerator, IScreenshotService screenshot)
        {
            _db = db;
            _pdb = pdb;
            _linkGenerator = linkGenerator; 
            _accessor = accessor;
            _screenshot = screenshot;
        }
        public int[] ValidSizes => ValidSizesStatic;

        public async Task<byte[]> GetPreview(TacMapAccess access, int size, int? phase = null)
        {
            if ( !ValidSizesStatic.Contains(size))
            {
                throw new ArgumentException(nameof(size));
            }

            var lastUpdate = await _db.TacMapMarkers.Where(m => m.TacMapID == access.TacMapID || m.TacMap.ParentTacMapID == access.TacMapID).MaxAsync(m => m.LastUpdate);
            if (lastUpdate == null)
            {
                lastUpdate = (await _db.TacMaps.FindAsync(access.TacMapID)).Created;
            }

            var phaseKey = phase == null ? string.Empty : phase.Value.ToString();

            var preview = await _pdb.TacMapPreviews.AsNoTracking().FirstOrDefaultAsync(p => p.TacMapID == access.TacMapID && p.Size == size && p.PhaseKey == phaseKey && p.LastUpdate >= lastUpdate);
            if (preview != null)
            {
                return preview.Data;
            }

            var bytes = await MakeScreenshot(access, phase);
            if (bytes == null)
            {
                return File.ReadAllBytes(@"wwwroot/img/transparent.png");
            }
            using (var image = Image.Load(bytes))
            {

                using (var transaction = _pdb.Database.BeginTransaction())
                {
                    await AddPreview(access, 2048, lastUpdate, phaseKey, ToJpeg(image, 2048));;
                    await AddPreview(access, 1024, lastUpdate, phaseKey, ToJpeg(image, 1024));
                    await AddPreview(access, 512, lastUpdate, phaseKey, ToPng(image, 512));
                    await AddPreview(access, 256, lastUpdate, phaseKey, ToPng(image, 256));
                    await _pdb.SaveChangesAsync();
                    await transaction.CommitAsync();
                }
            }

            return (await _pdb.TacMapPreviews.AsNoTracking().FirstAsync(p => p.TacMapID == access.TacMapID && p.Size == size)).Data;
        }

        private byte[] ToJpeg(Image image, int size)
        {
            using(var ms = new MemoryStream())
            {
                ToSize(image, size).SaveAsJpeg(ms);
                return ms.ToArray();
            }
        }

        private Image ToSize(Image image, int size)
        {
            if (image.Width != size)
            {
                return image.Clone(i => i.Resize(size, size));
            }
            return image;
        }

        private byte[] ToPng(Image image, int size)
        {
            using (var ms = new MemoryStream())
            {
                ToSize(image, size).SaveAsPng(ms);
                return ms.ToArray();
            }
        }

        private async Task AddPreview(TacMapAccess access, int size, DateTime? lastUpdate, string phaseKey, byte[] data)
        {
            var preview = await _pdb.TacMapPreviews.AsNoTracking().FirstOrDefaultAsync(p => p.TacMapID == access.TacMapID && p.Size == size && p.PhaseKey == phaseKey);
            if (preview == null)
            {
                preview = new TacMapPreview()
                {
                    Data = data,
                    LastUpdate = lastUpdate,
                    TacMapID = access.TacMapID,
                    Size = size,
                    PhaseKey = phaseKey
                };
                _pdb.TacMapPreviews.Add(preview);
            }
            else
            {
                preview.Data = data;
                preview.LastUpdate = lastUpdate;
                _pdb.TacMapPreviews.Update(preview);
            }
        }

        private async Task<byte[]?> MakeScreenshot(TacMapAccess access, int? phase)
        {
            var uri = _linkGenerator.GetUriByAction(_accessor.HttpContext, nameof(HomeController.ViewMapFullStatic), "Home", new { id = access.TacMapID, t = access.TacMap.ReadOnlyToken, phase = phase }, "https");
            return await _screenshot.MakeScreenshotAsync(uri);
        }
    }
}
