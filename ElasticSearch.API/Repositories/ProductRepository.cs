using Elastic.Clients.Elasticsearch;
using ElasticSearch.API.DTOs;
using ElasticSearch.API.Models;
using System.Collections.Immutable;

namespace ElasticSearch.API.Repositories
{
    public class ProductRepository
    {
        private readonly ElasticsearchClient _client;
        private const string IndexName = "products";

        public ProductRepository(ElasticsearchClient client)
        {
            _client = client;
        }

        public async Task<Product?> SaveAsync(Product product)
        {
            product.Created = DateTime.Now;

            var response = await _client.IndexAsync(product, x => x.Index(IndexName).Id(Guid.NewGuid().ToString()));

            if (!response.IsSuccess())
            {
                return null;
            }

            product.Id = response.Id;

            return product;
        }

        public async Task<IReadOnlyCollection<Product>> GetAllAsync()
        {
            var result = await _client.SearchAsync<Product>(s => s
                .Index(IndexName)
                .Query(q => q.MatchAll(_ => { }))
            );

            foreach (var item in result.Hits)
            {
                item.Source.Id = item.Id;
            }

            return result.Documents.ToImmutableList();
        }

        public async Task<Product?> GetByIdAsync(string id)
        {
            var response = await _client.GetAsync<Product>(id, x => x.Index(IndexName));
            if (!response.IsSuccess() || !response.Found)
            {
                return null;
            }
            response.Source.Id = response.Id;
            return response.Source;
        }

        public async Task<bool> UpdateAsync(ProductUpdateDto updateProduct)
        {
            var response = await _client.UpdateAsync<Product, ProductUpdateDto>(IndexName, updateProduct.Id, x => x.Doc(updateProduct));

            return response.IsSuccess() && response.Result == Result.Updated;
        }

        public async Task<DeleteResponse> DeleteAsync(string id)
        {
            var response = await _client.DeleteAsync<Product>(id, x => x.Index(IndexName));

            return response;
        }
    }
}
