using LuceneSearchExample.Search.Models;

namespace LuceneSearchExample.Search.Abstractions;

public interface ISearchService : IAsyncDisposable
{
    Task BuildIndexAsync(IEnumerable<User> users, CancellationToken cancellationToken = default);

    Task<SearchResponse> SearchAsync(SearchRequest request, CancellationToken cancellationToken = default);
}
