using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using KISMApplicationManagement.Constants;
using KISMApplicationManagement.Data;
using KISMApplicationManagement.Models.ReportViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace KISMApplicationManagement.Controllers
{
    [Authorize(Roles = "System Administrator,Admissions Administrator,Committee Member")]
    public class ReportsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ReportsController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index(
            int? intakeId,
            DateTime? fromDate,
            DateTime? toDate)
        {
            var model = new ReportsDashboardViewModel
            {
                IntakeId = intakeId,
                FromDate = fromDate,
                ToDate = toDate
            };

            // ============================================================
            // INTAKE FILTER OPTIONS
            // ============================================================

            model.Intakes = await _context.Intakes
                .AsNoTracking()
                .OrderByDescending(intake => intake.AcademicYear)
                .ThenByDescending(intake => intake.DisplayOrder)
                .ThenBy(intake => intake.IntakeName)
                .Select(intake => new ReportIntakeOptionViewModel
                {
                    IntakeId = intake.IntakeId,
                    IntakeName = intake.IntakeName,
                    AcademicYear = intake.AcademicYear,
                    IntakePeriod = intake.IntakePeriod
                })
                .ToListAsync();


            // ============================================================
            // APPLICATION QUERY
            // ============================================================

            var applicationsQuery = _context.Applications
                .AsNoTracking()
                .Include(application => application.Intake)
                .Include(application => application.ApplicantProfile)
                .Include(application => application.ProgrammeChoices)
                    .ThenInclude(choice => choice.Programme)
                .AsQueryable();

            if (intakeId.HasValue)
            {
                applicationsQuery = applicationsQuery
                    .Where(application => application.IntakeId == intakeId.Value);
            }

            if (fromDate.HasValue)
            {
                var startDate = fromDate.Value.Date;

                applicationsQuery = applicationsQuery
                    .Where(application =>
                        application.CreatedAtUtc >= startDate);
            }

            if (toDate.HasValue)
            {
                var endDate = toDate.Value.Date.AddDays(1);

                applicationsQuery = applicationsQuery
                    .Where(application =>
                        application.CreatedAtUtc < endDate);
            }

            var applications = await applicationsQuery
                .OrderByDescending(application => application.CreatedAtUtc)
                .ToListAsync();


            // ============================================================
            // BASIC APPLICATION COUNTS
            // ============================================================

            model.TotalApplications = applications.Count;

            model.DraftApplications = applications.Count(application =>
                application.Status == ApplicationStatuses.Draft);

            model.PendingReviewApplications = applications.Count(application =>
                application.Status == ApplicationStatuses.PendingReview);


            // ============================================================
            // APPLICATION IDs
            // ============================================================

            var applicationIds = applications
                .Select(application => application.ApplicationId)
                .ToList();


            // ============================================================
            // COMPLETED COMMITTEE RESULTS
            // ============================================================

            var committeeResults = applicationIds.Count == 0
                ? new List<CommitteeReportRecord>()
                : await _context.CommitteeSittingApplications
                    .AsNoTracking()
                    .Include(item => item.CommitteeSitting)
                    .Include(item => item.Decision)
                    .Where(item =>
                        applicationIds.Contains(item.ApplicationId) &&
                        item.CommitteeSitting.Status ==
                            CommitteeSittingStatuses.Completed &&
                        item.Decision != null)
                    .Select(item => new CommitteeReportRecord
                    {
                        CommitteeSittingApplicationId =
                            item.CommitteeSittingApplicationId,

                        ApplicationId =
                            item.ApplicationId,

                        CommitteeSittingId =
                            item.CommitteeSittingId,

                        IntakeId =
                            item.CommitteeSitting.IntakeId,

                        Decision =
                            item.Decision!.Decision,

                        DecisionDateUtc =
                            item.Decision.DecisionDateUtc,

                        CompletedAtUtc =
                            item.CommitteeSitting.CompletedAtUtc,

                        AssignedProgrammeId =
                            item.Decision.AssignedProgrammeId
                    })
                    .ToListAsync();


            // ============================================================
            // LATEST COMPLETED COMMITTEE RESULT PER APPLICATION
            // ============================================================

            var committeeResultsByApplication = committeeResults
                .GroupBy(result => result.ApplicationId)
                .ToDictionary(
                    group => group.Key,
                    group => group
                        .OrderByDescending(result => result.DecisionDateUtc)
                        .ThenByDescending(result =>
                            result.CompletedAtUtc ?? DateTime.MinValue)
                        .ThenByDescending(result =>
                            result.CommitteeSittingApplicationId)
                        .First());


            // ============================================================
            // COMMITTEE SUMMARY COUNTS
            // ============================================================

            model.ApplicationsWithCommitteeDecision =
                committeeResultsByApplication.Count;

            model.RecommendedApplications =
                committeeResultsByApplication.Values.Count(result =>
                    result.Decision ==
                    CommitteeDecisionResults.RecommendedForAdmission);

            model.NotRecommendedApplications =
                committeeResultsByApplication.Values.Count(result =>
                    result.Decision ==
                    CommitteeDecisionResults.NotRecommended);

            model.DeferredApplications =
                committeeResultsByApplication.Values.Count(result =>
                    result.Decision ==
                    CommitteeDecisionResults.Deferred);


            // ============================================================
            // APPLICATION STATUS SUMMARY
            // ============================================================

            model.StatusSummary = applications
                .GroupBy(application => application.Status)
                .Select(group => new ApplicationStatusSummaryViewModel
                {
                    Status = group.Key,
                    Count = group.Count(),
                    Percentage = model.TotalApplications == 0
                        ? 0
                        : Math.Round(
                            group.Count() * 100m /
                            model.TotalApplications,
                            1)
                })
                .OrderByDescending(item => item.Count)
                .ToList();


            // ============================================================
            // APPLICATION STATUS CHART
            // ============================================================

            model.ApplicationStatusChart = model.StatusSummary
                .Select(item => new ApplicationStatusChartViewModel
                {
                    Status = item.Status,
                    Count = item.Count,
                    Percentage = item.Percentage
                })
                .ToList();


            // ============================================================
            // APPLICATION TREND
            // ============================================================

            model.ApplicationTrend =
                BuildApplicationTrend(applications);


            // ============================================================
            // APPLICATIONS BY INTAKE
            // ============================================================

            var applicationsByIntake = applications
                .GroupBy(application => new
                {
                    application.IntakeId,

                    IntakeName =
                        application.Intake?.IntakeName ??
                        "Unknown Intake",

                    AcademicYear =
                        application.Intake?.AcademicYear ??
                        string.Empty,

                    IntakePeriod =
                        application.Intake?.IntakePeriod ??
                        string.Empty
                })
                .Select(group =>
                {
                    var intakeApplicationIds = group
                        .Select(application => application.ApplicationId)
                        .ToHashSet();

                    var intakeCommitteeResults =
                        committeeResultsByApplication.Values
                            .Where(result =>
                                intakeApplicationIds.Contains(
                                    result.ApplicationId))
                            .ToList();

                    return new ApplicationIntakeSummaryViewModel
                    {
                        IntakeId = group.Key.IntakeId,

                        IntakeName = group.Key.IntakeName,

                        AcademicYear = group.Key.AcademicYear,

                        IntakePeriod = group.Key.IntakePeriod,

                        TotalApplications = group.Count(),

                        DraftApplications = group.Count(application =>
                            application.Status ==
                            ApplicationStatuses.Draft),

                        PendingReviewApplications = group.Count(application =>
                            application.Status ==
                            ApplicationStatuses.PendingReview),

                        CommitteeDecisions =
                            intakeCommitteeResults.Count,

                        RecommendedApplications =
                            intakeCommitteeResults.Count(result =>
                                result.Decision ==
                                CommitteeDecisionResults
                                    .RecommendedForAdmission),

                        NotRecommendedApplications =
                            intakeCommitteeResults.Count(result =>
                                result.Decision ==
                                CommitteeDecisionResults
                                    .NotRecommended),

                        DeferredApplications =
                            intakeCommitteeResults.Count(result =>
                                result.Decision ==
                                CommitteeDecisionResults.Deferred)
                    };
                })
                .OrderByDescending(item => item.TotalApplications)
                .ThenBy(item => item.IntakeName)
                .ToList();

            model.IntakeSummary = applicationsByIntake;


            // ============================================================
            // INTAKE APPLICATION CHART
            // ============================================================

            model.IntakeApplicationChart = applicationsByIntake
                .Select(item => new IntakeApplicationChartViewModel
                {
                    IntakeId = item.IntakeId,

                    IntakeName = item.IntakeName,

                    AcademicYear = item.AcademicYear,

                    IntakePeriod = item.IntakePeriod,

                    TotalApplications =
                        item.TotalApplications,

                    DraftApplications =
                        item.DraftApplications,

                    PendingReviewApplications =
                        item.PendingReviewApplications,

                    CommitteeDecisions =
                        item.CommitteeDecisions
                })
                .ToList();


            // ============================================================
            // COMMITTEE DECISION CHART
            // ============================================================

            model.CommitteeDecisionChart =
                BuildCommitteeDecisionChart(model);


            // ============================================================
            // COMMITTEE RESULTS BY INTAKE
            // ============================================================

            model.CommitteeResultsByIntake =
                applications
                    .GroupBy(application => new
                    {
                        application.IntakeId,

                        IntakeName =
                            application.Intake?.IntakeName ??
                            "Unknown Intake",

                        AcademicYear =
                            application.Intake?.AcademicYear ??
                            string.Empty,

                        IntakePeriod =
                            application.Intake?.IntakePeriod ??
                            string.Empty
                    })
                    .Select(group =>
                    {
                        var applicationIdSet = group
                            .Select(application =>
                                application.ApplicationId)
                            .ToHashSet();

                        var results =
                            committeeResultsByApplication.Values
                                .Where(result =>
                                    applicationIdSet.Contains(
                                        result.ApplicationId))
                                .ToList();

                        return new CommitteeResultsByIntakeChartViewModel
                        {
                            IntakeId = group.Key.IntakeId,

                            IntakeName = group.Key.IntakeName,

                            AcademicYear = group.Key.AcademicYear,

                            IntakePeriod = group.Key.IntakePeriod,

                            RecommendedCount =
                                results.Count(result =>
                                    result.Decision ==
                                    CommitteeDecisionResults
                                        .RecommendedForAdmission),

                            NotRecommendedCount =
                                results.Count(result =>
                                    result.Decision ==
                                    CommitteeDecisionResults
                                        .NotRecommended),

                            DeferredCount =
                                results.Count(result =>
                                    result.Decision ==
                                    CommitteeDecisionResults.Deferred),

                            TotalDecisions =
                                results.Count
                        };
                    })
                    .Where(item => item.TotalDecisions > 0)
                    .OrderByDescending(item => item.TotalDecisions)
                    .ThenBy(item => item.IntakeName)
                    .ToList();


            // ============================================================
            // PROGRAMME DEMAND
            // ============================================================

            var programmeDemand = applications
                .SelectMany(application =>
                    application.ProgrammeChoices)
                .Where(choice => choice.Programme != null)
                .GroupBy(choice => new
                {
                    choice.ProgrammeId,

                    ProgrammeCode =
                        choice.Programme.ProgrammeCode,

                    ProgrammeName =
                        choice.Programme.ProgrammeName
                })
                .Select(group => new ProgrammeDemandChartViewModel
                {
                    ProgrammeId = group.Key.ProgrammeId,

                    ProgrammeCode =
                        group.Key.ProgrammeCode,

                    ProgrammeName =
                        group.Key.ProgrammeName,

                    ProgrammeLabel =
                        $"{group.Key.ProgrammeCode} - " +
                        $"{group.Key.ProgrammeName}",

                    TotalChoices =
                        group.Count(),

                    FirstChoiceCount =
                        group.Count(choice =>
                            choice.ChoiceNumber == 1),

                    SecondChoiceCount =
                        group.Count(choice =>
                            choice.ChoiceNumber == 2)
                })
                .OrderByDescending(item => item.TotalChoices)
                .ThenBy(item => item.ProgrammeName)
                .ToList();

            model.ProgrammeDemandChart = programmeDemand;


            // ============================================================
            // PROGRAMME FIRST / SECOND CHOICE COMPARISON
            // ============================================================

            model.ProgrammeChoiceComparison =
                programmeDemand
                    .Select(item =>
                        new ProgrammeChoiceComparisonChartViewModel
                        {
                            ProgrammeId =
                                item.ProgrammeId,

                            ProgrammeCode =
                                item.ProgrammeCode,

                            ProgrammeName =
                                item.ProgrammeName,

                            ProgrammeLabel =
                                item.ProgrammeLabel,

                            FirstChoiceCount =
                                item.FirstChoiceCount,

                            SecondChoiceCount =
                                item.SecondChoiceCount,

                            TotalChoices =
                                item.TotalChoices
                        })
                    .ToList();


            // ============================================================
            // RECENT APPLICATIONS
            // ============================================================

            model.RecentApplications = applications
                .Take(15)
                .Select(application =>
                {
                    var firstChoice = application.ProgrammeChoices
                        .Where(choice =>
                            choice.ChoiceNumber == 1)
                        .Select(choice =>
                            choice.Programme)
                        .FirstOrDefault();

                    var secondChoice = application.ProgrammeChoices
                        .Where(choice =>
                            choice.ChoiceNumber == 2)
                        .Select(choice =>
                            choice.Programme)
                        .FirstOrDefault();

                    return new RecentApplicationReportViewModel
                    {
                        ApplicationId =
                            application.ApplicationId,

                        ApplicationNumber =
                            application.ApplicationNumber,

                        ApplicantName =
                            application.ApplicantProfile?.FullName ??
                            "Unknown Applicant",

                        IntakeName =
                            application.Intake?.IntakeName ??
                            "Unknown Intake",

                        Status =
                            application.Status,

                        FirstChoice =
                            firstChoice == null
                                ? null
                                : $"{firstChoice.ProgrammeCode} - " +
                                  $"{firstChoice.ProgrammeName}",

                        SecondChoice =
                            secondChoice == null
                                ? null
                                : $"{secondChoice.ProgrammeCode} - " +
                                  $"{secondChoice.ProgrammeName}",

                        CreatedAtUtc =
                            application.CreatedAtUtc,

                        SubmittedAtUtc =
                            application.SubmittedAtUtc
                    };
                })
                .ToList();


            // ============================================================
            // RETURN DASHBOARD
            // ============================================================

            return View(model);
        }


        // ================================================================
        // APPLICATION TREND
        // ================================================================

        private static List<ApplicationTrendChartViewModel>
            BuildApplicationTrend(
                List<Models.Application> applications)
        {
            if (applications.Count == 0)
            {
                return new List<ApplicationTrendChartViewModel>();
            }

            var minimumDate = applications
                .Min(application =>
                    application.CreatedAtUtc)
                .Date;

            var maximumDate = applications
                .Max(application =>
                    application.CreatedAtUtc)
                .Date;

            var dateRange = maximumDate
                .Subtract(minimumDate)
                .TotalDays;

            // ------------------------------------------------------------
            // For shorter reporting periods use daily data.
            // For longer reporting periods use monthly data.
            // ------------------------------------------------------------

            if (dateRange <= 90)
            {
                var trend = applications
                    .GroupBy(application =>
                        application.CreatedAtUtc.Date)
                    .Select(group => new
                    {
                        Date = group.Key,

                        Count = group.Count(),

                        SubmittedCount = group.Count(application =>
                            application.SubmittedAtUtc.HasValue)
                    })
                    .ToDictionary(
                        item => item.Date,
                        item => item);

                var result =
                    new List<ApplicationTrendChartViewModel>();

                for (
                    var date = minimumDate;
                    date <= maximumDate;
                    date = date.AddDays(1))
                {
                    if (trend.TryGetValue(date, out var item))
                    {
                        result.Add(
                            new ApplicationTrendChartViewModel
                            {
                                Date = date,

                                Label = date.ToString(
                                    "dd MMM",
                                    CultureInfo.InvariantCulture),

                                Count = item.Count,

                                SubmittedCount =
                                    item.SubmittedCount
                            });
                    }
                    else
                    {
                        result.Add(
                            new ApplicationTrendChartViewModel
                            {
                                Date = date,

                                Label = date.ToString(
                                    "dd MMM",
                                    CultureInfo.InvariantCulture),

                                Count = 0,

                                SubmittedCount = 0
                            });
                    }
                }

                return result;
            }

            // ------------------------------------------------------------
            // Monthly trend for longer reporting periods.
            // ------------------------------------------------------------

            var monthlyTrend = applications
                .GroupBy(application => new
                {
                    application.CreatedAtUtc.Year,
                    application.CreatedAtUtc.Month
                })
                .Select(group => new
                {
                    Date = new DateTime(
                        group.Key.Year,
                        group.Key.Month,
                        1),

                    Count = group.Count(),

                    SubmittedCount = group.Count(application =>
                        application.SubmittedAtUtc.HasValue)
                })
                .ToDictionary(
                    item => item.Date,
                    item => item);

            var firstMonth = new DateTime(
                minimumDate.Year,
                minimumDate.Month,
                1);

            var lastMonth = new DateTime(
                maximumDate.Year,
                maximumDate.Month,
                1);

            var monthlyResult =
                new List<ApplicationTrendChartViewModel>();

            for (
                var month = firstMonth;
                month <= lastMonth;
                month = month.AddMonths(1))
            {
                if (monthlyTrend.TryGetValue(
                    month,
                    out var item))
                {
                    monthlyResult.Add(
                        new ApplicationTrendChartViewModel
                        {
                            Date = month,

                            Label = month.ToString(
                                "MMM yyyy",
                                CultureInfo.InvariantCulture),

                            Count = item.Count,

                            SubmittedCount =
                                item.SubmittedCount
                        });
                }
                else
                {
                    monthlyResult.Add(
                        new ApplicationTrendChartViewModel
                        {
                            Date = month,

                            Label = month.ToString(
                                "MMM yyyy",
                                CultureInfo.InvariantCulture),

                            Count = 0,

                            SubmittedCount = 0
                        });
                }
            }

            return monthlyResult;
        }


        // ================================================================
        // COMMITTEE DECISION CHART
        // ================================================================

        private static List<CommitteeDecisionChartViewModel>
            BuildCommitteeDecisionChart(
                ReportsDashboardViewModel model)
        {
            var total = model.ApplicationsWithCommitteeDecision;

            return new List<CommitteeDecisionChartViewModel>
            {
                new CommitteeDecisionChartViewModel
                {
                    Decision =
                        CommitteeDecisionResults
                            .RecommendedForAdmission,

                    Count =
                        model.RecommendedApplications,

                    Percentage =
                        CalculatePercentage(
                            model.RecommendedApplications,
                            total)
                },

                new CommitteeDecisionChartViewModel
                {
                    Decision =
                        CommitteeDecisionResults
                            .NotRecommended,

                    Count =
                        model.NotRecommendedApplications,

                    Percentage =
                        CalculatePercentage(
                            model.NotRecommendedApplications,
                            total)
                },

                new CommitteeDecisionChartViewModel
                {
                    Decision =
                        CommitteeDecisionResults
                            .Deferred,

                    Count =
                        model.DeferredApplications,

                    Percentage =
                        CalculatePercentage(
                            model.DeferredApplications,
                            total)
                }
            };
        }


        // ================================================================
        // PERCENTAGE HELPER
        // ================================================================

        private static decimal CalculatePercentage(
            int count,
            int total)
        {
            if (total == 0)
            {
                return 0;
            }

            return Math.Round(
                count * 100m / total,
                1);
        }


        // ================================================================
        // INTERNAL COMMITTEE REPORT RECORD
        // ================================================================

        private sealed class CommitteeReportRecord
        {
            public int CommitteeSittingApplicationId { get; set; }

            public int ApplicationId { get; set; }

            public int CommitteeSittingId { get; set; }

            public int IntakeId { get; set; }

            public string Decision { get; set; } = string.Empty;

            public DateTime DecisionDateUtc { get; set; }

            public DateTime? CompletedAtUtc { get; set; }

            public int? AssignedProgrammeId { get; set; }
        }
    }
}