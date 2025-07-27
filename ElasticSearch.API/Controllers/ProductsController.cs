using ElasticSearch.API.DTOs;
using ElasticSearch.API.Services;
using Microsoft.AspNetCore.Mvc;

namespace ElasticSearch.API.Controllers
{
    public class ProductsController : BaseController
    {
        private readonly ProductService _productService;

        public ProductsController(ProductService productService)
        {
            _productService = productService;
        }

        [HttpPost]
        public async Task<IActionResult> Save(ProductCreateDto request)
        {
            return CreateActionResult(await _productService.SaveAsync(request));
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var response = await _productService.GetAllAsync();
            return CreateActionResult(ResponseDto<List<ProductDto>>.Success(response, System.Net.HttpStatusCode.OK));
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(string id)
        {
            return CreateActionResult(ResponseDto<List<ProductDto>>.Success(await _productService.GetByIdAsync(id), System.Net.HttpStatusCode.OK));
        }

        [HttpPut]
        public async Task<IActionResult> Update(ProductUpdateDto request)
        {
            return CreateActionResult(await _productService.UpdateAsync(request));
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(string id)
        {
            return CreateActionResult(await _productService.DeleteAsync(id));
        }
    }
}
