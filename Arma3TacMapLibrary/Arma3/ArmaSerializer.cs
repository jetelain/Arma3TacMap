using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;

namespace Arma3TacMapLibrary.Arma3
{
    public static class ArmaSerializer
    {
        private static string ToArmaString(object obj)
        {
            if (obj == null)
            {
                return "null";
            }
            if (obj is string str)
            {
                return ToArmaString(str);
            }
            if (obj is double dnum)
            {
                return ToArmaString(dnum);
            }
            if (obj is int inum)
            {
                return ToArmaString(inum);
            }
            if (obj is bool boolean)
            {
                return ToArmaString(boolean);
            }
            if (obj is IEnumerable list)
            {
                return ToSimpleArrayString(list);
            }
            throw new ArgumentException($"Sorry, type '{obj.GetType().FullName}' is not supported");
        }

        private static string ToArmaString(string str)
        {
            return $"\"{Escape(str)}\"";
        }

        private static string ToArmaString(double num)
        {
            return num.ToString(CultureInfo.InvariantCulture);
        }

        private static string ToArmaString(bool boolean)
        {
            return boolean ? "true" : "false";
        }

        private static string ToArmaString(int num)
        {
            return num.ToString(CultureInfo.InvariantCulture);
        }

        /// <summary>
        /// Serialize an array for parseSimpleArray
        /// 
        /// https://community.bistudio.com/wiki/parseSimpleArray
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static string ToSimpleArrayString(IEnumerable list)
        {
            var sb = new StringBuilder("[");
            foreach(var item in list)
            {
                if (sb.Length > 1)
                {
                    sb.Append(",");
                }
                sb.Append(ToArmaString(item));
            }
            sb.Append("]");
            return sb.ToString();
        }

        public static string Escape(string str)
        {
            return str.Replace("\"", "\"\"");
        }

        public static double? ParseDouble(string str)
        {
            if (string.IsNullOrEmpty(str) || str == "null")
            {
                return null;
            }
            return double.Parse(str.Trim(), CultureInfo.InvariantCulture);
        }

        public static string ParseString(string str)
        {
            if (str == "null")
            {
                return null;
            }
            return ReadString(new StringReader(str));
        }

        private static string ReadString(StringReader str)
        {
            if (str.Peek() == '"')
            {
                var sb = new StringBuilder();
                str.Read(); // Consume '"'
                while (str.Peek() != -1)
                {
                    char c = (char)str.Read();
                    if (c == '"')
                    {
                        if (str.Peek() == '"')
                        {
                            str.Read(); // Consume second '"'
                            sb.Append(c);
                        }
                        else
                        {
                            return sb.ToString();
                        }
                    }
                    else
                    {
                        sb.Append(c);
                    }
                }
                return sb.ToString();
            }
            return null;
        }

        private static double? ReadNumber(StringReader str)
        {
            var sb = new StringBuilder();
            int i;
            while ((i = str.Peek()) != -1)
            {
                char c = (char)i;
                if (char.IsDigit(c) || c == '.' || c == '-' || c == 'e')
                {
                    str.Read();
                    sb.Append(c);
                }
                else
                {
                    return ParseDouble(sb.ToString());
                }
            }
            return ParseDouble(sb.ToString());
        }

        public static int[] ParseIntegerArray(string str)
        {
            return ParseMixedArray(str).Cast<double?>().Select(n => (int)n).ToArray();
        }

        public static double[] ParseDoubleArray(string str)
        {
            return ParseMixedArray(str).Cast<double?>().Select(n => (double)n).ToArray();
        }

        public static object[] ParseMixedArray(string str)
        {
            return ReadArray(new StringReader(str));
        }

        private static object[] ReadArray(StringReader str)
        {
            if (str.Peek() == '[')
            {
                var data = new List<object>();
                var expectItem = true;
                str.Read(); // Consume '['

                int i;
                while ((i = str.Peek()) != -1)
                {
                    char c = (char)i;
                    if (c == ']')
                    {
                        str.Read();
                        return data.ToArray();
                    }
                    if (c == ',')
                    {
                        str.Read();
                        expectItem = true;
                    }
                    else if (c != ' ' && expectItem)
                    {
                        if (c == '"')
                        {
                            data.Add(ReadString(str));
                        }
                        else if (c == '[')
                        {
                            data.Add(ReadArray(str));
                        }
                        else if (char.IsDigit(c) || c == '-')
                        {
                            data.Add(ReadNumber(str));
                        }
                        else if (c == 'n')
                        {
                            str.Read();
                            data.Add(null);
                        }
                        else if (c == 't')
                        {
                            str.Read();
                            data.Add(true);
                        }
                        else if (c == 'f')
                        {
                            str.Read();
                            data.Add(false);
                        }
                        expectItem = false;
                    }
                    else
                    {
                        str.Read();
                    }
                }
                return data.ToArray();
            }
            return null;
        }
    }
}
