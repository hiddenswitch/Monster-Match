namespace MonsterMatch
{
    public static class StringExtensions
    {
        public static string FixFancyQuotes(this string theString)
        {
            return theString
                .Replace("“", "\"")
                .Replace("”", "\"")
                .Replace("‘", "'")
                .Replace("’", "'");
        }
    }
}