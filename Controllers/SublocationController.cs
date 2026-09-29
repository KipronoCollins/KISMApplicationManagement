using KISMApplicationManagement.Data;
using KISMApplicationManagement.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace KISMApplicationManagement.Controllers
{
    [Authorize]
    public class SublocationController : Controller
    {
        private readonly ApplicationDbContext _context;

        public SublocationController(ApplicationDbContext context)
        {
            _context = context;
        }


        // ============================================================
        // INDEX
        // GET: /Sublocation
        // ============================================================

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var sublocations = await _context.Sublocations
                .AsNoTracking()
                .Include(s => s.Location)
                    .ThenInclude(l => l.Division)
                        .ThenInclude(d => d.Subcounty)
                            .ThenInclude(sc => sc.County)
                .OrderBy(s =>
                    s.Location.Division.Subcounty.County.CountyNumber)
                .ThenBy(s =>
                    s.Location.Division.Subcounty.SubcountyName)
                .ThenBy(s =>
                    s.Location.Division.DivisionName)
                .ThenBy(s =>
                    s.Location.LocationName)
                .ThenBy(s =>
                    s.SublocationName)
                .ToListAsync();


            var counties = await _context.Counties
                .AsNoTracking()
                .Where(c => c.IsActive)
                .OrderBy(c => c.CountyNumber)
                .ThenBy(c => c.CountyName)
                .ToListAsync();


            var subcounties = await _context.Subcounties
                .AsNoTracking()
                .Include(sc => sc.County)
                .Where(sc =>
                    sc.IsActive &&
                    sc.County.IsActive)
                .OrderBy(sc => sc.County.CountyNumber)
                .ThenBy(sc => sc.SubcountyName)
                .ToListAsync();


            var divisions = await _context.Divisions
                .AsNoTracking()
                .Include(d => d.Subcounty)
                    .ThenInclude(sc => sc.County)
                .Where(d =>
                    d.IsActive &&
                    d.Subcounty.IsActive &&
                    d.Subcounty.County.IsActive)
                .OrderBy(d =>
                    d.Subcounty.County.CountyNumber)
                .ThenBy(d =>
                    d.Subcounty.SubcountyName)
                .ThenBy(d =>
                    d.DivisionName)
                .ToListAsync();


            var locations = await _context.Locations
                .AsNoTracking()
                .Include(l => l.Division)
                    .ThenInclude(d => d.Subcounty)
                        .ThenInclude(sc => sc.County)
                .Where(l =>
                    l.IsActive &&
                    l.Division.IsActive &&
                    l.Division.Subcounty.IsActive &&
                    l.Division.Subcounty.County.IsActive)
                .OrderBy(l =>
                    l.Division.Subcounty.County.CountyNumber)
                .ThenBy(l =>
                    l.Division.Subcounty.SubcountyName)
                .ThenBy(l =>
                    l.Division.DivisionName)
                .ThenBy(l =>
                    l.LocationName)
                .ToListAsync();


            ViewBag.Counties = counties;

            ViewBag.Subcounties = subcounties;

            ViewBag.Divisions = divisions;

            ViewBag.Locations = locations;


            return View(sublocations);
        }


        // ============================================================
        // CREATE - GET
        // GET: /Sublocation/Create
        // ============================================================

        [HttpGet]
        public async Task<IActionResult> Create()
        {
            await LoadFormData();


            var model = new Sublocation
            {
                IsActive = true
            };


            return PartialView("_CreatePartial", model);
        }


        // ============================================================
        // CREATE - POST
        // POST: /Sublocation/Create
        // ============================================================

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Sublocation model)
        {
            if (!ModelState.IsValid)
            {
                await LoadFormData(model.LocationId);

                return PartialView(
                    "_CreatePartial",
                    model);
            }


            var location = await _context.Locations
                .AsNoTracking()
                .Include(l => l.Division)
                    .ThenInclude(d => d.Subcounty)
                        .ThenInclude(sc => sc.County)
                .FirstOrDefaultAsync(l =>
                    l.LocationId == model.LocationId);


            if (location == null)
            {
                ModelState.AddModelError(
                    nameof(model.LocationId),
                    "Please select a valid location.");

                await LoadFormData(model.LocationId);

                return PartialView(
                    "_CreatePartial",
                    model);
            }


            if (!location.IsActive)
            {
                ModelState.AddModelError(
                    nameof(model.LocationId),
                    "The selected location is inactive.");

                await LoadFormData(model.LocationId);

                return PartialView(
                    "_CreatePartial",
                    model);
            }


            if (!location.Division.IsActive)
            {
                ModelState.AddModelError(
                    nameof(model.LocationId),
                    "The division belonging to the selected location is inactive.");

                await LoadFormData(model.LocationId);

                return PartialView(
                    "_CreatePartial",
                    model);
            }


            if (!location.Division.Subcounty.IsActive)
            {
                ModelState.AddModelError(
                    nameof(model.LocationId),
                    "The subcounty belonging to the selected location is inactive.");

                await LoadFormData(model.LocationId);

                return PartialView(
                    "_CreatePartial",
                    model);
            }


            if (!location.Division.Subcounty.County.IsActive)
            {
                ModelState.AddModelError(
                    nameof(model.LocationId),
                    "The county belonging to the selected location is inactive.");

                await LoadFormData(model.LocationId);

                return PartialView(
                    "_CreatePartial",
                    model);
            }


            var sublocationName =
                model.SublocationName.Trim();


            var nameExists =
                await _context.Sublocations
                    .AnyAsync(s =>
                        s.LocationId == model.LocationId &&
                        s.SublocationName == sublocationName);


            if (nameExists)
            {
                ModelState.AddModelError(
                    nameof(model.SublocationName),
                    "A sublocation with this name already exists in the selected location.");

                await LoadFormData(model.LocationId);

                return PartialView(
                    "_CreatePartial",
                    model);
            }


            var sublocation = new Sublocation
            {
                LocationId = model.LocationId,

                SublocationName = sublocationName,

                IsActive = model.IsActive,

                CreatedAtUtc = DateTime.UtcNow
            };


            _context.Sublocations.Add(sublocation);

            await _context.SaveChangesAsync();


            TempData["SuccessMessage"] =
                "Sublocation created successfully.";


            return RedirectToAction(nameof(Index));
        }


        // ============================================================
        // EDIT - GET
        // GET: /Sublocation/Edit/5
        // ============================================================

        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var sublocation =
                await _context.Sublocations
                    .AsNoTracking()
                    .FirstOrDefaultAsync(s =>
                        s.SublocationId == id);


            if (sublocation == null)
            {
                return NotFound();
            }


            var location =
                await _context.Locations
                    .AsNoTracking()
                    .Include(l => l.Division)
                        .ThenInclude(d => d.Subcounty)
                            .ThenInclude(sc => sc.County)
                    .FirstOrDefaultAsync(l =>
                        l.LocationId ==
                        sublocation.LocationId);


            if (location == null)
            {
                return NotFound();
            }


            ViewBag.Locations =
                new List<Location>
                {
                    location
                };


            return PartialView(
                "_EditPartial",
                sublocation);
        }


        // ============================================================
        // EDIT - POST
        // POST: /Sublocation/Edit/5
        // ============================================================

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(
            int id,
            Sublocation model)
        {
            if (id != model.SublocationId)
            {
                return BadRequest();
            }


            if (!ModelState.IsValid)
            {
                await LoadFormData(model.LocationId);

                return PartialView(
                    "_EditPartial",
                    model);
            }


            var sublocation =
                await _context.Sublocations
                    .FirstOrDefaultAsync(s =>
                        s.SublocationId == id);


            if (sublocation == null)
            {
                return NotFound();
            }


            var location =
                await _context.Locations
                    .AsNoTracking()
                    .Include(l => l.Division)
                        .ThenInclude(d => d.Subcounty)
                            .ThenInclude(sc => sc.County)
                    .FirstOrDefaultAsync(l =>
                        l.LocationId ==
                        model.LocationId);


            if (location == null)
            {
                ModelState.AddModelError(
                    nameof(model.LocationId),
                    "Please select a valid location.");

                await LoadFormData(model.LocationId);

                return PartialView(
                    "_EditPartial",
                    model);
            }


            if (!location.IsActive)
            {
                ModelState.AddModelError(
                    nameof(model.LocationId),
                    "The selected location is inactive.");

                await LoadFormData(model.LocationId);

                return PartialView(
                    "_EditPartial",
                    model);
            }


            if (!location.Division.IsActive)
            {
                ModelState.AddModelError(
                    nameof(model.LocationId),
                    "The division belonging to the selected location is inactive.");

                await LoadFormData(model.LocationId);

                return PartialView(
                    "_EditPartial",
                    model);
            }


            if (!location.Division.Subcounty.IsActive)
            {
                ModelState.AddModelError(
                    nameof(model.LocationId),
                    "The subcounty belonging to the selected location is inactive.");

                await LoadFormData(model.LocationId);

                return PartialView(
                    "_EditPartial",
                    model);
            }


            if (!location.Division.Subcounty.County.IsActive)
            {
                ModelState.AddModelError(
                    nameof(model.LocationId),
                    "The county belonging to the selected location is inactive.");

                await LoadFormData(model.LocationId);

                return PartialView(
                    "_EditPartial",
                    model);
            }


            var sublocationName =
                model.SublocationName.Trim();


            var duplicateNameExists =
                await _context.Sublocations
                    .AnyAsync(s =>
                        s.SublocationId != id &&
                        s.LocationId == model.LocationId &&
                        s.SublocationName ==
                        sublocationName);


            if (duplicateNameExists)
            {
                ModelState.AddModelError(
                    nameof(model.SublocationName),
                    "A sublocation with this name already exists in the selected location.");

                await LoadFormData(model.LocationId);

                return PartialView(
                    "_EditPartial",
                    model);
            }


            sublocation.LocationId =
                model.LocationId;

            sublocation.SublocationName =
                sublocationName;

            sublocation.IsActive =
                model.IsActive;

            sublocation.UpdatedAtUtc =
                DateTime.UtcNow;


            await _context.SaveChangesAsync();


            TempData["SuccessMessage"] =
                "Sublocation updated successfully.";


            return RedirectToAction(nameof(Index));
        }


        // ============================================================
        // TOGGLE STATUS
        // POST: /Sublocation/ToggleStatus/5
        // ============================================================

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ToggleStatus(
            int id)
        {
            var sublocation =
                await _context.Sublocations
                    .FirstOrDefaultAsync(s =>
                        s.SublocationId == id);


            if (sublocation == null)
            {
                return NotFound();
            }


            sublocation.IsActive =
                !sublocation.IsActive;


            sublocation.UpdatedAtUtc =
                DateTime.UtcNow;


            await _context.SaveChangesAsync();


            TempData["SuccessMessage"] =
                sublocation.IsActive
                    ? "Sublocation activated successfully."
                    : "Sublocation deactivated successfully.";


            return RedirectToAction(nameof(Index));
        }


        // ============================================================
        // FORM DATA
        // ============================================================

        private async Task LoadFormData(
            int? selectedLocationId = null)
        {
            var counties =
                await _context.Counties
                    .AsNoTracking()
                    .Where(c =>
                        c.IsActive)
                    .OrderBy(c =>
                        c.CountyNumber)
                    .ThenBy(c =>
                        c.CountyName)
                    .ToListAsync();


            var subcounties =
                await _context.Subcounties
                    .AsNoTracking()
                    .Include(sc =>
                        sc.County)
                    .Where(sc =>
                        sc.IsActive &&
                        sc.County.IsActive)
                    .OrderBy(sc =>
                        sc.County.CountyNumber)
                    .ThenBy(sc =>
                        sc.SubcountyName)
                    .ToListAsync();


            var divisions =
                await _context.Divisions
                    .AsNoTracking()
                    .Include(d =>
                        d.Subcounty)
                        .ThenInclude(sc =>
                            sc.County)
                    .Where(d =>
                        d.IsActive &&
                        d.Subcounty.IsActive &&
                        d.Subcounty.County.IsActive)
                    .OrderBy(d =>
                        d.Subcounty.County.CountyNumber)
                    .ThenBy(d =>
                        d.Subcounty.SubcountyName)
                    .ThenBy(d =>
                        d.DivisionName)
                    .ToListAsync();


            var locations =
                await _context.Locations
                    .AsNoTracking()
                    .Include(l =>
                        l.Division)
                        .ThenInclude(d =>
                            d.Subcounty)
                            .ThenInclude(sc =>
                                sc.County)
                    .Where(l =>
                        l.IsActive &&
                        l.Division.IsActive &&
                        l.Division.Subcounty.IsActive &&
                        l.Division.Subcounty.County.IsActive)
                    .OrderBy(l =>
                        l.Division.Subcounty.County.CountyNumber)
                    .ThenBy(l =>
                        l.Division.Subcounty.SubcountyName)
                    .ThenBy(l =>
                        l.Division.DivisionName)
                    .ThenBy(l =>
                        l.LocationName)
                    .ToListAsync();


            ViewBag.Counties =
                counties;

            ViewBag.Subcounties =
                subcounties;

            ViewBag.Divisions =
                divisions;

            ViewBag.Locations =
                locations;

            ViewBag.SelectedLocationId =
                selectedLocationId;
        }
    }
}