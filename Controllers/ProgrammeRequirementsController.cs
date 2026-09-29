using KISMApplicationManagement.Data;
using KISMApplicationManagement.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace KISMApplicationManagement.Controllers
{
    [Authorize(Roles = "System Administrator,Admissions Administrator")]
    public class ProgrammeRequirementsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ProgrammeRequirementsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // =========================================================
        // INDEX
        // =========================================================

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var programmes = await _context.Programmes
                .AsNoTracking()
                .OrderBy(p => p.DisplayOrder)
                .ThenBy(p => p.ProgrammeName)
                .ToListAsync();

            return View(programmes);
        }

        // =========================================================
        // CREATE GROUP
        // =========================================================

        [HttpGet]
        public async Task<IActionResult> CreateGroup(
            int programmeId,
            int? parentGroupId = null)
        {
            var programme = await _context.Programmes
                .AsNoTracking()
                .FirstOrDefaultAsync(p => p.ProgrammeId == programmeId);

            if (programme == null)
            {
                return NotFound();
            }

            ProgrammeRequirementGroup? parentGroup = null;

            if (parentGroupId.HasValue)
            {
                parentGroup = await _context.ProgrammeRequirementGroups
                    .AsNoTracking()
                    .FirstOrDefaultAsync(g =>
                        g.ProgrammeRequirementGroupId == parentGroupId.Value &&
                        g.ProgrammeId == programmeId);

                if (parentGroup == null)
                {
                    return NotFound();
                }
            }

            var model = new ProgrammeRequirementGroup
            {
                ProgrammeId = programmeId,
                ParentGroupId = parentGroupId,
                SelectionRule = "ALL",
                MinimumRequired = 1,
                DisplayOrder = await GetNextGroupDisplayOrderAsync(
                    programmeId,
                    parentGroupId),
                IsActive = true
            };

            ViewBag.ProgrammeName = programme.ProgrammeName;
            ViewBag.ParentGroupName = parentGroup?.GroupName;

            return PartialView("_CreateGroupPartial", model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateGroup(
            ProgrammeRequirementGroup model)
        {
            ModelState.Remove(nameof(model.Programme));
            ModelState.Remove(nameof(model.ParentGroup));
            ModelState.Remove(nameof(model.ChildGroups));
            ModelState.Remove(nameof(model.Requirements));

            var programme = await _context.Programmes
                .FirstOrDefaultAsync(p => p.ProgrammeId == model.ProgrammeId);

            if (programme == null)
            {
                ModelState.AddModelError(
                    nameof(model.ProgrammeId),
                    "The selected programme does not exist.");
            }

            ProgrammeRequirementGroup? parentGroup = null;

            if (model.ParentGroupId.HasValue)
            {
                parentGroup = await _context.ProgrammeRequirementGroups
                    .FirstOrDefaultAsync(g =>
                        g.ProgrammeRequirementGroupId ==
                        model.ParentGroupId.Value);

                if (parentGroup == null)
                {
                    ModelState.AddModelError(
                        nameof(model.ParentGroupId),
                        "The selected parent group does not exist.");
                }
                else if (parentGroup.ProgrammeId != model.ProgrammeId)
                {
                    ModelState.AddModelError(
                        nameof(model.ParentGroupId),
                        "The parent group must belong to the selected programme.");
                }

                // Prevent a group from being made its own parent.
                if (parentGroup != null &&
                    parentGroup.ProgrammeRequirementGroupId ==
                    model.ProgrammeRequirementGroupId)
                {
                    ModelState.AddModelError(
                        nameof(model.ParentGroupId),
                        "A group cannot be its own parent.");
                }
            }

            NormalizeGroupSelectionRule(model);

            if (!ModelState.IsValid)
            {
                if (programme != null)
                {
                    ViewBag.ProgrammeName = programme.ProgrammeName;
                }

                ViewBag.ParentGroupName = parentGroup?.GroupName;

                return PartialView("_CreateGroupPartial", model);
            }

            model.GroupName = model.GroupName.Trim();

            if (!string.IsNullOrWhiteSpace(model.Description))
            {
                model.Description = model.Description.Trim();
            }

            model.DisplayOrder = await GetNextGroupDisplayOrderAsync(
                model.ProgrammeId,
                model.ParentGroupId);

            model.IsActive = true;
            model.CreatedAtUtc = DateTime.UtcNow;
            model.UpdatedAtUtc = null;

            _context.ProgrammeRequirementGroups.Add(model);

            try
            {
                await _context.SaveChangesAsync();

                return Json(new
                {
                    success = true,
                    message = "Requirement group created successfully."
                });
            }
            catch (DbUpdateException)
            {
                return StatusCode(
                    StatusCodes.Status500InternalServerError,
                    new
                    {
                        success = false,
                        message = "The requirement group could not be created."
                    });
            }
        }

        // =========================================================
        // EDIT GROUP
        // =========================================================

        [HttpGet]
        public async Task<IActionResult> EditGroup(int id)
        {
            var group = await _context.ProgrammeRequirementGroups
                .AsNoTracking()
                .FirstOrDefaultAsync(g =>
                    g.ProgrammeRequirementGroupId == id);

            if (group == null)
            {
                return NotFound();
            }

            var programme = await _context.Programmes
                .AsNoTracking()
                .FirstOrDefaultAsync(p =>
                    p.ProgrammeId == group.ProgrammeId);

            ViewBag.ProgrammeName = programme?.ProgrammeName;

            if (group.ParentGroupId.HasValue)
            {
                ViewBag.ParentGroupName = await _context
                    .ProgrammeRequirementGroups
                    .Where(g =>
                        g.ProgrammeRequirementGroupId ==
                        group.ParentGroupId.Value)
                    .Select(g => g.GroupName)
                    .FirstOrDefaultAsync();
            }

            return PartialView("_EditGroupPartial", group);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditGroup(
            ProgrammeRequirementGroup model)
        {
            ModelState.Remove(nameof(model.Programme));
            ModelState.Remove(nameof(model.ParentGroup));
            ModelState.Remove(nameof(model.ChildGroups));
            ModelState.Remove(nameof(model.Requirements));

            var group = await _context.ProgrammeRequirementGroups
                .FirstOrDefaultAsync(g =>
                    g.ProgrammeRequirementGroupId ==
                    model.ProgrammeRequirementGroupId);

            if (group == null)
            {
                return NotFound();
            }

            var programme = await _context.Programmes
                .FirstOrDefaultAsync(p =>
                    p.ProgrammeId == model.ProgrammeId);

            if (programme == null)
            {
                ModelState.AddModelError(
                    nameof(model.ProgrammeId),
                    "The selected programme does not exist.");
            }

            // The programme of an existing group should not be changed
            // through this operation.
            model.ProgrammeId = group.ProgrammeId;

            // ---------------------------------------------------------
            // Validate parent group
            // ---------------------------------------------------------

            ProgrammeRequirementGroup? parentGroup = null;

            if (model.ParentGroupId.HasValue)
            {
                parentGroup = await _context
                    .ProgrammeRequirementGroups
                    .FirstOrDefaultAsync(g =>
                        g.ProgrammeRequirementGroupId ==
                        model.ParentGroupId.Value);

                if (parentGroup == null)
                {
                    ModelState.AddModelError(
                        nameof(model.ParentGroupId),
                        "The selected parent group does not exist.");
                }
                else if (parentGroup.ProgrammeId != group.ProgrammeId)
                {
                    ModelState.AddModelError(
                        nameof(model.ParentGroupId),
                        "The parent group must belong to the same programme.");
                }
                else if (parentGroup.ProgrammeRequirementGroupId ==
                         group.ProgrammeRequirementGroupId)
                {
                    ModelState.AddModelError(
                        nameof(model.ParentGroupId),
                        "A group cannot be its own parent.");
                }
                else
                {
                    var createsCycle = await WouldCreateGroupCycleAsync(
                        group.ProgrammeRequirementGroupId,
                        model.ParentGroupId);

                    if (createsCycle)
                    {
                        ModelState.AddModelError(
                            nameof(model.ParentGroupId),
                            "The selected parent would create a circular group hierarchy.");
                    }
                }
            }

            NormalizeGroupSelectionRule(model);

            if (!ModelState.IsValid)
            {
                ViewBag.ProgrammeName = programme?.ProgrammeName;
                ViewBag.ParentGroupName = parentGroup?.GroupName;

                return PartialView("_EditGroupPartial", model);
            }

            group.GroupName = model.GroupName.Trim();
            group.Description = string.IsNullOrWhiteSpace(model.Description)
                ? null
                : model.Description.Trim();

            group.SelectionRule = model.SelectionRule;
            group.MinimumRequired = model.MinimumRequired;
            group.ParentGroupId = model.ParentGroupId;
            group.DisplayOrder = model.DisplayOrder;
            group.UpdatedAtUtc = DateTime.UtcNow;

            try
            {
                await _context.SaveChangesAsync();

                return Json(new
                {
                    success = true,
                    message = "Requirement group updated successfully."
                });
            }
            catch (DbUpdateException)
            {
                return StatusCode(
                    StatusCodes.Status500InternalServerError,
                    new
                    {
                        success = false,
                        message = "The requirement group could not be updated."
                    });
            }
        }

        // =========================================================
        // GROUP DETAILS
        // =========================================================

        [HttpGet]
        public async Task<IActionResult> GroupDetails(int id)
        {
            var group = await _context.ProgrammeRequirementGroups
                .AsNoTracking()
                .Include(g => g.Programme)
                .Include(g => g.ParentGroup)
                .Include(g => g.ChildGroups
                    .OrderBy(c => c.DisplayOrder)
                    .ThenBy(c => c.GroupName))
                .Include(g => g.Requirements
                    .OrderBy(r => r.DisplayOrder)
                    .ThenBy(r => r.RequirementName))
                .FirstOrDefaultAsync(g =>
                    g.ProgrammeRequirementGroupId == id);

            if (group == null)
            {
                return NotFound();
            }

            return PartialView("_GroupDetailsPartial", group);
        }

        // =========================================================
        // TOGGLE GROUP STATUS
        // =========================================================

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ToggleGroupStatus(int id)
        {
            var group = await _context.ProgrammeRequirementGroups
                .FirstOrDefaultAsync(g =>
                    g.ProgrammeRequirementGroupId == id);

            if (group == null)
            {
                return NotFound();
            }

            group.IsActive = !group.IsActive;
            group.UpdatedAtUtc = DateTime.UtcNow;

            // When a parent group is deactivated, its descendants are
            // also deactivated so the requirement hierarchy cannot
            // contain active children under an inactive branch.
            if (!group.IsActive)
            {
                var descendants = await GetDescendantGroupsAsync(id);

                foreach (var descendant in descendants)
                {
                    descendant.IsActive = false;
                    descendant.UpdatedAtUtc = DateTime.UtcNow;
                }
            }

            try
            {
                await _context.SaveChangesAsync();

                return Json(new
                {
                    success = true,
                    isActive = group.IsActive,
                    message = group.IsActive
                        ? "Requirement group activated successfully."
                        : "Requirement group deactivated successfully."
                });
            }
            catch (DbUpdateException)
            {
                return StatusCode(
                    StatusCodes.Status500InternalServerError,
                    new
                    {
                        success = false,
                        message = "The requirement group status could not be changed."
                    });
            }
        }

        // =========================================================
        // CREATE REQUIREMENT
        // =========================================================

        [HttpGet]
        public async Task<IActionResult> CreateRequirement(
            int programmeRequirementGroupId)
        {
            var group = await _context.ProgrammeRequirementGroups
                .AsNoTracking()
                .FirstOrDefaultAsync(g =>
                    g.ProgrammeRequirementGroupId ==
                    programmeRequirementGroupId);

            if (group == null)
            {
                return NotFound();
            }

            var model = new ProgrammeRequirement
            {
                ProgrammeRequirementGroupId =
                    programmeRequirementGroupId,
                RequirementType = "SubjectGrade",
                DisplayOrder = await GetNextRequirementDisplayOrderAsync(
                    programmeRequirementGroupId),
                IsActive = true
            };

            ViewBag.GroupName = group.GroupName;

            return PartialView("_CreateRequirementPartial", model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateRequirement(
            ProgrammeRequirement model)
        {
            ModelState.Remove(nameof(model.RequirementGroup));

            var group = await _context.ProgrammeRequirementGroups
                .FirstOrDefaultAsync(g =>
                    g.ProgrammeRequirementGroupId ==
                    model.ProgrammeRequirementGroupId);

            if (group == null)
            {
                ModelState.AddModelError(
                    nameof(model.ProgrammeRequirementGroupId),
                    "The selected requirement group does not exist.");
            }

            NormalizeRequirement(model);

            if (!ModelState.IsValid)
            {
                ViewBag.GroupName = group?.GroupName;

                return PartialView("_CreateRequirementPartial", model);
            }

            model.RequirementName = model.RequirementName.Trim();

            if (!string.IsNullOrWhiteSpace(model.SubjectName))
            {
                model.SubjectName = model.SubjectName.Trim();
            }

            if (!string.IsNullOrWhiteSpace(model.RequiredValue))
            {
                model.RequiredValue = model.RequiredValue.Trim();
            }

            if (!string.IsNullOrWhiteSpace(model.Description))
            {
                model.Description = model.Description.Trim();
            }

            model.DisplayOrder =
                await GetNextRequirementDisplayOrderAsync(
                    model.ProgrammeRequirementGroupId);

            model.IsActive = true;
            model.CreatedAtUtc = DateTime.UtcNow;
            model.UpdatedAtUtc = null;

            _context.ProgrammeRequirements.Add(model);

            try
            {
                await _context.SaveChangesAsync();

                return Json(new
                {
                    success = true,
                    message = "Requirement created successfully."
                });
            }
            catch (DbUpdateException)
            {
                return StatusCode(
                    StatusCodes.Status500InternalServerError,
                    new
                    {
                        success = false,
                        message = "The requirement could not be created."
                    });
            }
        }

        // =========================================================
        // EDIT REQUIREMENT
        // =========================================================

        [HttpGet]
        public async Task<IActionResult> EditRequirement(int id)
        {
            var requirement = await _context.ProgrammeRequirements
                .AsNoTracking()
                .Include(r => r.RequirementGroup)
                .FirstOrDefaultAsync(r =>
                    r.ProgrammeRequirementId == id);

            if (requirement == null)
            {
                return NotFound();
            }

            ViewBag.GroupName =
                requirement.RequirementGroup.GroupName;

            return PartialView(
                "_EditRequirementPartial",
                requirement);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditRequirement(
            ProgrammeRequirement model)
        {
            ModelState.Remove(nameof(model.RequirementGroup));

            var requirement = await _context.ProgrammeRequirements
                .FirstOrDefaultAsync(r =>
                    r.ProgrammeRequirementId ==
                    model.ProgrammeRequirementId);

            if (requirement == null)
            {
                return NotFound();
            }

            var group = await _context.ProgrammeRequirementGroups
                .FirstOrDefaultAsync(g =>
                    g.ProgrammeRequirementGroupId ==
                    requirement.ProgrammeRequirementGroupId);

            if (group == null)
            {
                return NotFound();
            }

            // Keep the requirement in its existing group.
            model.ProgrammeRequirementGroupId =
                requirement.ProgrammeRequirementGroupId;

            NormalizeRequirement(model);

            if (!ModelState.IsValid)
            {
                ViewBag.GroupName = group.GroupName;

                return PartialView(
                    "_EditRequirementPartial",
                    model);
            }

            requirement.RequirementType =
                model.RequirementType.Trim();

            requirement.RequirementName =
                model.RequirementName.Trim();

            requirement.SubjectName =
                string.IsNullOrWhiteSpace(model.SubjectName)
                    ? null
                    : model.SubjectName.Trim();

            requirement.ComparisonOperator =
                string.IsNullOrWhiteSpace(model.ComparisonOperator)
                    ? null
                    : model.ComparisonOperator.Trim();

            requirement.RequiredValue =
                string.IsNullOrWhiteSpace(model.RequiredValue)
                    ? null
                    : model.RequiredValue.Trim();

            requirement.Description =
                string.IsNullOrWhiteSpace(model.Description)
                    ? null
                    : model.Description.Trim();

            requirement.DisplayOrder = model.DisplayOrder;
            requirement.UpdatedAtUtc = DateTime.UtcNow;

            try
            {
                await _context.SaveChangesAsync();

                return Json(new
                {
                    success = true,
                    message = "Requirement updated successfully."
                });
            }
            catch (DbUpdateException)
            {
                return StatusCode(
                    StatusCodes.Status500InternalServerError,
                    new
                    {
                        success = false,
                        message = "The requirement could not be updated."
                    });
            }
        }

        // =========================================================
        // TOGGLE REQUIREMENT STATUS
        // =========================================================

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ToggleRequirementStatus(int id)
        {
            var requirement = await _context.ProgrammeRequirements
                .FirstOrDefaultAsync(r =>
                    r.ProgrammeRequirementId == id);

            if (requirement == null)
            {
                return NotFound();
            }

            requirement.IsActive = !requirement.IsActive;
            requirement.UpdatedAtUtc = DateTime.UtcNow;

            try
            {
                await _context.SaveChangesAsync();

                return Json(new
                {
                    success = true,
                    isActive = requirement.IsActive,
                    message = requirement.IsActive
                        ? "Requirement activated successfully."
                        : "Requirement deactivated successfully."
                });
            }
            catch (DbUpdateException)
            {
                return StatusCode(
                    StatusCodes.Status500InternalServerError,
                    new
                    {
                        success = false,
                        message = "The requirement status could not be changed."
                    });
            }
        }

        // =========================================================
        // GET GROUPS
        // =========================================================

        [HttpGet]
        public async Task<IActionResult> GetGroups(int programmeId)
        {
            var programmeExists = await _context.Programmes
                .AnyAsync(p => p.ProgrammeId == programmeId);

            if (!programmeExists)
            {
                return NotFound();
            }

            var groups = await _context
                .ProgrammeRequirementGroups
                .AsNoTracking()
                .Where(g => g.ProgrammeId == programmeId)
                .Include(g => g.ChildGroups)
                .Include(g => g.Requirements)
                .OrderBy(g => g.ParentGroupId)
                .ThenBy(g => g.DisplayOrder)
                .ThenBy(g => g.GroupName)
                .ToListAsync();

            return PartialView("_GroupsPartial", groups);
        }

        // =========================================================
        // GET CHILD GROUPS
        // =========================================================

        [HttpGet]
        public async Task<IActionResult> GetChildGroups(int parentGroupId)
        {
            var parent = await _context
                .ProgrammeRequirementGroups
                .AsNoTracking()
                .FirstOrDefaultAsync(g =>
                    g.ProgrammeRequirementGroupId == parentGroupId);

            if (parent == null)
            {
                return NotFound();
            }

            var groups = await _context
                .ProgrammeRequirementGroups
                .AsNoTracking()
                .Where(g =>
                    g.ProgrammeId == parent.ProgrammeId &&
                    g.ParentGroupId == parentGroupId)
                .Include(g => g.Requirements)
                .OrderBy(g => g.DisplayOrder)
                .ThenBy(g => g.GroupName)
                .ToListAsync();

            return PartialView("_GroupsPartial", groups);
        }

        // =========================================================
        // HELPERS
        // =========================================================

        private async Task<int> GetNextGroupDisplayOrderAsync(
            int programmeId,
            int? parentGroupId)
        {
            var maxOrder = await _context
                .ProgrammeRequirementGroups
                .Where(g =>
                    g.ProgrammeId == programmeId &&
                    g.ParentGroupId == parentGroupId)
                .Select(g => (int?)g.DisplayOrder)
                .MaxAsync();

            return (maxOrder ?? 0) + 1;
        }

        private async Task<int> GetNextRequirementDisplayOrderAsync(
            int groupId)
        {
            var maxOrder = await _context
                .ProgrammeRequirements
                .Where(r =>
                    r.ProgrammeRequirementGroupId == groupId)
                .Select(r => (int?)r.DisplayOrder)
                .MaxAsync();

            return (maxOrder ?? 0) + 1;
        }

        private static void NormalizeGroupSelectionRule(
            ProgrammeRequirementGroup model)
        {
            model.SelectionRule =
                string.IsNullOrWhiteSpace(model.SelectionRule)
                    ? "ALL"
                    : model.SelectionRule.Trim().ToUpperInvariant();

            if (model.SelectionRule != "ALL" &&
                model.SelectionRule != "ANY")
            {
                model.SelectionRule = "ALL";
            }

            if (model.SelectionRule == "ALL")
            {
                model.MinimumRequired = 1;
            }
            else if (model.MinimumRequired < 1)
            {
                model.MinimumRequired = 1;
            }
        }

        private static void NormalizeRequirement(
            ProgrammeRequirement model)
        {
            model.RequirementType =
                string.IsNullOrWhiteSpace(model.RequirementType)
                    ? "SubjectGrade"
                    : model.RequirementType.Trim();

            if (!string.IsNullOrWhiteSpace(model.ComparisonOperator))
            {
                model.ComparisonOperator =
                    model.ComparisonOperator.Trim();
            }

            if (!string.IsNullOrWhiteSpace(model.RequiredValue))
            {
                model.RequiredValue =
                    model.RequiredValue.Trim();
            }
        }

        private async Task<bool> WouldCreateGroupCycleAsync(
            int groupId,
            int? proposedParentGroupId)
        {
            if (!proposedParentGroupId.HasValue)
            {
                return false;
            }

            var currentId = proposedParentGroupId.Value;

            while (true)
            {
                if (currentId == groupId)
                {
                    return true;
                }

                var parentId = await _context
                    .ProgrammeRequirementGroups
                    .Where(g =>
                        g.ProgrammeRequirementGroupId == currentId)
                    .Select(g => g.ParentGroupId)
                    .FirstOrDefaultAsync();

                if (!parentId.HasValue)
                {
                    return false;
                }

                currentId = parentId.Value;
            }
        }

        private async Task<List<ProgrammeRequirementGroup>>
            GetDescendantGroupsAsync(int parentGroupId)
        {
            var descendants =
                new List<ProgrammeRequirementGroup>();

            var children = await _context
                .ProgrammeRequirementGroups
                .Where(g => g.ParentGroupId == parentGroupId)
                .ToListAsync();

            foreach (var child in children)
            {
                descendants.Add(child);

                var childDescendants =
                    await GetDescendantGroupsAsync(
                        child.ProgrammeRequirementGroupId);

                descendants.AddRange(childDescendants);
            }

            return descendants;
        }
    }
}