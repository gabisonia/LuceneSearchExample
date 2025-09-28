using System.Globalization;
using Lucene.Net.Analysis.Standard;
using Lucene.Net.Index;
using Lucene.Net.Search;
using Lucene.Net.Store;
using Lucene.Net.Util;
using LuceneSearchExample.Search.Models;
using LuceneSearchExample.Search.Options;
using LuceneSearchExample.Search.Utilities;
using Directory = System.IO.Directory;

namespace LuceneSearchExample.Search.Indexing;

internal sealed class LuceneIndex : IAsyncDisposable
{
    private static readonly LuceneVersion LuceneVersion = LuceneVersion.LUCENE_48;

    private readonly StandardAnalyzer _analyzer;
    private readonly IndexWriter _writer;
    private readonly SearcherManager _searcherManager;

    public LuceneIndex(LuceneIndexOptions options)
    {
        var options1 = options ?? throw new ArgumentNullException(nameof(options));

        PrepareIndexDirectory(options1);

        _analyzer = new StandardAnalyzer(LuceneVersion);
        _writer = new IndexWriter(FSDirectory.Open(options1.IndexDirectory),
            new IndexWriterConfig(LuceneVersion, _analyzer));
        _searcherManager = new SearcherManager(_writer, applyAllDeletes: true, new SearcherFactory());
    }

    public Task BuildAsync(IEnumerable<User> users, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(users);

        return Task.Run(() =>
        {
            foreach (var user in users)
            {
                cancellationToken.ThrowIfCancellationRequested();
                var document = LuceneDocumentFactory.Create(user);
                _writer.UpdateDocument(new Term(FieldNames.UserId, user.UserId.ToString(CultureInfo.InvariantCulture)),
                    document);
            }

            _writer.Flush(triggerMerge: true, applyAllDeletes: true);
            _writer.Commit();
        }, cancellationToken);
    }

    public Task<SearchResponse> SearchAsync(SearchRequest request, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(request);

        return Task.Run(() =>
        {
            var sanitizedQuery = SearchQuerySanitizer.Sanitize(request.Query);
            var query = BuildQuery(sanitizedQuery);

            _searcherManager.MaybeRefreshBlocking();

            IndexSearcher? searcher = null;
            try
            {
                searcher = _searcherManager.Acquire();
                var topDocs = searcher.Search(query, request.Limit);
                var users = new List<User>(topDocs.ScoreDocs.Length);

                foreach (var scoreDoc in topDocs.ScoreDocs)
                {
                    cancellationToken.ThrowIfCancellationRequested();
                    var document = searcher.Doc(scoreDoc.Doc);
                    users.Add(LuceneDocumentFactory.ToUser(document));
                }

                return new SearchResponse(topDocs.TotalHits, users);
            }
            finally
            {
                if (searcher is not null)
                {
                    _searcherManager.Release(searcher);
                }
            }
        }, cancellationToken);
    }

    public ValueTask DisposeAsync()
    {
        _searcherManager?.Dispose();
        _writer?.Dispose();
        _analyzer?.Dispose();
        return ValueTask.CompletedTask;
    }

    private static void PrepareIndexDirectory(LuceneIndexOptions options)
    {
        if (Directory.Exists(options.IndexDirectory) && options.RecreateOnStartup)
        {
            Directory.Delete(options.IndexDirectory, recursive: true);
        }

        Directory.CreateDirectory(options.IndexDirectory);
    }

    private static Query BuildQuery(string sanitizedQuery)
    {
        var tokens = sanitizedQuery.Split(' ', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
        if (tokens.Length == 0)
        {
            return CreateMatchNoneQuery();
        }

        var booleanQuery = new BooleanQuery
        {
            MinimumNumberShouldMatch = 1
        };

        foreach (var token in tokens)
        {
            var normalized = token.ToLowerInvariant();
            var wildcardValue = normalized.EndsWith("*", StringComparison.Ordinal) ? normalized : normalized + "*";

            booleanQuery.Add(new WildcardQuery(new Term(FieldNames.FirstName, wildcardValue)), Occur.SHOULD);
            booleanQuery.Add(new WildcardQuery(new Term(FieldNames.LastName, wildcardValue)), Occur.SHOULD);

            if (int.TryParse(normalized, NumberStyles.Integer, CultureInfo.InvariantCulture, out var numericValue))
            {
                var numericText = numericValue.ToString(CultureInfo.InvariantCulture);
                booleanQuery.Add(new TermQuery(new Term(FieldNames.UserId, numericText)), Occur.SHOULD);
                booleanQuery.Add(new TermQuery(new Term(FieldNames.Age, numericText)), Occur.SHOULD);
            }
        }

        return booleanQuery.Clauses.Count == 0 ? CreateMatchNoneQuery() : booleanQuery;
    }

    private static Query CreateMatchNoneQuery()
    {
        var query = new BooleanQuery { { new MatchAllDocsQuery(), Occur.MUST_NOT } };
        return query;
    }
}