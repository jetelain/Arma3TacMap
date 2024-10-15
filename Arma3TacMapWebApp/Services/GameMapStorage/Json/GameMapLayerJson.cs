using System;
using System.Text.Json.Serialization;

namespace Arma3TacMapWebApp.Services.GameMapStorage.Json
{
    public class GameMapLayerJson
    {
        public int GameMapLayerId { get; set; }

        public LayerType Type { get; set; }

        public LayerFormat Format { get; set; }

        public int MinZoom { get; set; }

        public int MaxZoom { get; set; }

        public int DefaultZoom { get; set; }

        public bool IsDefault { get; set; }

        public int TileSize { get; set; }

        public double FactorX { get; set; }

        public double FactorY { get; set; }

        public string? Culture { get; set; }

        public DateTime? LastChangeUtc { get; set; }

        public DateTime? DataLastChangeUtc { get; set; }

        public Guid? GameMapLayerGuid { get; set; }

        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public string? DownloadUri { get; set; }

        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public string? PatternPng { get; set; }

        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public string? PatternWebp { get; set; }

        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public string? PatternSvg { get; }

        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public string? Pattern { get; set; }
    }
}