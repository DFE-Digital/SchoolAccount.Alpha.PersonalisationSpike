using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;

namespace SchoolAccount.Alpha.Controllers;

public class ErrorController(ILogger<ErrorController> logger) : Controller
{
    [Route("/Error")]
    public IActionResult Index()
    {
        var exceptionFeature = HttpContext.Features.Get<IExceptionHandlerPathFeature>();

        if (exceptionFeature != null)
        {
            logger.LogError(exceptionFeature.Error,
                "Unhandled exception occurred at {Path}",
                exceptionFeature.Path);
        }

        return View("Error");
    }

    [Route("/Error/{statusCode}")]
    public IActionResult ErrorStatusCode(int statusCode)
    {
        logger.LogWarning("HTTP {StatusCode} error occurred at {Path}",
            statusCode, HttpContext.Request.Path);

        return View("StatusCode", statusCode);
    }
}
