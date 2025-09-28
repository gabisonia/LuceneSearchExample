using System.Text;

namespace LuceneSearchExample.Search.Utilities;

public static class SearchQuerySanitizer
{
    private static readonly char[] RemovedCharacters = { '*', '?', '%', '+' };
    private static readonly char[] SpaceReplacementCharacters = { '-' };

    public static string Sanitize(string query)
    {
        if (string.IsNullOrWhiteSpace(query))
        {
            return string.Empty;
        }

        var builder = new StringBuilder(query.Length);
        foreach (var character in query)
        {
            if (Array.IndexOf(RemovedCharacters, character) >= 0)
            {
                continue;
            }

            builder.Append(Array.IndexOf(SpaceReplacementCharacters, character) >= 0 ? ' ' : character);
        }

        return builder.ToString().Trim();
    }
}