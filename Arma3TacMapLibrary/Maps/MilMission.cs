using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Linq;
using Esprima;
using Esprima.Ast;
using Jint;
using Jint.Native;

namespace Arma3TacMapLibrary.Maps
{
    public class MilMission
    {
        private readonly Script script;

        public MilMission() 
            : this(ReadEmbedded())
        {

        }

        public MilMission(string script)
        {
            var parser = new JavaScriptParser(script, new ParserOptions() { Tolerant = true });
            this.script = parser.ParseScript();
        }

        private static string ReadEmbedded()
        {
            using (var reader = new StreamReader(typeof(MilMission).Assembly.GetManifestResourceStream("Arma3TacMapLibrary.milMissions.js")))
            {
                return reader.ReadToEnd();
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="mission">Mission</param>
        /// <param name="points">[[y,x]], or [[y1,x1],[y2,x2]], or [[y1,x1],[y2,x2],[y3,x3],[y4,x4]]</param>
        /// <returns></returns>
        /// <remarks>
        /// X and Y are provided in the Leaflet order
        /// </remarks>
        public MilMissionResult RenderMission(string mission, double[][] points, double scale)
        {
            var engine = new Engine();
            engine.Execute(script);
            var missionDef = engine.GetValue("MilMissions").Get(new JsString("missions")).Get(new JsString(mission));
            if (missionDef.IsObject())
            {
                return RenderMission(engine, missionDef, points, scale);
            }
            return null;
        }

        private MilMissionResult RenderMission(Engine engine, JsValue missionDef, double[][] points, double scale)
        {
            var generate = missionDef.Get(new JsString("generate"));
            var jsPoints = JsValue.FromObject(engine, points);
            var x = ToArray2(jsPoints, v => v.AsNumber());
            var jsLines = engine.Invoke(generate, jsPoints, new JsNumber(scale));
            var lines = ToArray3(jsLines, v => v.AsNumber());
            var labels = missionDef.Get(new JsString("labels"));
            if (!labels.IsUndefined())
            {
                var generateLabels = missionDef.Get(new JsString("generateLabels"));
                var labelPoints = engine.Invoke(generateLabels, jsPoints, new JsNumber(scale), jsLines);
                return new MilMissionResult(lines, ToArray(labels, l => l.AsString()), ToArray2(labelPoints, v => v.AsNumber()));
            }
            return new MilMissionResult(lines);
        }

        private static T[][][] ToArray3<T>(JsValue array3, Func<JsValue,T> convert)
        {
            return ToArray(array3, v => ToArray2(v, convert));
        }
        private static T[][] ToArray2<T>(JsValue array2, Func<JsValue, T> convert)
        {
            return ToArray(array2, v => ToArray(v, convert));
        }
        private static T[] ToArray<T>(JsValue array, Func<JsValue, T> convert)
        {
            return array.AsArray().Select(convert).ToArray();
        }
    }
}
