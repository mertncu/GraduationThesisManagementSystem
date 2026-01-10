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
