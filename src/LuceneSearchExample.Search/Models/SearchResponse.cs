namespace LuceneSearchExample.Search.Models;

public sealed class SearchResponse
{
    public SearchResponse(int totalCount, IReadOnlyList<User> users)
    {
        TotalCount = totalCount;
        Users = users ?? throw new ArgumentNullException(nameof(users));
    }

    public int TotalCount { get; }

    public IReadOnlyList<User> Users { get; }
}