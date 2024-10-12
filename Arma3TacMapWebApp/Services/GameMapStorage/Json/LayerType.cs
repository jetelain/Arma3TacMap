using System.ComponentModel.DataAnnotations;

namespace Arma3TacMapWebApp.Services.GameMapStorage.Json
{
    public enum LayerType
    {
        [Display(Name = "Topographic (Game)")]
        Topographic,

        [Display(Name = "Satellite (Game)")]
        Satellite,

        Aerial,

        Elevation,

        [Display(Name = "Topographic (Atlas)")]
        TopographicAtlas
    }
}
