using Lucene.Net.Analysis;
using Lucene.Net.Analysis.Core;
using Lucene.Net.Analysis.Miscellaneous;
using Lucene.Net.Analysis.Standard;
using Lucene.Net.Documents;
using Lucene.Net.Index;
using Lucene.Net.QueryParsers.Classic;
using Lucene.Net.Search;
using Lucene.Net.Store;
using Lucene.Net.Util;
using LuceneSearchExample.Services.Models;
using System;
using System.Collections.Generic;

namespace LuceneSearchExample.Services.Concrete
{
    internal class Index : IDisposable
    {
        private const LuceneVersion MATCH_LUCENE_VERSION = LuceneVersion.LUCENE_48;
        private const int SNIPPET_LENGTH = 100;
        private readonly IndexWriter writer;
        private readonly Analyzer analyzer;
        private readonly QueryParser queryParser;
        private readonly SearcherManager searchManager;

        public Index(string indexPath)
        {
            analyzer = SetupAnalyzer();
            queryParser = SetupQueryParser(analyzer);
            writer = new IndexWriter(FSDirectory.Open(indexPath), new IndexWriterConfig(MATCH_LUCENE_VERSION, analyzer));
            searchManager = new SearcherManager(writer, true, null);
        }

        private Analyzer SetupAnalyzer()
        {
            return new StandardAnalyzer(MATCH_LUCENE_VERSION);
            //return Analyzer.NewAnonymous((field, reader) =>
            //{
            //    var tokenizer = new StandardTokenizer(MATCH_LUCENE_VERSION, reader);
            //    TokenStream tokenStream = new StandardFilter(MATCH_LUCENE_VERSION, tokenizer);
            //    //tokenStream = new ASCIIFoldingFilter(tokenStream);
            //    //tokenStream = new LowerCaseFilter(MATCH_LUCENE_VERSION, tokenStream);
            //    //tokenStream = new StopFilter(MATCH_LUCENE_VERSION, tokenStream);
            //    return new TokenStreamComponents(tokenizer, tokenStream);
            //});
        }

        private QueryParser SetupQueryParser(Analyzer analyzer)
        {
            return new MultiFieldQueryParser(
                MATCH_LUCENE_VERSION,
                new[] { "firstname", "lastname", "age", "userid" },            
                analyzer
            );
        }

        public void Build(IEnumerable<User> users)
        {
            if (users == null)
            {
                throw new ArgumentNullException();
            }
            foreach (var user in users)
            {
                writer.UpdateDocument(new Term("userid", user.UserId.ToString()), BuildDocument(user));
            }
            writer.Flush(true, true);
            writer.Commit();
        }

        private Document BuildDocument(User user)
        {
            Document doc = new Document
            {
                new StringField("userid", user.UserId.ToString(),Field.Store.YES),
                new TextField("firstname", user.FirstName, Field.Store.YES),
                new TextField("lastname", user.LastName, Field.Store.YES),
                new StringField("age", user.Age.ToString(), Field.Store.YES)
            };
            return doc;
        }

        public SearchResponse Search(string queryString)
        {
            int resultsPerPage = 10;
            Query query = BuildQuery(queryString);
            searchManager.MaybeRefreshBlocking();
            IndexSearcher searcher = searchManager.Acquire();
            try
            {
                TopDocs topdDocs = searcher.Search(query, resultsPerPage);
                return CompileResults(searcher, topdDocs);
            }
            finally
            {
                searchManager.Release(searcher);
                searcher = null;
            }
        }

        private SearchResponse CompileResults(IndexSearcher searcher, TopDocs topdDocs)
        {
            var searchResults = new SearchResponse() { TotalCount = topdDocs.TotalHits };
            foreach (var result in topdDocs.ScoreDocs)
            {
                var document = searcher.Doc(result.Doc);
                var searchResult = new User
                {
                    Age = Convert.ToInt32(document.GetField("age")?.GetStringValue()),
                    FirstName = document.GetField("firstname")?.GetStringValue(),
                    UserId = Convert.ToInt32(document.GetField("userid")?.GetStringValue()),
                    LastName = document.GetField("lastname")?.GetStringValue()
                };
                searchResults.Users.Add(searchResult);
            }
            return searchResults;
        }

        private Query BuildQuery(string queryString)
        {
            return queryParser.Parse(Sanitize(queryString));
        }

        private string Sanitize(string queryString)
        {
            string[] removed = { "*", "?", "%", "+" };
            string[] spaces = { "-" };
            foreach (var r in removed)
            {
                queryString = queryString.Replace(r, string.Empty);
            }
            foreach (var s in spaces)
            {
                queryString = queryString.Replace(s, " ");
            }
            return queryString;
        }

        public void Dispose()
        {
            searchManager?.Dispose();
            analyzer?.Dispose();
            writer?.Dispose();
        }
    }
}
