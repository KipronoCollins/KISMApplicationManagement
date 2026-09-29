using KISMApplicationManagement.Constants;
using KISMApplicationManagement.Data;
using KISMApplicationManagement.Models;
using KISMApplicationManagement.Models.CommitteeSittingViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace KISMApplicationManagement.Controllers
{
    [Authorize(Roles = "System Administrator,Admissions Administrator")]
    public class CommitteeWorkbenchController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ILogger<CommitteeWorkbenchController> _logger;

        public CommitteeWorkbenchController(
            ApplicationDbContext context,
            UserManager<ApplicationUser> userManager,
            ILogger<CommitteeWorkbenchController> logger)
        {
            _context = context;
            _userManager = userManager;
            _logger = logger;
        }

        // ============================================================
        // WORKBENCH
        // ============================================================

        [HttpGet]
        public async Task<IActionResult> Index(
            int committeeSittingId,
            int position = 1)
        {
            var sitting = await _context.CommitteeSittings
                .AsNoTracking()
                .Include(s => s.Intake)
                .FirstOrDefaultAsync(s =>
                    s.CommitteeSittingId == committeeSittingId);

            if (sitting == null)
            {
                return NotFound();
            }

            if (sitting.Status != CommitteeSittingStatuses.InProgress)
            {
                TempData["ErrorMessage"] =
                    "The committee workbench is only available for a sitting that is in progress.";

                return RedirectToAction(
                    "Manage",
                    "CommitteeSittings",
                    new { id = committeeSittingId });
            }

            var applications = await _context.CommitteeSittingApplications
                .AsNoTracking()
                .Where(x =>
                    x.CommitteeSittingId == committeeSittingId)
                .OrderBy(x => x.DisplayOrder)
                .Select(x => new
                {
                    x.CommitteeSittingApplicationId,
                    x.ApplicationId,
                    x.DisplayOrder
                })
                .ToListAsync();

            if (applications.Count == 0)
            {
                TempData["ErrorMessage"] =
                    "There are no applications in this committee sitting.";

                return RedirectToAction(
                    "Manage",
                    "CommitteeSittings",
                    new { id = committeeSittingId });
            }

            if (position < 1)
            {
                position = 1;
            }

            if (position > applications.Count)
            {
                position = applications.Count;
            }

            var selectedApplication = applications[position - 1];

            var sittingApplication =
                await _context.CommitteeSittingApplications
                    .AsNoTracking()
                    .Include(x => x.Application)
                        .ThenInclude(x => x.ApplicantProfile)
                            .ThenInclude(x => x.County)
                    .Include(x => x.Application)
                        .ThenInclude(x => x.ProgrammeChoices)
                            .ThenInclude(x => x.Programme)
                    .Include(x => x.Decision)
                    .FirstOrDefaultAsync(x =>
                        x.CommitteeSittingApplicationId ==
                        selectedApplication.CommitteeSittingApplicationId);

            if (sittingApplication == null)
            {
                return NotFound();
            }

            var applicantUserId =
                sittingApplication.Application.ApplicantProfile.UserId;

            var qualifications = await _context.ApplicantQualifications
                .AsNoTracking()
                .Include(q => q.Subjects)
                .Where(q => q.UserId == applicantUserId)
                .OrderByDescending(q => q.QualificationYear)
                .ThenBy(q => q.ApplicantQualificationId)
                .ToListAsync();

            var programmes = await _context.Programmes
                .AsNoTracking()
                .Where(p => p.IsActive)
                .OrderBy(p => p.DisplayOrder)
                .ThenBy(p => p.ProgrammeName)
                .ToListAsync();

            var applicant =
                sittingApplication.Application.ApplicantProfile;

            var viewModel = new CommitteeWorkbenchViewModel
            {
                CommitteeSittingId =
                    sitting.CommitteeSittingId,

                ReferenceNumber =
                    sitting.ReferenceNumber,

                IntakeName =
                    BuildIntakeName(sitting.Intake),

                SittingDate =
                    sitting.SittingDate,

                CurrentPosition =
                    position,

                TotalApplications =
                    applications.Count,

                HasPrevious =
                    position > 1,

                HasNext =
                    position < applications.Count,

                CommitteeSittingApplicationId =
                    sittingApplication.CommitteeSittingApplicationId,

                ApplicationId =
                    sittingApplication.ApplicationId,

                ApplicationNumber =
                    sittingApplication.Application.ApplicationNumber,

                Applicant = new ApplicantCommitteeInformationViewModel
                {
                    FullName =
                        applicant.FullName,

                    IdPassportBirthCertificateNo =
                        applicant.IdPassportBirthCertificateNo,

                    MobileNumber =
                        applicant.MobileNumber,

                    ContactEmail =
                        applicant.ContactEmail,

                    DateOfBirth =
                        applicant.DateOfBirth,

                    PlaceOfBirth =
                        applicant.PlaceOfBirth,

                    ContactAddress =
                        applicant.ContactAddress,

                    Town =
                        applicant.Town,

                    CountyName =
                        applicant.County?.CountyName
                },

                Qualifications =
                    qualifications
                        .Select(q => new CommitteeQualificationViewModel
                        {
                            ApplicantQualificationId =
                                q.ApplicantQualificationId,

                            QualificationRoute =
                                q.QualificationRoute,

                            QualificationYear =
                                q.QualificationYear,

                            ExaminationIndexNumber =
                                q.ExaminationIndexNumber,

                            MeanGrade =
                                q.MeanGrade,

                            QualificationType =
                                q.QualificationType,

                            InstitutionName =
                                q.InstitutionName,

                            FieldOfStudy =
                                q.FieldOfStudy,

                            CertificateNumber =
                                q.CertificateNumber,

                            Description =
                                q.Description,

                            VerificationStatus =
                                q.VerificationStatus,

                            Subjects =
                                q.Subjects
                                    .OrderBy(s => s.DisplayOrder)
                                    .ThenBy(s => s.SubjectName)
                                    .Select(s =>
                                        new CommitteeQualificationSubjectViewModel
                                        {
                                            SubjectName =
                                                s.SubjectName,

                                            SubjectCode =
                                                s.SubjectCode,

                                            Grade =
                                                s.Grade,

                                            DisplayOrder =
                                                s.DisplayOrder
                                        })
                                    .ToList()
                        })
                        .ToList(),

                ProgrammeChoices =
                    sittingApplication
                        .Application
                        .ProgrammeChoices
                        .OrderBy(x => x.ChoiceNumber)
                        .Select(x =>
                            new CommitteeProgrammeChoiceViewModel
                            {
                                ChoiceNumber =
                                    x.ChoiceNumber,

                                ProgrammeId =
                                    x.ProgrammeId,

                                ProgrammeCode =
                                    x.Programme.ProgrammeCode,

                                ProgrammeName =
                                    x.Programme.ProgrammeName,

                                AwardType =
                                    x.Programme.AwardType,

                                DurationYears =
                                    x.Programme.DurationYears
                            })
                        .ToList(),

                Decision = new CommitteeDecisionEntryViewModel
                {
                    Decision =
                        sittingApplication.Decision?.Decision
                        ?? string.Empty,

                    AssignedProgrammeId =
                        sittingApplication.Decision?.AssignedProgrammeId,

                    Remarks =
                        sittingApplication.Decision?.Remarks
                },

                AvailableProgrammes =
                    programmes
                        .Select(p =>
                            new CommitteeProgrammeOptionViewModel
                            {
                                ProgrammeId =
                                    p.ProgrammeId,

                                ProgrammeCode =
                                    p.ProgrammeCode,

                                ProgrammeName =
                                    p.ProgrammeName,

                                AwardType =
                                    p.AwardType
                            })
                        .ToList()
            };

            return View(viewModel);
        }


        // ============================================================
        // SAVE DECISION
        // ============================================================

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SaveDecision(
            CommitteeDecisionEntryViewModel model,
            int committeeSittingId,
            int committeeSittingApplicationId,
            int position)
        {
            if (!ModelState.IsValid)
            {
                return Json(new
                {
                    success = false,
                    message = "Please select a committee decision."
                });
            }

            var sitting = await _context.CommitteeSittings
                .FirstOrDefaultAsync(x =>
                    x.CommitteeSittingId ==
                    committeeSittingId);

            if (sitting == null)
            {
                return Json(new
                {
                    success = false,
                    message = "Committee sitting was not found."
                });
            }

            if (sitting.Status != CommitteeSittingStatuses.InProgress)
            {
                return Json(new
                {
                    success = false,
                    message =
                        "This committee sitting is no longer open for decision entry."
                });
            }

            var sittingApplication =
                await _context.CommitteeSittingApplications
                    .Include(x => x.Decision)
                    .FirstOrDefaultAsync(x =>
                        x.CommitteeSittingApplicationId ==
                        committeeSittingApplicationId &&
                        x.CommitteeSittingId ==
                        committeeSittingId);

            if (sittingApplication == null)
            {
                return Json(new
                {
                    success = false,
                    message =
                        "The application could not be found in this sitting."
                });
            }

            var decisionValue =
                NormalizeDecision(model.Decision);

            if (decisionValue == null)
            {
                return Json(new
                {
                    success = false,
                    message =
                        "Please select a valid committee decision."
                });
            }

            // ========================================================
            // RECOMMENDED FOR ADMISSION
            // Assigned Programme is REQUIRED.
            // ========================================================

            if (decisionValue ==
                CommitteeDecisionResults.RecommendedForAdmission)
            {
                if (!model.AssignedProgrammeId.HasValue)
                {
                    return Json(new
                    {
                        success = false,
                        message =
                            "Please select the programme recommended for admission."
                    });
                }

                var assignedProgrammeExists =
                    await _context.Programmes
                        .AnyAsync(p =>
                            p.ProgrammeId ==
                            model.AssignedProgrammeId.Value &&
                            p.IsActive);

                if (!assignedProgrammeExists)
                {
                    return Json(new
                    {
                        success = false,
                        message =
                            "The selected programme is not available."
                    });
                }
            }

            // ========================================================
            // NOT RECOMMENDED
            // Assigned Programme MUST be empty.
            // ========================================================

            if (decisionValue ==
                CommitteeDecisionResults.NotRecommended)
            {
                model.AssignedProgrammeId = null;
            }

            // ========================================================
            // DEFERRED
            // Assigned Programme is optional.
            // ========================================================

            if (decisionValue ==
                CommitteeDecisionResults.Deferred)
            {
                if (model.AssignedProgrammeId.HasValue)
                {
                    var programmeExists =
                        await _context.Programmes
                            .AnyAsync(p =>
                                p.ProgrammeId ==
                                model.AssignedProgrammeId.Value &&
                                p.IsActive);

                    if (!programmeExists)
                    {
                        return Json(new
                        {
                            success = false,
                            message =
                                "The selected programme is not available."
                        });
                    }
                }
            }

            var currentUserId =
                _userManager.GetUserId(User);

            if (string.IsNullOrWhiteSpace(currentUserId))
            {
                return Json(new
                {
                    success = false,
                    message =
                        "Unable to identify the current user."
                });
            }

            var now = DateTime.UtcNow;

            if (sittingApplication.Decision == null)
            {
                sittingApplication.Decision =
                    new CommitteeDecision
                    {
                        CommitteeSittingApplicationId =
                            committeeSittingApplicationId,

                        Decision =
                            decisionValue,

                        AssignedProgrammeId =
                            model.AssignedProgrammeId,

                        Remarks =
                            NormalizeNullable(model.Remarks),

                        DecisionDateUtc =
                            now,

                        RecordedByUserId =
                            currentUserId,

                        CreatedAtUtc =
                            now,

                        UpdatedAtUtc =
                            null
                    };

                _context.CommitteeDecisions.Add(
                    sittingApplication.Decision);
            }
            else
            {
                sittingApplication.Decision.Decision =
                    decisionValue;

                sittingApplication.Decision.AssignedProgrammeId =
                    model.AssignedProgrammeId;

                sittingApplication.Decision.Remarks =
                    NormalizeNullable(model.Remarks);

                sittingApplication.Decision.DecisionDateUtc =
                    now;

                sittingApplication.Decision.UpdatedAtUtc =
                    now;

                sittingApplication.Decision.RecordedByUserId =
                    currentUserId;
            }

            await _context.SaveChangesAsync();

            return Json(new
            {
                success = true,
                message = "Committee decision saved successfully."
            });
        }


        // ============================================================
        // SAVE & NEXT
        // ============================================================

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SaveAndNext(
            CommitteeDecisionEntryViewModel model,
            int committeeSittingId,
            int committeeSittingApplicationId,
            int position)
        {
            var saveResult =
                await SaveDecisionInternal(
                    model,
                    committeeSittingId,
                    committeeSittingApplicationId);

            if (!saveResult.Success)
            {
                return Json(new
                {
                    success = false,
                    message = saveResult.Message
                });
            }

            var totalApplications =
                await _context.CommitteeSittingApplications
                    .CountAsync(x =>
                        x.CommitteeSittingId ==
                        committeeSittingId);

            if (position >= totalApplications)
            {
                return Json(new
                {
                    success = true,
                    completed = true,
                    message =
                        "Decision saved. You have reached the last application."
                });
            }

            return Json(new
            {
                success = true,
                completed = false,
                nextPosition = position + 1
            });
        }


        // ============================================================
        // INTERNAL SAVE
        // ============================================================

        private async Task<(bool Success, string Message)>
            SaveDecisionInternal(
                CommitteeDecisionEntryViewModel model,
                int committeeSittingId,
                int committeeSittingApplicationId)
        {
            var sitting = await _context.CommitteeSittings
                .FirstOrDefaultAsync(x =>
                    x.CommitteeSittingId ==
                    committeeSittingId);

            if (sitting == null)
            {
                return (
                    false,
                    "Committee sitting was not found.");
            }

            if (sitting.Status !=
                CommitteeSittingStatuses.InProgress)
            {
                return (
                    false,
                    "This committee sitting is not open for decisions.");
            }

            var sittingApplication =
                await _context.CommitteeSittingApplications
                    .Include(x => x.Decision)
                    .FirstOrDefaultAsync(x =>
                        x.CommitteeSittingApplicationId ==
                        committeeSittingApplicationId &&
                        x.CommitteeSittingId ==
                        committeeSittingId);

            if (sittingApplication == null)
            {
                return (
                    false,
                    "The application could not be found in this sitting.");
            }

            var decisionValue =
                NormalizeDecision(model.Decision);

            if (decisionValue == null)
            {
                return (
                    false,
                    "Please select a valid committee decision.");
            }

            if (decisionValue ==
                CommitteeDecisionResults.RecommendedForAdmission)
            {
                if (!model.AssignedProgrammeId.HasValue)
                {
                    return (
                        false,
                        "Please select the programme recommended for admission.");
                }

                var exists =
                    await _context.Programmes
                        .AnyAsync(p =>
                            p.ProgrammeId ==
                            model.AssignedProgrammeId.Value &&
                            p.IsActive);

                if (!exists)
                {
                    return (
                        false,
                        "The selected programme is not available.");
                }
            }

            if (decisionValue ==
                CommitteeDecisionResults.NotRecommended)
            {
                model.AssignedProgrammeId = null;
            }

            if (decisionValue ==
                CommitteeDecisionResults.Deferred &&
                model.AssignedProgrammeId.HasValue)
            {
                var exists =
                    await _context.Programmes
                        .AnyAsync(p =>
                            p.ProgrammeId ==
                            model.AssignedProgrammeId.Value &&
                            p.IsActive);

                if (!exists)
                {
                    return (
                        false,
                        "The selected programme is not available.");
                }
            }

            var currentUserId =
                _userManager.GetUserId(User);

            if (string.IsNullOrWhiteSpace(currentUserId))
            {
                return (
                    false,
                    "Unable to identify the current user.");
            }

            var now = DateTime.UtcNow;

            if (sittingApplication.Decision == null)
            {
                sittingApplication.Decision =
                    new CommitteeDecision
                    {
                        CommitteeSittingApplicationId =
                            committeeSittingApplicationId,

                        Decision =
                            decisionValue,

                        AssignedProgrammeId =
                            model.AssignedProgrammeId,

                        Remarks =
                            NormalizeNullable(model.Remarks),

                        DecisionDateUtc =
                            now,

                        RecordedByUserId =
                            currentUserId,

                        CreatedAtUtc =
                            now
                    };

                _context.CommitteeDecisions.Add(
                    sittingApplication.Decision);
            }
            else
            {
                sittingApplication.Decision.Decision =
                    decisionValue;

                sittingApplication.Decision.AssignedProgrammeId =
                    model.AssignedProgrammeId;

                sittingApplication.Decision.Remarks =
                    NormalizeNullable(model.Remarks);

                sittingApplication.Decision.DecisionDateUtc =
                    now;

                sittingApplication.Decision.UpdatedAtUtc =
                    now;

                sittingApplication.Decision.RecordedByUserId =
                    currentUserId;
            }

            await _context.SaveChangesAsync();

            return (
                true,
                "Committee decision saved successfully.");
        }


        // ============================================================
        // HELPERS
        // ============================================================

        private static string? NormalizeDecision(
            string? decision)
        {
            if (string.IsNullOrWhiteSpace(decision))
            {
                return null;
            }

            var value =
                decision.Trim();

            if (value ==
                CommitteeDecisionResults.RecommendedForAdmission)
            {
                return CommitteeDecisionResults.RecommendedForAdmission;
            }

            if (value ==
                CommitteeDecisionResults.NotRecommended)
            {
                return CommitteeDecisionResults.NotRecommended;
            }

            if (value ==
                CommitteeDecisionResults.Deferred)
            {
                return CommitteeDecisionResults.Deferred;
            }

            return null;
        }


        private static string? NormalizeNullable(
            string? value)
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                return null;
            }

            return value.Trim();
        }


        private static string BuildIntakeName(
            Intake intake)
        {
            return $"{intake.AcademicYear} - {intake.IntakePeriod}";
        }
    }
}