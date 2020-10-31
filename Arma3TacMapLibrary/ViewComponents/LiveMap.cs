using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using Arma3TacMapLibrary.Arma3;
using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Razor.TagHelpers;

namespace Arma3TacMapLibrary.ViewComponents
{
    public class LiveMap : ViewComponent
    {
        private readonly ITagHelperComponentManager _manager;

        private const string baseLineStyle = @"<link rel=""stylesheet"" href=""https://unpkg.com/leaflet@1.6.0/dist/leaflet.css""/>
<link rel=""stylesheet"" href=""https://unpkg.com/bootstrap-select@1.13.14/dist/css/bootstrap-select.min.css"" />
<link rel=""stylesheet"" href=""https://use.fontawesome.com/releases/v5.5.0/css/all.css"" />
<link rel=""stylesheet"" href=""/css/arma3TacMap.css?{version}"" />";
        

        private const string baseLineScript = @"<script src=""https://unpkg.com/leaflet@1.6.0/dist/leaflet.js"">
</script>
<script src=""https://unpkg.com/@microsoft/signalr@3.1.4/dist/browser/signalr.min.js"">
</script>
<script src=""https://unpkg.com/milsymbol@2.0.0/dist/milsymbol.js"">
</script>
<script src=""https://unpkg.com/milstd@0.1.6/milstd.js"">
</script>
<script src=""https://unpkg.com/bootstrap-select@1.13.14/dist/js/bootstrap-select.min.js"">
</script>
<script src=""https://jetelain.github.io/Arma3Map/js/mapUtils.js"">
</script>
<script src=""https://jetelain.github.io/Arma3Map/maps/all.js"">
</script>
<script src=""/js/milstdExtra.js?{version}"">
</script>
<script src=""/js/arma3TacMap.js?{version}"">
</script>";

        public LiveMap(ITagHelperComponentManager manager)
        {
            _manager = manager;
        }

        public IViewComponentResult Invoke(object mapId, string worldName, bool isReadonly)
        {
            var vm = new LiveMapModel();
            vm.isReadOnly = isReadonly;
            vm.worldName = worldName ?? "altis";
            vm.mapId = mapId;

            var scripts = new List<IHtmlContent>();
            scripts.Add(new HtmlString(baseLineScript.Replace("{version}", File.GetLastWriteTimeUtc(typeof(LiveMap).Assembly.Location).Ticks.ToString())));
            scripts.Add(new HtmlString($"<script>initLiveMap({JsonSerializer.Serialize(vm)});</script>"));
            _manager.Components.Add(new InjectTagHelperComponent("head", new HtmlString(baseLineStyle)));
            _manager.Components.Add(new InjectTagHelperComponent("body", scripts));
            return View(vm);
        }
    }
}
