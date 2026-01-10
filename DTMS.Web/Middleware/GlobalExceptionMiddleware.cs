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
        bool isAjax = IsAjaxRequest(context);
        var (statusCode, message) = GetResponseDetails(exception);
        if (isAjax)
        {
            context.Response.ContentType = "application/json";
            context.Response.StatusCode = (int)statusCode;

            var response = new { error = message, status = statusCode };
            await context.Response.WriteAsync(JsonSerializer.Serialize(response));
        }
        else
        {

            if (exception is ValidationException || exception is BusinessRuleException)
            {
                 var returnUrl = context.Request.Headers["Referer"].ToString();
                 if (string.IsNullOrEmpty(returnUrl)) returnUrl = "/";
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
                 context.Response.Redirect($"/Home/Error?code=404&message={WebUtility.UrlEncode(message)}");
            }
            else
            {
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
            ValidationException ve => (HttpStatusCode.BadRequest, ve.Message),
            BusinessRuleException bre => (HttpStatusCode.BadRequest, bre.Message),
            NotFoundException nfe => (HttpStatusCode.NotFound, nfe.Message),
            UnauthorizedException _ => (HttpStatusCode.Unauthorized, "Unauthorized"),
            UnauthorizedAccessException _ => (HttpStatusCode.Unauthorized, "Unauthorized"),
            ForbiddenException _ => (HttpStatusCode.Forbidden, "Forbidden"),
            _ => (HttpStatusCode.InternalServerError, "An unexpected error occurred.")
        };
    }
}