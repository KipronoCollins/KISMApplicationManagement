using KISMApplicationManagement.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;

namespace KISMApplicationManagement.Data
{
    public static class IdentityDataSeeder
    {
        private static readonly string[] Roles =
        {
            "Applicant",
            "Admissions Officer",
            "Committee Member",
            "Admissions Administrator",
            "System Administrator"
        };

        private const string InitialAdministratorEmail =
            "test@kism.ac.ke";

        private const string InitialAdministratorPassword =
            "KismAdmin@2026";

        private const string InitialAdministratorRole =
            "System Administrator";

        public static async Task SeedRolesAsync(
            IServiceProvider serviceProvider)
        {
            var roleManager =
                serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();

            var userManager =
                serviceProvider.GetRequiredService<UserManager<ApplicationUser>>();

            /*
             * =========================================================
             * 1. CREATE KISM ROLES
             * =========================================================
             */

            foreach (var roleName in Roles)
            {
                if (await roleManager.RoleExistsAsync(roleName))
                {
                    continue;
                }

                var role = new IdentityRole(roleName);

                var result =
                    await roleManager.CreateAsync(role);

                if (!result.Succeeded)
                {
                    var errors = string.Join(
                        "; ",
                        result.Errors.Select(error =>
                            $"{error.Code}: {error.Description}")
                    );

                    throw new InvalidOperationException(
                        $"Failed to create role '{roleName}'. {errors}"
                    );
                }
            }

            /*
             * =========================================================
             * 2. FIND INITIAL ADMINISTRATOR
             * =========================================================
             */

            var administrator =
                await userManager.FindByEmailAsync(
                    InitialAdministratorEmail
                );

            /*
             * =========================================================
             * 3. CREATE INITIAL ADMINISTRATOR IF MISSING
             * =========================================================
             */

            if (administrator == null)
            {
                administrator = new ApplicationUser
                {
                    UserName = InitialAdministratorEmail,
                    Email = InitialAdministratorEmail,

                    EmailConfirmed = true,

                    FirstName = "System",
                    MiddleName = null,
                    LastName = "Administrator",

                    IsActive = true,

                    CreatedAtUtc = DateTime.UtcNow,

                    LastLoginAtUtc = null
                };

                var createResult =
                    await userManager.CreateAsync(
                        administrator,
                        InitialAdministratorPassword
                    );

                if (!createResult.Succeeded)
                {
                    var errors = string.Join(
                        "; ",
                        createResult.Errors.Select(error =>
                            $"{error.Code}: {error.Description}")
                    );

                    throw new InvalidOperationException(
                        $"Failed to create initial administrator " +
                        $"'{InitialAdministratorEmail}'. {errors}"
                    );
                }
            }
            else
            {
                /*
                 * =====================================================
                 * 4. REPAIR EXISTING ADMINISTRATOR
                 * =====================================================
                 */

                var userChanged = false;

                if (string.IsNullOrWhiteSpace(
                    administrator.FirstName))
                {
                    administrator.FirstName = "System";
                    userChanged = true;
                }

                if (string.IsNullOrWhiteSpace(
                    administrator.LastName))
                {
                    administrator.LastName = "Administrator";
                    userChanged = true;
                }

                if (string.IsNullOrWhiteSpace(
                    administrator.UserName))
                {
                    administrator.UserName =
                        InitialAdministratorEmail;

                    userChanged = true;
                }

                if (string.IsNullOrWhiteSpace(
                    administrator.Email))
                {
                    administrator.Email =
                        InitialAdministratorEmail;

                    userChanged = true;
                }

                if (!administrator.EmailConfirmed)
                {
                    administrator.EmailConfirmed = true;
                    userChanged = true;
                }

                if (administrator.CreatedAtUtc == default)
                {
                    administrator.CreatedAtUtc =
                        DateTime.UtcNow;

                    userChanged = true;
                }

                if (!administrator.IsActive)
                {
                    administrator.IsActive = true;
                    userChanged = true;
                }

                if (userChanged)
                {
                    administrator.UpdatedAtUtc =
                        DateTime.UtcNow;

                    var updateResult =
                        await userManager.UpdateAsync(
                            administrator
                        );

                    if (!updateResult.Succeeded)
                    {
                        var errors = string.Join(
                            "; ",
                            updateResult.Errors.Select(error =>
                                $"{error.Code}: {error.Description}")
                        );

                        throw new InvalidOperationException(
                            $"Failed to update initial administrator " +
                            $"'{InitialAdministratorEmail}'. {errors}"
                        );
                    }
                }
            }

            /*
             * =========================================================
             * 5. ENSURE SYSTEM ADMINISTRATOR ROLE
             * =========================================================
             */

            if (!await userManager.IsInRoleAsync(
                    administrator,
                    InitialAdministratorRole))
            {
                var roleResult =
                    await userManager.AddToRoleAsync(
                        administrator,
                        InitialAdministratorRole
                    );

                if (!roleResult.Succeeded)
                {
                    var errors = string.Join(
                        "; ",
                        roleResult.Errors.Select(error =>
                            $"{error.Code}: {error.Description}")
                    );

                    throw new InvalidOperationException(
                        $"Failed to assign role " +
                        $"'{InitialAdministratorRole}' to " +
                        $"'{InitialAdministratorEmail}'. {errors}"
                    );
                }
            }
        }
    }
}