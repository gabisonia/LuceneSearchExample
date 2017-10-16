using LuceneSearchExample.Services.Concrete;
using LuceneSearchExample.Services.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace LuceneSearchExample.Services.Abstract
{
    public interface ISearchService
    {
        void BuildIndex(BuildIndexRequest request);
        SearchResponse Search(SearchRequest request);
    }
}
