using Microsoft.AspNetCore.Mvc;
using SchoolAccount.Alpha.Services;

namespace SchoolAccount.Alpha.Controllers
{
    public class ContentController(IGovUkSearchService govUkSearchService) : Controller
    {

        public async Task<IActionResult> Latest(int pageNo = 1)
        {
            var results = await govUkSearchService.GetLatestDfeDocs(12, pageNo);
            return  Json(results);
        }
    }
}
