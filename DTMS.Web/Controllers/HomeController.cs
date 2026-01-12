using GTMS.Application.Common.Interfaces;
using GTMS.Application.Features.Dashboard.Student.Queries.GetStudentDashboard;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using DTMS.Web.Models;

namespace DTMS.Web.Controllers;

[Authorize]
public class HomeController : Controller
{
    private readonly IMediator _mediator;
    private readonly ILogger<HomeController> _logger;

    public HomeController(IMediator mediator, ILogger<HomeController> logger)
    {
        _mediator = mediator;
        _logger = logger;
    }

    public async Task<IActionResult> Index()
    {
        if (User.IsInRole("Student"))
        {
            var model = await _mediator.Send(new GetStudentDashboardQuery());
            if (model != null)
            {
                return View("StudentDashboard", model);
            }
        }
        else if (User.IsInRole("Advisor"))
        {
            // Assuming we have ICurrentUserService to get ID, or we extract from User claims here
            // GetStudentDashboardQuery handled it internally via service, let's do same for Advisor
            // Actually GetStudentDashboardQuery didn't take args, it used current user service inside handler.
            // My new GetAdvisorDashboardQuery takes ID. I should fix that to match pattern OR pass ID here.
            // Let's pass ID here.
            
            var userIdClaim = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            if (Guid.TryParse(userIdClaim, out Guid userId))
            {
                 var model = await _mediator.Send(new GTMS.Application.Features.Dashboard.Advisor.Queries.GetAdvisorDashboard.GetAdvisorDashboardQuery { AdvisorUserId = userId });
                 return View("AdvisorDashboard", model);
            }
        }

        return View();
    }

    public IActionResult Privacy()
    {
        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
