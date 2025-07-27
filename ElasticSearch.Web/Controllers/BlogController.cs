using ElasticSearch.Web.Models;
using ElasticSearch.Web.Services;
using ElasticSearch.Web.ViewModels;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace ElasticSearch.Web.Controllers
{
    public class BlogController : Controller
    {
        private readonly BlogService _blogService;

        public BlogController(BlogService blogService)
        {
            _blogService = blogService;
        }
        public IActionResult Save()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Save(BlogCreateViewModel model)
        {
            var isSuccess = await _blogService.SaveAsync(model);

            if (!isSuccess)
            {
                TempData["result"] = "kayıt başarısız";
                return RedirectToAction(nameof(BlogController.Save));
            }

            TempData["result"] = "kayıt başarılı";
            return RedirectToAction(nameof(BlogController.Save));
        }

        public async Task<IActionResult> Search()
        {
            return View(await _blogService.SearchAsync(string.Empty));
        }

        [HttpPost]
        public async Task<IActionResult> Search(string searchText)
        {
            ViewBag.searchText = searchText;

            var blogs = await _blogService.SearchAsync(searchText);

            return View(blogs);
        }
    }
}
