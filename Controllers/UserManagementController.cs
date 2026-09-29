using KISMApplicationManagement.Models;
using KISMApplicationManagement.Models.UserManagementViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace KISMApplicationManagement.Controllers
{
    [Authorize(Roles = "System Administrator,Admissions Administrator")]
    public class UserManagementController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        public UserManagementController(
            UserManager<ApplicationUser> userManager,
            RoleManager<IdentityRole> roleManager)
        {
            _userManager = userManager;
            _roleManager = roleManager;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var users = await _userManager.Users
                .AsNoTracking()
                .OrderBy(u => u.LastName)
                .ThenBy(u => u.FirstName)
                .ThenBy(u => u.UserName)
                .ToListAsync();

            var userRows = new List<UserManagementRowViewModel>();

            foreach (var user in users)
            {
                var roles = await _userManager.GetRolesAsync(user);

                userRows.Add(new UserManagementRowViewModel
                {
                    UserId = user.Id,
                    UserName = user.UserName ?? string.Empty,
                    Email = user.Email ?? string.Empty,
                    PhoneNumber = user.PhoneNumber ?? string.Empty,
                    FirstName = user.FirstName,
                    MiddleName = user.MiddleName,
                    LastName = user.LastName,
                    EmailConfirmed = user.EmailConfirmed,
                    IsActive = user.IsActive,
                    Roles = roles.ToList(),
                    LastLogin = user.LastLoginAtUtc,
                    CreatedAt = user.CreatedAtUtc
                });
            }

            return View(userRows);
        }

        // ---------------------------------------------------------
        // CREATE USER
        // ---------------------------------------------------------

        [HttpGet]
        public async Task<IActionResult> Create()
        {
            await LoadRolesAsync();

            var model = new CreateUserViewModel
            {
                IsActive = true
            };

            return PartialView("_CreatePartial", model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(
            CreateUserViewModel model)
        {
            if (!ModelState.IsValid)
            {
                await LoadRolesAsync();

                return PartialView("_CreatePartial", model);
            }

            var normalizedEmail = model.Email.Trim();

            var existingEmail =
                await _userManager.FindByEmailAsync(
                    normalizedEmail);

            if (existingEmail != null)
            {
                ModelState.AddModelError(
                    nameof(model.Email),
                    "A user with this email address already exists.");

                await LoadRolesAsync();

                return PartialView("_CreatePartial", model);
            }

            var username = model.UserName.Trim();

            var existingUsername =
                await _userManager.FindByNameAsync(
                    username);

            if (existingUsername != null)
            {
                ModelState.AddModelError(
                    nameof(model.UserName),
                    "A user with this username already exists.");

                await LoadRolesAsync();

                return PartialView("_CreatePartial", model);
            }

            var selectedRoles = model.SelectedRoles
                .Where(role =>
                    !string.IsNullOrWhiteSpace(role))
                .Select(role =>
                    role.Trim())
                .Distinct(
                    StringComparer.OrdinalIgnoreCase)
                .ToList();

            if (!selectedRoles.Any())
            {
                ModelState.AddModelError(
                    nameof(model.SelectedRoles),
                    "Please select at least one role.");

                await LoadRolesAsync();

                return PartialView("_CreatePartial", model);
            }

            foreach (var roleName in selectedRoles)
            {
                if (!await _roleManager.RoleExistsAsync(
                        roleName))
                {
                    ModelState.AddModelError(
                        nameof(model.SelectedRoles),
                        $"The selected role '{roleName}' is not valid.");

                    await LoadRolesAsync();

                    return PartialView(
                        "_CreatePartial",
                        model);
                }
            }

            var user = new ApplicationUser
            {
                UserName = username,
                Email = normalizedEmail,

                PhoneNumber =
                    string.IsNullOrWhiteSpace(
                        model.PhoneNumber)
                        ? null
                        : model.PhoneNumber.Trim(),

                FirstName =
                    model.FirstName.Trim(),

                MiddleName =
                    string.IsNullOrWhiteSpace(
                        model.MiddleName)
                        ? null
                        : model.MiddleName.Trim(),

                LastName =
                    model.LastName.Trim(),

                IsActive =
                    model.IsActive,

                CreatedAtUtc =
                    DateTime.UtcNow
            };

            var createResult =
                await _userManager.CreateAsync(
                    user,
                    model.Password);

            if (!createResult.Succeeded)
            {
                foreach (var error in createResult.Errors)
                {
                    ModelState.AddModelError(
                        string.Empty,
                        error.Description);
                }

                await LoadRolesAsync();

                return PartialView(
                    "_CreatePartial",
                    model);
            }

            var roleResult =
                await _userManager.AddToRolesAsync(
                    user,
                    selectedRoles);

            if (!roleResult.Succeeded)
            {
                await _userManager.DeleteAsync(user);

                foreach (var error in roleResult.Errors)
                {
                    ModelState.AddModelError(
                        string.Empty,
                        error.Description);
                }

                await LoadRolesAsync();

                return PartialView(
                    "_CreatePartial",
                    model);
            }

            return Json(new
            {
                success = true,
                message = "User created successfully."
            });
        }

        // ---------------------------------------------------------
        // DETAILS
        // ---------------------------------------------------------

        [HttpGet]
        public async Task<IActionResult> Details(string id)
        {
            if (string.IsNullOrWhiteSpace(id))
            {
                return BadRequest();
            }

            var user =
                await _userManager.FindByIdAsync(id);

            if (user == null)
            {
                return NotFound();
            }

            var roles =
                await _userManager.GetRolesAsync(user);

            var model = new UserDetailsViewModel
            {
                UserId = user.Id,

                FirstName =
                    user.FirstName,

                MiddleName =
                    user.MiddleName,

                LastName =
                    user.LastName,

                UserName =
                    user.UserName ?? string.Empty,

                Email =
                    user.Email ?? string.Empty,

                PhoneNumber =
                    user.PhoneNumber ?? string.Empty,

                EmailConfirmed =
                    user.EmailConfirmed,

                IsActive =
                    user.IsActive,

                Roles =
                    roles.ToList(),

                CreatedAtUtc =
                    user.CreatedAtUtc,

                UpdatedAtUtc =
                    user.UpdatedAtUtc,

                LastLoginAtUtc =
                    user.LastLoginAtUtc
            };

            return PartialView(
                "_DetailsPartial",
                model);
        }

        // ---------------------------------------------------------
        // EDIT USER
        // ---------------------------------------------------------

        [HttpGet]
        public async Task<IActionResult> Edit(string id)
        {
            if (string.IsNullOrWhiteSpace(id))
            {
                return BadRequest();
            }

            var user =
                await _userManager.FindByIdAsync(id);

            if (user == null)
            {
                return NotFound();
            }

            var model = new EditUserViewModel
            {
                UserId =
                    user.Id,

                FirstName =
                    user.FirstName,

                MiddleName =
                    user.MiddleName,

                LastName =
                    user.LastName,

                UserName =
                    user.UserName ?? string.Empty,

                Email =
                    user.Email ?? string.Empty,

                PhoneNumber =
                    user.PhoneNumber,

                EmailConfirmed =
                    user.EmailConfirmed,

                IsActive =
                    user.IsActive
            };

            return PartialView(
                "_EditPartial",
                model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(
            string id,
            EditUserViewModel model)
        {
            if (string.IsNullOrWhiteSpace(id) ||
                id != model.UserId)
            {
                return BadRequest();
            }

            var user =
                await _userManager.FindByIdAsync(id);

            if (user == null)
            {
                return NotFound();
            }

            if (!ModelState.IsValid)
            {
                return PartialView(
                    "_EditPartial",
                    model);
            }

            var username =
                model.UserName.Trim();

            var duplicateUsername =
                await _userManager.Users
                    .AnyAsync(u =>
                        u.Id != id &&
                        u.UserName == username);

            if (duplicateUsername)
            {
                ModelState.AddModelError(
                    nameof(model.UserName),
                    "A user with this username already exists.");

                return PartialView(
                    "_EditPartial",
                    model);
            }

            var email =
                model.Email.Trim();

            var duplicateEmail =
                await _userManager.Users
                    .AnyAsync(u =>
                        u.Id != id &&
                        u.Email == email);

            if (duplicateEmail)
            {
                ModelState.AddModelError(
                    nameof(model.Email),
                    "A user with this email address already exists.");

                return PartialView(
                    "_EditPartial",
                    model);
            }

            user.FirstName =
                model.FirstName.Trim();

            user.MiddleName =
                string.IsNullOrWhiteSpace(
                    model.MiddleName)
                    ? null
                    : model.MiddleName.Trim();

            user.LastName =
                model.LastName.Trim();

            user.UserName =
                username;

            user.Email =
                email;

            user.PhoneNumber =
                string.IsNullOrWhiteSpace(
                    model.PhoneNumber)
                    ? null
                    : model.PhoneNumber.Trim();

            user.EmailConfirmed =
                model.EmailConfirmed;

            user.IsActive =
                model.IsActive;

            user.UpdatedAtUtc =
                DateTime.UtcNow;

            var result =
                await _userManager.UpdateAsync(user);

            if (!result.Succeeded)
            {
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(
                        string.Empty,
                        error.Description);
                }

                return PartialView(
                    "_EditPartial",
                    model);
            }

            return Json(new
            {
                success = true,
                message = "User updated successfully."
            });
        }

        // ---------------------------------------------------------
        // ACTIVATE / DEACTIVATE USER
        // ---------------------------------------------------------

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ToggleStatus(
            string id)
        {
            if (string.IsNullOrWhiteSpace(id))
            {
                return BadRequest(new
                {
                    success = false,
                    message = "Invalid user."
                });
            }

            var user =
                await _userManager.FindByIdAsync(id);

            if (user == null)
            {
                return NotFound(new
                {
                    success = false,
                    message = "User not found."
                });
            }

            var currentUserId =
                _userManager.GetUserId(User);

            if (string.Equals(
                    currentUserId,
                    user.Id,
                    StringComparison.Ordinal))
            {
                return BadRequest(new
                {
                    success = false,
                    message =
                        "You cannot activate or deactivate your own account."
                });
            }

            var newStatus =
                !user.IsActive;

            user.IsActive =
                newStatus;

            user.UpdatedAtUtc =
                DateTime.UtcNow;

            var result =
                await _userManager.UpdateAsync(user);

            if (!result.Succeeded)
            {
                var errors =
                    result.Errors
                        .Select(error =>
                            error.Description)
                        .ToList();

                return StatusCode(
                    StatusCodes.Status500InternalServerError,
                    new
                    {
                        success = false,
                        message =
                            "Unable to update the user's status.",
                        errors
                    });
            }

            return Json(new
            {
                success = true,
                isActive = newStatus,
                statusText =
                    newStatus
                        ? "Active"
                        : "Inactive",
                message =
                    newStatus
                        ? "User activated successfully."
                        : "User deactivated successfully."
            });
        }

        // ---------------------------------------------------------
        // MANAGE ROLES - GET
        // ---------------------------------------------------------

        [HttpGet]
        public async Task<IActionResult> ManageRoles(
            string id)
        {
            if (string.IsNullOrWhiteSpace(id))
            {
                return BadRequest();
            }

            var user =
                await _userManager.FindByIdAsync(id);

            if (user == null)
            {
                return NotFound();
            }

            var currentRoles =
                await _userManager.GetRolesAsync(user);

            var availableRoles =
                await _roleManager.Roles
                    .AsNoTracking()
                    .Where(role =>
                        role.Name != null)
                    .OrderBy(role =>
                        role.Name)
                    .Select(role =>
                        role.Name!)
                    .ToListAsync();

            var model =
                new ManageUserRolesViewModel
                {
                    UserId =
                        user.Id,

                    UserDisplayName =
                        string.Join(
                            " ",
                            new[]
                            {
                                user.FirstName,
                                user.MiddleName,
                                user.LastName
                            }
                            .Where(value =>
                                !string.IsNullOrWhiteSpace(
                                    value))),

                    UserName =
                        user.UserName ?? string.Empty,

                    SelectedRoles =
                        currentRoles.ToList(),

                    AvailableRoles =
                        availableRoles
                            .Select(roleName =>
                                new RoleSelectionViewModel
                                {
                                    RoleName =
                                        roleName,

                                    Selected =
                                        currentRoles.Contains(
                                            roleName,
                                            StringComparer
                                                .OrdinalIgnoreCase)
                                })
                            .ToList()
                };

            return PartialView(
                "_ManageRolesPartial",
                model);
        }

        // ---------------------------------------------------------
        // MANAGE ROLES - POST
        // ---------------------------------------------------------

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ManageRoles(
            ManageUserRolesViewModel model)
        {
            if (string.IsNullOrWhiteSpace(
                    model.UserId))
            {
                return BadRequest();
            }

            var user =
                await _userManager.FindByIdAsync(
                    model.UserId);

            if (user == null)
            {
                return NotFound();
            }

            var selectedRoles =
                model.SelectedRoles
                    .Where(role =>
                        !string.IsNullOrWhiteSpace(
                            role))
                    .Select(role =>
                        role.Trim())
                    .Distinct(
                        StringComparer
                            .OrdinalIgnoreCase)
                    .ToList();

            if (!selectedRoles.Any())
            {
                ModelState.AddModelError(
                    nameof(model.SelectedRoles),
                    "Please select at least one role.");

                await LoadManageRolesDataAsync(
                    model,
                    user);

                return PartialView(
                    "_ManageRolesPartial",
                    model);
            }

            var allRoles =
                await _roleManager.Roles
                    .AsNoTracking()
                    .Where(role =>
                        role.Name != null)
                    .Select(role =>
                        role.Name!)
                    .ToListAsync();

            var invalidRoles =
                selectedRoles
                    .Where(selected =>
                        !allRoles.Contains(
                            selected,
                            StringComparer
                                .OrdinalIgnoreCase))
                    .ToList();

            if (invalidRoles.Any())
            {
                ModelState.AddModelError(
                    nameof(model.SelectedRoles),
                    "One or more selected roles are invalid.");

                await LoadManageRolesDataAsync(
                    model,
                    user);

                return PartialView(
                    "_ManageRolesPartial",
                    model);
            }

            var currentRoles =
                await _userManager.GetRolesAsync(
                    user);

            var rolesToRemove =
                currentRoles
                    .Where(current =>
                        !selectedRoles.Contains(
                            current,
                            StringComparer
                                .OrdinalIgnoreCase))
                    .ToList();

            var rolesToAdd =
                selectedRoles
                    .Where(selected =>
                        !currentRoles.Contains(
                            selected,
                            StringComparer
                                .OrdinalIgnoreCase))
                    .ToList();

            if (rolesToRemove.Any())
            {
                var removeResult =
                    await _userManager
                        .RemoveFromRolesAsync(
                            user,
                            rolesToRemove);

                if (!removeResult.Succeeded)
                {
                    foreach (var error
                        in removeResult.Errors)
                    {
                        ModelState.AddModelError(
                            string.Empty,
                            error.Description);
                    }

                    await LoadManageRolesDataAsync(
                        model,
                        user);

                    return PartialView(
                        "_ManageRolesPartial",
                        model);
                }
            }

            if (rolesToAdd.Any())
            {
                var addResult =
                    await _userManager
                        .AddToRolesAsync(
                            user,
                            rolesToAdd);

                if (!addResult.Succeeded)
                {
                    foreach (var error
                        in addResult.Errors)
                    {
                        ModelState.AddModelError(
                            string.Empty,
                            error.Description);
                    }

                    await LoadManageRolesDataAsync(
                        model,
                        user);

                    return PartialView(
                        "_ManageRolesPartial",
                        model);
                }
            }

            return Json(new
            {
                success = true,
                message = "User roles updated successfully."
            });
        }

        // ---------------------------------------------------------
        // HELPERS
        // ---------------------------------------------------------

        private async Task LoadRolesAsync()
        {
            ViewBag.Roles =
                await _roleManager.Roles
                    .AsNoTracking()
                    .Where(role =>
                        role.Name != null)
                    .OrderBy(role =>
                        role.Name)
                    .ToListAsync();
        }

        private async Task LoadManageRolesDataAsync(
            ManageUserRolesViewModel model,
            ApplicationUser user)
        {
            var availableRoles =
                await _roleManager.Roles
                    .AsNoTracking()
                    .Where(role =>
                        role.Name != null)
                    .OrderBy(role =>
                        role.Name)
                    .Select(role =>
                        role.Name!)
                    .ToListAsync();

            model.UserDisplayName =
                string.Join(
                    " ",
                    new[]
                    {
                        user.FirstName,
                        user.MiddleName,
                        user.LastName
                    }
                    .Where(value =>
                        !string.IsNullOrWhiteSpace(
                            value)));

            model.UserName =
                user.UserName ?? string.Empty;

            model.AvailableRoles =
                availableRoles
                    .Select(roleName =>
                        new RoleSelectionViewModel
                        {
                            RoleName =
                                roleName,

                            Selected =
                                model.SelectedRoles.Contains(
                                    roleName,
                                    StringComparer
                                        .OrdinalIgnoreCase)
                        })
                    .ToList();
        }
    }
}