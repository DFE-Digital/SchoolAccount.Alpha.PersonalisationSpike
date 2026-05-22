using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SchoolAccount.Alpha.Services;
using SchoolAccount.Alpha.ViewModels;

namespace SchoolAccount.Alpha.Controllers
{
    [Authorize]
    public class HomeController(ILogger<HomeController> logger,
        IDsiApiService dsiApiService,
        IAcademiesApiService academiesApiService,
        ICollectApiService collectApiService) : Controller
    {

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
            return View(new UserViewModel(user, filteredOrgs));
        }

        public async Task<IActionResult> School(string ukprn)
        {
            if (string.IsNullOrEmpty(ukprn))
            {
                return BadRequest("UKPRN is required");
            }

            var user = UserService.GetUser(User.Claims);

            var schoolDetails = await academiesApiService.GetOrganisationDetails(ukprn);
            if (schoolDetails == null)
            {
                return NotFound($"School with UKPRN {ukprn} not found");
            }

            var censusSummary = await collectApiService.GetSchoolCensusStatus(schoolDetails.Laestab);

            return View(new SchoolViewModel(user, schoolDetails, censusSummary));
        }

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

            var laestabs = trust.Establishments.Select(e => e.Laestab).ToList();
            string censusHeadline = await collectApiService.GetTrustCensusHeadline(laestabs);

            return View(new GroupViewModel(trust, censusHeadline));
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
