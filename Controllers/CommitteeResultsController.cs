using KISMApplicationManagement.Constants;
using KISMApplicationManagement.Data;
using KISMApplicationManagement.Models.CommitteeResultViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace KISMApplicationManagement.Controllers
{
    [Authorize(Roles = "System Administrator,Admissions Administrator")]
    public class CommitteeResultsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<CommitteeResultsController> _logger;

        public CommitteeResultsController(
            ApplicationDbContext context,
            ILogger<CommitteeResultsController> logger)
        {
            _context = context;
            _logger = logger;
        }

        [HttpGet]
        public async Task<IActionResult> Index(int id)
        {
            var sitting = await _context.CommitteeSittings
                .AsNoTracking()
                .Include(s => s.Intake)
                .Include(s => s.Applications)
                    .ThenInclude(sa => sa.Application)
                        .ThenInclude(a => a.ApplicantProfile)
                .Include(s => s.Applications)
                    .ThenInclude(sa => sa.Decision)
                        .ThenInclude(d => d!.AssignedProgramme)
                .Include(s => s.Applications)
                    .ThenInclude(sa => sa.Decision)
                        .ThenInclude(d => d!.RecordedByUser)
                .FirstOrDefaultAsync(
                    s => s.CommitteeSittingId == id);

            if (sitting == null)
            {
                return NotFound();
            }

            if (sitting.Status != CommitteeSittingStatuses.Completed)
            {
                TempData["ErrorMessage"] =
                    "Committee results are only available for completed sittings.";

                return RedirectToAction(
                    "Manage",
                    "CommitteeSittings",
                    new { id = sitting.CommitteeSittingId });
            }

            var orderedApplications = sitting.Applications
                .OrderBy(a => a.DisplayOrder)
                .ToList();

            var applications = orderedApplications
                .Select(sittingApplication =>
                {
                    var decision = sittingApplication.Decision;

                    return new CommitteeResultApplicationViewModel
                    {
                        CommitteeSittingApplicationId =
                            sittingApplication.CommitteeSittingApplicationId,

                        ApplicationId =
                            sittingApplication.ApplicationId,

                        ApplicationNumber =
                            sittingApplication.Application?.ApplicationNumber
                            ?? string.Empty,

                        ApplicantName =
                            sittingApplication.Application?.ApplicantProfile?.FullName
                            ?? "N/A",

                        DisplayOrder =
                            sittingApplication.DisplayOrder,

                        Decision =
                            decision?.Decision ?? "No Decision",

                        AssignedProgrammeId =
                            decision?.AssignedProgrammeId,

                        AssignedProgrammeCode =
                            decision?.AssignedProgramme?.ProgrammeCode,

                        AssignedProgrammeName =
                            decision?.AssignedProgramme?.ProgrammeName,

                        AssignedProgrammeAwardType =
                            decision?.AssignedProgramme?.AwardType,

                        CommitteeRemarks =
                            decision?.Remarks,

                        DecisionDateUtc =
                            decision?.DecisionDateUtc ?? DateTime.MinValue,

                        RecordedByName =
                            decision?.RecordedByUser == null
                                ? string.Empty
                                : BuildUserName(
                                    decision.RecordedByUser.FirstName,
                                    decision.RecordedByUser.MiddleName,
                                    decision.RecordedByUser.LastName)
                    };
                })
                .ToList();

            var viewModel = new CommitteeResultsViewModel
            {
                CommitteeSittingId =
                    sitting.CommitteeSittingId,

                ReferenceNumber =
                    sitting.ReferenceNumber,

                IntakeName =
                    sitting.Intake?.IntakeName
                    ?? string.Empty,

                AcademicYear =
                    sitting.Intake?.AcademicYear
                    ?? string.Empty,

                IntakePeriod =
                    sitting.Intake?.IntakePeriod
                    ?? string.Empty,

                SittingDate =
                    sitting.SittingDate,

                Status =
                    sitting.Status,

                CompletedAtUtc =
                    sitting.CompletedAtUtc,

                TotalApplications =
                    applications.Count,

                RecommendedCount =
                    applications.Count(a =>
                        a.Decision ==
                        CommitteeDecisionResults.RecommendedForAdmission),

                NotRecommendedCount =
                    applications.Count(a =>
                        a.Decision ==
                        CommitteeDecisionResults.NotRecommended),

                DeferredCount =
                    applications.Count(a =>
                        a.Decision ==
                        CommitteeDecisionResults.Deferred),

                Remarks =
                    sitting.Remarks,

                Applications =
                    applications
            };

            return View(viewModel);
        }

        private static string BuildUserName(
            string? firstName,
            string? middleName,
            string? lastName)
        {
            return string.Join(
                " ",
                new[]
                {
                    firstName,
                    middleName,
                    lastName
                }
                .Where(value =>
                    !string.IsNullOrWhiteSpace(value)));
        }
    }
}