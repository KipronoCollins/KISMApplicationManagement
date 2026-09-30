using KISMApplicationManagement.Data;
using KISMApplicationManagement.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Routing;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// ============================================================
// KISM APPLICATION MANAGEMENT DATABASE
// ============================================================

var connectionString =
    builder.Configuration.GetConnectionString("DefaultConnection")
    ?? throw new InvalidOperationException(
        "Connection string 'DefaultConnection' was not found.");

builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseMySQL(connectionString));


// ============================================================
// ASP.NET CORE IDENTITY
// ============================================================

builder.Services
    .AddDefaultIdentity<ApplicationUser>(options =>
    {
        options.SignIn.RequireConfirmedAccount = false;

        options.Password.RequireDigit = true;
        options.Password.RequireLowercase = true;
        options.Password.RequireUppercase = true;
        options.Password.RequireNonAlphanumeric = false;
        options.Password.RequiredLength = 8;
    })
    .AddRoles<IdentityRole>()
    .AddEntityFrameworkStores<ApplicationDbContext>();


// ============================================================
// MVC
// ============================================================

builder.Services.AddControllersWithViews();


// ============================================================
// BUILD APPLICATION
// ============================================================

var app = builder.Build();


// ============================================================
// MIDDLEWARE
// ============================================================

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();


// ============================================================
// MVC ROUTING
// ============================================================

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");


// ============================================================
// RAZOR PAGES / ASP.NET CORE IDENTITY
// ============================================================

app.MapRazorPages();


// ============================================================
// COMMAND: routes:list
//
// Usage:
//
//     dotnet run -- routes:list
//
// Lists all registered MVC and Razor endpoints.
// The application does not start Kestrel in this mode.
// Database seeders are also skipped.
// ============================================================

if (args.Contains(
        "routes:list",
        StringComparer.OrdinalIgnoreCase))
{
    IEndpointRouteBuilder endpointRouteBuilder = app;

    var routes = endpointRouteBuilder.DataSources
        .SelectMany(dataSource => dataSource.Endpoints)
        .OfType<RouteEndpoint>()
        .SelectMany(endpoint =>
        {
            var methods = endpoint.Metadata
                .GetMetadata<HttpMethodMetadata>()
                ?.HttpMethods;

            var methodList =
                methods is { Count: > 0 }
                    ? methods
                    : ["ANY"];

            return methodList.Select(method => new
            {
                Method = method,
                Route = endpoint.RoutePattern.RawText ?? "",
                Name = endpoint.Metadata
                    .GetMetadata<IEndpointNameMetadata>()
                    ?.EndpointName,
                DisplayName = endpoint.DisplayName ?? "",
                Order = endpoint.Order
            });
        })
        .OrderBy(x => x.Route)
        .ThenBy(x => x.Method)
        .ToList();

    Console.WriteLine();
    Console.WriteLine(
        "KISM APPLICATION MANAGEMENT - ROUTE LIST");
    Console.WriteLine(
        "=========================================");
    Console.WriteLine();

    Console.WriteLine(
        $"{ "METHOD",-8} " +
        $"{ "ROUTE",-55} " +
        $"{ "NAME",-30} " +
        "DISPLAY NAME");

    Console.WriteLine(
        new string('-', 125));

    foreach (var route in routes)
    {
        Console.WriteLine(
            $"{route.Method,-8} " +
            $"{route.Route,-55} " +
            $"{(route.Name ?? "-"),-30} " +
            $"{route.DisplayName}");
    }

    Console.WriteLine();
    Console.WriteLine(
        $"Total endpoints: {routes.Count}");
    Console.WriteLine();

    return;
}


// ============================================================
// COMMAND: --reset-test-passwords
//
// Usage:
//
//     dotnet run -- --reset-test-passwords
// ============================================================

if (args.Contains(
        "--reset-test-passwords",
        StringComparer.OrdinalIgnoreCase))
{
    using var scope =
        app.Services.CreateScope();

    await PasswordResetSeeder
        .ResetStressUserPasswordsAsync(
            scope.ServiceProvider);

    return;
}


// ============================================================
// DATABASE SEEDING
// ============================================================

using (var scope = app.Services.CreateScope())
{
    var serviceProvider = scope.ServiceProvider;

    await IdentityDataSeeder.SeedRolesAsync(
        serviceProvider);

    var context =
        serviceProvider.GetRequiredService<ApplicationDbContext>();

    await ProgrammeDataSeeder.SeedProgrammesAsync(
        context);
}


// ============================================================
// START APPLICATION
// ============================================================

app.Run();
