using System.Net;
using System.Text.Json;
using GTMS.Application.Common.Exceptions;

namespace DTMS.Web.Middleware;

public class GlobalExceptionMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<GlobalExceptionMiddleware> _logger;

    public GlobalExceptionMiddleware(RequestDelegate next, ILogger<GlobalExceptionMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An unhandled exception occurred.");
            await HandleExceptionAsync(context, ex);
        }
    }

    private async Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        // 1. Determine Response Type (JSON vs HTML)
        bool isAjax = IsAjaxRequest(context);

        // 2. Map Exception to Status Code & Message
        var (statusCode, message) = GetResponseDetails(exception);

        // 3. Handle Response
        if (isAjax)
        {
            // API / AJAX Response
            context.Response.ContentType = "application/json";
            context.Response.StatusCode = (int)statusCode;

            var response = new { error = message, status = statusCode };
            await context.Response.WriteAsync(JsonSerializer.Serialize(response));
        }
        else
        {
            // MVC View Response
            // For Validation/Business errors, we usually want to show a message on the same page or a redirect.
            // But middleware can't easily "return View" without knowing the view.
            // Best Practice for Global Handler: Redirect to an Error page OR a generic error handling path.
            
            // However, typical "Try-Catch" in Controller normally puts error in ModelState/TempData and returns View.
            // Global Middleware is TOO HIGH LEVEL to "return View(model)".
            // STRATEGY:
            // - Validation/BusinessRule -> Set TempData["Error"] and Redirect to Referer (if possible)
            // - NotFound -> Redirect to 404 Page
            // - Auth -> Redirect to Login
            // - ServerError -> Redirect to /Home/Error

            if (exception is ValidationException || exception is BusinessRuleException)
            {
                 // Store error in TempData (using a cookie or session because response hasn't started?)
                 // Actually, we can't easily access TempData here without ITempDataDictionaryFactory unless we inject it.
                 // A simpler approach for MVC usually involves ExceptionFilters. 
                 // BUT user asked for Middleware. Let's do our best.
                 
                 // If we can't write to TempData easily, we can redirect with error query param? No, ugly.
                 // Let's use the valid "Error" controller notion.
                 
                 // IMPROVEMENT: Use the query string for simple errors if Referer exists.
                 // OR, just redirect to a generic "Error" page with the message.
                 
                 var returnUrl = context.Request.Headers["Referer"].ToString();
                 if (string.IsNullOrEmpty(returnUrl)) returnUrl = "/";
                 
                 // Just redirect to Error page with message for now for critical failures,
                 // BUT for "Business Rules" ideally we want to stay on page.
                 // NOTE: Middleware is bad for "staying on page with form data".
                 // ExceptionFilter is better for MVC.
                 // User asked for Middleware OR IExceptionHandler.
                 // IExceptionHandler is also high level.
                 
                 // DECISION: 
                 // Critical errors (500, 404, 401) -> Redirect.
                 // BusinessLogic (400) -> Ideally caught in Controller? 
                 // User Requirement: "Controllers must NOT contain try-catch".
                 // OK, implies we accept Redirect behavior or we implement a smart Filter.
                 // I will implement a Smart Redirect to /Home/Error?message=... for now.
                 
                 context.Response.Redirect($"/Home/Error?message={WebUtility.UrlEncode(message)}");
            }
            else if (exception is UnauthorizedException || exception is UnauthorizedAccessException)
            {
                context.Response.Redirect("/Account/Login");
            }
            else if (exception is ForbiddenException)
            {
                context.Response.Redirect("/Account/AccessDenied");
            }
            else if (exception is NotFoundException)
            {
                 // Create a specific 404 view or generic error
                 context.Response.Redirect($"/Home/Error?code=404&message={WebUtility.UrlEncode(message)}");
            }
            else
            {
                // 500
                context.Response.Redirect("/Home/Error");
            }
        }
    }

    private static bool IsAjaxRequest(HttpContext context)
    {
        return context.Request.Headers["X-Requested-With"] == "XMLHttpRequest" || 
               context.Request.Headers["Accept"].ToString().Contains("application/json");
    }

    private static (HttpStatusCode code, string message) GetResponseDetails(Exception exception)
    {
        return exception switch
        {
            ValidationException ve => (HttpStatusCode.BadRequest, ve.Message), // Or join errors
            BusinessRuleException bre => (HttpStatusCode.BadRequest, bre.Message),
            NotFoundException nfe => (HttpStatusCode.NotFound, nfe.Message),
            UnauthorizedException _ => (HttpStatusCode.Unauthorized, "Unauthorized"),
            UnauthorizedAccessException _ => (HttpStatusCode.Unauthorized, "Unauthorized"),
            ForbiddenException _ => (HttpStatusCode.Forbidden, "Forbidden"),
            _ => (HttpStatusCode.InternalServerError, "An unexpected error occurred.")
        };
    }
}
