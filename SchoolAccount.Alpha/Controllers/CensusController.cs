using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SchoolAccount.Alpha.Services;

namespace SchoolAccount.Alpha.Controllers
{
    [Authorize]
    public class CensusController(ICollectApiService censusService)
        : Controller
    {
        public async Task<IActionResult> Trust(string ukprn)
        {
            var result = await censusService.GetTrustCensusStatuses(ukprn);
            
            if (result == null)
            {
                return NotFound($"Trust with UKPRN {ukprn} not found");
            }

           
            return View("Trust", result);
        }
    }
}



