using LuceneSearchExample.Search.Abstractions;
using LuceneSearchExample.Search.Indexing;
using LuceneSearchExample.Search.Models;
using LuceneSearchExample.Search.Options;

namespace LuceneSearchExample.Search;

public sealed class LuceneSearchService : ISearchService
{
    private readonly LuceneIndex _index;

    public LuceneSearchService(LuceneIndexOptions options)
    {
        ArgumentNullException.ThrowIfNull(options);
        _index = new LuceneIndex(options);
    }

    public Task BuildIndexAsync(IEnumerable<User> users, CancellationToken cancellationToken = default)
        => _index.BuildAsync(users, cancellationToken);

    public Task<SearchResponse> SearchAsync(SearchRequest request, CancellationToken cancellationToken = default)
        => _index.SearchAsync(request, cancellationToken);

    public ValueTask DisposeAsync() => _index.DisposeAsync();
}