using ElasticSearch.API.Models;

namespace ElasticSearch.API.DTOs
{
    public record ProductUpdateDto(string Id, string Name, decimal Price, int Stock, ProductFeatureDto Feature, EColor Color)
    {
       
    }
}
