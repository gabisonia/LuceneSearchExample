using LuceneSearchExample.Services.Concrete;
using LuceneSearchExample.Services.Models;
using System;
using System.Collections.Generic;

namespace LuceneSearchExample
{
    class Program
    {
        static void Main(string[] args)
        {
            var searchService = new SearchService(@"D:\index");
            searchService.BuildIndex(new BuildIndexRequest()
            {
                Users = new List<User>()
                {
                    new User { UserId = 1 , Age = 27 , FirstName = "Dani", LastName = "Daniels" },
                    new User { UserId = 2 , Age = 29 , FirstName = "Sasha", LastName = "Grey" },
                    new User { UserId = 3 , Age = 31 , FirstName = "Nicole", LastName = "Aniston" },
                    new User { UserId = 4 , Age = 28 , FirstName = "Tori", LastName = "Black" },
                }
            });
            var searchRequest = new SearchRequest()
            {
                Query = "sash"
            };
            var searchResponse = searchService.Search(searchRequest);
            foreach (var item in searchResponse.Users)
            {
                Console.WriteLine($"{item.UserId} - {item.FirstName} - {item.LastName} - {item.Age}");
            }
            Console.ReadLine();
        }
    }
}
