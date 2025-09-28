namespace LuceneSearchExample.Search.Models;

public sealed record SearchRequest
{
    public SearchRequest(string query, int limit = 10)
    {
        if (string.IsNullOrWhiteSpace(query))
        {
            throw new ArgumentException("Query cannot be empty.", nameof(query));
        }

        if (limit <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(limit), "Limit must be greater than zero.");
        }

        Query = query;
        Limit = limit;
    }

    public string Query { get; }

    public int Limit { get; }
}