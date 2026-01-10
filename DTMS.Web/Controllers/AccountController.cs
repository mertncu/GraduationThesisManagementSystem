using System.Security.Claims;
using GTMS.Application.Features.Identity.Auth.Commands.Login;
using GTMS.Application.Features.Identity.Auth.Commands.Register;
using MediatR;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;

namespace DTMS.Web.Controllers;

public class AccountController : Controller
{
    private readonly IMediator _mediator;

    public AccountController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet]
    public IActionResult Login()
    {
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> Login(LoginViewModel model)
    {
        if (!ModelState.IsValid) return View(model);

        var command = new LoginCommand(model.Email, model.Password);

        try
        {
            var response = await _mediator.Send(command);

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, response.UserId.ToString()),
                new Claim(ClaimTypes.Name, $"{response.FirstName} {response.LastName}"),
                new Claim(ClaimTypes.Role, response.UserRole),
                new Claim("Token", response.AccessToken) 
            };
            
            var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            var authProperties = new AuthenticationProperties
            {
                IsPersistent = true,
                ExpiresUtc = response.ExpiresAt
            };

            await HttpContext.SignInAsync(
                CookieAuthenticationDefaults.AuthenticationScheme,
                new ClaimsPrincipal(claimsIdentity),
                authProperties);

            if (response.UserRole == "Admin")
            {
                return RedirectToAction("Index", "Admin");
            }

            return RedirectToAction("Index", "Home");
        }
        catch (Exception ex)
        {
            ModelState.AddModelError(string.Empty, "Login failed: " + ex.Message);
            return View(model);
        }
    }

    [HttpGet]
    public IActionResult Register()
    {
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> Register(RegisterViewModel model)
    {
        if (!ModelState.IsValid) return View(model);

        if (model.Password != model.ConfirmPassword)
        {
             ModelState.AddModelError("ConfirmPassword", "Passwords do not match.");
             return View(model);
        }

        var command = new RegisterCommand(model.FirstName, model.LastName, model.Email, model.Password, model.StudentNumber);

        try
        {
            await _mediator.Send(command);
            return RedirectToAction("Login");
        }
        catch (Exception ex)
        {
            ModelState.AddModelError(string.Empty, "Registration failed: " + ex.Message);
            return View(model);
        }
    }

    public async Task<IActionResult> Logout()
    {
        await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
        return RedirectToAction("Login");
    }

    public class LoginViewModel
    {
        public string Email { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
    }

    public class RegisterViewModel
    {
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public string ConfirmPassword { get; set; } = string.Empty;
        public string? StudentNumber { get; set; }
    }
}
