using KISMApplicationManagement.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace KISMApplicationManagement.Data
{
    public static class PasswordResetSeeder
    {
        private const string TestEmailDomain = "@kism.test";

        private const string TestPassword =
            "StressTest@2026";

        public static async Task ResetStressUserPasswordsAsync(
            IServiceProvider serviceProvider)
        {
            var userManager =
                serviceProvider.GetRequiredService<
                    UserManager<ApplicationUser>>();

            /*
             * =========================================================
             * FIND TEST USERS
             * =========================================================
             *
             * Do the EndsWith filtering in SQL using LIKE.
             *
             * This avoids:
             *
             * EndsWith(..., StringComparison.OrdinalIgnoreCase)
             *
             * which EF Core cannot translate to SQL Server.
             * =========================================================
             */

            var users = await userManager.Users
                .Where(user =>
                    user.Email != null &&
                    EF.Functions.Like(
                        user.Email,
                        "%" + TestEmailDomain))
                .OrderBy(user => user.Email)
                .ToListAsync();

            Console.WriteLine();

            Console.WriteLine(
                "====================================================");

            Console.WriteLine(
                " KISM TEST USER PASSWORD RESET");

            Console.WriteLine(
                "====================================================");

            Console.WriteLine(
                $"Email domain : {TestEmailDomain}");

            Console.WriteLine(
                $"Users found  : {users.Count}");

            Console.WriteLine();

            if (users.Count == 0)
            {
                Console.WriteLine(
                    "No @kism.test users were found.");

                Console.WriteLine();

                return;
            }

            /*
             * =========================================================
             * CONFIRM WHAT WILL BE CHANGED
             * =========================================================
             */

            Console.WriteLine(
                "The following users will have their passwords reset:");

            Console.WriteLine();

            foreach (var user in users)
            {
                Console.WriteLine(
                    $"  {user.Email}");
            }

            Console.WriteLine();

            /*
             * =========================================================
             * RESET PASSWORDS
             * =========================================================
             */

            var successCount = 0;
            var failedCount = 0;

            foreach (var user in users)
            {
                if (string.IsNullOrWhiteSpace(user.Email))
                {
                    failedCount++;

                    Console.WriteLine(
                        "FAILED  User has no email address.");

                    continue;
                }

                try
                {
                    /*
                     * -------------------------------------------------
                     * Remove existing password.
                     * -------------------------------------------------
                     */

                    if (await userManager.HasPasswordAsync(user))
                    {
                        var removeResult =
                            await userManager.RemovePasswordAsync(
                                user);

                        if (!removeResult.Succeeded)
                        {
                            var errors = string.Join(
                                "; ",
                                removeResult.Errors.Select(error =>
                                    $"{error.Code}: {error.Description}")
                            );

                            Console.WriteLine(
                                $"FAILED  {user.Email}"
                            );

                            Console.WriteLine(
                                $"        Remove password: {errors}"
                            );

                            failedCount++;

                            continue;
                        }
                    }

                    /*
                     * -------------------------------------------------
                     * Add new password.
                     *
                     * IMPORTANT:
                     *
                     * UserManager uses the configured ASP.NET Core
                     * Identity password hasher.
                     *
                     * We are NOT hashing the password manually.
                     * -------------------------------------------------
                     */

                    var addResult =
                        await userManager.AddPasswordAsync(
                            user,
                            TestPassword);

                    if (!addResult.Succeeded)
                    {
                        var errors = string.Join(
                            "; ",
                            addResult.Errors.Select(error =>
                                $"{error.Code}: {error.Description}")
                        );

                        Console.WriteLine(
                            $"FAILED  {user.Email}"
                        );

                        Console.WriteLine(
                            $"        Add password: {errors}"
                        );

                        failedCount++;

                        continue;
                    }

                    Console.WriteLine(
                        $"OK      {user.Email}"
                    );

                    successCount++;
                }
                catch (Exception ex)
                {
                    Console.WriteLine(
                        $"FAILED  {user.Email}"
                    );

                    Console.WriteLine(
                        $"        {ex.GetType().Name}: {ex.Message}"
                    );

                    failedCount++;
                }
            }

            /*
             * =========================================================
             * FINAL REPORT
             * =========================================================
             */

            Console.WriteLine();

            Console.WriteLine(
                "====================================================");

            Console.WriteLine(
                " PASSWORD RESET COMPLETE");

            Console.WriteLine(
                "====================================================");

            Console.WriteLine(
                $"Total users : {users.Count}");

            Console.WriteLine(
                $"Successful  : {successCount}");

            Console.WriteLine(
                $"Failed      : {failedCount}");

            Console.WriteLine();

            Console.WriteLine(
                $"Test password: {TestPassword}");

            Console.WriteLine();

            if (failedCount > 0)
            {
                Console.WriteLine(
                    "WARNING: Some passwords were not reset.");
            }
            else
            {
                Console.WriteLine(
                    "All @kism.test passwords were reset successfully.");
            }

            Console.WriteLine();
        }
    }
}