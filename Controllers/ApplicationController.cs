using System.Security.Claims;
using KISMApplicationManagement.Constants;
using KISMApplicationManagement.Data;
using KISMApplicationManagement.Models;
using KISMApplicationManagement.Models.ApplicationViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace KISMApplicationManagement.Controllers
{
    [Authorize]
    public class ApplicationController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ILogger<ApplicationController> _logger;

        public ApplicationController(
            ApplicationDbContext context,
            UserManager<ApplicationUser> userManager,
            ILogger<ApplicationController> logger)
        {
            _context = context;
            _userManager = userManager;
            _logger = logger;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var applicantProfile = await GetCurrentApplicantProfileAsync();

            if (applicantProfile == null)
            {
                return NotFound("Your applicant profile could not be found.");
            }

            var applications = await _context.Applications
                .AsNoTracking()
                .Where(application =>
                    application.ApplicantProfileId ==
                    applicantProfile.ApplicantProfileId)
                .Include(application => application.Intake)
                .Include(application => application.ProgrammeChoices)
                    .ThenInclude(choice => choice.Programme)
                .OrderByDescending(application => application.CreatedAtUtc)
                .ToListAsync();

            var viewModels = applications
                .Select(application =>
                {
                    var firstChoice = application.ProgrammeChoices
                        .FirstOrDefault(choice => choice.ChoiceNumber == 1);

                    var secondChoice = application.ProgrammeChoices
                        .FirstOrDefault(choice => choice.ChoiceNumber == 2);

                    return new ApplicationListViewModel
                    {
                        ApplicationId = application.ApplicationId,
                        ApplicationNumber = application.ApplicationNumber,
                        IntakeName = application.Intake.IntakeName,
                        AcademicYear = application.Intake.AcademicYear,
                        IntakePeriod = application.Intake.IntakePeriod,
                        Status = application.Status,
                        FirstChoiceProgramme = firstChoice?.Programme.ProgrammeName,
                        SecondChoiceProgramme = secondChoice?.Programme.ProgrammeName,
                        SubmittedAtUtc = application.SubmittedAtUtc,
                        CreatedAtUtc = application.CreatedAtUtc
                    };
                })
                .ToList();

            return View(viewModels);
        }

        // ============================================================
        // CREATE
        // ============================================================

        [HttpGet]
        public async Task<IActionResult> Create()
        {
            var availableIntakes = await GetAvailableIntakesAsync();
            var availableProgrammes = await GetAvailableProgrammesAsync();

            ViewBag.AvailableIntakes = availableIntakes;
            ViewBag.AvailableProgrammes = availableProgrammes;

            var model = new ApplicationCreateViewModel();

            if (IsAjaxRequest())
            {
                return PartialView("_CreatePartial", model);
            }

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(ApplicationCreateViewModel model)
        {
            if (!ModelState.IsValid)
            {
                await LoadCreateFormDataAsync();

                if (IsAjaxRequest())
                {
                    return PartialView("_CreatePartial", model);
                }

                return View(model);
            }

            if (model.FirstChoiceProgrammeId == model.SecondChoiceProgrammeId)
            {
                ModelState.AddModelError(
                    string.Empty,
                    "The first and second programme choices must be different.");

                await LoadCreateFormDataAsync();

                if (IsAjaxRequest())
                {
                    return PartialView("_CreatePartial", model);
                }

                return View(model);
            }

            var applicantProfile = await GetCurrentApplicantProfileAsync();

            if (applicantProfile == null)
            {
                if (IsAjaxRequest())
                {
                    return Json(new
                    {
                        success = false,
                        message = "Your applicant profile could not be found."
                    });
                }

                return NotFound("Your applicant profile could not be found.");
            }

            var intake = await _context.Intakes
                .FirstOrDefaultAsync(currentIntake =>
                    currentIntake.IntakeId == model.IntakeId);

            if (intake == null)
            {
                ModelState.AddModelError(
                    nameof(model.IntakeId),
                    "The selected intake could not be found.");

                await LoadCreateFormDataAsync();

                if (IsAjaxRequest())
                {
                    return PartialView("_CreatePartial", model);
                }

                return View(model);
            }

            var today = DateTime.UtcNow.Date;

            if (!intake.IsActive)
            {
                ModelState.AddModelError(
                    nameof(model.IntakeId),
                    "The selected intake is not active.");

                await LoadCreateFormDataAsync();

                if (IsAjaxRequest())
                {
                    return PartialView("_CreatePartial", model);
                }

                return View(model);
            }

            if (!intake.ApplicationsOpen)
            {
                ModelState.AddModelError(
                    nameof(model.IntakeId),
                    "Applications are not currently open for this intake.");

                await LoadCreateFormDataAsync();

                if (IsAjaxRequest())
                {
                    return PartialView("_CreatePartial", model);
                }

                return View(model);
            }

            if (today < intake.ApplicationOpeningDate.Date)
            {
                ModelState.AddModelError(
                    nameof(model.IntakeId),
                    "Applications have not opened for this intake.");

                await LoadCreateFormDataAsync();

                if (IsAjaxRequest())
                {
                    return PartialView("_CreatePartial", model);
                }

                return View(model);
            }

            if (today > intake.ApplicationClosingDate.Date)
            {
                ModelState.AddModelError(
                    nameof(model.IntakeId),
                    "The application deadline for this intake has passed.");

                await LoadCreateFormDataAsync();

                if (IsAjaxRequest())
                {
                    return PartialView("_CreatePartial", model);
                }

                return View(model);
            }

            var alreadyExists = await _context.Applications
                .AnyAsync(application =>
                    application.ApplicantProfileId ==
                    applicantProfile.ApplicantProfileId
                    && application.IntakeId == model.IntakeId);

            if (alreadyExists)
            {
                ModelState.AddModelError(
                    nameof(model.IntakeId),
                    "You already have an application for this intake.");

                await LoadCreateFormDataAsync();

                if (IsAjaxRequest())
                {
                    return PartialView("_CreatePartial", model);
                }

                return View(model);
            }

            var selectedProgrammes = await _context.Programmes
                .Where(programme =>
                    programme.IsActive &&
                    (programme.ProgrammeId == model.FirstChoiceProgrammeId ||
                     programme.ProgrammeId == model.SecondChoiceProgrammeId))
                .ToListAsync();

            if (selectedProgrammes.Count != 2)
            {
                ModelState.AddModelError(
                    string.Empty,
                    "Both selected programmes must be active and valid.");

                await LoadCreateFormDataAsync();

                if (IsAjaxRequest())
                {
                    return PartialView("_CreatePartial", model);
                }

                return View(model);
            }

            var now = DateTime.UtcNow;

            var application = new Application
            {
                ApplicationNumber = await GenerateApplicationNumberAsync(),
                ApplicantProfileId = applicantProfile.ApplicantProfileId,
                IntakeId = model.IntakeId,
                Status = ApplicationStatuses.Draft,
                CreatedAtUtc = now,
                UpdatedAtUtc = now
            };

            application.ProgrammeChoices.Add(
                new ApplicationProgrammeChoice
                {
                    ProgrammeId = model.FirstChoiceProgrammeId,
                    ChoiceNumber = 1,
                    CreatedAtUtc = now
                });

            application.ProgrammeChoices.Add(
                new ApplicationProgrammeChoice
                {
                    ProgrammeId = model.SecondChoiceProgrammeId,
                    ChoiceNumber = 2,
                    CreatedAtUtc = now
                });

            _context.Applications.Add(application);

            try
            {
                await _context.SaveChangesAsync();

                _logger.LogInformation(
                    "Application {ApplicationNumber} created for applicant profile {ApplicantProfileId}.",
                    application.ApplicationNumber,
                    applicantProfile.ApplicantProfileId);

                if (IsAjaxRequest())
                {
                    return Json(new
                    {
                        success = true,
                        message = "Application draft created successfully.",
                        applicationId = application.ApplicationId,
                        applicationNumber = application.ApplicationNumber
                    });
                }

                TempData["SuccessMessage"] =
                    "Application draft created successfully.";

                return RedirectToAction(nameof(Index));
            }
            catch (DbUpdateException exception)
            {
                _logger.LogError(
                    exception,
                    "Error creating application for applicant profile {ApplicantProfileId}.",
                    applicantProfile.ApplicantProfileId);

                if (IsAjaxRequest())
                {
                    return Json(new
                    {
                        success = false,
                        message =
                            "An error occurred while creating your application. Please try again."
                    });
                }

                ModelState.AddModelError(
                    string.Empty,
                    "An error occurred while creating your application. Please try again.");

                await LoadCreateFormDataAsync();

                return View(model);
            }
        }

        // ============================================================
        // EDIT
        // ============================================================

        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var applicantProfile = await GetCurrentApplicantProfileAsync();

            if (applicantProfile == null)
            {
                if (IsAjaxRequest())
                {
                    return Json(new
                    {
                        success = false,
                        message = "Your applicant profile could not be found."
                    });
                }

                return NotFound("Your applicant profile could not be found.");
            }

            var application = await _context.Applications
                .AsNoTracking()
                .Include(currentApplication =>
                    currentApplication.ProgrammeChoices)
                .FirstOrDefaultAsync(currentApplication =>
                    currentApplication.ApplicationId == id &&
                    currentApplication.ApplicantProfileId ==
                    applicantProfile.ApplicantProfileId);

            if (application == null)
            {
                if (IsAjaxRequest())
                {
                    return Json(new
                    {
                        success = false,
                        message = "The application could not be found."
                    });
                }

                return NotFound();
            }

            if (application.Status != ApplicationStatuses.Draft)
            {
                if (IsAjaxRequest())
                {
                    return Json(new
                    {
                        success = false,
                        message = "Only draft applications can be edited."
                    });
                }

                TempData["ErrorMessage"] =
                    "Only draft applications can be edited.";

                return RedirectToAction(nameof(Details), new { id });
            }

            var firstChoice = application.ProgrammeChoices
                .FirstOrDefault(choice => choice.ChoiceNumber == 1);

            var secondChoice = application.ProgrammeChoices
                .FirstOrDefault(choice => choice.ChoiceNumber == 2);

            var model = new ApplicationEditViewModel
            {
                ApplicationId = application.ApplicationId,
                FirstChoiceProgrammeId = firstChoice?.ProgrammeId ?? 0,
                SecondChoiceProgrammeId = secondChoice?.ProgrammeId ?? 0
            };

            await LoadEditFormDataAsync();

            if (IsAjaxRequest())
            {
                return PartialView("_EditPartial", model);
            }

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(ApplicationEditViewModel model)
        {
            if (!ModelState.IsValid)
            {
                await LoadEditFormDataAsync();

                if (IsAjaxRequest())
                {
                    return PartialView("_EditPartial", model);
                }

                return View(model);
            }

            if (model.FirstChoiceProgrammeId ==
                model.SecondChoiceProgrammeId)
            {
                ModelState.AddModelError(
                    string.Empty,
                    "The first and second programme choices must be different.");

                await LoadEditFormDataAsync();

                if (IsAjaxRequest())
                {
                    return PartialView("_EditPartial", model);
                }

                return View(model);
            }

            var applicantProfile = await GetCurrentApplicantProfileAsync();

            if (applicantProfile == null)
            {
                if (IsAjaxRequest())
                {
                    return Json(new
                    {
                        success = false,
                        message = "Your applicant profile could not be found."
                    });
                }

                return NotFound("Your applicant profile could not be found.");
            }

            var application = await _context.Applications
                .Include(currentApplication =>
                    currentApplication.ProgrammeChoices)
                .FirstOrDefaultAsync(currentApplication =>
                    currentApplication.ApplicationId == model.ApplicationId &&
                    currentApplication.ApplicantProfileId ==
                    applicantProfile.ApplicantProfileId);

            if (application == null)
            {
                if (IsAjaxRequest())
                {
                    return Json(new
                    {
                        success = false,
                        message = "The application could not be found."
                    });
                }

                return NotFound();
            }

            if (application.Status != ApplicationStatuses.Draft)
            {
                if (IsAjaxRequest())
                {
                    return Json(new
                    {
                        success = false,
                        message = "Only draft applications can be edited."
                    });
                }

                TempData["ErrorMessage"] =
                    "Only draft applications can be edited.";

                return RedirectToAction(
                    nameof(Details),
                    new { id = model.ApplicationId });
            }

            var selectedProgrammes = await _context.Programmes
                .Where(programme =>
                    programme.IsActive &&
                    (programme.ProgrammeId ==
                        model.FirstChoiceProgrammeId ||
                     programme.ProgrammeId ==
                        model.SecondChoiceProgrammeId))
                .ToListAsync();

            if (selectedProgrammes.Count != 2)
            {
                ModelState.AddModelError(
                    string.Empty,
                    "Both selected programmes must be active and valid.");

                await LoadEditFormDataAsync();

                if (IsAjaxRequest())
                {
                    return PartialView("_EditPartial", model);
                }

                return View(model);
            }

            var now = DateTime.UtcNow;

            var firstChoice = application.ProgrammeChoices
                .FirstOrDefault(choice => choice.ChoiceNumber == 1);

            var secondChoice = application.ProgrammeChoices
                .FirstOrDefault(choice => choice.ChoiceNumber == 2);

            if (firstChoice == null)
            {
                application.ProgrammeChoices.Add(
                    new ApplicationProgrammeChoice
                    {
                        ProgrammeId = model.FirstChoiceProgrammeId,
                        ChoiceNumber = 1,
                        CreatedAtUtc = now
                    });
            }
            else
            {
                firstChoice.ProgrammeId =
                    model.FirstChoiceProgrammeId;
            }

            if (secondChoice == null)
            {
                application.ProgrammeChoices.Add(
                    new ApplicationProgrammeChoice
                    {
                        ProgrammeId =
                            model.SecondChoiceProgrammeId,
                        ChoiceNumber = 2,
                        CreatedAtUtc = now
                    });
            }
            else
            {
                secondChoice.ProgrammeId =
                    model.SecondChoiceProgrammeId;
            }

            application.UpdatedAtUtc = now;

            try
            {
                await _context.SaveChangesAsync();

                _logger.LogInformation(
                    "Application {ApplicationId} updated.",
                    model.ApplicationId);

                if (IsAjaxRequest())
                {
                    return Json(new
                    {
                        success = true,
                        message = "Application draft updated successfully.",
                        applicationId = model.ApplicationId
                    });
                }

                TempData["SuccessMessage"] =
                    "Application draft updated successfully.";

                return RedirectToAction(
                    nameof(Details),
                    new { id = model.ApplicationId });
            }
            catch (DbUpdateException exception)
            {
                _logger.LogError(
                    exception,
                    "Error updating application {ApplicationId}.",
                    model.ApplicationId);

                if (IsAjaxRequest())
                {
                    return Json(new
                    {
                        success = false,
                        message =
                            "An error occurred while updating your application. Please try again."
                    });
                }

                ModelState.AddModelError(
                    string.Empty,
                    "An error occurred while updating your application. Please try again.");

                await LoadEditFormDataAsync();

                return View(model);
            }
        }

        // ============================================================
        // DETAILS
        // ============================================================

        [HttpGet]
        public async Task<IActionResult> Details(int id)
        {
            var applicantProfile = await GetCurrentApplicantProfileAsync();

            if (applicantProfile == null)
            {
                if (IsAjaxRequest())
                {
                    return Json(new
                    {
                        success = false,
                        message = "Your applicant profile could not be found."
                    });
                }

                return NotFound("Your applicant profile could not be found.");
            }

            var application = await _context.Applications
                .AsNoTracking()
                .Include(currentApplication =>
                    currentApplication.ApplicantProfile)
                .Include(currentApplication =>
                    currentApplication.Intake)
                .Include(currentApplication =>
                    currentApplication.ProgrammeChoices)
                    .ThenInclude(choice => choice.Programme)
                .FirstOrDefaultAsync(currentApplication =>
                    currentApplication.ApplicationId == id &&
                    currentApplication.ApplicantProfileId ==
                    applicantProfile.ApplicantProfileId);

            if (application == null)
            {
                if (IsAjaxRequest())
                {
                    return Json(new
                    {
                        success = false,
                        message = "The application could not be found."
                    });
                }

                return NotFound();
            }

            var firstChoice = application.ProgrammeChoices
                .FirstOrDefault(choice => choice.ChoiceNumber == 1);

            var secondChoice = application.ProgrammeChoices
                .FirstOrDefault(choice => choice.ChoiceNumber == 2);

            var model = new ApplicationDetailsViewModel
            {
                ApplicationId = application.ApplicationId,
                ApplicationNumber = application.ApplicationNumber,
                ApplicantName = application.ApplicantProfile.FullName,
                IntakeName = application.Intake.IntakeName,
                AcademicYear = application.Intake.AcademicYear,
                IntakePeriod = application.Intake.IntakePeriod,
                ApplicationOpeningDate =
                    application.Intake.ApplicationOpeningDate,
                ApplicationClosingDate =
                    application.Intake.ApplicationClosingDate,
                Status = application.Status,
                FirstChoiceProgramme =
                    firstChoice?.Programme.ProgrammeName,
                SecondChoiceProgramme =
                    secondChoice?.Programme.ProgrammeName,
                SubmittedAtUtc = application.SubmittedAtUtc,
                CreatedAtUtc = application.CreatedAtUtc,
                UpdatedAtUtc = application.UpdatedAtUtc
            };

            if (IsAjaxRequest())
            {
                return PartialView("_DetailsPartial", model);
            }

            return View(model);
        }

        // ============================================================
        // SUBMIT APPLICATION
        // ============================================================

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Submit(int id)
        {
            var applicantProfile = await GetCurrentApplicantProfileAsync();

            if (applicantProfile == null)
            {
                return HandleSubmitError(
                    id,
                    "Your applicant profile could not be found.",
                    notFound: true);
            }

            var application = await _context.Applications
                .Include(currentApplication =>
                    currentApplication.Intake)
                .Include(currentApplication =>
                    currentApplication.ProgrammeChoices)
                .FirstOrDefaultAsync(currentApplication =>
                    currentApplication.ApplicationId == id &&
                    currentApplication.ApplicantProfileId ==
                    applicantProfile.ApplicantProfileId);

            if (application == null)
            {
                return HandleSubmitError(
                    id,
                    "The application could not be found.",
                    notFound: true);
            }

            if (application.Status != ApplicationStatuses.Draft)
            {
                return HandleSubmitError(
                    id,
                    "Only draft applications can be submitted.");
            }

            var choices = application.ProgrammeChoices.ToList();

            if (choices.Count != 2)
            {
                return HandleSubmitError(
                    id,
                    "Your application must contain exactly two programme choices.",
                    nameof(Edit));
            }

            var hasChoiceOne = choices.Any(
                choice => choice.ChoiceNumber == 1);

            var hasChoiceTwo = choices.Any(
                choice => choice.ChoiceNumber == 2);

            var hasDuplicateProgrammes = choices
                .GroupBy(choice => choice.ProgrammeId)
                .Any(group => group.Count() > 1);

            if (!hasChoiceOne ||
                !hasChoiceTwo ||
                hasDuplicateProgrammes)
            {
                return HandleSubmitError(
                    id,
                    "Your application must contain two different programme choices: first and second.",
                    nameof(Edit));
            }

            var intake = application.Intake;
            var today = DateTime.UtcNow.Date;

            if (!intake.IsActive ||
                !intake.ApplicationsOpen ||
                today < intake.ApplicationOpeningDate.Date ||
                today > intake.ApplicationClosingDate.Date)
            {
                return HandleSubmitError(
                    id,
                    "This intake is no longer accepting applications.");
            }

            var now = DateTime.UtcNow;

            application.Status = ApplicationStatuses.PendingReview;
            application.SubmittedAtUtc = now;
            application.SubmittedByUserId =
                _userManager.GetUserId(User);
            application.UpdatedAtUtc = now;

            try
            {
                await _context.SaveChangesAsync();

                _logger.LogInformation(
                    "Application {ApplicationId} submitted by user {UserId}.",
                    id,
                    _userManager.GetUserId(User));

                if (IsAjaxRequest())
                {
                    return Json(new
                    {
                        success = true,
                        message =
                            "Your application was submitted successfully.",
                        applicationId = id
                    });
                }

                TempData["SuccessMessage"] =
                    "Your application was submitted successfully.";

                return RedirectToAction(
                    nameof(Details),
                    new { id });
            }
            catch (DbUpdateException exception)
            {
                _logger.LogError(
                    exception,
                    "Error submitting application {ApplicationId}.",
                    id);

                return HandleSubmitError(
                    id,
                    "An error occurred while submitting your application. Please try again.");
            }
        }

        // ============================================================
        // PRIVATE HELPERS
        // ============================================================

        private async Task<ApplicantProfile?>
            GetCurrentApplicantProfileAsync()
        {
            var userId = _userManager.GetUserId(User);

            if (string.IsNullOrWhiteSpace(userId))
            {
                return null;
            }

            return await _context.ApplicantProfiles
                .FirstOrDefaultAsync(profile =>
                    profile.UserId == userId);
        }

        private async Task<List<Intake>>
            GetAvailableIntakesAsync()
        {
            var today = DateTime.UtcNow.Date;

            return await _context.Intakes
                .AsNoTracking()
                .Where(intake =>
                    intake.IsActive &&
                    intake.ApplicationsOpen &&
                    today >= intake.ApplicationOpeningDate.Date &&
                    today <= intake.ApplicationClosingDate.Date)
                .OrderBy(intake =>
                    intake.ApplicationOpeningDate)
                .ThenBy(intake =>
                    intake.DisplayOrder)
                .ToListAsync();
        }

        private async Task<List<Programme>>
            GetAvailableProgrammesAsync()
        {
            return await _context.Programmes
                .AsNoTracking()
                .Where(programme =>
                    programme.IsActive)
                .OrderBy(programme =>
                    programme.DisplayOrder)
                .ThenBy(programme =>
                    programme.ProgrammeName)
                .ToListAsync();
        }

        private async Task LoadCreateFormDataAsync()
        {
            ViewBag.AvailableIntakes =
                await GetAvailableIntakesAsync();

            ViewBag.AvailableProgrammes =
                await GetAvailableProgrammesAsync();
        }

        private async Task LoadEditFormDataAsync()
        {
            ViewBag.AvailableProgrammes =
                await GetAvailableProgrammesAsync();
        }

        private async Task<string>
            GenerateApplicationNumberAsync()
        {
            var year = DateTime.UtcNow.Year;
            var prefix = $"APP-{year}-";

            var lastApplicationNumber =
                await _context.Applications
                    .AsNoTracking()
                    .Where(application =>
                        application.ApplicationNumber
                            .StartsWith(prefix))
                    .OrderByDescending(application =>
                        application.ApplicationId)
                    .Select(application =>
                        application.ApplicationNumber)
                    .FirstOrDefaultAsync();

            var nextNumber = 1;

            if (!string.IsNullOrWhiteSpace(
                lastApplicationNumber))
            {
                var numberPart =
                    lastApplicationNumber.Substring(
                        prefix.Length);

                if (int.TryParse(
                    numberPart,
                    out var lastNumber))
                {
                    nextNumber = lastNumber + 1;
                }
            }

            return $"{prefix}{nextNumber:D6}";
        }

        private bool IsAjaxRequest()
        {
            return string.Equals(
                Request.Headers["X-Requested-With"],
                "XMLHttpRequest",
                StringComparison.OrdinalIgnoreCase);
        }

        private IActionResult HandleSubmitError(
            int id,
            string message,
            string redirectAction = nameof(Details),
            bool notFound = false)
        {
            if (IsAjaxRequest())
            {
                return Json(new
                {
                    success = false,
                    message,
                    applicationId = id
                });
            }

            if (notFound)
            {
                return NotFound(message);
            }

            TempData["ErrorMessage"] = message;

            return RedirectToAction(
                redirectAction,
                new { id });
        }
    }
}