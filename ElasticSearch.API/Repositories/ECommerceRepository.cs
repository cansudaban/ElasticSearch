using Elastic.Clients.Elasticsearch;
using Elastic.Clients.Elasticsearch.QueryDsl;
using Elastic.Clients.Elasticsearch.SearchApplication;
using ElasticSearch.API.Models.ECommerceModels;
using System.Collections.Immutable;

namespace ElasticSearch.API.Repositories
{
    public class ECommerceRepository
    {
        private readonly ElasticsearchClient _client;
        private const string IndexName = "kibana_sample_data_ecommerce";

        public ECommerceRepository(ElasticsearchClient client)
        {
            _client = client;
        }

        public async Task<ImmutableList<ECommerce>> TermQuery(string customerFirstName)
        {
            // 1. way
            //var result = await _client.SearchAsync<ECommerce>(s => s.Index(IndexName).Query(q => q.Term(t =>
            //t.Field("customer_first_name.keyword").Value(customFirstName))));

            // 2. way
            //var result = await _client.SearchAsync<ECommerce>(s => s
            //        .Index(IndexName)
            //        .Query(q => q.Term(t => t
            //            .Field(f => f.CustomerFirstName.Suffix("keyword")) // strongly-typed alan
            //            .Value(customerFirstName)
            //        ))
            //    );

            // 3. way
            var termQuery = new TermQuery("customer_first_name.keyword")
            {
                Value = customerFirstName,
                CaseInsensitive = true,
            };

            var result = await _client.SearchAsync<ECommerce>(s => s.Index(IndexName).Query(termQuery));

            foreach (var item in result.Hits)
            {
                item.Source.Id = item.Id;
            }

            return result.Documents.ToImmutableList();
        }

        public async Task<ImmutableList<ECommerce>> TermsQuery(List<string> customerFirstNameList)
        {
            List<FieldValue> terms = new List<FieldValue>();
            customerFirstNameList.ForEach(x =>
            {
                terms.Add(x);
            });

            // 1. way
            //var termsQuery = new TermsQuery()
            //{
            //    Field = "customer_first_name.keyword",
            //    Terms = new TermsQueryField(terms.AsReadOnly())
            //};

            //var result = await _client.SearchAsync<ECommerce>(s => s.Index(IndexName).Query(termsQuery));

            var result = await _client.SearchAsync<ECommerce>(s => s.Index(IndexName).Size(10000)
            .Query(q => q
            .Terms(t => t
            .Field(f => f.CustomerFirstName
            .Suffix("keyword"))
            .Terms(new TermsQueryField(terms.AsReadOnly())))));

            return result.Documents.ToImmutableList();
        }

        public async Task<ImmutableList<ECommerce>> PrefixQuery(string customerFullName)
        {
            var result = await _client.SearchAsync<ECommerce>(s => s.Index(IndexName).Size(10000)
            .Query(q => q
                .Prefix(p => p
                    .Field(f => f.CustomerFullName
                        .Suffix("keyword"))
                            .Value(customerFullName))));

            return result.Documents.ToImmutableList();
        }

        public async Task<ImmutableList<ECommerce>> RangeQuery(double fromPrice, double toPrice)
        {
            var result = await _client.SearchAsync<ECommerce>(s => s.Index(IndexName).Size(10000)
            .Query(q => q
                .Range(r => r
                    .NumberRange(nr => nr
                        .Field(f => f.TaxfulTotalPrice)
                            .Gte(fromPrice)
                                .Lte(toPrice)))));

            return result.Documents.ToImmutableList();
        }

        public async Task<ImmutableList<ECommerce>> MatchAllQuery()
        { 
            var MatchAllQuery = new MatchAllQuery();
            var result = await _client.SearchAsync<ECommerce>(s => s.Index(IndexName)
                .Size(100)
                .Query(q => q
                    .MatchAll(MatchAllQuery)
                ));

            foreach (var hit in result.Hits)
            {
                hit.Source.Id = hit.Id;
            }

            return result.Documents.ToImmutableList();
        }

        public async Task<ImmutableList<ECommerce>> PaginationQuery(int page = 1, int pageSize = 3)
        {
            var MatchAllQuery = new MatchAllQuery();

            var pageFrom = (page - 1) * pageSize;

            var result = await _client.SearchAsync<ECommerce>(s => s.Index(IndexName)
                .Size(pageSize)
                .From(pageFrom)
                .Query(q => q
                    .MatchAll(MatchAllQuery)
                ));

            foreach (var hit in result.Hits)
            {
                hit.Source.Id = hit.Id;
            }

            return result.Documents.ToImmutableList();
        }

        public async Task<ImmutableList<ECommerce>> WildCarQuery(string customerFullName)
        {
            var result = await _client.SearchAsync<ECommerce>(s => s.Index(IndexName)
                .Query(q => q
                    .Wildcard(w => w
                    .Field(f => f.CustomerFullName
                    .Suffix("keyword"))
                    .Wildcard(customerFullName))
                ));

            foreach (var hit in result.Hits)
            {
                hit.Source.Id = hit.Id;
            }

            return result.Documents.ToImmutableList();
        }

