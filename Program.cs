using KISMApplicationManagement.Data;
using KISMApplicationManagement.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// KISM Application Management Database
var connectionString =
    builder.Configuration.GetConnectionString("DefaultConnection")
    ?? throw new InvalidOperationException(
        "Connection string 'DefaultConnection' was not found.");

builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseMySQL(connectionString));

// ASP.NET Core Identity
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

// MVC
builder.Services.AddControllersWithViews();

var app = builder.Build();

// Seed Identity roles and initial administrator
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

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.MapRazorPages();
// Start of password
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
// end of password
app.Run();
