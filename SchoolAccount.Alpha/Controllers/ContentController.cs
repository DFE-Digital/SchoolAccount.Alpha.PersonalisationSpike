using Microsoft.AspNetCore.Mvc;
using SchoolAccount.Alpha.Services;
using SchoolAccount.Alpha.ViewModels;

namespace SchoolAccount.Alpha.Controllers
{
    public class ContentController(IGovUkSearchService govUkSearchService) : Controller
    {

        const int DefaultPageSize = 12;
        public async Task<IActionResult> Latest(int pageNo)
        {
            pageNo = Math.Max(1, pageNo);
            var results = await govUkSearchService.GetLatestDfeDocs(DefaultPageSize, pageNo);

            var model = new ContentViewModel()
            {
                PageTitle = "Latest DfE publications, newest to oldest",
                Action = "Latest",
                CurrentPage = pageNo,
                TotalPages = results == null ? 0 : (int)Math.Ceiling((decimal)results.Total / DefaultPageSize),
                Results = results?.Results ?? []
            };
            return View("Results", model);
        }

        public IActionResult Search()
        {
            var model = new ContentViewModel()
            {
                PageTitle = "Search DfE publications"
            };
            return View(model);
        }

        public async Task<IActionResult> SearchResults(int pageNo, string query)
        {
            pageNo = Math.Max(1, pageNo);
            var results = await govUkSearchService.SearchDfeDocs(DefaultPageSize, pageNo, query);

            var model = new ContentViewModel()
            {
                PageTitle = $"Documents matching '{query}', ordered by relevance",
                Action = "SearchResults",
                Query = query,
                CurrentPage = pageNo,
                TotalPages = results == null ? 0 : (int)Math.Ceiling((decimal)results.Total / DefaultPageSize),
                Results = results?.Results ?? []
            };
            return View("Results", model);
        }
    }
}