        public async Task<ImmutableList<ECommerce>> FuzzyQuery(string customerName)
        {
            var result = await _client.SearchAsync<ECommerce>(s => s.Index(IndexName)
                .Query(q => q
                    .Fuzzy(w => w
                    .Field(f => f.CustomerFirstName
                    .Suffix("keyword"))
                    .Value(customerName).Fuzziness(new Fuzziness(1)))
                ).Sort(sort => sort.Field(f => f.TaxfulTotalPrice, new FieldSort()
                {
                    Order = SortOrder.Desc
                })));

            foreach (var hit in result.Hits)
            {
                hit.Source.Id = hit.Id;
            }

            return result.Documents.ToImmutableList();
        }

        public async Task<ImmutableList<ECommerce>> MatchQueryFullTextSearch(string categoryName)
        {
            var result = await _client.SearchAsync<ECommerce>(s => s.Index(IndexName)
                .Query(q => q
                    .Match(m => m
                        .Field(f => f.Category)
                        .Query(categoryName).Operator(Operator.And))));

            foreach (var hit in result.Hits)
            {
                hit.Source.Id = hit.Id;
            }

            return result.Documents.ToImmutableList();
        }

        public async Task<ImmutableList<ECommerce>> MatchBoolPrefixFullText(string customerFullName)
        {
            var result = await _client.SearchAsync<ECommerce>(s => s.Index(IndexName)
                .Query(q => q
                    .MatchBoolPrefix(m => m
                        .Field(f => f.CustomerFullName)
                        .Query(customerFullName).Operator(Operator.And))));

            foreach (var hit in result.Hits)
            {
                hit.Source.Id = hit.Id;
            }

            return result.Documents.ToImmutableList();
        }

        public async Task<ImmutableList<ECommerce>> MatchPhrasePrefixFullText(string customerFullName)
        {
            var result = await _client.SearchAsync<ECommerce>(s => s.Index(IndexName)
                .Query(q => q
                    .MatchPhrasePrefix(m => m
                        .Field(f => f.CustomerFullName)
                        .Query(customerFullName))));

            foreach (var hit in result.Hits)
            {
                hit.Source.Id = hit.Id;
            }

            return result.Documents.ToImmutableList();
        }

        public async Task<ImmutableList<ECommerce>> CompoundQueryExampleOne(string cityName, double taxfulTotalPrice, string categoryName, string manufacturer)
        {
            var result = await _client.SearchAsync<ECommerce>(s => s.Index(IndexName)
                .Size(1000).Query(q => q
                .Bool(b => b
                    .Must(m => m
                        .Term(t => t
                            .Field("geoip.city_name")
                                .Value(cityName)))
                    .MustNot(mn => mn
                        .Range(r => r
                            .NumberRange(nr => nr
                                .Field(f => f.TaxfulTotalPrice)
                                .Lte(taxfulTotalPrice))))
                    .Should(s => s
                        .Term(t => t
                            .Field(f => f
                                .Suffix("keyword"))
                                .Value(categoryName)))
                    .Filter(f => f
                        .Term(t => t
                            .Field("manufacturer.keyword")
                            .Value(manufacturer)))
                )));

            foreach (var hit in result.Hits)
            {
                hit.Source.Id = hit.Id;
            }

            return result.Documents.ToImmutableList();
        }

        public async Task<ImmutableList<ECommerce>> CompoundQueryExampleTwo(string customerFullName)
        {
            var result = await _client.SearchAsync<ECommerce>(s => s.Index(IndexName)
                .Size(1000).Query(q => q
                .Bool(b => b
                    .Should(m => m
                        .Match(t => t
                            .Field(f => f.CustomerFullName)
                            .Query(customerFullName))
                        .Prefix(p => p.Field(f => f.CustomerFullName).Value(customerFullName)))
                )));

            //var result = await _client.SearchAsync<ECommerce>(s => s.Index(IndexName)
            //    .Size(1000).Query(q => q
            //    .MatchPhrasePrefix(m => m
            //        .Field(f => f.CustomerFullName)
            //            .Query(customerFullName)
            //    )));

            foreach (var hit in result.Hits)
            {
                hit.Source.Id = hit.Id;
            }

            return result.Documents.ToImmutableList();
        }

        public async Task<ImmutableList<ECommerce>> MultiMatchQueryFullTextSearch(string name)
        {
            var result = await _client.SearchAsync<ECommerce>(s => s
                .Index(IndexName)
                .Query(q => q
                    .MultiMatch(mm => mm
                        .Fields(new[] { "customer_first_name", "customer_last_name", "customer_full_name" })
                        .Query(name)
                    )
                )
            );

            foreach (var hit in result.Hits)
            {
                hit.Source.Id = hit.Id;
            }

            return result.Documents.ToImmutableList();
        }
    }
}
