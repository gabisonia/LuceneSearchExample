using FluentAssertions;
using LuceneSearchExample.Search;
using LuceneSearchExample.Search.Models;
using LuceneSearchExample.Search.Options;
using LuceneSearchExample.Search.Utilities;
using Xunit;

namespace LuceneSearchExample.IntegrationTests;

public sealed class SearchServiceIntegrationTests : IAsyncLifetime
{
    private readonly string _indexDirectory = Path.Combine(Path.GetTempPath(), $"lucene-tests-{Guid.NewGuid():N}");
    private LuceneSearchService? _service;

    public async Task InitializeAsync()
    {
        _service = new LuceneSearchService(new LuceneIndexOptions(_indexDirectory));

        var seedUsers = new List<User>
        {
            new(1, "Dani", "Daniels", 27),
            new(2, "Sasha", "Grey", 29),
            new(3, "Nicole", "Aniston", 31),
            new(4, "Tori", "Black", 28)
        };

        await _service.BuildIndexAsync(seedUsers);
    }

    public async Task DisposeAsync()
    {
        if (_service is not null)
        {
            await _service.DisposeAsync();
        }

        if (Directory.Exists(_indexDirectory))
        {
            Directory.Delete(_indexDirectory, recursive: true);
        }
    }

    [Theory]
    [InlineData("sasha", "Sasha", "Grey")]
    [InlineData("NIC", "Nicole", "Aniston")]
    public async Task SearchAsync_FindsExpectedUser(string query, string expectedFirstName, string expectedLastName)
    {
        var service = AssertService();

        var response = await service.SearchAsync(new SearchRequest(query));

        response.TotalCount.Should().BeGreaterThan(0);
        response.Users.Should().ContainSingle(user =>
            string.Equals(user.FirstName, expectedFirstName, StringComparison.OrdinalIgnoreCase) &&
            string.Equals(user.LastName, expectedLastName, StringComparison.OrdinalIgnoreCase));
    }

    [Fact]
    public async Task SearchAsync_IgnoresUnsupportedCharacters()
    {
        var service = AssertService();
        var sanitized = SearchQuerySanitizer.Sanitize("s*a?s+ha");

        sanitized.Should().Be("sasha");

        var response = await service.SearchAsync(new SearchRequest(sanitized));

        response.Users.Should().Contain(user => user.FirstName == "Sasha");
    }

    private LuceneSearchService AssertService()
        => _service ?? throw new InvalidOperationException("Service has not been initialised.");
}
