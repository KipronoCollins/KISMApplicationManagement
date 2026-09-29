using KISMApplicationManagement.Data;
using KISMApplicationManagement.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace KISMApplicationManagement.Controllers
{
    [Authorize]
    public class SubcountyController : Controller
    {
        private readonly ApplicationDbContext _context;

        public SubcountyController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: /Subcounty
        public async Task<IActionResult> Index()
        {
            var subcounties = await _context.Subcounties
                .AsNoTracking()
                .Include(s => s.County)
                .OrderBy(s => s.County.CountyNumber)
                .ThenBy(s => s.SubcountyName)
                .ToListAsync();

            return View(subcounties);
        }

        // GET: /Subcounty/Create
        [HttpGet]
        public async Task<IActionResult> Create()
        {
            await LoadCounties();

            var model = new Subcounty
            {
                IsActive = true
            };

            return PartialView("_CreatePartial", model);
        }

        // POST: /Subcounty/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Subcounty model)
        {
            if (!ModelState.IsValid)
            {
                await LoadCounties();
                return PartialView("_CreatePartial", model);
            }

            var county = await _context.Counties
                .AsNoTracking()
                .FirstOrDefaultAsync(c =>
                    c.CountyId == model.CountyId);

            if (county == null)
            {
                ModelState.AddModelError(
                    nameof(model.CountyId),
                    "Please select a valid county.");

                await LoadCounties();

                return PartialView("_CreatePartial", model);
            }

            if (!county.IsActive)
            {
                ModelState.AddModelError(
                    nameof(model.CountyId),
                    "The selected county is inactive.");

                await LoadCounties();

                return PartialView("_CreatePartial", model);
            }

            var subcountyName = model.SubcountyName.Trim();

            var nameExists = await _context.Subcounties
                .AnyAsync(s =>
                    s.CountyId == model.CountyId &&
                    s.SubcountyName == subcountyName);

            if (nameExists)
            {
                ModelState.AddModelError(
                    nameof(model.SubcountyName),
                    "A subcounty with this name already exists in the selected county.");

                await LoadCounties();

                return PartialView("_CreatePartial", model);
            }

            var subcounty = new Subcounty
            {
                CountyId = model.CountyId,
                SubcountyName = subcountyName,
                IsActive = model.IsActive,
                CreatedAtUtc = DateTime.UtcNow
            };

            _context.Subcounties.Add(subcounty);

            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] =
                "Subcounty created successfully.";

            return RedirectToAction(nameof(Index));
        }

        // GET: /Subcounty/Edit/5
        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var subcounty = await _context.Subcounties
                .AsNoTracking()
                .FirstOrDefaultAsync(s =>
                    s.SubcountyId == id);

            if (subcounty == null)
            {
                return NotFound();
            }

            await LoadCounties();

            return PartialView("_EditPartial", subcounty);
        }

        // POST: /Subcounty/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(
            int id,
            Subcounty model)
        {
            if (id != model.SubcountyId)
            {
                return BadRequest();
            }

            if (!ModelState.IsValid)
            {
                await LoadCounties();
                return PartialView("_EditPartial", model);
            }

            var subcounty = await _context.Subcounties
                .FirstOrDefaultAsync(s =>
                    s.SubcountyId == id);

            if (subcounty == null)
            {
                return NotFound();
            }

            var county = await _context.Counties
                .AsNoTracking()
                .FirstOrDefaultAsync(c =>
                    c.CountyId == model.CountyId);

            if (county == null)
            {
                ModelState.AddModelError(
                    nameof(model.CountyId),
                    "Please select a valid county.");

                await LoadCounties();

                return PartialView("_EditPartial", model);
            }

            if (!county.IsActive)
            {
                ModelState.AddModelError(
                    nameof(model.CountyId),
                    "The selected county is inactive.");

                await LoadCounties();

                return PartialView("_EditPartial", model);
            }

            var subcountyName = model.SubcountyName.Trim();

            var duplicateNameExists =
                await _context.Subcounties
                    .AnyAsync(s =>
                        s.SubcountyId != id &&
                        s.CountyId == model.CountyId &&
                        s.SubcountyName == subcountyName);

            if (duplicateNameExists)
            {
                ModelState.AddModelError(
                    nameof(model.SubcountyName),
                    "A subcounty with this name already exists in the selected county.");

                await LoadCounties();

                return PartialView("_EditPartial", model);
            }

            subcounty.CountyId = model.CountyId;
            subcounty.SubcountyName = subcountyName;
            subcounty.IsActive = model.IsActive;
            subcounty.UpdatedAtUtc = DateTime.UtcNow;

            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] =
                "Subcounty updated successfully.";

            return RedirectToAction(nameof(Index));
        }

        // POST: /Subcounty/ToggleStatus/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ToggleStatus(int id)
        {
            var subcounty = await _context.Subcounties
                .FirstOrDefaultAsync(s =>
                    s.SubcountyId == id);

            if (subcounty == null)
            {
                return NotFound();
            }

            subcounty.IsActive = !subcounty.IsActive;
            subcounty.UpdatedAtUtc = DateTime.UtcNow;

            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] =
                subcounty.IsActive
                    ? "Subcounty activated successfully."
                    : "Subcounty deactivated successfully.";

            return RedirectToAction(nameof(Index));
        }

        private async Task LoadCounties()
        {
            ViewBag.Counties = await _context.Counties
                .AsNoTracking()
                .Where(c => c.IsActive)
                .OrderBy(c => c.CountyNumber)
                .ThenBy(c => c.CountyName)
                .ToListAsync();
        }
    }
}