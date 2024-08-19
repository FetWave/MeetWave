using System.Text.RegularExpressions;

namespace FetWaveWWW.Helper
{
    public static  class StringExtensions
    {
        public static string StripHtml (this string input)
            => Regex.Replace(input, "<.*?>", String.Empty);
    }
}
