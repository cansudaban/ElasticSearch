using ElasticSearch.Web.Repositories;
using ElasticSearch.Web.ViewModels;

namespace ElasticSearch.Web.Services
{
    public class ECommerceService
    {
        private readonly ECommerceRepository _eCommerceRepository;
        public ECommerceService(ECommerceRepository eCommerceRepository)
        {
            _eCommerceRepository = eCommerceRepository;
        }

        public async Task<(List<ECommerceViewModel>, long totalCount, long pageLinkCount)> SearchAsync(ECommerceSearchViewModel searchModel, int page, int pageSize)
        {
            var (eCommerceList, totalCount) = await _eCommerceRepository.SearchAsync(searchModel, page, pageSize);

            var pageLinkCountCalculate = totalCount % pageSize;
            long pageLinkCount = 0;

            if (pageLinkCountCalculate == 0)
            {
                pageLinkCount = totalCount / pageSize;
            }
            else
            {
                pageLinkCount = (totalCount / pageSize) + 1;
            }

            var eCommerceListViewModel = eCommerceList.Select(x => new ECommerceViewModel()
            {
                Category = string.Join(",", x.Category),
                CustomerFirstName = x.CustomerFirstName,
                CustomerLastName = x.CustomerLastName,
                CustomerFullName = x.CustomerFullName,
                OrderDate = x.OrderDate.ToShortDateString(),
                Gender = x.Gender.ToLower(),
                Id = x.Id,
                OrderId = x.OrderId,
                TaxfulTotalPrice = x.TaxfulTotalPrice
            }).ToList();

            return (eCommerceListViewModel, totalCount, pageLinkCount);
        }
    }
}
