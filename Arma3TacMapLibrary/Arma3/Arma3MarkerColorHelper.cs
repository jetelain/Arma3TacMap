using System;
using System.Collections.Generic;
using System.Linq;

namespace Arma3TacMapLibrary.Arma3
{
    public static class Arma3MarkerColorHelper
    {
        private static readonly Dictionary<Arma3MarkerColor,double[]> Colors = 
            new Dictionary<Arma3MarkerColor,double[]>() {
                {Arma3MarkerColor.ColorBlack 	, new double[]{0,0,0,1}},
                {Arma3MarkerColor.ColorGrey 	, new double[]{0.5,0.5,0.5,1}},
                {Arma3MarkerColor.ColorRed 	, new double[]{0.9,0,0,1}},
                {Arma3MarkerColor.ColorBrown 	, new double[]{0.5,0.25,0,1}},
                {Arma3MarkerColor.ColorOrange 	, new double[]{0.85,0.4,0,1}},
                {Arma3MarkerColor.ColorYellow 	, new double[]{0.85,0.85,0,1}},
                {Arma3MarkerColor.ColorKhaki 	, new double[]{0.5,0.6,0.4,1}},
                {Arma3MarkerColor.ColorGreen 	, new double[]{0,0.8,0,1}},
                {Arma3MarkerColor.ColorBlue 	, new double[]{0,0,1,1}},
                {Arma3MarkerColor.ColorPink 	, new double[]{1,0.3,0.4,1}},
                {Arma3MarkerColor.ColorWhite 	, new double[]{1,1,1,1}},
                {Arma3MarkerColor.ColorUNKNOWN 	, new double[]{0.7,0.6,0,1}},
                {Arma3MarkerColor.colorBLUFOR 	, new double[]{0,0.3,0.6,1}},
                {Arma3MarkerColor.colorOPFOR 	, new double[]{0.5,0,0,1}},
                {Arma3MarkerColor.colorIndependent 	, new double[]{0,0.5,0,1}},
                {Arma3MarkerColor.colorCivilian 	, new double[]{0.4,0,0.5,1}}
        };

        public static string ToHexa(this Arma3MarkerColor color)
        {
            var data = Colors[color];
            return $"{(int)(data[0]*255):X2}{(int)(data[1] * 255):X2}{(int)(data[2] * 255):X2}";
        }
        public static double[] ToColor(this Arma3MarkerColor color)
        {
            return Colors[color];
        }
        public static IEnumerable<Arma3MarkerColor> GetAll()
        {
            return Enum.GetValues(typeof(Arma3MarkerColor)).Cast<Arma3MarkerColor>();
        }
    }
}
