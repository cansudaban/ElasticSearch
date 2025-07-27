using ElasticSearch.Web.Services;
using ElasticSearch.Web.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace ElasticSearch.Web.Controllers
{
    public class ECommerceController : Controller
    {
        private readonly ECommerceService _eCommerceService;

        public ECommerceController(ECommerceService eCommerceService)
        {
            _eCommerceService = eCommerceService;
        }

        public async Task<IActionResult> Search([FromQuery] SearchPageViewModel searchPageView)
        {
            var (list, totalCount, pageLinkCount) = await _eCommerceService.SearchAsync(searchPageView.SearchViewModel, searchPageView.Page, searchPageView.PageSize);

            searchPageView.List = list;
            searchPageView.TotalCount = totalCount;
            searchPageView.PageLinkCount = pageLinkCount;

            return View(searchPageView);
        }
    }
}
