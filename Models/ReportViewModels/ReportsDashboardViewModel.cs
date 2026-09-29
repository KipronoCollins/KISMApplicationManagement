using System;
using System.Collections.Generic;

namespace KISMApplicationManagement.Models.ReportViewModels
{
    public class ReportsDashboardViewModel
    {
        // ============================================================
        // FILTERS
        // ============================================================

        public int? IntakeId { get; set; }

        public DateTime? FromDate { get; set; }

        public DateTime? ToDate { get; set; }


        // ============================================================
        // INTAKE FILTER OPTIONS
        // ============================================================

        public List<ReportIntakeOptionViewModel> Intakes { get; set; }
            = new List<ReportIntakeOptionViewModel>();


        // ============================================================
        // APPLICATION SUMMARY
        // ============================================================

        public int TotalApplications { get; set; }

        public int DraftApplications { get; set; }

        public int PendingReviewApplications { get; set; }


        // ============================================================
        // COMMITTEE SUMMARY
        // ============================================================

        public int ApplicationsWithCommitteeDecision { get; set; }

        public int RecommendedApplications { get; set; }

        public int NotRecommendedApplications { get; set; }

        public int DeferredApplications { get; set; }


        // ============================================================
        // EXISTING REPORT TABLES
        // ============================================================

        public List<ApplicationStatusSummaryViewModel> StatusSummary { get; set; }
            = new List<ApplicationStatusSummaryViewModel>();

        public List<ApplicationIntakeSummaryViewModel> IntakeSummary { get; set; }
            = new List<ApplicationIntakeSummaryViewModel>();

        public List<RecentApplicationReportViewModel> RecentApplications { get; set; }
            = new List<RecentApplicationReportViewModel>();


        // ============================================================
        // CHART DATA
        // ============================================================

        /// <summary>
        /// Application status distribution used by the status doughnut chart.
        /// </summary>
        public List<ApplicationStatusChartViewModel> ApplicationStatusChart { get; set; }
            = new List<ApplicationStatusChartViewModel>();


        /// <summary>
        /// Applications created over time.
        /// </summary>
        public List<ApplicationTrendChartViewModel> ApplicationTrend { get; set; }
            = new List<ApplicationTrendChartViewModel>();


        /// <summary>
        /// Application volume grouped by intake.
        /// </summary>
        public List<IntakeApplicationChartViewModel> IntakeApplicationChart { get; set; }
            = new List<IntakeApplicationChartViewModel>();


        /// <summary>
        /// Programme demand based on applicants' programme choices.
        /// </summary>
        public List<ProgrammeDemandChartViewModel> ProgrammeDemandChart { get; set; }
            = new List<ProgrammeDemandChartViewModel>();


        /// <summary>
        /// Committee decision distribution.
        /// </summary>
        public List<CommitteeDecisionChartViewModel> CommitteeDecisionChart { get; set; }
            = new List<CommitteeDecisionChartViewModel>();


        /// <summary>
        /// Committee results grouped by intake.
        /// </summary>
        public List<CommitteeResultsByIntakeChartViewModel> CommitteeResultsByIntake { get; set; }
            = new List<CommitteeResultsByIntakeChartViewModel>();


        /// <summary>
        /// Comparison between first-choice and second-choice programme demand.
        /// </summary>
        public List<ProgrammeChoiceComparisonChartViewModel> ProgrammeChoiceComparison { get; set; }
            = new List<ProgrammeChoiceComparisonChartViewModel>();
    }


    // ================================================================
    // INTAKE FILTER OPTION
    // ================================================================

    public class ReportIntakeOptionViewModel
    {
        public int IntakeId { get; set; }

        public string IntakeName { get; set; } = string.Empty;

        public string AcademicYear { get; set; } = string.Empty;

        public string IntakePeriod { get; set; } = string.Empty;
    }


    // ================================================================
    // APPLICATION STATUS SUMMARY
    // ================================================================

    public class ApplicationStatusSummaryViewModel
    {
        public string Status { get; set; } = string.Empty;

        public int Count { get; set; }

        public decimal Percentage { get; set; }
    }


    // ================================================================
    // APPLICATIONS BY INTAKE
    // ================================================================

    public class ApplicationIntakeSummaryViewModel
    {
        public int IntakeId { get; set; }

        public string IntakeName { get; set; } = string.Empty;

