using KISMApplicationManagement.Data;
using KISMApplicationManagement.Models;
using KISMApplicationManagement.Models.ProgrammeViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace KISMApplicationManagement.Controllers
{
    [Authorize(Roles = "System Administrator,Admissions Administrator")]
    public class ProgrammesController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<ProgrammesController> _logger;

        public ProgrammesController(
            ApplicationDbContext context,
            ILogger<ProgrammesController> logger)
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
            var programmes =
                await _context.Programmes
                    .AsNoTracking()
                    .OrderBy(programme =>
                        programme.DisplayOrder)
                    .ThenBy(programme =>
                        programme.ProgrammeName)
                    .ToListAsync();

            return View(programmes);
        }


        // =========================================================
        // Create - GET
        // =========================================================

        [HttpGet]
        public IActionResult Create()
        {
            var model =
                new ProgrammeViewModel
                {
                    IsActive = true,
                    DurationYears = 1,
                    DisplayOrder = 0
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
            ProgrammeViewModel model)
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

            var programmeCode =
                model.ProgrammeCode.Trim();

            var programmeName =
                model.ProgrammeName.Trim();

            var awardType =
                model.AwardType.Trim();

            var duplicate =
                await _context.Programmes
                    .AsNoTracking()
                    .AnyAsync(programme =>
                        programme.ProgrammeCode
                            .ToLower() ==
                        programmeCode.ToLower());

            if (duplicate)
            {
                return BadRequest(new
                {
                    success = false,
                    message =
                        "A programme with this programme code already exists."
                });
            }

            var programme =
                new Programme
                {
                    ProgrammeCode =
                        programmeCode,

                    ProgrammeName =
                        programmeName,

                    AwardType =
                        awardType,

                    DurationYears =
                        model.DurationYears,

                    Description =
                        Normalize(
                            model.Description),

                    DisplayOrder =
                        model.DisplayOrder,

                    IsActive =
                        model.IsActive,

                    CreatedAtUtc =
                        DateTime.UtcNow
                };

            _context.Programmes.Add(
                programme);

            await _context.SaveChangesAsync();

            _logger.LogInformation(
                "Programme {ProgrammeCode} created.",
                programme.ProgrammeCode);

            return Json(new
            {
                success = true,
                message =
                    "Programme created successfully."
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

            var programme =
                await _context.Programmes
                    .AsNoTracking()
                    .FirstOrDefaultAsync(
                        programme =>
                            programme.ProgrammeId == id);

            if (programme == null)
            {
                return NotFound();
            }

            var model =
                new ProgrammeViewModel
                {
                    ProgrammeId =
                        programme.ProgrammeId,

                    ProgrammeCode =
                        programme.ProgrammeCode,

                    ProgrammeName =
                        programme.ProgrammeName,

                    AwardType =
                        programme.AwardType,

                    DurationYears =
                        programme.DurationYears,

                    Description =
                        programme.Description,

                    DisplayOrder =
                        programme.DisplayOrder,

                    IsActive =
                        programme.IsActive
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
            ProgrammeViewModel model)
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

            var programme =
                await _context.Programmes
                    .FirstOrDefaultAsync(
                        item =>
                            item.ProgrammeId ==
                            model.ProgrammeId);

            if (programme == null)
            {
                return NotFound(new
                {
                    success = false,
                    message =
                        "Programme not found."
                });
            }

            var programmeCode =
                model.ProgrammeCode.Trim();

            var duplicate =
                await _context.Programmes
                    .AsNoTracking()
                    .AnyAsync(item =>
                        item.ProgrammeId !=
                            model.ProgrammeId &&
                        item.ProgrammeCode
                            .ToLower() ==
                        programmeCode.ToLower());

            if (duplicate)
            {
                return BadRequest(new
                {
                    success = false,
                    message =
                        "A programme with this programme code already exists."
                });
            }

            programme.ProgrammeCode =
                programmeCode;

            programme.ProgrammeName =
                model.ProgrammeName.Trim();

            programme.AwardType =
                model.AwardType.Trim();

            programme.DurationYears =
                model.DurationYears;

            programme.Description =
                Normalize(
                    model.Description);

            programme.DisplayOrder =
                model.DisplayOrder;

            programme.IsActive =
                model.IsActive;

            programme.UpdatedAtUtc =
                DateTime.UtcNow;

            await _context.SaveChangesAsync();

            _logger.LogInformation(
                "Programme {ProgrammeId} updated.",
                programme.ProgrammeId);

            return Json(new
            {
                success = true,
                message =
                    "Programme updated successfully."
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

            var programme =
                await _context.Programmes
                    .AsNoTracking()
                    .FirstOrDefaultAsync(
                        item =>
                            item.ProgrammeId == id);

            if (programme == null)
            {
                return NotFound();
            }

            return PartialView(
                "_DetailsPartial",
                programme);
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
                        "Invalid programme."
                });
            }

            var programme =
                await _context.Programmes
                    .FirstOrDefaultAsync(
                        item =>
                            item.ProgrammeId == id);

            if (programme == null)
            {
                return NotFound(new
                {
                    success = false,
                    message =
                        "Programme not found."
                });
            }

            programme.IsActive =
                !programme.IsActive;

            programme.UpdatedAtUtc =
                DateTime.UtcNow;

            await _context.SaveChangesAsync();

            var statusText =
                programme.IsActive
                    ? "activated"
                    : "deactivated";

            return Json(new
            {
                success = true,

                isActive =
                    programme.IsActive,

                statusText =
                    programme.IsActive
                        ? "Active"
                        : "Inactive",

                message =
                    $"Programme {statusText} successfully."
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