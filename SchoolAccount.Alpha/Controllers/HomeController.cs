using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SchoolAccount.Alpha.Services;
using SchoolAccount.Alpha.ViewModels;
using System.Diagnostics;
using System.Linq;

namespace SchoolAccount.Alpha.Controllers
{
    [Authorize]
    public class HomeController(ILogger<HomeController> logger, IDsiApiService dsiApiService, IAcademiesApiService academiesApiService, ICollectApiService collectApiService) : Controller
    {
        private readonly ILogger<HomeController> _logger = logger;

        [AllowAnonymous]
        public IActionResult Index()
        {
            if (User.Identity?.IsAuthenticated == true)
            {
                return RedirectToAction("Organisations");
            }
            return View("Login");
        }

        public async Task<IActionResult> Organisations()
        {
            var user = UserService.GetUser(User.Claims);
            var allOrgs = await dsiApiService.GetUserOrganisations(user.DsiId);
            // remove orgs that aren't school account related
            var filteredOrgs = allOrgs.Where(o => !string.IsNullOrEmpty(o.Ukprn)).ToList();
            return View(new UserViewModel { Name = user.GivenName, LastName = user.LastName, Organisations = filteredOrgs });
        }

        public async Task<IActionResult> School(string ukprn)
        {
            if (string.IsNullOrEmpty(ukprn))
            {
                return BadRequest("UKPRN is required");
            }

            var user = UserService.GetUser(User.Claims);

            var academyDetails = await academiesApiService.GetOrganisationDetails(ukprn);
            if (academyDetails == null)
            {
                return NotFound($"School with UKPRN {ukprn} not found");
            }

            var censusSummary = await collectApiService.GetSubmissionSummary("SchoolCensus 2025_Spring", academyDetails.Laestab);
            CensusDetailsViewModel? censusDetails = censusSummary == null
                ? null
                : new CensusDetailsViewModel
                {
                    Name = censusSummary.Collection,
                    Status = censusSummary.ReturnStatus,
                    Errors = censusSummary.Errors,
                    Queries = censusSummary.Queries,
                    OkdErrorsQueries = censusSummary.OkdErrorsQueries,
                    SubmittedDate = censusSummary.SubmittedDate,
                    ApprovedDate = censusSummary.ApprovedDate,
                    AuthorisedDate = censusSummary.AuthorisedDate,

                };


            var viewModel = new SchoolViewModel
            {
                UserName = $"{user.GivenName} {user.LastName}",
                SchoolName = academyDetails.EstablishmentName,
                SchoolType = academyDetails.EstablishmentType?.Name ?? "Unknown",
                PhaseOfEducation = academyDetails.PhaseOfEducation?.Name ?? "Unknown",
                NumberOfPupils = academyDetails.Census?.NumberOfPupils ?? "Unknown",
                PercentageEligibleForFsm = academyDetails.Census?.PercentageEligableForFSM6Years ?? "Unknown",
                PercentageFsm = academyDetails.Census?.PercentageFsm ?? "Unknown",
                CensusDetails = censusDetails
            };

            return View(viewModel);
        }

        [Authorize]
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
            string censusSummaryMessage = String.Empty;
            var trustLaestabs = trust.Establishments.Select(e => e.Laestab).ToList();
            if (trustLaestabs.Any())
            {
                var censusDetails = await collectApiService.GetSubmissionSummaries("SchoolCensus 2025_Spring", trustLaestabs);

                if (censusDetails.Any())
                {
                    var totalApproved = censusDetails.Count(d => d.ReturnStatusCode == CollectApiService.ApprovedCode);
                    censusSummaryMessage =
                        $"{censusDetails.Count - totalApproved} of {censusDetails.Count} censuses are incomplete for Spring 2025";
                }
            }

            return View(new GroupViewModel
            {
                TrustName = trust.GiasData?.GroupName ?? "Unknown", 
                TrustUkPrn = trust.GiasData?.Ukprn ?? "Unknown",
                CensusSummaryMessage = censusSummaryMessage,
                Establishments = trust.Establishments
            });
        }

        [AllowAnonymous]
        public IActionResult Login(string? returnUrl = null)
        {
            // If already authenticated, redirect to return URL or home
            if (User.Identity?.IsAuthenticated == true)
            {
                return LocalRedirect(returnUrl ?? "~/");
            }

            returnUrl ??= Url.Action("Index", "Home");

            return Challenge(
                new AuthenticationProperties { RedirectUri = returnUrl },
                OpenIdConnectDefaults.AuthenticationScheme);
        }

        [AllowAnonymous]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            await HttpContext.SignOutAsync(OpenIdConnectDefaults.AuthenticationScheme);

            return RedirectToAction("Index", "Home");
        }
    }
}
