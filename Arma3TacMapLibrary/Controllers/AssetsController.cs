using System.IO;
using Arma3TacMapLibrary.Arma3;
using Microsoft.AspNetCore.Mvc;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;

namespace Arma3TacMapLibrary.Controllers
{
    public class AssetsController : Controller
    {
        [HttpGet("/img/markers/{color}/{marker}.png")]
        [ResponseCache(Duration = 1440)]
        public IActionResult Arma3Marker(Arma3MarkerColor color, Arma3MarkerType marker)
        {
            if (color == Arma3MarkerColor.ColorWhite || marker >= Arma3MarkerType.flag_aaf)
            {
                return RedirectPermanent($"/img/markers/{marker}.png");
            }
            var targetColor = color.ToColor();

            using (var file = typeof(AssetsController).Assembly.GetManifestResourceStream($"Arma3TacMapLibrary.wwwroot.img.markers.{marker}.png"))
            {
                using (var img = Image.Load<Rgba32>(file))
                {
                    for (int x = 0; x < img.Width; ++x)
                    {
                        for (int y = 0; y < img.Height; ++y)
                        {
                            var pixel = img[x, y];
                            img[x, y] = new Rgba32((byte)(pixel.R * targetColor[0]), (byte)(pixel.G * targetColor[1]), (byte)(pixel.B * targetColor[2]), pixel.A);
                        }
                    }
                    using (var stream = new MemoryStream())
                    {
                        img.SaveAsPng(stream);
                        return File(stream.ToArray(), "image/png");
                    }
                }
            }
        }
    }
}
