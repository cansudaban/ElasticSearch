using ElasticSearch.Web.Models;
using ElasticSearch.Web.Repositories;
using ElasticSearch.Web.ViewModels;

namespace ElasticSearch.Web.Services
{
    public class BlogService
    {
        private readonly BlogRepository _blogRepository;
        public BlogService(BlogRepository blogRepository)
        {
            _blogRepository = blogRepository;
        }
        public async Task<Blog?> SaveAsync(Blog newBlog)
        {
            if (newBlog == null)
            {
                throw new ArgumentNullException(nameof(newBlog), "Blog cannot be null");
            }
            return await _blogRepository.SaveAsync(newBlog);
        }

        public async Task<bool> SaveAsync(BlogCreateViewModel model)
        {
            Blog newBlog = new Blog
            {
                Title = model.Title,
                Content = model.Content,
                Tags = model.Tags.Split(","),
                UserId = Guid.NewGuid()
            };

            var isCreated = await _blogRepository.SaveAsync(newBlog);

            return isCreated != null;
        }

        public async Task<List<BlogViewModel>> SearchAsync(string searchText)
        {
            var blogList = await _blogRepository.SearchAsync(searchText);

            return blogList.Select(b => new BlogViewModel()
            {
                Id = b.Id,
                Title = b.Title,
                Content = b.Content,
                Tags = String.Join(",", b.Tags),
                Created = b.Created.ToShortDateString(),
                UserId = b.UserId.ToString()
            }).ToList();
        }
    }
}
