using KISMApplicationManagement.Data;
using KISMApplicationManagement.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace KISMApplicationManagement.Controllers
{
    [Authorize]
    public class CountyController : Controller
    {
        private readonly ApplicationDbContext _context;

        public CountyController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: /County
        public async Task<IActionResult> Index()
        {
            var counties = await _context.Counties
                .AsNoTracking()
                .OrderBy(c => c.CountyNumber)
                .ThenBy(c => c.CountyName)
                .ToListAsync();

            return View(counties);
        }

        // GET: /County/Create
        [HttpGet]
        public IActionResult Create()
        {
            var model = new County
            {
                IsActive = true
            };

            return PartialView("_CreatePartial", model);
        }

        // POST: /County/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(County model)
        {
            if (!ModelState.IsValid)
            {
                return PartialView("_CreatePartial", model);
            }

            var countyName = model.CountyName.Trim();

            var nameExists = await _context.Counties
                .AnyAsync(c => c.CountyName == countyName);

            if (nameExists)
            {
                ModelState.AddModelError(
                    nameof(model.CountyName),
                    "A county with this name already exists.");

                return PartialView("_CreatePartial", model);
            }

            var numberExists = await _context.Counties
                .AnyAsync(c => c.CountyNumber == model.CountyNumber);

            if (numberExists)
            {
                ModelState.AddModelError(
                    nameof(model.CountyNumber),
                    "A county with this number already exists.");

                return PartialView("_CreatePartial", model);
            }

            var county = new County
            {
                CountyNumber = model.CountyNumber,
                CountyName = countyName,
                IsActive = model.IsActive,
                CreatedAtUtc = DateTime.UtcNow
            };

            _context.Counties.Add(county);

            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = "County created successfully.";

            return RedirectToAction(nameof(Index));
        }

        // GET: /County/Edit/5
        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var county = await _context.Counties
                .AsNoTracking()
                .FirstOrDefaultAsync(c => c.CountyId == id);

            if (county == null)
            {
                return NotFound();
            }

            return PartialView("_EditPartial", county);
        }

        // POST: /County/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, County model)
        {
            if (id != model.CountyId)
            {
                return BadRequest();
            }

            if (!ModelState.IsValid)
            {
                return PartialView("_EditPartial", model);
            }

            var county = await _context.Counties
                .FirstOrDefaultAsync(c => c.CountyId == id);

            if (county == null)
            {
                return NotFound();
            }

            var countyName = model.CountyName.Trim();

            var duplicateNameExists = await _context.Counties
                .AnyAsync(c =>
                    c.CountyId != id &&
                    c.CountyName == countyName);

            if (duplicateNameExists)
            {
                ModelState.AddModelError(
                    nameof(model.CountyName),
                    "A county with this name already exists.");

                return PartialView("_EditPartial", model);
            }

            var duplicateNumberExists = await _context.Counties
                .AnyAsync(c =>
                    c.CountyId != id &&
                    c.CountyNumber == model.CountyNumber);

            if (duplicateNumberExists)
            {
                ModelState.AddModelError(
                    nameof(model.CountyNumber),
                    "A county with this number already exists.");

                return PartialView("_EditPartial", model);
            }

            county.CountyNumber = model.CountyNumber;
            county.CountyName = countyName;
            county.IsActive = model.IsActive;
            county.UpdatedAtUtc = DateTime.UtcNow;

            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = "County updated successfully.";

            return RedirectToAction(nameof(Index));
        }

        // POST: /County/ToggleStatus/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ToggleStatus(int id)
        {
            var county = await _context.Counties
                .FirstOrDefaultAsync(c => c.CountyId == id);

            if (county == null)
            {
                return NotFound();
            }

            county.IsActive = !county.IsActive;
            county.UpdatedAtUtc = DateTime.UtcNow;

            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = county.IsActive
                ? "County activated successfully."
                : "County deactivated successfully.";

            return RedirectToAction(nameof(Index));
        }
    }
}