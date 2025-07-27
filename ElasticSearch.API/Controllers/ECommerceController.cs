using ElasticSearch.API.Models.ECommerceModels;
using ElasticSearch.API.Repositories;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Immutable;

namespace ElasticSearch.API.Controllers
{
    [Route("api/[controller]/[action]")]
    public class ECommerceController : ControllerBase
    {
        private readonly ECommerceRepository _eCommerceRepository;

        public ECommerceController(ECommerceRepository eCommerceRepository)
        {
            _eCommerceRepository = eCommerceRepository;
        }

        [HttpGet]
        public async Task<IActionResult> TermQuery(string customerFirstName)
        {
            return Ok(await _eCommerceRepository.TermQuery(customerFirstName));
        }

        [HttpPost]
        public async Task<IActionResult> TermsQuery([FromBody] List<string> customerFirstNameList)
        {
            return Ok(await _eCommerceRepository.TermsQuery(customerFirstNameList));
        }

        [HttpGet]
        public async Task<IActionResult> PrefixQuery(string customerFullName)
        {
            return Ok(await _eCommerceRepository.PrefixQuery(customerFullName));
        }

        [HttpGet]
        public async Task<IActionResult> RangeQuery(double fromPrice, double toPrice)
        {
            return Ok(await _eCommerceRepository.RangeQuery(fromPrice, toPrice));
        }

        [HttpGet]
        public async Task<IActionResult> MatchAllQuery()
        {
            return Ok(await _eCommerceRepository.MatchAllQuery());
        }

        [HttpGet]
        public async Task<IActionResult> PaginationQuery(int page, int pageSize)
        {
            return Ok(await _eCommerceRepository.PaginationQuery(page, pageSize));
        }

        [HttpGet]
        public async Task<IActionResult> WildCarQuery(string customerFullName)
        {
            return Ok(await _eCommerceRepository.WildCarQuery(customerFullName));
        }

        [HttpGet]
        public async Task<IActionResult> FuzzyQuery(string customerName)
        {
            return Ok(await _eCommerceRepository.FuzzyQuery(customerName));
        }

        [HttpGet]
        public async Task<IActionResult> MatchQueryFullTextSearch(string category)
        {
            return Ok(await _eCommerceRepository.MatchQueryFullTextSearch(category));
        }

        [HttpGet]
        public async Task<IActionResult> MatchBoolPrefixFullText(string customerFullName)
        {
            return Ok(await _eCommerceRepository.MatchBoolPrefixFullText(customerFullName));
        }

        [HttpGet]
        public async Task<IActionResult> MatchPhrasePrefixFullText(string customerFullName)
        {
            return Ok(await _eCommerceRepository.MatchPhrasePrefixFullText(customerFullName));
        }

        [HttpGet]
        public async Task<IActionResult> CompoundQueryExampleOne(string cityName, double taxfulTotalPrice, string categoryName, string manufacturer)
        {
            return Ok(await _eCommerceRepository.CompoundQueryExampleOne(cityName, taxfulTotalPrice, categoryName, manufacturer));
        }

        [HttpGet]
        public async Task<IActionResult> CompoundQueryExampleTwo(string customerFullName)
        {
            return Ok(await _eCommerceRepository.CompoundQueryExampleTwo(customerFullName));
        }

        [HttpGet]
        public async Task<IActionResult> MultiMatchQueryFullTextSearch(string name)
        {
            return Ok(await _eCommerceRepository.MultiMatchQueryFullTextSearch(name));
        }
    }
}
