using Arma3TacMapWebApp.Services.GameMapStorage.Json;
using Microsoft.AspNetCore.Mvc.Localization;

namespace Arma3TacMapWebApp.Views
{
    public static class ViewHelper
    {

        public static LocalizedHtmlString GetLabel(IHtmlLocalizer<SharedResource> localizer, LayerType type)
        {
            switch (type)
            {
                case LayerType.Topographic: return localizer["Topographic"];
                case LayerType.Satellite: return localizer["Satellite view"];
                case LayerType.Aerial: return localizer["Aerial view"];
                case LayerType.TopographicAtlas: return localizer["Topographic (Atlas)"];
                default: return new LocalizedHtmlString(type.ToString(), type.ToString());
            }
        }
    }
}
