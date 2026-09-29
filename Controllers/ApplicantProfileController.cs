using KISMApplicationManagement.Data;
using KISMApplicationManagement.Models;
using KISMApplicationManagement.Models.ApplicantProfileViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace KISMApplicationManagement.Controllers
{
    [Authorize(Roles = "Applicant")]
    public class ApplicantProfileController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ILogger<ApplicantProfileController> _logger;

        public ApplicantProfileController(
            ApplicationDbContext context,
            UserManager<ApplicationUser> userManager,
            ILogger<ApplicantProfileController> logger)
        {
            _context = context;
            _userManager = userManager;
            _logger = logger;
        }

        // =========================================================
        // Profile
        // =========================================================

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
                new ApplicantProfileViewModel
                {
                    FirstName =
                        user.FirstName,

                    MiddleName =
                        user.MiddleName,

                    LastName =
                        user.LastName,

                    PhoneNumber =
                        user.PhoneNumber ?? string.Empty,

                    Email =
                        user.Email ?? string.Empty
                };

            if (profile != null)
            {
                model.ApplicantProfileId =
                    profile.ApplicantProfileId;

                model.IdPassportBirthCertificateNo =
                    profile.IdPassportBirthCertificateNo;

                model.MobileNumber =
                    profile.MobileNumber;

                model.ContactAddress =
                    profile.ContactAddress;

                model.AddressCode =
                    profile.AddressCode;

                model.Town =
                    profile.Town;

                model.ContactEmail =
                    profile.ContactEmail;

                model.DateOfBirth =
                    profile.DateOfBirth;

                model.PlaceOfBirth =
                    profile.PlaceOfBirth;

                model.CountyId =
                    profile.CountyId;

                model.SubcountyId =
                    profile.SubcountyId;

                model.DivisionId =
                    profile.DivisionId;

                model.LocationId =
                    profile.LocationId;

                model.SublocationId =
                    profile.SublocationId;

                model.MaritalStatus =
                    profile.MaritalStatus;

                model.ParentGuardianName =
                    profile.ParentGuardianName;

                model.ParentGuardianMobile =
                    profile.ParentGuardianMobile;

                model.AlternativeContact =
                    profile.AlternativeContact;
            }

            model.Counties =
                await _context.Counties
                    .AsNoTracking()
                    .Where(
                        county =>
                            county.IsActive)
                    .OrderBy(
                        county =>
                            county.CountyNumber)
                    .Select(
                        county =>
                            new CountyOptionViewModel
                            {
                                CountyId =
                                    county.CountyId,

                                CountyName =
                                    county.CountyName
                            })
                    .ToListAsync();

            return View(model);
        }


        // =========================================================
        // Save Profile
        // =========================================================

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Save(
            ApplicantProfileViewModel model)
        {
            var userId =
                _userManager.GetUserId(User);

            if (string.IsNullOrWhiteSpace(userId))
            {
                return Unauthorized(new
                {
                    success = false,
                    message =
                        "Your session has expired."
                });
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(new
                {
                    success = false,
                    message =
                        "Please correct the highlighted fields."
                });
            }

            var user =
                await _userManager.FindByIdAsync(userId);

            if (user == null)
            {
                return Unauthorized(new
                {
                    success = false,
                    message =
                        "Your account could not be found."
                });
            }

            var locationIsValid =
                await ValidateLocationHierarchyAsync(model);

            if (!locationIsValid)
            {
                return BadRequest(new
                {
                    success = false,
                    message =
                        "The selected location information is invalid. " +
                        "Please select the location again."
                });
            }

            // =====================================================
            // Update ApplicationUser
            // =====================================================

            user.FirstName =
                model.FirstName.Trim();

            user.MiddleName =
                Normalize(model.MiddleName);

            user.LastName =
                model.LastName.Trim();

            user.PhoneNumber =
                Normalize(model.PhoneNumber);

            user.UpdatedAtUtc =
                DateTime.UtcNow;

            var userUpdateResult =
                await _userManager.UpdateAsync(user);

            if (!userUpdateResult.Succeeded)
            {
                var errors =
                    userUpdateResult.Errors
                        .Select(
                            error =>
                                error.Description)
                        .ToList();

                _logger.LogError(
                    "Failed to update account information " +
                    "for applicant {UserId}: {Errors}",
                    userId,
                    string.Join(
                        "; ",
                        errors));

                return StatusCode(
                    StatusCodes.Status500InternalServerError,
                    new
                    {
                        success = false,
                        message =
                            "Unable to update your account information.",
                        errors
                    });
            }

            // =====================================================
            // Get or create ApplicantProfile
            // =====================================================

            var existingProfile =
                await _context.ApplicantProfiles
                    .FirstOrDefaultAsync(
                        profile =>
                            profile.UserId == userId);

            if (existingProfile == null)
            {
                existingProfile =
                    new ApplicantProfile
                    {
                        UserId =
                            userId,

                        CreatedAtUtc =
                            DateTime.UtcNow
                    };

                _context.ApplicantProfiles.Add(
                    existingProfile);
            }

            // =====================================================
            // Automatically generate FullName
            // =====================================================

            existingProfile.FullName =
                BuildFullName(
                    user.FirstName,
                    user.MiddleName,
                    user.LastName);

            // =====================================================
            // Applicant-specific information
            // =====================================================

            existingProfile.IdPassportBirthCertificateNo =
                Normalize(
                    model.IdPassportBirthCertificateNo);

            existingProfile.MobileNumber =
                Normalize(
                    model.MobileNumber);

            existingProfile.ContactAddress =
                Normalize(
                    model.ContactAddress);

            existingProfile.AddressCode =
                Normalize(
                    model.AddressCode);

            existingProfile.Town =
                Normalize(
                    model.Town);

            existingProfile.ContactEmail =
                Normalize(
                    model.ContactEmail);

            existingProfile.DateOfBirth =
                model.DateOfBirth;

            existingProfile.PlaceOfBirth =
                Normalize(
                    model.PlaceOfBirth);

            existingProfile.CountyId =
                model.CountyId;

            existingProfile.SubcountyId =
                model.SubcountyId;

            existingProfile.DivisionId =
                model.DivisionId;

            existingProfile.LocationId =
                model.LocationId;

            existingProfile.SublocationId =
                model.SublocationId;

            existingProfile.MaritalStatus =
                Normalize(
                    model.MaritalStatus);

            existingProfile.ParentGuardianName =
                Normalize(
                    model.ParentGuardianName);

            existingProfile.ParentGuardianMobile =
                Normalize(
                    model.ParentGuardianMobile);

            existingProfile.AlternativeContact =
                Normalize(
                    model.AlternativeContact);

            existingProfile.UpdatedAtUtc =
                DateTime.UtcNow;

            await _context.SaveChangesAsync();

            _logger.LogInformation(
                "Applicant profile and account information " +
                "saved for user {UserId}.",
                userId);

            return Json(new
            {
                success = true,
                message =
                    "Your profile was saved successfully."
            });
        }


        // =========================================================
        // Get Subcounties
        // =========================================================

        [HttpGet]
        public async Task<IActionResult> GetSubcounties(
            int countyId)
        {
            if (countyId <= 0)
            {
                return BadRequest(new
                {
                    success = false,
                    message =
                        "Invalid county."
                });
            }

            var exists =
                await _context.Counties
                    .AsNoTracking()
                    .AnyAsync(
                        county =>
                            county.CountyId ==
                                countyId &&
                            county.IsActive);

            if (!exists)
            {
                return BadRequest(new
                {
                    success = false,
                    message =
                        "County not found."
                });
            }

            var subcounties =
                await _context.Subcounties
                    .AsNoTracking()
                    .Where(
                        subcounty =>
                            subcounty.CountyId ==
                                countyId &&
                            subcounty.IsActive)
                    .OrderBy(
                        subcounty =>
                            subcounty.SubcountyName)
                    .Select(
                        subcounty =>
                            new
                            {
                                id =
                                    subcounty.SubcountyId,

                                name =
                                    subcounty.SubcountyName
                            })
                    .ToListAsync();

            return Json(subcounties);
        }


        // =========================================================
        // Get Divisions
        // =========================================================

        [HttpGet]
        public async Task<IActionResult> GetDivisions(
            int subcountyId)
        {
            if (subcountyId <= 0)
            {
                return BadRequest(new
                {
                    success = false,
                    message =
                        "Invalid subcounty."
                });
            }

            var exists =
                await _context.Subcounties
                    .AsNoTracking()
                    .AnyAsync(
                        subcounty =>
                            subcounty.SubcountyId ==
                                subcountyId &&
                            subcounty.IsActive);

            if (!exists)
            {
                return BadRequest(new
                {
                    success = false,
                    message =
                        "Subcounty not found."
                });
            }

            var divisions =
                await _context.Divisions
                    .AsNoTracking()
                    .Where(
                        division =>
                            division.SubcountyId ==
                                subcountyId &&
                            division.IsActive)
                    .OrderBy(
                        division =>
                            division.DivisionName)
                    .Select(
                        division =>
                            new
                            {
                                id =
                                    division.DivisionId,

                                name =
                                    division.DivisionName
                            })
                    .ToListAsync();

            return Json(divisions);
        }


        // =========================================================
        // Get Locations
        // =========================================================

        [HttpGet]
        public async Task<IActionResult> GetLocations(
            int divisionId)
        {
            if (divisionId <= 0)
            {
                return BadRequest(new
                {
                    success = false,
                    message =
                        "Invalid division."
                });
            }

            var exists =
                await _context.Divisions
                    .AsNoTracking()
                    .AnyAsync(
                        division =>
                            division.DivisionId ==
                                divisionId &&
                            division.IsActive);

            if (!exists)
            {
                return BadRequest(new
                {
                    success = false,
                    message =
                        "Division not found."
                });
            }

            var locations =
                await _context.Locations
                    .AsNoTracking()
                    .Where(
                        location =>
                            location.DivisionId ==
                                divisionId &&
                            location.IsActive)
                    .OrderBy(
                        location =>
                            location.LocationName)
                    .Select(
                        location =>
                            new
                            {
                                id =
                                    location.LocationId,

                                name =
                                    location.LocationName
                            })
                    .ToListAsync();

            return Json(locations);
        }


        // =========================================================
        // Get Sublocations
        // =========================================================

        [HttpGet]
        public async Task<IActionResult> GetSublocations(
            int locationId)
        {
            if (locationId <= 0)
            {
                return BadRequest(new
                {
                    success = false,
                    message =
                        "Invalid location."
                });
            }

            var exists =
                await _context.Locations
                    .AsNoTracking()
                    .AnyAsync(
                        location =>
                            location.LocationId ==
                                locationId &&
                            location.IsActive);

            if (!exists)
            {
                return BadRequest(new
                {
                    success = false,
                    message =
                        "Location not found."
                });
            }

            var sublocations =
                await _context.Sublocations
                    .AsNoTracking()
                    .Where(
                        sublocation =>
                            sublocation.LocationId ==
                                locationId)
                    .OrderBy(
                        sublocation =>
                            sublocation.SublocationName)
                    .Select(
                        sublocation =>
                            new
                            {
                                id =
                                    sublocation.SublocationId,

                                name =
                                    sublocation.SublocationName
                            })
                    .ToListAsync();

            return Json(sublocations);
        }


        // =========================================================
        // Validate Location Hierarchy
        // =========================================================

        private async Task<bool>
            ValidateLocationHierarchyAsync(
                ApplicantProfileViewModel model)
        {
            if (model.CountyId.HasValue)
            {
                var countyExists =
                    await _context.Counties
                        .AsNoTracking()
                        .AnyAsync(
                            county =>
                                county.CountyId ==
                                    model.CountyId.Value &&
                                county.IsActive);

                if (!countyExists)
                {
                    return false;
                }
            }

            if (model.SubcountyId.HasValue)
            {
                if (!model.CountyId.HasValue)
                {
                    return false;
                }

                var subcountyExists =
                    await _context.Subcounties
                        .AsNoTracking()
                        .AnyAsync(
                            subcounty =>
                                subcounty.SubcountyId ==
                                    model.SubcountyId.Value &&
                                subcounty.CountyId ==
                                    model.CountyId.Value &&
                                subcounty.IsActive);

                if (!subcountyExists)
                {
                    return false;
                }
            }

            if (model.DivisionId.HasValue)
            {
                if (!model.SubcountyId.HasValue)
                {
                    return false;
                }

                var divisionExists =
                    await _context.Divisions
                        .AsNoTracking()
                        .AnyAsync(
                            division =>
                                division.DivisionId ==
                                    model.DivisionId.Value &&
                                division.SubcountyId ==
                                    model.SubcountyId.Value &&
                                division.IsActive);

                if (!divisionExists)
                {
                    return false;
                }
            }

            if (model.LocationId.HasValue)
            {
                if (!model.DivisionId.HasValue)
                {
                    return false;
                }

                var locationExists =
                    await _context.Locations
                        .AsNoTracking()
                        .AnyAsync(
                            location =>
                                location.LocationId ==
                                    model.LocationId.Value &&
                                location.DivisionId ==
                                    model.DivisionId.Value &&
                                location.IsActive);

                if (!locationExists)
                {
                    return false;
                }
            }

            if (model.SublocationId.HasValue)
            {
                if (!model.LocationId.HasValue)
                {
                    return false;
                }

                var sublocationExists =
                    await _context.Sublocations
                        .AsNoTracking()
                        .AnyAsync(
                            sublocation =>
                                sublocation.SublocationId ==
                                    model.SublocationId.Value &&
                                sublocation.LocationId ==
                                    model.LocationId.Value);

                if (!sublocationExists)
                {
                    return false;
                }
            }

            return true;
        }


        // =========================================================
        // Build Full Name
        // =========================================================

        private static string BuildFullName(
            string? firstName,
            string? middleName,
            string? lastName)
        {
            return string.Join(
                " ",
                new[]
                {
                    firstName,
                    middleName,
                    lastName
                }
                .Where(
                    value =>
                        !string.IsNullOrWhiteSpace(value)));
        }


        // =========================================================
        // Normalize
        // =========================================================

        private static string? Normalize(
            string? value)
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                return null;
            }

            return value.Trim();
        }
    }
}