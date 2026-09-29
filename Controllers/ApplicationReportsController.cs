using KISMApplicationManagement.Constants;
using KISMApplicationManagement.Data;
using KISMApplicationManagement.Models.ReportViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace KISMApplicationManagement.Controllers
{
    [Authorize(Roles = "Admissions Officer,Admissions Administrator,System Administrator")]
    public class ApplicationReportsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<ApplicationReportsController> _logger;

        public ApplicationReportsController(
            ApplicationDbContext context,
            ILogger<ApplicationReportsController> logger)
        {
            _context = context;
            _logger = logger;
        }


        // ============================================================
        // APPLICATION SUMMARY
        // ============================================================

        [HttpGet]
        public async Task<IActionResult> Summary(
            int? intakeId = null,
            string? status = null)
        {
            var applicationsQuery = _context.Applications
                .AsNoTracking()
                .Include(application => application.ApplicantProfile)
                .Include(application => application.Intake)
                .Include(application => application.ProgrammeChoices)
                    .ThenInclude(choice => choice.Programme)
                .AsQueryable();

            if (intakeId.HasValue)
            {
                applicationsQuery = applicationsQuery
                    .Where(application =>
                        application.IntakeId == intakeId.Value);
            }

            if (!string.IsNullOrWhiteSpace(status))
            {
                applicationsQuery = applicationsQuery
                    .Where(application =>
                        application.Status == status);
            }

            var applications = await applicationsQuery
                .OrderByDescending(application =>
                    application.CreatedAtUtc)
                .ToListAsync();


            // ========================================================
            // APPLICATION DETAIL ROWS
            // ========================================================

            var applicationRows = applications
                .Select(application =>
                {
                    var firstChoice = application.ProgrammeChoices
                        .FirstOrDefault(choice =>
                            choice.ChoiceNumber == 1);

                    var secondChoice = application.ProgrammeChoices
                        .FirstOrDefault(choice =>
                            choice.ChoiceNumber == 2);

                    return new ApplicationSummaryReportItemViewModel
                    {
                        ApplicationId =
                            application.ApplicationId,

                        ApplicationNumber =
                            application.ApplicationNumber,

                        ApplicantName =
                            application.ApplicantProfile?.FullName
                            ?? "N/A",

                        IntakeName =
                            application.Intake?.IntakeName
                            ?? "N/A",

                        AcademicYear =
                            application.Intake?.AcademicYear
                            ?? string.Empty,

                        IntakePeriod =
                            application.Intake?.IntakePeriod
                            ?? string.Empty,

                        Status =
                            application.Status,

                        FirstChoiceProgramme =
                            firstChoice?.Programme?.ProgrammeName,

                        SecondChoiceProgramme =
                            secondChoice?.Programme?.ProgrammeName,

                        SubmittedAtUtc =
                            application.SubmittedAtUtc,

                        CreatedAtUtc =
                            application.CreatedAtUtc
                    };
                })
                .ToList();


            // ========================================================
            // STATUS SUMMARY
            // ========================================================

            var statusSummary = applications
                .GroupBy(application => application.Status)
                .Select(group => new ApplicationSummaryStatusViewModel
                {
                    Status = group.Key,
                    Count = group.Count()
                })
                .OrderByDescending(item => item.Count)
                .ToList();


            // ========================================================
            // PROGRAMME SUMMARY
            // ========================================================

            var programmeChoices = applications
                .SelectMany(application =>
                    application.ProgrammeChoices
                        .Select(choice => new
                        {
                            choice.ProgrammeId,
                            ProgrammeCode =
                                choice.Programme?.ProgrammeCode
                                ?? string.Empty,
                            ProgrammeName =
                                choice.Programme?.ProgrammeName
                                ?? string.Empty,
                            choice.ChoiceNumber
                        }))
                .ToList();

            var programmeSummary = programmeChoices
                .GroupBy(choice => new
                {
                    choice.ProgrammeId,
                    choice.ProgrammeCode,
                    choice.ProgrammeName
                })
                .Select(group => new ApplicationSummaryProgrammeViewModel
                {
                    ProgrammeId = group.Key.ProgrammeId,

                    ProgrammeCode = group.Key.ProgrammeCode,

                    ProgrammeName = group.Key.ProgrammeName,

                    FirstChoiceCount =
                        group.Count(choice =>
                            choice.ChoiceNumber == 1),

                    SecondChoiceCount =
                        group.Count(choice =>
                            choice.ChoiceNumber == 2)
                })
                .OrderByDescending(item =>
                    item.TotalChoiceCount)
                .ThenBy(item =>
                    item.ProgrammeName)
                .ToList();


            // ========================================================
            // INTAKE OPTIONS
            // ========================================================

            var intakeOptions = await _context.Intakes
                .AsNoTracking()
                .OrderByDescending(intake =>
                    intake.AcademicYear)
                .ThenBy(intake =>
                    intake.DisplayOrder)
                .ThenBy(intake =>
                    intake.IntakeName)
                .Select(intake =>
                    new ApplicationSummaryIntakeViewModel
                    {
                        IntakeId = intake.IntakeId,

                        IntakeName = intake.IntakeName,

                        AcademicYear = intake.AcademicYear,

                        IntakePeriod = intake.IntakePeriod
                    })
                .ToListAsync();


            // ========================================================
            // COUNTS
            // ========================================================

            var totalApplications = applications.Count;

            var draftCount = applications.Count(application =>
                application.Status == ApplicationStatuses.Draft);

            var pendingReviewCount = applications.Count(application =>
                application.Status == ApplicationStatuses.PendingReview);

            var passedCount = applications.Count(application =>
                application.Status == ApplicationStatuses.Passed);

            var failedCount = applications.Count(application =>
                application.Status == ApplicationStatuses.Failed);

            var submittedCount = applications.Count(application =>
                application.SubmittedAtUtc.HasValue);


            // ========================================================
            // VIEW MODEL
            // ========================================================

            var viewModel = new ApplicationSummaryReportViewModel
            {
                IntakeId = intakeId,

                Status = status,

                Applications = applicationRows,

                StatusSummary = statusSummary,

                ProgrammeSummary = programmeSummary,

                IntakeOptions = intakeOptions,

                TotalApplications = totalApplications,

                DraftCount = draftCount,

                PendingReviewCount = pendingReviewCount,

                PassedCount = passedCount,

                FailedCount = failedCount,

                SubmittedCount = submittedCount
            };

            _logger.LogInformation(
                "Application summary report generated. IntakeId: {IntakeId}, Status: {Status}, Total: {TotalApplications}.",
                intakeId,
                status,
                totalApplications);

            return View(viewModel);
        }
    }
}