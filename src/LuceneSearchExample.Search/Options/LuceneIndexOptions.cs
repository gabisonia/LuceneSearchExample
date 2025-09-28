namespace LuceneSearchExample.Search.Options;

public sealed class LuceneIndexOptions
{
    public LuceneIndexOptions(string indexDirectory, bool recreateOnStartup = true)
    {
        if (string.IsNullOrWhiteSpace(indexDirectory))
        {
            throw new ArgumentException("Index directory cannot be empty.", nameof(indexDirectory));
        }

        IndexDirectory = indexDirectory;
        RecreateOnStartup = recreateOnStartup;
    }

    public string IndexDirectory { get; }

    public bool RecreateOnStartup { get; }

    public static LuceneIndexOptions FromTemporaryDirectory(string uniqueName, bool recreateOnStartup = true)
    {
        if (string.IsNullOrWhiteSpace(uniqueName))
        {
            throw new ArgumentException("Name cannot be empty.", nameof(uniqueName));
        }

        var tempPath = Path.Combine(Path.GetTempPath(), uniqueName);
        return new LuceneIndexOptions(tempPath, recreateOnStartup);
    }
}