using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using Arma3TacMapLibrary.Arma3;
using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Razor.TagHelpers;
using Microsoft.Extensions.Configuration;

namespace Arma3TacMapLibrary.ViewComponents
{
    public class LiveMap : ViewComponent
    {
        private readonly ITagHelperComponentManager _manager;
        private readonly string _endpoint;

        private const string baseLineStyle = @"<link rel=""stylesheet"" href=""https://cdn.jsdelivr.net/npm/leaflet@1.7.1/dist/leaflet.min.css""/>
<link rel=""stylesheet"" href=""https://cdn.jsdelivr.net/npm/bootstrap-select@1.13.18/dist/css/bootstrap-select.min.css"" />
<link rel=""stylesheet"" href=""https://use.fontawesome.com/releases/v5.5.0/css/all.css"" />
<link rel=""stylesheet"" href=""/css/arma3TacMap.css?{version}"" />";
        

        private const string baseLineScript = @"<script src=""https://cdn.jsdelivr.net/npm/leaflet@1.7.1/dist/leaflet.min.js"">
</script>
<script src=""https://cdn.jsdelivr.net/npm/@microsoft/signalr@5.0.4/dist/browser/signalr.min.js"">
</script>
<script src=""https://cdn.jsdelivr.net/npm/milsymbol@2.0.0/dist/milsymbol.min.js"">
</script>
<script src=""https://cdn.jsdelivr.net/npm/milstd@0.1.6/milstd.min.js"">
</script>
<script src=""https://cdn.jsdelivr.net/npm/bootstrap-select@1.13.18/dist/js/bootstrap-select.min.js"">
</script>
<script src=""{endpoint}/js/mapUtils.js"">
</script>
<script src=""{endpoint}/maps/all.js"">
</script>
<script src=""/js/milstdExtra.js?{version}"">
</script>
<script src=""/js/arma3TacMap.js?{version}"">
</script>";

        public LiveMap(ITagHelperComponentManager manager, IConfiguration configuration)
        {
            _manager = manager;
            _endpoint = Arma3MapHelper.GetEndpoint(configuration);
        }

        public IViewComponentResult Invoke(object mapId, string worldName, bool isReadonly, string hub)
        {
            var vm = new LiveMapModel();
            vm.isReadOnly = isReadonly;
            vm.worldName = worldName ?? "altis";
            vm.mapId = mapId;
            vm.endpoint = _endpoint;
            vm.hub = hub ?? "/MapHub";

            var version = File.GetLastWriteTimeUtc(typeof(LiveMap).Assembly.Location).Ticks.ToString();

            var scripts = new List<IHtmlContent>();
            scripts.Add(new HtmlString(baseLineScript
                .Replace("{version}", version)
                .Replace("{endpoint}", _endpoint)
                ));

            scripts.Add(new HtmlString($"<script>Arma3TacMap.initLiveMap({JsonSerializer.Serialize(vm)});</script>"));
            _manager.Components.Add(new InjectTagHelperComponent("head", new HtmlString(baseLineStyle.Replace("{version}", version))));
            _manager.Components.Add(new InjectTagHelperComponent("body", scripts));
            return View(vm);
        }
    }
}
