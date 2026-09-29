using KISMApplicationManagement.Data;
using KISMApplicationManagement.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace KISMApplicationManagement.Controllers
{
    [Authorize]
    public class LocationController : Controller
    {
        private readonly ApplicationDbContext _context;

        public LocationController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: /Location
        public async Task<IActionResult> Index()
        {
            var locations = await _context.Locations
                .AsNoTracking()
                .Include(l => l.Division)
                    .ThenInclude(d => d.Subcounty)
                        .ThenInclude(s => s.County)
                .OrderBy(l => l.Division.Subcounty.County.CountyNumber)
                .ThenBy(l => l.Division.Subcounty.SubcountyName)
                .ThenBy(l => l.Division.DivisionName)
                .ThenBy(l => l.LocationName)
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

            var divisions = await _context.Divisions
                .AsNoTracking()
                .Include(d => d.Subcounty)
                    .ThenInclude(s => s.County)
                .Where(d =>
                    d.IsActive &&
                    d.Subcounty.IsActive &&
                    d.Subcounty.County.IsActive)
                .OrderBy(d => d.Subcounty.County.CountyNumber)
                .ThenBy(d => d.Subcounty.SubcountyName)
                .ThenBy(d => d.DivisionName)
                .ToListAsync();

            ViewBag.Counties = counties;
            ViewBag.Subcounties = subcounties;
            ViewBag.Divisions = divisions;

            return View(locations);
        }

        // GET: /Location/Create
        [HttpGet]
        public async Task<IActionResult> Create()
        {
            await LoadFormData();

            var model = new Location
            {
                IsActive = true
            };

            return PartialView("_CreatePartial", model);
        }

        // POST: /Location/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Location model)
        {
            if (!ModelState.IsValid)
            {
                await LoadFormData(model.DivisionId);

                return PartialView("_CreatePartial", model);
            }

            var division = await _context.Divisions
                .AsNoTracking()
                .Include(d => d.Subcounty)
                    .ThenInclude(s => s.County)
                .FirstOrDefaultAsync(d =>
                    d.DivisionId == model.DivisionId);

            if (division == null)
            {
                ModelState.AddModelError(
                    nameof(model.DivisionId),
                    "Please select a valid division.");

                await LoadFormData(model.DivisionId);

                return PartialView("_CreatePartial", model);
            }

            if (!division.IsActive)
            {
                ModelState.AddModelError(
                    nameof(model.DivisionId),
                    "The selected division is inactive.");

                await LoadFormData(model.DivisionId);

                return PartialView("_CreatePartial", model);
            }

            if (!division.Subcounty.IsActive)
            {
                ModelState.AddModelError(
                    nameof(model.DivisionId),
                    "The subcounty belonging to the selected division is inactive.");

                await LoadFormData(model.DivisionId);

                return PartialView("_CreatePartial", model);
            }

            if (!division.Subcounty.County.IsActive)
            {
                ModelState.AddModelError(
                    nameof(model.DivisionId),
                    "The county belonging to the selected division is inactive.");

                await LoadFormData(model.DivisionId);

                return PartialView("_CreatePartial", model);
            }

            var locationName = model.LocationName.Trim();

            var nameExists = await _context.Locations
                .AnyAsync(l =>
                    l.DivisionId == model.DivisionId &&
                    l.LocationName == locationName);

            if (nameExists)
            {
                ModelState.AddModelError(
                    nameof(model.LocationName),
                    "A location with this name already exists in the selected division.");

                await LoadFormData(model.DivisionId);

                return PartialView("_CreatePartial", model);
            }

            var location = new Location
            {
                DivisionId = model.DivisionId,
                LocationName = locationName,
                IsActive = model.IsActive,
                CreatedAtUtc = DateTime.UtcNow
            };

            _context.Locations.Add(location);

            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] =
                "Location created successfully.";

            return RedirectToAction(nameof(Index));
        }

        // GET: /Location/Edit/5
        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var location = await _context.Locations
                .AsNoTracking()
                .FirstOrDefaultAsync(l =>
                    l.LocationId == id);

            if (location == null)
            {
                return NotFound();
            }

            var division = await _context.Divisions
                .AsNoTracking()
                .Include(d => d.Subcounty)
                    .ThenInclude(s => s.County)
                .FirstOrDefaultAsync(d =>
                    d.DivisionId == location.DivisionId);

            if (division == null)
            {
                return NotFound();
            }

            ViewBag.Divisions = new List<Division>
    {
        division
    };

            return PartialView("_EditPartial", location);
        }

        // POST: /Location/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(
            int id,
            Location model)
        {
            if (id != model.LocationId)
            {
                return BadRequest();
            }

            if (!ModelState.IsValid)
            {
                await LoadFormData(model.DivisionId);

                return PartialView("_EditPartial", model);
            }

            var location = await _context.Locations
                .FirstOrDefaultAsync(l =>
                    l.LocationId == id);

            if (location == null)
            {
                return NotFound();
            }

            var division = await _context.Divisions
                .AsNoTracking()
                .Include(d => d.Subcounty)
                    .ThenInclude(s => s.County)
                .FirstOrDefaultAsync(d =>
                    d.DivisionId == model.DivisionId);

            if (division == null)
            {
                ModelState.AddModelError(
                    nameof(model.DivisionId),
                    "Please select a valid division.");

                await LoadFormData(model.DivisionId);

                return PartialView("_EditPartial", model);
            }

            if (!division.IsActive)
            {
                ModelState.AddModelError(
                    nameof(model.DivisionId),
                    "The selected division is inactive.");

                await LoadFormData(model.DivisionId);

                return PartialView("_EditPartial", model);
            }

            if (!division.Subcounty.IsActive)
            {
                ModelState.AddModelError(
                    nameof(model.DivisionId),
                    "The subcounty belonging to the selected division is inactive.");

                await LoadFormData(model.DivisionId);

                return PartialView("_EditPartial", model);
            }

            if (!division.Subcounty.County.IsActive)
            {
                ModelState.AddModelError(
                    nameof(model.DivisionId),
                    "The county belonging to the selected division is inactive.");

                await LoadFormData(model.DivisionId);

                return PartialView("_EditPartial", model);
            }

            var locationName = model.LocationName.Trim();

            var duplicateNameExists =
                await _context.Locations
                    .AnyAsync(l =>
                        l.LocationId != id &&
                        l.DivisionId == model.DivisionId &&
                        l.LocationName == locationName);

            if (duplicateNameExists)
            {
                ModelState.AddModelError(
                    nameof(model.LocationName),
                    "A location with this name already exists in the selected division.");

                await LoadFormData(model.DivisionId);

                return PartialView("_EditPartial", model);
            }

            location.DivisionId = model.DivisionId;
            location.LocationName = locationName;
            location.IsActive = model.IsActive;
            location.UpdatedAtUtc = DateTime.UtcNow;

            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] =
                "Location updated successfully.";

            return RedirectToAction(nameof(Index));
        }

        // POST: /Location/ToggleStatus/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ToggleStatus(int id)
        {
            var location = await _context.Locations
                .FirstOrDefaultAsync(l =>
                    l.LocationId == id);

            if (location == null)
            {
                return NotFound();
            }

            location.IsActive = !location.IsActive;
            location.UpdatedAtUtc = DateTime.UtcNow;

            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] =
                location.IsActive
                    ? "Location activated successfully."
                    : "Location deactivated successfully.";

            return RedirectToAction(nameof(Index));
        }

        private async Task LoadFormData(
            int? selectedDivisionId = null)
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

            var divisions = await _context.Divisions
                .AsNoTracking()
                .Include(d => d.Subcounty)
                    .ThenInclude(s => s.County)
                .Where(d =>
                    d.IsActive &&
                    d.Subcounty.IsActive &&
                    d.Subcounty.County.IsActive)
                .OrderBy(d => d.Subcounty.County.CountyNumber)
                .ThenBy(d => d.Subcounty.SubcountyName)
                .ThenBy(d => d.DivisionName)
                .ToListAsync();

            ViewBag.Counties = counties;
            ViewBag.Subcounties = subcounties;
            ViewBag.Divisions = divisions;
            ViewBag.SelectedDivisionId = selectedDivisionId;
        }
    }
}