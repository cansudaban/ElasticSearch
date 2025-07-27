using Elastic.Clients.Elasticsearch;
using Elastic.Clients.Elasticsearch.QueryDsl;
using ElasticSearch.Web.Models;

namespace ElasticSearch.Web.Repositories
{
    public class BlogRepository
    {
        private readonly ElasticsearchClient _elasticsearchClient;
        private const string IndexName = "blog";

        public BlogRepository(ElasticsearchClient elasticsearchClient)
        {
            _elasticsearchClient = elasticsearchClient;
        }

        public async Task<Blog?> SaveAsync(Blog newBlog)
        {
            newBlog.Created = DateTime.Now;

            var response = await _elasticsearchClient.IndexAsync(newBlog, i => i
                .Index(IndexName));

            if (!response.IsValidResponse)
            {
                return null;
            }

            newBlog.Id = response.Id;

            return newBlog;
        }

        public async Task<List<Blog>> SearchAsync(string searchText)
        {
            List <Action<QueryDescriptor<Blog>>> listQuery = new();  

            Action<QueryDescriptor<Blog>> matchAll = (q) => q.MatchAll(new MatchAllQuery());
            Action<QueryDescriptor<Blog>> matchContent = (q) => q.Match(m => m.Field(f => f.Content).Query(searchText));
            Action<QueryDescriptor<Blog>> titleMatchBoolPrefix = (q) => q.MatchBoolPrefix(m => m.Field(f => f.Content).Query(searchText));

            if (string.IsNullOrWhiteSpace(searchText))
            {
                listQuery.Add(matchAll);
            }
            else
            {
                listQuery.Add(matchContent);
                listQuery.Add(titleMatchBoolPrefix);
            }

                var response = await _elasticsearchClient.SearchAsync<Blog>(s => s
                    .Index(IndexName)
                    .Size(1000) // Adjust size as needed
                    .Query(q => q
                        .Bool(b => b
                            .Should(listQuery.ToArray())
                        )
                    ));

            foreach (var hits in response.Hits)
            {
                hits.Source!.Id = hits.Id;
            }

            if (!response.IsValidResponse || response.Documents == null)
            {
                return new List<Blog>();
            }
            return response.Documents.ToList();
        }
    }
}