        public string AcademicYear { get; set; } = string.Empty;

        public string IntakePeriod { get; set; } = string.Empty;

        public int TotalApplications { get; set; }

        public int DraftApplications { get; set; }

        public int PendingReviewApplications { get; set; }

        public int CommitteeDecisions { get; set; }

        public int RecommendedApplications { get; set; }

        public int NotRecommendedApplications { get; set; }

        public int DeferredApplications { get; set; }
    }


    // ================================================================
    // RECENT APPLICATIONS
    // ================================================================

    public class RecentApplicationReportViewModel
    {
        public int ApplicationId { get; set; }

        public string ApplicationNumber { get; set; } = string.Empty;

        public string ApplicantName { get; set; } = string.Empty;

        public string IntakeName { get; set; } = string.Empty;

        public string Status { get; set; } = string.Empty;

        public string? FirstChoice { get; set; }

        public string? SecondChoice { get; set; }

        public DateTime CreatedAtUtc { get; set; }

        public DateTime? SubmittedAtUtc { get; set; }
    }


    // ================================================================
    // APPLICATION STATUS CHART
    // ================================================================

    public class ApplicationStatusChartViewModel
    {
        public string Status { get; set; } = string.Empty;

        public int Count { get; set; }

        public decimal Percentage { get; set; }
    }


    // ================================================================
    // APPLICATION TREND CHART
    // ================================================================

    public class ApplicationTrendChartViewModel
    {
        /// <summary>
        /// Date represented by this data point.
        /// </summary>
        public DateTime Date { get; set; }

        /// <summary>
        /// Display label used by the chart.
        /// Example: 01 Sep, 02 Sep, 03 Sep.
        /// </summary>
        public string Label { get; set; } = string.Empty;

        /// <summary>
        /// Number of applications created on the date.
        /// </summary>
        public int Count { get; set; }

        /// <summary>
        /// Number of applications submitted on the date.
        /// </summary>
        public int SubmittedCount { get; set; }
    }


    // ================================================================
    // APPLICATIONS BY INTAKE CHART
    // ================================================================

    public class IntakeApplicationChartViewModel
    {
        public int IntakeId { get; set; }

        public string IntakeName { get; set; } = string.Empty;

        public string AcademicYear { get; set; } = string.Empty;

        public string IntakePeriod { get; set; } = string.Empty;

        public int TotalApplications { get; set; }

        public int DraftApplications { get; set; }

        public int PendingReviewApplications { get; set; }

        public int CommitteeDecisions { get; set; }
    }


    // ================================================================
    // PROGRAMME DEMAND CHART
    // ================================================================

    public class ProgrammeDemandChartViewModel
    {
        public int ProgrammeId { get; set; }

        public string ProgrammeCode { get; set; } = string.Empty;

        public string ProgrammeName { get; set; } = string.Empty;

        public string ProgrammeLabel { get; set; } = string.Empty;

        public int TotalChoices { get; set; }

        public int FirstChoiceCount { get; set; }

        public int SecondChoiceCount { get; set; }
    }


    // ================================================================
    // COMMITTEE DECISION CHART
    // ================================================================

    public class CommitteeDecisionChartViewModel
    {
        public string Decision { get; set; } = string.Empty;

        public int Count { get; set; }

        public decimal Percentage { get; set; }
    }


    // ================================================================
    // COMMITTEE RESULTS BY INTAKE
    // ================================================================

    public class CommitteeResultsByIntakeChartViewModel
    {
        public int IntakeId { get; set; }

        public string IntakeName { get; set; } = string.Empty;

        public string AcademicYear { get; set; } = string.Empty;

        public string IntakePeriod { get; set; } = string.Empty;

        public int RecommendedCount { get; set; }

        public int NotRecommendedCount { get; set; }

        public int DeferredCount { get; set; }

        public int TotalDecisions { get; set; }
    }


    // ================================================================
    // PROGRAMME CHOICE COMPARISON
    // ================================================================

    public class ProgrammeChoiceComparisonChartViewModel
    {
        public int ProgrammeId { get; set; }

        public string ProgrammeCode { get; set; } = string.Empty;

        public string ProgrammeName { get; set; } = string.Empty;

        public string ProgrammeLabel { get; set; } = string.Empty;

        public int FirstChoiceCount { get; set; }

        public int SecondChoiceCount { get; set; }

        public int TotalChoices { get; set; }
    }
}