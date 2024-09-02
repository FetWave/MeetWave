using System.Text.RegularExpressions;

namespace MeetWave.Helper
{
    public static  class StringExtensions
    {
        public static string StripHtml (this string input)
            => Regex.Replace(input, "<.*?>", String.Empty);
    }
}
