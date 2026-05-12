using Microsoft.AspNetCore.Mvc;
using SchoolAccount.Alpha.Services;
using SchoolAccount.Alpha.ViewModels;

namespace SchoolAccount.Alpha.Controllers
{
    public class ContentController(IGovUkSearchService govUkSearchService) : Controller
    {

        public async Task<IActionResult> Latest(int pageNo = 1)
        {
            const int PageSize = 12;
            var results = await govUkSearchService.GetLatestDfeDocs(PageSize, pageNo);
            var model = new ContentViewModel()
            {
                                CurrentPage = pageNo,
                                TotalPages = (int)Math.Ceiling((decimal)results.Total / PageSize),
                                Results = results.Results
            };
            return  View(model);
        }
    }
}
