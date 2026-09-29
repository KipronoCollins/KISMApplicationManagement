using System.Security.Claims;
using KISMApplicationManagement.Constants;
using KISMApplicationManagement.Data;
using KISMApplicationManagement.Models;
using KISMApplicationManagement.Models.CommitteeSittingViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace KISMApplicationManagement.Controllers
{
    [Authorize(Roles = "System Administrator,Admissions Administrator")]
    public class CommitteeSittingsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<CommitteeSittingsController> _logger;

        public CommitteeSittingsController(
            ApplicationDbContext context,
            ILogger<CommitteeSittingsController> logger)
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
            var sittings = await _context.CommitteeSittings
                .AsNoTracking()
                .Include(sitting => sitting.Intake)
                .Include(sitting => sitting.Applications)
                    .ThenInclude(application => application.Decision)
                .OrderByDescending(sitting => sitting.SittingDate)
                .ThenByDescending(sitting => sitting.CommitteeSittingId)
                .ToListAsync();

            return View(sittings);
        }

        // =========================================================
        // Create - GET
        // =========================================================

        [HttpGet]
        public async Task<IActionResult> Create()
        {
            var model = new CommitteeSittingViewModel
            {
                SittingDate = DateTime.Today,
                Status = CommitteeSittingStatuses.Scheduled
            };

            await LoadIntakes(model);

            return PartialView("_CreatePartial", model);
        }

        // =========================================================
        // Create - POST
        // =========================================================

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(
            CommitteeSittingViewModel model)
        {
            model.ReferenceNumber =
                Normalize(model.ReferenceNumber);

            if (!ModelState.IsValid)
            {
                await LoadIntakes(model);

                return BadRequest(new
                {
                    success = false,
                    message = "Please correct the highlighted errors and try again."
                });
            }

            var intake = await _context.Intakes
                .AsNoTracking()
                .FirstOrDefaultAsync(intake =>
                    intake.IntakeId == model.IntakeId);

            if (intake == null)
            {
                return BadRequest(new
                {
                    success = false,
                    message = "The selected intake could not be found."
                });
            }

            if (!intake.IsActive)
            {
                return BadRequest(new
                {
                    success = false,
                    message = "The selected intake is not active."
                });
            }

            var normalizedReference =
                model.ReferenceNumber.ToUpperInvariant();

            var referenceExists = await _context.CommitteeSittings
                .AnyAsync(sitting =>
                    sitting.ReferenceNumber.ToUpper() ==
                    normalizedReference);

            if (referenceExists)
            {
                return BadRequest(new
                {
                    success = false,
                    message = "A committee sitting with this reference number already exists."
                });
            }

            var currentUserId =
                User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (string.IsNullOrWhiteSpace(currentUserId))
            {
                return BadRequest(new
                {
                    success = false,
                    message = "The current user could not be identified."
                });
            }

            var sitting = new CommitteeSitting
            {
                IntakeId = model.IntakeId,
                SittingDate = model.SittingDate.Date,
                ReferenceNumber = model.ReferenceNumber,
                Remarks = NormalizeNullable(model.Remarks),
                Status = CommitteeSittingStatuses.Scheduled,
                CreatedAtUtc = DateTime.UtcNow,
                CreatedByUserId = currentUserId
            };

            _context.CommitteeSittings.Add(sitting);

            await _context.SaveChangesAsync();

            _logger.LogInformation(
                "Committee sitting {CommitteeSittingId} with reference {ReferenceNumber} was created by user {UserId}.",
                sitting.CommitteeSittingId,
                sitting.ReferenceNumber,
                currentUserId);

            return Json(new
            {
                success = true,
                message = "Committee sitting created successfully."
            });
        }

        // =========================================================
        // Edit - GET
        // =========================================================

        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var sitting = await _context.CommitteeSittings
                .AsNoTracking()
                .FirstOrDefaultAsync(sitting =>
                    sitting.CommitteeSittingId == id);

            if (sitting == null)
            {
                return NotFound();
            }

            if (sitting.Status != CommitteeSittingStatuses.Scheduled)
            {
                return BadRequest(new
                {
                    success = false,
                    message = "Only scheduled committee sittings can be edited."
                });
            }

            var model = new CommitteeSittingViewModel
            {
                CommitteeSittingId = sitting.CommitteeSittingId,
                IntakeId = sitting.IntakeId,
                SittingDate = sitting.SittingDate,
                ReferenceNumber = sitting.ReferenceNumber,
                Remarks = sitting.Remarks,
                Status = sitting.Status
            };

            await LoadIntakes(model);

            return PartialView("_EditPartial", model);
        }

        // =========================================================
        // Edit - POST
        // =========================================================

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(
            CommitteeSittingViewModel model)
        {
            model.ReferenceNumber =
                Normalize(model.ReferenceNumber);

            if (!ModelState.IsValid)
            {
                await LoadIntakes(model);

                return BadRequest(new
                {
                    success = false,
                    message = "Please correct the highlighted errors and try again."
                });
            }

            var sitting = await _context.CommitteeSittings
                .FirstOrDefaultAsync(sitting =>
                    sitting.CommitteeSittingId ==
                    model.CommitteeSittingId);

            if (sitting == null)
            {
                return NotFound(new
                {
                    success = false,
                    message = "The committee sitting could not be found."
                });
            }

            if (sitting.Status != CommitteeSittingStatuses.Scheduled)
            {
                return BadRequest(new
                {
                    success = false,
                    message = "Only scheduled committee sittings can be edited."
                });
            }

            var intake = await _context.Intakes
                .AsNoTracking()
                .FirstOrDefaultAsync(intake =>
                    intake.IntakeId == model.IntakeId);

            if (intake == null)
            {
                return BadRequest(new
                {
                    success = false,
                    message = "The selected intake could not be found."
                });
            }

            if (!intake.IsActive)
            {
                return BadRequest(new
                {
                    success = false,
                    message = "The selected intake is not active."
                });
            }

            var normalizedReference =
                model.ReferenceNumber.ToUpperInvariant();

            var referenceExists = await _context.CommitteeSittings
                .AnyAsync(existing =>
                    existing.CommitteeSittingId !=
                        model.CommitteeSittingId &&
                    existing.ReferenceNumber.ToUpper() ==
                        normalizedReference);

            if (referenceExists)
            {
                return BadRequest(new
                {
                    success = false,
                    message = "A committee sitting with this reference number already exists."
                });
            }

            sitting.IntakeId = model.IntakeId;
            sitting.SittingDate = model.SittingDate.Date;
            sitting.ReferenceNumber = model.ReferenceNumber;
            sitting.Remarks = NormalizeNullable(model.Remarks);

            await _context.SaveChangesAsync();

            _logger.LogInformation(
                "Committee sitting {CommitteeSittingId} was updated.",
                sitting.CommitteeSittingId);

            return Json(new
            {
                success = true,
                message = "Committee sitting updated successfully."
            });
        }

        // =========================================================
        // Details
        // =========================================================

        [HttpGet]
        public async Task<IActionResult> Details(int id)
        {
            var sitting = await _context.CommitteeSittings
                .AsNoTracking()
                .Include(sitting => sitting.Intake)
                .Include(sitting => sitting.Applications)
                    .ThenInclude(application => application.Decision)
                .Include(sitting => sitting.Applications)
                    .ThenInclude(application => application.Application)
                .FirstOrDefaultAsync(sitting =>
                    sitting.CommitteeSittingId == id);

            if (sitting == null)
            {
                return NotFound();
            }

            return PartialView("_DetailsPartial", sitting);
        }

        // =========================================================
        // Start
        // =========================================================

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Start(int id)
        {
            var sitting = await _context.CommitteeSittings
                .Include(sitting => sitting.Applications)
                .FirstOrDefaultAsync(sitting =>
                    sitting.CommitteeSittingId == id);

            if (sitting == null)
            {
                return NotFound(new
                {
                    success = false,
                    message = "The committee sitting could not be found."
                });
            }

            if (sitting.Status != CommitteeSittingStatuses.Scheduled)
            {
                return BadRequest(new
                {
                    success = false,
                    message = "Only scheduled committee sittings can be started."
                });
            }

            if (sitting.Applications == null ||
                sitting.Applications.Count == 0)
            {
                return BadRequest(new
                {
                    success = false,
                    message = "The committee sitting must have at least one application before it can be started."
                });
            }

            sitting.Status =
                CommitteeSittingStatuses.InProgress;

            sitting.StartedAtUtc =
                DateTime.UtcNow;

            await _context.SaveChangesAsync();

            _logger.LogInformation(
                "Committee sitting {CommitteeSittingId} was started.",
                sitting.CommitteeSittingId);

            return Json(new
            {
                success = true,
                message = "Committee sitting started successfully."
            });
        }

        // =========================================================
        // Complete Sitting
        // =========================================================

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Complete(int id)
        {
            var sitting = await _context.CommitteeSittings
                .Include(sitting => sitting.Applications)
                    .ThenInclude(application => application.Decision)
                .FirstOrDefaultAsync(sitting =>
                    sitting.CommitteeSittingId == id);

            if (sitting == null)
            {
                return NotFound(new
                {
                    success = false,
                    message = "The committee sitting could not be found."
                });
            }

            if (sitting.Status != CommitteeSittingStatuses.InProgress)
            {
                return BadRequest(new
                {
                    success = false,
                    message = "Only an in-progress committee sitting can be completed."
                });
            }

            if (sitting.Applications == null ||
                sitting.Applications.Count == 0)
            {
                return BadRequest(new
                {
                    success = false,
                    message = "The committee sitting has no applications."
                });
            }

            var applicationsWithoutDecision =
                sitting.Applications
                    .Where(application =>
                        application.Decision == null)
                    .ToList();

            if (applicationsWithoutDecision.Count > 0)
            {
                return BadRequest(new
                {
                    success = false,
                    message =
                        $"{applicationsWithoutDecision.Count} application(s) do not have a committee decision. Please record a decision for every application before completing the sitting.",
                    incompleteCount =
                        applicationsWithoutDecision.Count
                });
            }

            var invalidDecisions =
                sitting.Applications
                    .Where(application =>
                        application.Decision != null &&
                        !IsValidDecision(application.Decision))
                    .ToList();

            if (invalidDecisions.Count > 0)
            {
                return BadRequest(new
                {
                    success = false,
                    message =
                        "One or more committee decisions are invalid. Please review the decisions before completing the sitting."
                });
            }

            sitting.Status =
                CommitteeSittingStatuses.Completed;

            sitting.CompletedAtUtc =
                DateTime.UtcNow;

            await _context.SaveChangesAsync();

            _logger.LogInformation(
                "Committee sitting {CommitteeSittingId} was completed.",
                sitting.CommitteeSittingId);

            return Json(new
            {
                success = true,
                message = "Committee sitting completed successfully."
            });
        }

        // =========================================================
        // Cancel
        // =========================================================

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Cancel(int id)
        {
            var sitting = await _context.CommitteeSittings
                .FirstOrDefaultAsync(sitting =>
                    sitting.CommitteeSittingId == id);

            if (sitting == null)
            {
                return NotFound(new
                {
                    success = false,
                    message = "The committee sitting could not be found."
                });
            }

            if (sitting.Status ==
                CommitteeSittingStatuses.Completed)
            {
                return BadRequest(new
                {
                    success = false,
                    message = "A completed committee sitting cannot be cancelled."
                });
            }

            if (sitting.Status ==
                CommitteeSittingStatuses.Cancelled)
            {
                return BadRequest(new
                {
                    success = false,
                    message = "This committee sitting has already been cancelled."
                });
            }

            sitting.Status =
                CommitteeSittingStatuses.Cancelled;

            await _context.SaveChangesAsync();

            _logger.LogInformation(
                "Committee sitting {CommitteeSittingId} was cancelled.",
                sitting.CommitteeSittingId);

            return Json(new
            {
                success = true,
                message = "Committee sitting cancelled successfully."
            });
        }

        // =========================================================
        // Manage Applications - GET
        // =========================================================

        [HttpGet]
        public async Task<IActionResult> Manage(int id)
        {
            var sitting = await _context.CommitteeSittings
                .AsNoTracking()
                .Include(sitting => sitting.Intake)
                .Include(sitting => sitting.Applications)
                    .ThenInclude(sittingApplication =>
                        sittingApplication.Application)
                        .ThenInclude(application =>
                            application.ApplicantProfile)
                .Include(sitting => sitting.Applications)
                    .ThenInclude(sittingApplication =>
                        sittingApplication.Decision)
                .FirstOrDefaultAsync(sitting =>
                    sitting.CommitteeSittingId == id);

            if (sitting == null)
            {
                return NotFound();
            }

            if (sitting.Status != CommitteeSittingStatuses.Scheduled &&
                sitting.Status != CommitteeSittingStatuses.InProgress &&
                sitting.Status != CommitteeSittingStatuses.Completed)
            {
                return BadRequest(new
                {
                    success = false,
                    message = "This committee sitting cannot be managed in its current status."
                });
            }

            var model = new CommitteeSittingManagementViewModel
            {
                CommitteeSittingId = sitting.CommitteeSittingId,
                IntakeId = sitting.IntakeId,
                IntakeName = sitting.Intake?.IntakeName ?? string.Empty,
                AcademicYear = sitting.Intake?.AcademicYear ?? string.Empty,
                IntakePeriod = sitting.Intake?.IntakePeriod ?? string.Empty,
                SittingDate = sitting.SittingDate,
                ReferenceNumber = sitting.ReferenceNumber,
                Status = sitting.Status,
                Remarks = sitting.Remarks,
                ApplicationCount = sitting.Applications.Count
            };

            model.SittingApplications = sitting.Applications
                .OrderBy(application => application.DisplayOrder)
                .Select(application => new CommitteeSittingApplicationItemViewModel
                {
                    CommitteeSittingApplicationId =
                        application.CommitteeSittingApplicationId,

                    ApplicationId =
                        application.ApplicationId,

                    ApplicationNumber =
                        application.Application?.ApplicationNumber
                        ?? string.Empty,

                    ApplicantName =
                        application.Application?.ApplicantProfile?.FullName
                        ?? string.Empty,

                    DisplayOrder =
                        application.DisplayOrder,

                    Decision =
                        application.Decision?.Decision,

                    HasDecision =
                        application.Decision != null
                })
                .ToList();

            if (sitting.Status ==
                CommitteeSittingStatuses.Scheduled)
            {
                model.EligibleApplications =
                    await LoadEligibleApplications(
                        sitting.CommitteeSittingId,
                        sitting.IntakeId);
            }

            return View("Manage", model);
        }

        // =========================================================
        // Add Applications - POST
        // =========================================================

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddApplications(
            AddCommitteeApplicationsViewModel model)
        {
            if (model.CommitteeSittingId <= 0)
            {
                return BadRequest(new
                {
                    success = false,
                    message = "Invalid committee sitting."
                });
            }

            var sitting = await _context.CommitteeSittings
                .FirstOrDefaultAsync(sitting =>
                    sitting.CommitteeSittingId ==
                    model.CommitteeSittingId);

            if (sitting == null)
            {
                return NotFound(new
                {
                    success = false,
                    message = "The committee sitting could not be found."
                });
            }

            if (sitting.Status != CommitteeSittingStatuses.Scheduled)
            {
                return BadRequest(new
                {
                    success = false,
                    message = "Applications can only be added while the committee sitting is scheduled."
                });
            }

            var selectedApplicationIds =
                model.ApplicationIds
                    .Where(id => id > 0)
                    .Distinct()
                    .ToList();

            if (selectedApplicationIds.Count == 0)
            {
                return BadRequest(new
                {
                    success = false,
                    message = "Please select at least one application."
                });
            }

            var currentApplicationIds =
                await _context.CommitteeSittingApplications
                    .Where(sittingApplication =>
                        sittingApplication.CommitteeSittingId ==
                        sitting.CommitteeSittingId)
                    .Select(sittingApplication =>
                        sittingApplication.ApplicationId)
                    .ToListAsync();

            var alreadyInThisSitting =
                selectedApplicationIds
                    .Intersect(currentApplicationIds)
                    .ToList();

            if (alreadyInThisSitting.Count > 0)
            {
                return BadRequest(new
                {
                    success = false,
                    message = "One or more selected applications are already in this committee sitting."
                });
            }

            var eligibleApplications = await _context.Applications
                .AsNoTracking()
                .Where(application =>
                    application.IntakeId == sitting.IntakeId &&
                    application.Status ==
                        ApplicationStatuses.PendingReview &&
                    selectedApplicationIds.Contains(
                        application.ApplicationId))
                .Select(application =>
                    application.ApplicationId)
                .ToListAsync();

            var invalidApplicationIds =
                selectedApplicationIds
                    .Except(eligibleApplications)
                    .ToList();

            if (invalidApplicationIds.Count > 0)
            {
                return BadRequest(new
                {
                    success = false,
                    message = "One or more selected applications are no longer eligible for this committee sitting. Please refresh the page and try again."
                });
            }

            var activeSittingApplicationIds =
                await _context.CommitteeSittingApplications
                    .Where(sittingApplication =>
                        selectedApplicationIds.Contains(
                            sittingApplication.ApplicationId) &&
                        (sittingApplication.CommitteeSitting.Status ==
                            CommitteeSittingStatuses.Scheduled ||
                         sittingApplication.CommitteeSitting.Status ==
                            CommitteeSittingStatuses.InProgress))
                    .Select(sittingApplication =>
                        sittingApplication.ApplicationId)
                    .Distinct()
                    .ToListAsync();

            if (activeSittingApplicationIds.Count > 0)
            {
                return BadRequest(new
                {
                    success = false,
                    message = "One or more selected applications are already assigned to another active committee sitting."
                });
            }

            var currentMaximumDisplayOrder =
                await _context.CommitteeSittingApplications
                    .Where(sittingApplication =>
                        sittingApplication.CommitteeSittingId ==
                        sitting.CommitteeSittingId)
                    .Select(sittingApplication =>
                        (int?)sittingApplication.DisplayOrder)
                    .MaxAsync() ?? 0;

            var currentUserId =
                User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (string.IsNullOrWhiteSpace(currentUserId))
            {
                return BadRequest(new
                {
                    success = false,
                    message = "The current user could not be identified."
                });
            }

            var now = DateTime.UtcNow;

            var newSittingApplications =
                eligibleApplications
                    .OrderBy(applicationId =>
                        selectedApplicationIds.IndexOf(applicationId))
                    .Select((applicationId, index) =>
                        new CommitteeSittingApplication
                        {
                            CommitteeSittingId =
                                sitting.CommitteeSittingId,

                            ApplicationId =
                                applicationId,

                            DisplayOrder =
                                currentMaximumDisplayOrder +
                                index +
                                1,

                            AddedAtUtc =
                                now,

                            AddedByUserId =
                                currentUserId
                        })
                    .ToList();

            _context.CommitteeSittingApplications.AddRange(
                newSittingApplications);

            await _context.SaveChangesAsync();

            _logger.LogInformation(
                "User {UserId} added {ApplicationCount} applications to committee sitting {CommitteeSittingId}.",
                currentUserId,
                newSittingApplications.Count,
                sitting.CommitteeSittingId);

            return Json(new
            {
                success = true,
                message =
                    $"{newSittingApplications.Count} application(s) added to the committee sitting successfully."
            });
        }

        // =========================================================
        // Remove Application From Sitting
        // =========================================================

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RemoveApplication(
            int id)
        {
            var sittingApplication =
                await _context.CommitteeSittingApplications
                    .Include(application =>
                        application.CommitteeSitting)
                    .FirstOrDefaultAsync(application =>
                        application.CommitteeSittingApplicationId ==
                        id);

            if (sittingApplication == null)
            {
                return NotFound(new
                {
                    success = false,
                    message = "The selected application could not be found in the committee sitting."
                });
            }

            if (sittingApplication.CommitteeSitting.Status !=
                CommitteeSittingStatuses.Scheduled)
            {
                return BadRequest(new
                {
                    success = false,
                    message = "Applications can only be removed while the committee sitting is scheduled."
                });
            }

            _context.CommitteeSittingApplications.Remove(
                sittingApplication);

            await _context.SaveChangesAsync();

            _logger.LogInformation(
                "Application {ApplicationId} was removed from committee sitting {CommitteeSittingId}.",
                sittingApplication.ApplicationId,
                sittingApplication.CommitteeSittingId);

            return Json(new
            {
                success = true,
                message = "Application removed from the committee sitting."
            });
        }

        // =========================================================
        // Load Eligible Applications
        // =========================================================

        private async Task<List<EligibleCommitteeApplicationViewModel>>
            LoadEligibleApplications(
                int committeeSittingId,
                int intakeId)
        {
            var currentSittingApplicationIds =
                _context.CommitteeSittingApplications
                    .Where(sittingApplication =>
                        sittingApplication.CommitteeSittingId ==
                        committeeSittingId)
                    .Select(sittingApplication =>
                        sittingApplication.ApplicationId);

            var activeCommitteeApplicationIds =
                _context.CommitteeSittingApplications
                    .Where(sittingApplication =>
                        sittingApplication.CommitteeSittingId !=
                            committeeSittingId &&
                        (sittingApplication.CommitteeSitting.Status ==
                            CommitteeSittingStatuses.Scheduled ||
                         sittingApplication.CommitteeSitting.Status ==
                            CommitteeSittingStatuses.InProgress))
                    .Select(sittingApplication =>
                        sittingApplication.ApplicationId);

            var applications = await _context.Applications
                .AsNoTracking()
                .Where(application =>
                    application.IntakeId == intakeId &&
                    application.Status ==
                        ApplicationStatuses.PendingReview &&
                    !currentSittingApplicationIds.Contains(
                        application.ApplicationId) &&
                    !activeCommitteeApplicationIds.Contains(
                        application.ApplicationId))
                .Include(application =>
                    application.ApplicantProfile)
                .Include(application =>
                    application.ProgrammeChoices)
                    .ThenInclude(choice =>
                        choice.Programme)
                .OrderByDescending(application =>
                    application.SubmittedAtUtc)
                .ThenBy(application =>
                    application.ApplicationNumber)
                .ToListAsync();

            return applications
                .Select(application =>
                    new EligibleCommitteeApplicationViewModel
                    {
                        ApplicationId =
                            application.ApplicationId,

                        ApplicationNumber =
                            application.ApplicationNumber,

                        ApplicantName =
                            application.ApplicantProfile?.FullName
                            ?? string.Empty,

                        Status =
                            application.Status,

                        SubmittedAtUtc =
                            application.SubmittedAtUtc,

                        ProgrammeChoices =
                            application.ProgrammeChoices
                                .OrderBy(choice =>
                                    choice.ChoiceNumber)
                                .Select(choice =>
                                    new CommitteeApplicationProgrammeChoiceViewModel
                                    {
                                        ChoiceNumber =
                                            choice.ChoiceNumber,

                                        ProgrammeCode =
                                            choice.Programme?.ProgrammeCode
                                            ?? string.Empty,

                                        ProgrammeName =
                                            choice.Programme?.ProgrammeName
                                            ?? string.Empty
                                    })
                                .ToList()
                    })
                .ToList();
        }

        // =========================================================
        // Load Active Intakes
        // =========================================================

        private async Task LoadIntakes(
            CommitteeSittingViewModel model)
        {
            model.Intakes = await _context.Intakes
                .AsNoTracking()
                .Where(intake => intake.IsActive)
                .OrderByDescending(intake =>
                    intake.ApplicationOpeningDate)
                .ThenBy(intake => intake.DisplayOrder)
                .ThenBy(intake => intake.IntakeName)
                .Select(intake => new IntakeSelectItemViewModel
                {
                    IntakeId = intake.IntakeId,
                    IntakeName = intake.IntakeName,
                    AcademicYear = intake.AcademicYear,
                    IntakePeriod = intake.IntakePeriod
                })
                .ToListAsync();
        }

        // =========================================================
        // Validate Committee Decision
        // =========================================================

        private static bool IsValidDecision(
            CommitteeDecision decision)
        {
            if (decision.Decision ==
                CommitteeDecisionResults.RecommendedForAdmission)
            {
                return decision.AssignedProgrammeId.HasValue;
            }

            if (decision.Decision ==
                CommitteeDecisionResults.NotRecommended)
            {
                return !decision.AssignedProgrammeId.HasValue;
            }

            if (decision.Decision ==
                CommitteeDecisionResults.Deferred)
            {
                return true;
            }

            return false;
        }

        // =========================================================
        // Normalize Required String
        // =========================================================

        private static string Normalize(string? value)
        {
            return string.IsNullOrWhiteSpace(value)
                ? string.Empty
                : value.Trim();
        }

        // =========================================================
        // Normalize Nullable String
        // =========================================================

        private static string? NormalizeNullable(string? value)
        {
            return string.IsNullOrWhiteSpace(value)
                ? null
                : value.Trim();
        }
    }
}