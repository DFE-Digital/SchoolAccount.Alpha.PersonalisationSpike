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
            var organisations = await apiService.GetUserOrganisations(user.DsiId);
            foreach (var dsiOrganisation in organisations)
            {
                var orgDetails = await acService.GetOrganisationDetails(dsiOrganisation.Ukprn);
                if (orgDetails != null)
                {
                    dsiOrganisation.PercentageFsm = orgDetails?.Census?.PercentageFsm ?? string.Empty;
                }
            }

            return View(new UserViewModel { Name = user.GivenName, LastName = user.LastName, Organisations = organisations });

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
