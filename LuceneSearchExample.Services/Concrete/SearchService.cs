using LuceneSearchExample.Services.Abstract;
using LuceneSearchExample.Services.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace LuceneSearchExample.Services.Concrete
{
    public class SearchService : ISearchService
    {
        private readonly Index index;

        public SearchService(string indexDirectory)
        {
            index = new Index(indexDirectory);
        }

        public void BuildIndex(BuildIndexRequest request)
        {
            index.Build(request.Users);
        }

        public SearchResponse Search(SearchRequest request) => index.Search(request.Query);
    }
}
