using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace FileStore.Application.Helpers
{
    public static class TemplateHelper
    {
        public static readonly string[] SizeSuffixes = { "bytes", "KB", "MB", "GB", "TB", "PB", "EB", "ZB", "YB" };

        public static string FillTemplate(string template, IDictionary<string, object> templateData = null)
        {
            if (string.IsNullOrWhiteSpace(template))
            {
                return string.Empty;
            }
            if (templateData == null || templateData.Count == 0)
            {
                return template;
            }
            Regex re = new Regex(@"\{(\w+)\}", RegexOptions.Compiled);

            string output = re.Replace(template, match => { return templateData.ContainsKey(match.Groups[1].Value) ? templateData[match.Groups[1].Value].ToString() : match.Value; });
            return output;
        }

        public static string PrepareContent(string template, IDictionary<string, string> replacements)
        {
            if (string.IsNullOrWhiteSpace(template))
            {
                return string.Empty;
            }
            if (replacements == null || replacements.Count == 0)
            {
                return template;
            }
            try
            {
                Regex re = new Regex(@"\{(.*?)\}", RegexOptions.Compiled);

                string output = re.Replace(template, match => { return replacements.ContainsKey(match.Groups[1].Value) ? replacements[match.Groups[1].Value].ToString() : match.Value; });
                return output;
            }
            catch
            {
                return template;
            }
        }

        public static string SizeSuffix(Int64 value, int decimalPlaces = 1)
        {
            if (decimalPlaces < 0) { throw new ArgumentOutOfRangeException("decimalPlaces"); }
            if (value < 0) { return "-" + SizeSuffix(-value, decimalPlaces); }
            if (value == 0) { return string.Format("{0:n" + decimalPlaces + "} bytes", 0); }

            // mag is 0 for bytes, 1 for KB, 2, for MB, etc.
            int mag = (int)Math.Log(value, 1024);

            // 1L << (mag * 10) == 2 ^ (10 * mag) 
            // [i.e. the number of bytes in the unit corresponding to mag]
            decimal adjustedSize = (decimal)value / (1L << (mag * 10));

            // make adjustment when the value is large enough that
            // it would round up to 1000 or more
            if (Math.Round(adjustedSize, decimalPlaces) >= 1000)
            {
                mag += 1;
                adjustedSize /= 1024;
            }

            return string.Format("{0:n" + decimalPlaces + "} {1}",
                adjustedSize,
                SizeSuffixes[mag]);
        }
    }
}
