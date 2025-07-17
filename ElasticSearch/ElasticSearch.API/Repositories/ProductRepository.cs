using ElasticSearch.API.DTOs;
using ElasticSearch.API.Models;
using Nest;
using System.Collections.Immutable;

namespace ElasticSearch.API.Repositories
{
    public class ProductRepository
    {
        private readonly ElasticClient _client;
        private const string IndexName = "products";

        public ProductRepository(ElasticClient client)
        {
            _client = client;
        }

        public async Task<Product?> SaveAsync(Product product)
        {
            product.Created = DateTime.Now;

            var response = await _client.IndexAsync(product, x => x.Index(IndexName).Id(Guid.NewGuid().ToString()));

            if (!response.IsValid)
            {
                return null;
            }

            product.Id = response.Id;

            return product;
        }

        public async Task<IReadOnlyCollection<Product>> GetAllAsync()
        {
            var response = await _client.SearchAsync<Product>(s => s.Index(IndexName).Query(q => q.MatchAll()));

            foreach (var item in response.Hits)
            {
                item.Source.Id = item.Id;
            }

            return response.Documents.ToImmutableList();
        }

        public async Task<Product?> GetByIdAsync(string id)
        {
            var response = await _client.GetAsync<Product>(id, x => x.Index(IndexName));
            if (!response.IsValid || !response.Found)
            {
                return null;
            }
            response.Source.Id = response.Id;
            return response.Source;
        }

        public async Task<bool> UpdateAsync(ProductUpdateDto updateProduct)
        {
            var response = await _client.UpdateAsync<Product, object>(updateProduct.Id, u => u
                .Index(IndexName)
                .Doc(new
                {
                    updateProduct.Name,
                    updateProduct.Price,
                    updateProduct.Stock,
                    updateProduct.Feature.Width,
                    updateProduct.Feature.Height,
                    Color = updateProduct.Color.ToString(),
                    Updated = DateTime.UtcNow
                }));

            return response.IsValid && response.Result == Result.Updated;
        }

        public async Task<DeleteResponse> DeleteAsync(string id)
        {
            var response = await _client.DeleteAsync<Product>(id, x => x.Index(IndexName));

            return response;
        }
    }
}
