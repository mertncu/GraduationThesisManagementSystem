using GTMS.Persistence;
using GTMS.Application;
using GTMS.Infrastructure;
using DTMS.Web.Services;
using GTMS.Application.Common.Interfaces;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddApplicationServices();
builder.Services.AddPersistenceServices(builder.Configuration);
builder.Services.AddInfrastructureServices();
builder.Services.AddHttpContextAccessor();
builder.Services.AddScoped<ICurrentUserService, CurrentUserService>();

builder.Services.AddAuthentication(Microsoft.AspNetCore.Authentication.Cookies.CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/Account/Login";
        options.AccessDeniedPath = "/Account/AccessDenied";
    });

builder.Services.AddControllersWithViews();

builder.Services.AddScoped<GTMS.Persistence.Seeding.DbSeeder>();

var app = builder.Build();

// Global Exception Handler
app.UseMiddleware<DTMS.Web.Middleware.GlobalExceptionMiddleware>();

if (!app.Environment.IsDevelopment())
{
    // The middleware handles exceptions, but HSTS is still needed
    app.UseHsts();
}

using (var scope = app.Services.CreateScope())
{
    var seeder = scope.ServiceProvider.GetRequiredService<GTMS.Persistence.Seeding.DbSeeder>();
    await seeder.SeedAsync();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Account}/{action=Login}/{id?}");

app.Run();