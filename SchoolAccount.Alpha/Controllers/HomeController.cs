using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SchoolAccount.Alpha.Services;
using System.Diagnostics;
using SchoolAccount.Alpha.ViewModels;

namespace SchoolAccount.Alpha.Controllers
{
    public class HomeController(ILogger<HomeController> logger, IDsiApiService apiService, IAcademiesApiService acService) : Controller
    {
        private readonly ILogger<HomeController> _logger = logger;

        public IActionResult Index()
        {
            if (User.Identity?.IsAuthenticated == true)
            {
                return RedirectToAction("Organisations");
            }
            return View("Login");
        }

        [Authorize]
        public async Task<IActionResult> Organisations()
        {

            var user = UserService.GetUser(User.Claims);
            var allOrgs = await apiService.GetUserOrganisations(user.DsiId);
            // remove orgs that aren't school account related
            var filteredOrgs = allOrgs.Where(o => !string.IsNullOrEmpty(o.Ukprn)).ToList();
            return View(new UserViewModel { Name = user.GivenName, LastName = user.LastName, Organisations = filteredOrgs });
        }

        [Authorize]
        public async Task<IActionResult> School(string ukprn)
        {
            if (string.IsNullOrEmpty(ukprn))
            {
                return BadRequest("UKPRN is required");
            }

            var user = UserService.GetUser(User.Claims);

            var academyDetails = await acService.GetOrganisationDetails(ukprn);

            if (academyDetails == null)
            {
                return NotFound($"School with UKPRN {ukprn} not found");
            }

            var viewModel = new SchoolViewModel
            {
                UserName = $"{user.GivenName} {user.LastName}",
                SchoolName = academyDetails.EstablishmentName,
                SchoolType = academyDetails.EstablishmentType?.Name ?? "Unknown",
                PhaseOfEducation = academyDetails.PhaseOfEducation?.Name ?? "Unknown",
                NumberOfPupils = academyDetails.Census?.NumberOfPupils ?? "Unknown",
                PercentageEligibleForFsm = academyDetails.Census?.PercentageEligableForFSM6Years ?? "Unknown",
                PercentageFsm = academyDetails.Census?.PercentageFsm ?? "Unknown"
            };

            return View(viewModel);
        }

        [Authorize]
        public async Task<IActionResult> Group(string ukprn)
        {
            if (string.IsNullOrEmpty(ukprn))
            {
                return BadRequest("UKPRN is required");
            }

            var trust = await acService.GetTrustDetails(ukprn);

            if (trust == null)
            {
                return NotFound($"Group with UKPRN {ukprn} not found");
            }

            return View(new GroupViewModel { TrustName = trust.GiasData?.GroupName ?? "Unknown", TrustUkPrn = trust.GiasData?.Ukprn ?? "Unknown", Establishments = trust.Establishments });
        }

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

        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            await HttpContext.SignOutAsync(OpenIdConnectDefaults.AuthenticationScheme);

            return RedirectToAction("Index", "Home");
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
