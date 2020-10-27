using System;
using System.Collections.Generic;
using System.Text.Json;

namespace Arma3TacMapLibrary.Maps
{
    public static class MapExporter
    {
        public static string GetSqf(IEnumerable<StoredMarker> list)
        {
            return @"private _data = " + GetData(list) + @";

_data params ['_icons', '_rects', '_metis'];

if (!isNil 'gtd_map_allMarkers') then {
  {
    deleteMarker _x;
  } forEach gtd_map_allMarkers;
};

if (!isNil 'gtd_map_allMetisMarkers') then {
  {
    [_x] call mts_markers_fnc_deleteMarker
  } forEach gtd_map_allMetisMarkers;
};

gtd_map_allMarkers = [];
gtd_map_allMetisMarkers = [];

{
  _x params ['_id', '_x', '_y', '_w', '_h', '_color', '_rotate'];
  private _marker = createMarker [ format ['_USER_DEFINED #0/planops%1/0', _id], [_x, _y]];
  _marker setMarkerShape 'RECTANGLE';
  _marker setMarkerBrush 'SolidBorder';
  _marker setMarkerDir _rotate;
  _marker setMarkerColor _color; 
  _marker setMarkerSize [_w,2];
  gtd_map_allMarkers pushBack _marker;
} forEach _rects;

{
  _x params ['_id', '_x', '_y', '_icon', '_color', '_text', '_rotate'];
  private _marker = createMarker [ format ['_USER_DEFINED #0/planops%1/0', _id], [_x, _y]];
  _marker setMarkerShape 'ICON';
  _marker setMarkerDir _rotate;
  _marker setMarkerColor _color; 
  _marker setMarkerText _text;
  _marker setMarkerType _icon;
  gtd_map_allMarkers pushBack _marker;
} forEach _icons;

{
  _x params ['_id', '_x', '_y', '_sideid', '_dashed', '_icon', '_mod1', '_mod2', '_size', '_designation'];
  private _marker = [[_x,_y], 0, true, [_sideid, _dashed], [_icon, _mod1, _mod2], [_size, false, false], [], _designation] call mts_markers_fnc_createMarker;
  gtd_map_allMetisMarkers pushBack _marker;
} forEach _metis;

publicVariable 'gtd_map_allMarkers';
publicVariable 'gtd_map_allMetisMarkers';";
        }

        public static string GetData(IEnumerable<StoredMarker> markers)
        {
            var iconMarkers = new List<List<object>>();
            var rectMarkers = new List<List<object>>();
            var metisMarkers = new List<List<object>>();
            foreach (var marker in markers)
            {
                var data = JsonSerializer.Deserialize<MarkerData>(marker.MarkerData);
                if (data.type == "basic")
                {
                    var dir = Get(data.config, "dir", "");

                    iconMarkers.Add(new List<object>() {
                        marker.Id,
                        data.pos[1],
                        data.pos[0],
                        data.symbol,
                        Get(data.config, "color", "ColorBlack"),
                        Get(data.config, "label", ""),
                        !string.IsNullOrEmpty(dir) ? (double.Parse(dir) * 360d / 6400d) : 0d });
                }
                else if (data.type == "line")
                {
                    for (int i = 2; i < data.pos.Length; i += 2)
                    {
                        var y1 = data.pos[i - 2];
                        var x1 = data.pos[i - 1];
                        var y2 = data.pos[i];
                        var x2 = data.pos[i + 1];

                        var length = Math.Sqrt(Math.Pow(x1 - x2, 2) + Math.Pow(y1 - y2, 2));

                        rectMarkers.Add(new List<object>() {
                            marker.Id,
                            x1 + ((x2 - x1) / 2d),
                            y1 + ((y2 - y1) / 2d),
                            length / 2d,
                            2,
                            Get(data.config, "color", "ColorBlack"),
                            GetAngle((x2 - x1),(y2 - y1)) * -1 });
                    }
                }
                else if (data.type == "mil")
                {
                    metisMarkers.Add(new List<object>()
                    {
                        marker.Id,
                        data.pos[1],
                        data.pos[0],
                        ToIdentify(data.symbol[3]),
                        ToDashed(data.symbol[3], data.symbol[6]),
                        ToIcon(data.symbol.Substring(10, 6)),
                        ToMod1(data.symbol.Substring(16, 2)),
                        ToMod2(data.symbol.Substring(18, 2)),
                        ToSize(data.symbol.Substring(8, 2)),
                        Get(data.config, "uniqueDesignation", null) ?? Get(data.config, "higherFormation", null) ?? ""
                    });
                }
            }
            return JsonSerializer.Serialize(new[] { iconMarkers, rectMarkers, metisMarkers }); // FIXME: This is an approximation
        }

        private static int ToSize(string v)
        {
            switch (v)
            {
                case "11": return 1;
                case "12": return 2;
                case "13": return 3;
                case "14": return 4;
                case "15": return 5;
                case "16": return 6;
                case "17": return 7;
                case "18": return 8;
                case "21": return 9;
                case "22": return 10;
                case "23": return 11;
                case "24": return 12;
            }
            return 0;
        }

        private static int ToMod2(string v)
        {
            return 0;
        }

        private static int ToMod1(string v)
        {
            switch (v)
            {
                case "98": return 7;
            }
            return 0;
        }

        private static int ToIcon(string v)
        {
            switch (v)
            {
                case "121100": return 1; // Infantry
                case "121102": return 2; // Mechanized Infantry
                case "121103": return 1; // Infantry with Main Gun System
                case "121104": return 3; // Motorized Infantry
                case "121105": return 2; // Mechanized Infantry with Main Gun System
                case "120500": return 4; // Armor
                case "120600": return 12; // Rotary Wing
                case "121000": return 37; // Combined Arms
                case "150600": return 0; // Intercept
            }
            return 0;
        }

        private static bool ToDashed(char i, char v)
        {
            return v == '1' || i == '5' || i == '2';
        }

        private static string ToIdentify(char v)
        {
            switch (v)
            {
                case '2':
                case '3':
                    return "blu";
                case '4':
                    return "neu";
                case '5':
                case '6':
                    return "red";
            }
            return "unk";
        }

        private static double GetAngle(double dx, double dy)
        {
            return Math.Atan2(dy, dx) * 180d / Math.PI;
        }

        private static string Get(Dictionary<string, string> config, string key, string defaultValue)
        {
            string value;
            if (config.TryGetValue(key, out value))
            {
                return value;
            }
            return defaultValue;
        }
    }
}
