using System;
using System.Collections.Generic;
using System.Text;

namespace LuceneSearchExample.Services.Models
{
    public class SearchRequest
    {
        public string Query { get; set; }
    }

    public class SearchResponse
    {
        public SearchResponse() => Users = new List<User>();
        public int TotalCount{ get; set; }
        public IList<User> Users { get; set; }
    }
}
