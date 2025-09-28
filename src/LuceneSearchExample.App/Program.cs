using LuceneSearchExample.Search;
using LuceneSearchExample.Search.Models;
using LuceneSearchExample.Search.Options;

var indexOptions = LuceneIndexOptions.FromTemporaryDirectory("LuceneSearchExampleIndex");
await using var searchService = new LuceneSearchService(indexOptions);

var users = new List<User>
{
    new(1, "Dani", "Daniels", 27),
    new(2, "Sasha", "Grey", 29),
    new(3, "Nicole", "Aniston", 31),
    new(4, "Tori", "Black", 28)
};

await searchService.BuildIndexAsync(users);

var response = await searchService.SearchAsync(new SearchRequest("sasha"));

Console.WriteLine($"Index created at: {indexOptions.IndexDirectory}");
foreach (var user in response.Users)
{
    Console.WriteLine($"{user.UserId} - {user.FirstName} - {user.LastName} - {user.Age}");
}
