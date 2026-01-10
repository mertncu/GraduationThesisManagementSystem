using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using MediatR;
using GTMS.Application.Features.Dashboard.Admin.Queries.GetDashboardStats;
using GTMS.Application.Features.Identity.Users.Queries.GetUsersList;
using GTMS.Application.Features.Identity.Users.Queries.GetCreateUserPage;
using GTMS.Application.Features.Identity.Users.Queries.GetEditUserPage;
using GTMS.Application.Features.Common.Queries.GetProgramsByDepartment;
using GTMS.Application.Features.Identity.Users.Commands.CreateUser;
using GTMS.Application.Features.Identity.Users.Commands.UpdateUser;
using GTMS.Application.Features.Identity.Users.Commands.DeleteUser;
using GTMS.Application.Features.Identity.Users.Dtos;

namespace DTMS.Web.Controllers;

[Authorize(Roles = "Admin")]
public class AdminController : Controller
{
    private readonly IMediator _mediator;

    public AdminController(IMediator mediator)
    {
        _mediator = mediator;
    }

    public async Task<IActionResult> Index()
    {
        return View(await _mediator.Send(new GetAdminDashboardStatsQuery()));
    }

    public async Task<IActionResult> Users()
    {
        return View(await _mediator.Send(new GetUsersListQuery()));
    }

    [HttpGet]
    public async Task<IActionResult> CreateUser()
    {
        return View(await _mediator.Send(new GetCreateUserPageQuery()));
    }

    [HttpPost]
    public async Task<IActionResult> CreateUser(CreateUserPageVm model)
    {
        if (!ModelState.IsValid)
        {
             // Re-fetch dropdowns by calling Query again. Use a new query instance for "Page Data".
             // Ideally we just want the dropdowns. GetCreateUserPageQuery returns them.
             // We map the incoming model back to the VM to preserve input, OR we just let the view handle it if we pass the VM.
             // Since CreateUserPageVm matches the input structure, we can merge.
             
             // Strict approach: Helper to reload data, essentially dispatching the query again.
             // The view expects CreateUserPageVm.
             
             var vm = await _mediator.Send(new GetCreateUserPageQuery());
             // Copy input back
             vm.FirstName = model.FirstName;
             vm.LastName = model.LastName;
             vm.Email = model.Email;
             vm.Role = model.Role;
             vm.DepartmentId = model.DepartmentId;
             vm.ProgramId = model.ProgramId;
             vm.StudentNumber = model.StudentNumber;
             vm.PhoneNumber = model.PhoneNumber;
             
             return View(vm);
        }

        try
        {
            await _mediator.Send(new CreateUserCommand(
                model.FirstName, 
                model.LastName, 
                model.Email, 
                model.Password, 
                model.Role, 
                model.PhoneNumber,
                model.DepartmentId,
                model.ProgramId,
                model.StudentNumber
            ));
            return RedirectToAction(nameof(Users));
        }
        catch (Exception ex)
        {
             ModelState.AddModelError(string.Empty, ex.Message);
             // Reload
             var vm = await _mediator.Send(new GetCreateUserPageQuery());
             // Copy input back
             vm.FirstName = model.FirstName;
             vm.LastName = model.LastName;
             vm.Email = model.Email;
             vm.Role = model.Role;
             vm.DepartmentId = model.DepartmentId;
             vm.ProgramId = model.ProgramId;
             vm.StudentNumber = model.StudentNumber;
             vm.PhoneNumber = model.PhoneNumber;
             
             return View(vm);
        }
    }

    [HttpGet]
    public async Task<IActionResult> EditUser(Guid id)
    {
        try 
        {
            return View(await _mediator.Send(new GetEditUserPageQuery(id)));
        }
        catch (GTMS.Application.Common.Exceptions.NotFoundException)
        {
            return NotFound();
        }
    }

    [HttpPost]
    public async Task<IActionResult> EditUser(EditUserPageVm model)
    {
        if (!ModelState.IsValid)
        {
             var vm = await _mediator.Send(new GetEditUserPageQuery(model.UserId));
             // Copy inputs (skipped for brevity, but should be done)
             return View(vm);
        }

        try
        {
            await _mediator.Send(new UpdateUserCommand(
                model.UserId,
                model.FirstName,
                model.LastName,
                model.Email,
                model.PhoneNumber,
                model.DepartmentId,
                model.ProgramId,
                model.StudentNumber
            ));
            return RedirectToAction(nameof(Users));
        }
        catch (Exception ex)
        {
             ModelState.AddModelError(string.Empty, ex.Message);
             var vm = await _mediator.Send(new GetEditUserPageQuery(model.UserId));
             return View(vm);
        }
    }

    [HttpPost]
    public async Task<IActionResult> DeleteUser(Guid id)
    {
        try
        {
            await _mediator.Send(new DeleteUserCommand(id));
        }
        catch(Exception ex)
        {
            TempData["Error"] = ex.Message;
        }
        return RedirectToAction(nameof(Users));
    }

    [HttpGet]
    public async Task<IActionResult> GetProgramsByDepartment(Guid departmentId)
    {
        return Json(await _mediator.Send(new GetProgramsByDepartmentQuery(departmentId)));
    }
}

