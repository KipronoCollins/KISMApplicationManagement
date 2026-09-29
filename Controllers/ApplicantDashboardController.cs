using KISMApplicationManagement.Data;
using KISMApplicationManagement.Models;
using KISMApplicationManagement.Models.ApplicantDashboardViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace KISMApplicationManagement.Controllers
{
    [Authorize(Roles = "Applicant")]
    public class ApplicantDashboardController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public ApplicantDashboardController(
            ApplicationDbContext context,
            UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var userId =
                _userManager.GetUserId(User);

            if (string.IsNullOrWhiteSpace(userId))
            {
                return Challenge();
            }

            var user =
                await _userManager.FindByIdAsync(userId);

            if (user == null)
            {
                return Challenge();
            }

            var profile =
                await _context.ApplicantProfiles
                    .AsNoTracking()
                    .FirstOrDefaultAsync(
                        profile =>
                            profile.UserId == userId);

            var model =
                new ApplicantDashboardViewModel
                {
                    FirstName =
                        user.FirstName,

                    LastName =
                        user.LastName,

                    Email =
                        user.Email ?? string.Empty,

                    PhoneNumber =
                        user.PhoneNumber,

                    ProfileComplete =
                        IsProfileComplete(
                            user,
                            profile)
                };

            return View(model);
        }


        // =========================================================
        // Profile Completion
        // =========================================================

        private static bool IsProfileComplete(
            ApplicationUser user,
            ApplicantProfile? profile)
        {
            if (string.IsNullOrWhiteSpace(
                    user.FirstName) ||
                string.IsNullOrWhiteSpace(
                    user.LastName) ||
                string.IsNullOrWhiteSpace(
                    user.PhoneNumber))
            {
                return false;
            }

            if (profile == null)
            {
                return false;
            }

            return
                !string.IsNullOrWhiteSpace(
                    profile.IdPassportBirthCertificateNo) &&
                profile.DateOfBirth.HasValue &&
                profile.CountyId.HasValue &&
                profile.SubcountyId.HasValue &&
                profile.DivisionId.HasValue &&
                profile.LocationId.HasValue &&
                profile.SublocationId.HasValue;
        }
    }
}