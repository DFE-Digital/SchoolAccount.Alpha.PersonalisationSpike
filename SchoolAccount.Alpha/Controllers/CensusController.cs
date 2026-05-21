using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SchoolAccount.Alpha.Services;
using SchoolAccount.Alpha.ViewModels;

namespace SchoolAccount.Alpha.Controllers
{
    [Authorize]
    public class CensusController(IAcademiesApiService academiesApiService, ICollectApiService collectApiService)
        : Controller
    {
        public async Task<IActionResult> Trust(string ukprn)
        {
            if (string.IsNullOrEmpty(ukprn))
            {
                return BadRequest("UKPRN is required");
            }

            var trust = await academiesApiService.GetTrustDetails(ukprn);
            if (trust == null)
            {
                return NotFound($"Trust with UKPRN {ukprn} not found");
            }

            var trustLaestabs = trust.Establishments.Select(e => e.Laestab).ToList();
            if (!trustLaestabs.Any())
            {
                return NotFound($"Trust with UKPRN {ukprn} has no establishments");
            }

            var censusDetails = await collectApiService.GetSubmissionSummaries("SchoolCensus 2025_Spring", trustLaestabs);

            return View(new CensusTrustViewModel(trust, censusDetails));
        }
    }
}



