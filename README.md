# LuceneSearchExample

A refreshed Lucene.Net sample targeting .NET 8.0. The console app demonstrates how to build and query a Lucene index, while the integration tests exercise the flow end-to-end using Testcontainers and PostgreSQL.

## Requirements

- .NET 8 SDK (see `global.json`)
- Docker (required for the Testcontainers-powered integration tests)

## Getting Started

```bash
# Restore dependencies
dotnet restore

# Run the console sample
DOTNET_ENVIRONMENT=Development dotnet run --project src/LuceneSearchExample.App
```

The console application writes the temporary Lucene index location to stdout and prints matching users for a sample query.

## Testing

The integration tests spin up a PostgreSQL container, pull sample users into the Lucene index, and assert the expected match.

```bash
dotnet test
```

If Docker is not available, either skip the tests or mark the `SearchServiceIntegrationTests` class with your preferred test trait filter.

## Project Layout

- `src/LuceneSearchExample.App` – Console host that drives the demo workflow.
- `src/LuceneSearchExample.Search` – Reusable library that wraps Lucene.Net index creation and querying.
- `tests/LuceneSearchExample.IntegrationTests` – xUnit-based tests using Testcontainers and PostgreSQL for realistic data setup.

## Additional Resources

- [Lucene.Net Documentation](https://lucenenet.apache.org/)
- [Testcontainers for .NET](https://dotnet.testcontainers.org/)
