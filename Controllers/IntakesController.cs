using KISMApplicationManagement.Data;
using KISMApplicationManagement.Models;
using KISMApplicationManagement.Models.IntakeViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace KISMApplicationManagement.Controllers
{
    [Authorize(Roles = "System Administrator,Admissions Administrator")]
    public class IntakesController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<IntakesController> _logger;

        public IntakesController(
            ApplicationDbContext context,
            ILogger<IntakesController> logger)
        {
            _context = context;
            _logger = logger;
        }


        // =========================================================
        // Index
        // =========================================================

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var intakes =
                await _context.Intakes
                    .AsNoTracking()
                    .OrderBy(intake =>
                        intake.DisplayOrder)
                    .ThenByDescending(intake =>
                        intake.ApplicationOpeningDate)
                    .ThenBy(intake =>
                        intake.IntakeName)
                    .ToListAsync();

            return View(intakes);
        }


        // =========================================================
        // Create - GET
        // =========================================================

        [HttpGet]
        public IActionResult Create()
        {
            var today =
                DateTime.Today;

            var model =
                new IntakeViewModel
                {
                    AcademicYear =
                        today.Year.ToString(),

                    IntakePeriod =
                        "January",

                    ApplicationOpeningDate =
                        today,

                    ApplicationClosingDate =
                        today.AddMonths(1),

                    AdmissionStartDate =
                        today.AddMonths(2),

                    AdmissionEndDate =
                        today.AddMonths(5),

                    DisplayOrder =
                        0,

                    IsActive =
                        true,

                    // Applications are always closed
                    // when a new intake is created.
                    ApplicationsOpen =
                        false
                };

            return PartialView(
                "_CreatePartial",
                model);
        }


        // =========================================================
        // Create - POST
        // =========================================================

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(
            IntakeViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new
                {
                    success = false,
                    message =
                        "Please correct the highlighted fields."
                });
            }

            var intakeName =
                model.IntakeName.Trim();

            var academicYear =
                model.AcademicYear.Trim();

            var intakePeriod =
                model.IntakePeriod.Trim();

            if (model.ApplicationClosingDate <
                model.ApplicationOpeningDate)
            {
                return BadRequest(new
                {
                    success = false,
                    message =
                        "Application closing date cannot be before the application opening date."
                });
            }

            if (model.AdmissionStartDate.HasValue &&
                model.AdmissionEndDate.HasValue &&
                model.AdmissionEndDate.Value <
                model.AdmissionStartDate.Value)
            {
                return BadRequest(new
                {
                    success = false,
                    message =
                        "Admission end date cannot be before the admission start date."
                });
            }

            if (model.AdmissionStartDate.HasValue &&
                model.AdmissionStartDate.Value <
                model.ApplicationOpeningDate)
            {
                return BadRequest(new
                {
                    success = false,
                    message =
                        "Admission start date cannot be before the application opening date."
                });
            }

            if (model.AdmissionEndDate.HasValue &&
                model.AdmissionEndDate.Value <
                model.ApplicationClosingDate)
            {
                return BadRequest(new
                {
                    success = false,
                    message =
                        "Admission end date cannot be before the application closing date."
                });
            }

            var duplicate =
                await _context.Intakes
                    .AsNoTracking()
                    .AnyAsync(intake =>
                        intake.AcademicYear
                            .ToLower() ==
                        academicYear.ToLower() &&
                        intake.IntakePeriod
                            .ToLower() ==
                        intakePeriod.ToLower());

            if (duplicate)
            {
                return BadRequest(new
                {
                    success = false,
                    message =
                        "An intake for this academic year and intake period already exists."
                });
            }

            var intake =
                new Intake
                {
                    IntakeName =
                        intakeName,

                    AcademicYear =
                        academicYear,

                    IntakePeriod =
                        intakePeriod,

                    ApplicationOpeningDate =
                        model.ApplicationOpeningDate,

                    ApplicationClosingDate =
                        model.ApplicationClosingDate,

                    AdmissionStartDate =
                        model.AdmissionStartDate,

                    AdmissionEndDate =
                        model.AdmissionEndDate,

                    Description =
                        Normalize(
                            model.Description),

                    DisplayOrder =
                        model.DisplayOrder,

                    IsActive =
                        model.IsActive,

                    // Always start with applications closed.
                    // Opening applications is handled only
                    // through ToggleApplications().
                    ApplicationsOpen =
                        false,

                    CreatedAtUtc =
                        DateTime.UtcNow
                };

            _context.Intakes.Add(
                intake);

            await _context.SaveChangesAsync();

            _logger.LogInformation(
                "Intake {IntakeName} created for academic year {AcademicYear}.",
                intake.IntakeName,
                intake.AcademicYear);

            return Json(new
            {
                success = true,
                message =
                    "Intake created successfully."
            });
        }


        // =========================================================
        // Edit - GET
        // =========================================================

        [HttpGet]
        public async Task<IActionResult> Edit(
            int id)
        {
            if (id <= 0)
            {
                return BadRequest();
            }

            var intake =
                await _context.Intakes
                    .AsNoTracking()
                    .FirstOrDefaultAsync(
                        intake =>
                            intake.IntakeId == id);

            if (intake == null)
            {
                return NotFound();
            }

            var model =
                new IntakeViewModel
                {
                    IntakeId =
                        intake.IntakeId,

                    IntakeName =
                        intake.IntakeName,

                    AcademicYear =
                        intake.AcademicYear,

                    IntakePeriod =
                        intake.IntakePeriod,

                    ApplicationOpeningDate =
                        intake.ApplicationOpeningDate,

                    ApplicationClosingDate =
                        intake.ApplicationClosingDate,

                    AdmissionStartDate =
                        intake.AdmissionStartDate,

                    AdmissionEndDate =
                        intake.AdmissionEndDate,

                    Description =
                        intake.Description,

                    DisplayOrder =
                        intake.DisplayOrder,

                    IsActive =
                        intake.IsActive,

                    // Display/state information only.
                    // It is not editable from the form.
                    ApplicationsOpen =
                        intake.ApplicationsOpen
                };

            return PartialView(
                "_EditPartial",
                model);
        }


        // =========================================================
        // Edit - POST
        // =========================================================

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(
            IntakeViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new
                {
                    success = false,
                    message =
                        "Please correct the highlighted fields."
                });
            }

            var intake =
                await _context.Intakes
                    .FirstOrDefaultAsync(
                        item =>
                            item.IntakeId ==
                            model.IntakeId);

            if (intake == null)
            {
                return NotFound(new
                {
                    success = false,
                    message =
                        "Intake not found."
                });
            }

            var intakeName =
                model.IntakeName.Trim();

            var academicYear =
                model.AcademicYear.Trim();

            var intakePeriod =
                model.IntakePeriod.Trim();

            if (model.ApplicationClosingDate <
                model.ApplicationOpeningDate)
            {
                return BadRequest(new
                {
                    success = false,
                    message =
                        "Application closing date cannot be before the application opening date."
                });
            }

            if (model.AdmissionStartDate.HasValue &&
                model.AdmissionEndDate.HasValue &&
                model.AdmissionEndDate.Value <
                model.AdmissionStartDate.Value)
            {
                return BadRequest(new
                {
                    success = false,
                    message =
                        "Admission end date cannot be before the admission start date."
                });
            }

            if (model.AdmissionStartDate.HasValue &&
                model.AdmissionStartDate.Value <
                model.ApplicationOpeningDate)
            {
                return BadRequest(new
                {
                    success = false,
                    message =
                        "Admission start date cannot be before the application opening date."
                });
            }

            if (model.AdmissionEndDate.HasValue &&
                model.AdmissionEndDate.Value <
                model.ApplicationClosingDate)
            {
                return BadRequest(new
                {
                    success = false,
                    message =
                        "Admission end date cannot be before the application closing date."
                });
            }

            var duplicate =
                await _context.Intakes
                    .AsNoTracking()
                    .AnyAsync(item =>
                        item.IntakeId !=
                            model.IntakeId &&
                        item.AcademicYear
                            .ToLower() ==
                        academicYear.ToLower() &&
                        item.IntakePeriod
                            .ToLower() ==
                        intakePeriod.ToLower());

            if (duplicate)
            {
                return BadRequest(new
                {
                    success = false,
                    message =
                        "An intake for this academic year and intake period already exists."
                });
            }

            intake.IntakeName =
                intakeName;

            intake.AcademicYear =
                academicYear;

            intake.IntakePeriod =
                intakePeriod;

            intake.ApplicationOpeningDate =
                model.ApplicationOpeningDate;

            intake.ApplicationClosingDate =
                model.ApplicationClosingDate;

            intake.AdmissionStartDate =
                model.AdmissionStartDate;

            intake.AdmissionEndDate =
                model.AdmissionEndDate;

            intake.Description =
                Normalize(
                    model.Description);

            intake.DisplayOrder =
                model.DisplayOrder;

            intake.IsActive =
                model.IsActive;

            // ApplicationsOpen is intentionally NOT taken
            // from the posted ViewModel.
            //
            // Application status is controlled exclusively
            // by ToggleApplications().
            //
            // If the intake is made inactive, applications
            // must automatically be closed.
            if (!intake.IsActive)
            {
                intake.ApplicationsOpen =
                    false;
            }

            intake.UpdatedAtUtc =
                DateTime.UtcNow;

            await _context.SaveChangesAsync();

            _logger.LogInformation(
                "Intake {IntakeId} updated.",
                intake.IntakeId);

            return Json(new
            {
                success = true,
                message =
                    "Intake updated successfully."
            });
        }


        // =========================================================
        // Details
        // =========================================================

        [HttpGet]
        public async Task<IActionResult> Details(
            int id)
        {
            if (id <= 0)
            {
                return BadRequest();
            }

            var intake =
                await _context.Intakes
                    .AsNoTracking()
                    .FirstOrDefaultAsync(
                        item =>
                            item.IntakeId == id);

            if (intake == null)
            {
                return NotFound();
            }

            return PartialView(
                "_DetailsPartial",
                intake);
        }


        // =========================================================
        // Toggle Status
        // =========================================================

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ToggleStatus(
            int id)
        {
            if (id <= 0)
            {
                return BadRequest(new
                {
                    success = false,
                    message =
                        "Invalid intake."
                });
            }

            var intake =
                await _context.Intakes
                    .FirstOrDefaultAsync(
                        item =>
                            item.IntakeId == id);

            if (intake == null)
            {
                return NotFound(new
                {
                    success = false,
                    message =
                        "Intake not found."
                });
            }

            intake.IsActive =
                !intake.IsActive;

            if (!intake.IsActive)
            {
                intake.ApplicationsOpen =
                    false;
            }

            intake.UpdatedAtUtc =
                DateTime.UtcNow;

            await _context.SaveChangesAsync();

            var statusText =
                intake.IsActive
                    ? "activated"
                    : "deactivated";

            return Json(new
            {
                success = true,

                isActive =
                    intake.IsActive,

                applicationsOpen =
                    intake.ApplicationsOpen,

                statusText =
                    intake.IsActive
                        ? "Active"
                        : "Inactive",

                message =
                    $"Intake {statusText} successfully."
            });
        }


        // =========================================================
        // Toggle Applications
        // =========================================================

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ToggleApplications(
            int id)
        {
            if (id <= 0)
            {
                return BadRequest(new
                {
                    success = false,
                    message =
                        "Invalid intake."
                });
            }

            var intake =
                await _context.Intakes
                    .FirstOrDefaultAsync(
                        item =>
                            item.IntakeId == id);

            if (intake == null)
            {
                return NotFound(new
                {
                    success = false,
                    message =
                        "Intake not found."
                });
            }

            if (!intake.IsActive)
            {
                return BadRequest(new
                {
                    success = false,
                    message =
                        "Applications cannot be opened for an inactive intake."
                });
            }

            if (!intake.ApplicationsOpen)
            {
                var today =
                    DateTime.Today;

                if (today <
                    intake.ApplicationOpeningDate)
                {
                    return BadRequest(new
                    {
                        success = false,
                        message =
                            "Applications cannot be opened before the application opening date."
                    });
                }

                if (today >
                    intake.ApplicationClosingDate)
                {
                    return BadRequest(new
                    {
                        success = false,
                        message =
                            "Applications cannot be opened after the application closing date."
                    });
                }
            }

            intake.ApplicationsOpen =
                !intake.ApplicationsOpen;

            intake.UpdatedAtUtc =
                DateTime.UtcNow;

            await _context.SaveChangesAsync();

            var statusText =
                intake.ApplicationsOpen
                    ? "opened"
                    : "closed";

            return Json(new
            {
                success = true,

                applicationsOpen =
                    intake.ApplicationsOpen,

                statusText =
                    intake.ApplicationsOpen
                        ? "Open"
                        : "Closed",

                message =
                    $"Applications {statusText} successfully."
            });
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