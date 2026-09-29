using KISMApplicationManagement.Data;
using KISMApplicationManagement.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace KISMApplicationManagement.Controllers
{
    [Authorize]
    public class DivisionController : Controller
    {
        private readonly ApplicationDbContext _context;

        public DivisionController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: /Division
        public async Task<IActionResult> Index()
        {
            var divisions = await _context.Divisions
                .AsNoTracking()
                .Include(d => d.Subcounty)
                    .ThenInclude(s => s.County)
                .OrderBy(d => d.Subcounty.County.CountyNumber)
                .ThenBy(d => d.Subcounty.SubcountyName)
                .ThenBy(d => d.DivisionName)
                .ToListAsync();

            var counties = await _context.Counties
                .AsNoTracking()
                .Where(c => c.IsActive)
                .OrderBy(c => c.CountyNumber)
                .ThenBy(c => c.CountyName)
                .ToListAsync();

            var subcounties = await _context.Subcounties
                .AsNoTracking()
                .Include(s => s.County)
                .Where(s =>
                    s.IsActive &&
                    s.County.IsActive)
                .OrderBy(s => s.County.CountyNumber)
                .ThenBy(s => s.SubcountyName)
                .ToListAsync();

            ViewBag.Counties = counties;
            ViewBag.Subcounties = subcounties;

            return View(divisions);
        }

        // GET: /Division/Create
        [HttpGet]
        public async Task<IActionResult> Create()
        {
            await LoadFormData();

            var model = new Division
            {
                IsActive = true
            };

            return PartialView("_CreatePartial", model);
        }

        // POST: /Division/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Division model)
        {
            if (!ModelState.IsValid)
            {
                await LoadFormData(model.SubcountyId);

                return PartialView("_CreatePartial", model);
            }

            var subcounty = await _context.Subcounties
                .AsNoTracking()
                .Include(s => s.County)
                .FirstOrDefaultAsync(s =>
                    s.SubcountyId == model.SubcountyId);

            if (subcounty == null)
            {
                ModelState.AddModelError(
                    nameof(model.SubcountyId),
                    "Please select a valid subcounty.");

                await LoadFormData(model.SubcountyId);

                return PartialView("_CreatePartial", model);
            }

            if (!subcounty.IsActive)
            {
                ModelState.AddModelError(
                    nameof(model.SubcountyId),
                    "The selected subcounty is inactive.");

                await LoadFormData(model.SubcountyId);

                return PartialView("_CreatePartial", model);
            }

            if (!subcounty.County.IsActive)
            {
                ModelState.AddModelError(
                    nameof(model.SubcountyId),
                    "The county belonging to the selected subcounty is inactive.");

                await LoadFormData(model.SubcountyId);

                return PartialView("_CreatePartial", model);
            }

            var divisionName = model.DivisionName.Trim();

            var nameExists = await _context.Divisions
                .AnyAsync(d =>
                    d.SubcountyId == model.SubcountyId &&
                    d.DivisionName == divisionName);

            if (nameExists)
            {
                ModelState.AddModelError(
                    nameof(model.DivisionName),
                    "A division with this name already exists in the selected subcounty.");

                await LoadFormData(model.SubcountyId);

                return PartialView("_CreatePartial", model);
            }

            var division = new Division
            {
                SubcountyId = model.SubcountyId,
                DivisionName = divisionName,
                IsActive = model.IsActive,
                CreatedAtUtc = DateTime.UtcNow
            };

            _context.Divisions.Add(division);

            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] =
                "Division created successfully.";

            return RedirectToAction(nameof(Index));
        }

        // GET: /Division/Edit/5
        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var division = await _context.Divisions
                .AsNoTracking()
                .FirstOrDefaultAsync(d =>
                    d.DivisionId == id);

            if (division == null)
            {
                return NotFound();
            }

            await LoadFormData(division.SubcountyId);

            return PartialView("_EditPartial", division);
        }

        // POST: /Division/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(
            int id,
            Division model)
        {
            if (id != model.DivisionId)
            {
                return BadRequest();
            }

            if (!ModelState.IsValid)
            {
                await LoadFormData(model.SubcountyId);

                return PartialView("_EditPartial", model);
            }

            var division = await _context.Divisions
                .FirstOrDefaultAsync(d =>
                    d.DivisionId == id);

            if (division == null)
            {
                return NotFound();
            }

            var subcounty = await _context.Subcounties
                .AsNoTracking()
                .Include(s => s.County)
                .FirstOrDefaultAsync(s =>
                    s.SubcountyId == model.SubcountyId);

            if (subcounty == null)
            {
                ModelState.AddModelError(
                    nameof(model.SubcountyId),
                    "Please select a valid subcounty.");

                await LoadFormData(model.SubcountyId);

                return PartialView("_EditPartial", model);
            }

            if (!subcounty.IsActive)
            {
                ModelState.AddModelError(
                    nameof(model.SubcountyId),
                    "The selected subcounty is inactive.");

                await LoadFormData(model.SubcountyId);

                return PartialView("_EditPartial", model);
            }

            if (!subcounty.County.IsActive)
            {
                ModelState.AddModelError(
                    nameof(model.SubcountyId),
                    "The county belonging to the selected subcounty is inactive.");

                await LoadFormData(model.SubcountyId);

                return PartialView("_EditPartial", model);
            }

            var divisionName = model.DivisionName.Trim();

            var duplicateNameExists =
                await _context.Divisions
                    .AnyAsync(d =>
                        d.DivisionId != id &&
                        d.SubcountyId == model.SubcountyId &&
                        d.DivisionName == divisionName);

            if (duplicateNameExists)
            {
                ModelState.AddModelError(
                    nameof(model.DivisionName),
                    "A division with this name already exists in the selected subcounty.");

                await LoadFormData(model.SubcountyId);

                return PartialView("_EditPartial", model);
            }

            division.SubcountyId = model.SubcountyId;
            division.DivisionName = divisionName;
            division.IsActive = model.IsActive;
            division.UpdatedAtUtc = DateTime.UtcNow;

            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] =
                "Division updated successfully.";

            return RedirectToAction(nameof(Index));
        }

        // POST: /Division/ToggleStatus/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ToggleStatus(int id)
        {
            var division = await _context.Divisions
                .FirstOrDefaultAsync(d =>
                    d.DivisionId == id);

            if (division == null)
            {
                return NotFound();
            }

            division.IsActive = !division.IsActive;
            division.UpdatedAtUtc = DateTime.UtcNow;

            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] =
                division.IsActive
                    ? "Division activated successfully."
                    : "Division deactivated successfully.";

            return RedirectToAction(nameof(Index));
        }

        private async Task LoadFormData(
            int? selectedSubcountyId = null)
        {
            var counties = await _context.Counties
                .AsNoTracking()
                .Where(c => c.IsActive)
                .OrderBy(c => c.CountyNumber)
                .ThenBy(c => c.CountyName)
                .ToListAsync();

            var subcounties = await _context.Subcounties
                .AsNoTracking()
                .Include(s => s.County)
                .Where(s =>
                    s.IsActive &&
                    s.County.IsActive)
                .OrderBy(s => s.County.CountyNumber)
                .ThenBy(s => s.SubcountyName)
                .ToListAsync();

            ViewBag.Counties = counties;
            ViewBag.Subcounties = subcounties;
            ViewBag.SelectedSubcountyId = selectedSubcountyId;
        }
    }
}