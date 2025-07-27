using Elastic.Clients.Elasticsearch;
using ElasticSearch.API.DTOs;
using ElasticSearch.API.Models;
using ElasticSearch.API.Repositories;
using System.Collections.Immutable;
using System.Net;

namespace ElasticSearch.API.Services
{
    public class ProductService
    {
        private readonly ProductRepository _repository;
        private readonly ILogger<ProductService> _logger;
        public ProductService(ProductRepository repository, ILogger<ProductService> logger)
        {
            _repository = repository;
            _logger = logger;
        }

        public async Task<ResponseDto<ProductDto>> SaveAsync(ProductCreateDto request)
        {
            var response = await _repository.SaveAsync(request.CreateProduct());

            if (response is null)
            {
                return ResponseDto<ProductDto>.Fail(new List<string> { "kayıt esnasında hata oluştu" }, HttpStatusCode.InternalServerError);
            }

            return ResponseDto<ProductDto>.Success(response.CreateDto(), HttpStatusCode.Created);
        }

        public async Task<ResponseDto<List<ProductDto>>> GetAllAsync()
        {
            var products = await _repository.GetAllAsync();

            var productListDto = new List<ProductDto>();

            foreach (var product in products)
            {
                if (product.Feature is null)
                {
                    productListDto.Add(new ProductDto(product.Id, product.Name, product.Price, product.Stock, null));
                    continue;
                }

                productListDto.Add(new ProductDto(product.Id, product.Name, product.Price, product.Stock,
                        new ProductFeatureDto(product.Feature.Width, product.Feature.Height, product.Feature.Color.ToString())));
            }

            return ResponseDto<List<ProductDto>>.Success(productListDto, HttpStatusCode.OK);
        }

        public async Task<ResponseDto<ProductDto>> GetByIdAsync(string id)
        {
            var product = await _repository.GetByIdAsync(id);
            if (product is null)
            {
                return ResponseDto<ProductDto>.Fail("Product not found", HttpStatusCode.NotFound);
            }

            return ResponseDto<ProductDto>.Success(product.CreateDto(), HttpStatusCode.OK);
        }

        public async Task<ResponseDto<bool>> UpdateAsync(ProductUpdateDto updateProduct)
        {
            var isSuccess = await _repository.UpdateAsync(updateProduct);

            if (!isSuccess)
            {
                return ResponseDto<bool>.Fail(new List<string> { "update esnasında bir hata meydana geldi." }, HttpStatusCode.InternalServerError);
            }

            return ResponseDto<bool>.Success(true, HttpStatusCode.NoContent);
        }

        public async Task<ResponseDto<bool>> DeleteAsync(string id)
        {
            var deleteResponse = await _repository.DeleteAsync(id);

            if (!deleteResponse.IsValidResponse && deleteResponse.Result == Result.NotFound)
            {
                return ResponseDto<bool>.Fail("Silmeye çalıştığınız kayıt bulunamamıştır", HttpStatusCode.InternalServerError);
            }

            if (!deleteResponse.IsValidResponse)
            {
                deleteResponse.TryGetOriginalException(out Exception? exception);
                _logger.LogError(exception, deleteResponse.ElasticsearchServerError?.Error?.Reason);
                return ResponseDto<bool>.Fail(new List<string> { "delete esnasında bir hata meydana geldi." }, HttpStatusCode.InternalServerError);
            }
            return ResponseDto<bool>.Success(true, HttpStatusCode.NoContent);
        }
    }
}
