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


            var trustLaestabs = trust?.Establishments.Select(e => e.Laestab).ToList() ?? new List<string>();
            if (trustLaestabs.Any())
            {
                var censusDetails =
                    await collectApiService.GetSubmissionSummaries("SchoolCensus 2025_Spring", trustLaestabs);
                CensusTrustViewModel viewModel = new CensusTrustViewModel()
                {
                    TrustName = trust.GiasData.GroupName,
                    TrustUkprn = ukprn,
                    CensusDetails = censusDetails.Select(c => new CensusDetailsViewModel()
                    {
                        Name = c.SchoolName,
                        Errors = c.Errors,
                        Queries = c.Queries,
                        OkdErrorsQueries = c.OkdErrorsQueries,
                        Status = c.ReturnStatus,
                        SubmittedDate = c.SubmittedDate,
                        ApprovedDate = c.ApprovedDate,
                        AuthorisedDate = c.AuthorisedDate
                    }).ToList()
                };
                return View(viewModel);
            }

            return NotFound($"Trust with UKPRN {ukprn} not found");
        }
    }
}



