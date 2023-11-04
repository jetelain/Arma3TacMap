using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using Arma3TacMapLibrary.Arma3;

namespace Arma3TacMapLibrary.Maps
{
    public static class MapExporter
    {

        private static readonly Lazy<MilMission> milMission = new Lazy<MilMission>(() => new MilMission(), true);

        public static string GetSqf(IEnumerable<StoredMarker> list, int channel = 0)
        {
            return @"private _data = " + GetData(list) + @";

_data params ['_icons', '_poly', '_metis'];

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
  _x params ['_id', '_points', '_color'];
  _points params ['_x', '_y'];
  private _marker = createMarker [ format ['_USER_DEFINED #%1/planops%2/" + channel + @"', clientOwner, _id], [_x, _y], " + channel + @"];
  _marker setMarkerShape 'polyline';
  _marker setMarkerPolyline _points;
  _marker setMarkerColor _color; 
  gtd_map_allMarkers pushBack _marker;
} forEach _poly;

{
  _x params ['_id', '_x', '_y', '_icon', '_color', '_text', '_rotate',['_scale',1]];
  private _marker = createMarker [ format ['_USER_DEFINED #%1/planops%2/" + channel + @"', clientOwner, _id], [_x, _y], " + channel + @"];
  _marker setMarkerShape 'ICON';
  _marker setMarkerDir _rotate;
  _marker setMarkerColor _color; 
  _marker setMarkerText _text;
  _marker setMarkerType _icon;
  _marker setMarkerSize [_scale,_scale];
  gtd_map_allMarkers pushBack _marker;
} forEach _icons;

{
  _x params ['_id', '_x', '_y', '_sideid', '_dashed', '_icon', '_mod1', '_mod2', '_size', '_designation',['_scale',1]];
  private _marker = [[_x,_y], " + channel + @", true, [[_sideid, _dashed], [_icon, _mod1, _mod2], [_size, false, false], [], _designation], _scale * 1.3] call mts_markers_fnc_createMarker;
  gtd_map_allMetisMarkers pushBack _marker;
} forEach _metis;

publicVariable 'gtd_map_allMarkers';
publicVariable 'gtd_map_allMetisMarkers';";
        }

        public static IEnumerable<string> GetMarkerData(int id, MarkerData data)
        {
            if (data.type == "basic")
            {
                return new[] { ArmaSerializer.ToSimpleArrayString(new object[] { "icon", GetBasic(id, data) }) };
            }
            if (data.type == "line")
            {
                return new[] { ArmaSerializer.ToSimpleArrayString(new object[] { "poly", GetLine(id, data) }) };
            }
            if (data.type == "mil")
            {
                return new[] { ArmaSerializer.ToSimpleArrayString(new object[] { "mtis", GetMilAsMetis(id, data) }) };
            }
            if ( data.type == "mission")
            {
                return GetMissionLines(id, data).Select(line => ArmaSerializer.ToSimpleArrayString(new object[] { "poly", line })).ToList();
            }
            return Enumerable.Empty<string>();
        }

        public static string GetData(IEnumerable<StoredMarker> markers)
        {
            var iconMarkers = new List<List<object>>();
            var polyMarkers = new List<List<object>>();
            var metisMarkers = new List<List<object>>();
            foreach (var marker in markers)
            {
                var data = MarkerData.Deserialize(marker.MarkerData);
                if (data.type == "basic")
                {
                    iconMarkers.Add(GetBasic(marker.Id, data));
                }
                else if (data.type == "line")
                {
                    polyMarkers.Add(GetLine(marker.Id, data));
                }
                else if (data.type == "mil")
                {
                    metisMarkers.Add(GetMilAsMetis(marker.Id, data));
                }
                else if (data.type == "mission")
                {
                    polyMarkers.AddRange(GetMissionLines(marker.Id, data));
                }
            }
            return ArmaSerializer.ToSimpleArrayString(new[] { iconMarkers, polyMarkers, metisMarkers });
        }

        private static double[][] ToLeafletPoints(double[] pos)
        {
            var points = new List<double[]>();
            for (int i = 0; i < pos.Length; i += 2)
            {
                points.Add(new [] { pos[i], pos[i + 1] });
            }
            return points.ToArray();
        }

        private static IEnumerable<List<object>> GetMissionLines(int id, MarkerData data)
        {
            var lines = new List<List<object>>();
            var color = Get(data.config, "color", "ColorBlack");
            var size = Get(data.config, "size", "13");
            var scale = 1000d;
            switch (size)
            {
                case "12": scale = 25; break;
                case "13": scale = 50; break;
                case "14": scale = 250; break;
            }
            var mil = milMission.Value;
            var result = mil.RenderMission(data.symbol, ToLeafletPoints(data.pos), scale);
            if (result != null)
            {
                var index = 0;
                foreach (var line in result.Lines)
                {
                    var points = new List<double>();
                    foreach (var point in line)
                    {
                        points.Add(Math.Round(point[1], 2));
                        points.Add(Math.Round(point[0], 2));
                    }
                    lines.Add(new List<object>() {
                        id + "." + index,
                        points,
                        color});
                    index++;
                }
            }
            return lines;
        }

        private static List<object> GetMilAsMetis(int id, MarkerData data)
        {
            return new List<object>()
                    {
                        id,
                        data.pos[1],
                        data.pos[0],
                        ToIdentify(data.symbol[3]),
                        ToDashed(data.symbol[3], data.symbol[6]),
                        ToIcon(data.symbol.Substring(10, 6)),
                        ToMod1(data.symbol.Substring(16, 2)),
                        ToMod2(data.symbol.Substring(18, 2)),
                        ToSize(data.symbol.Substring(8, 2)),
                        Get(data.config, "uniqueDesignation", null) ?? Get(data.config, "higherFormation", null) ?? "",
                        data.scale ?? 1d
                    };
        }

        private static List<object> GetLine(int id, MarkerData data)
        {
            var points = new List<double>();
            for (int i = 0; i < data.pos.Length; i += 2)
            {
                var x = data.pos[i + 1];
                var y = data.pos[i];
                points.Add(x);
                points.Add(y);
            }
            return new List<object>() {
                        id,
                        points,
                        Get(data.config, "color", "ColorBlack")};
        }

        private static List<object> GetBasic(int id, MarkerData data)
        {
            var dir = Get(data.config, "dir", "");

            return new List<object>() {
                        id,
                        data.pos[1],
                        data.pos[0],
                        data.symbol,
                        Get(data.config, "color", "ColorBlack"),
                        Get(data.config, "label", ""),
                        !string.IsNullOrEmpty(dir) ? (double.Parse(dir) * 360d / 6400d) : 0d,
                        data.scale ?? 1d };
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
            switch(v)
            {
                case "51": return 7;
            }
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
                case "120501": return 22; // Intercept
                case "111000": return 28; // Signal
            }
            return 0;
        } 

        private static bool ToDashed(char i, char v)
        {
            if (i == '4')
            {
                return false; // Unsupported
            }
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

        //private static double GetAngle(double dx, double dy)
        //{
        //    return Math.Atan2(dy, dx) * 180d / Math.PI;
        //}

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
