using System;
using System.Collections.Generic;

namespace KISMApplicationManagement.Models.ReportViewModels
{
    public class ApplicationSummaryReportViewModel
    {
        public int? IntakeId { get; set; }

        public string? Status { get; set; }

        public List<ApplicationSummaryReportItemViewModel> Applications { get; set; }
            = new List<ApplicationSummaryReportItemViewModel>();

        public List<ApplicationSummaryStatusViewModel> StatusSummary { get; set; }
            = new List<ApplicationSummaryStatusViewModel>();

        public List<ApplicationSummaryProgrammeViewModel> ProgrammeSummary { get; set; }
            = new List<ApplicationSummaryProgrammeViewModel>();

        public List<ApplicationSummaryIntakeViewModel> IntakeOptions { get; set; }
            = new List<ApplicationSummaryIntakeViewModel>();

        public int TotalApplications { get; set; }

        public int DraftCount { get; set; }

        public int PendingReviewCount { get; set; }

        public int PassedCount { get; set; }

        public int FailedCount { get; set; }

        public int SubmittedCount { get; set; }
    }


    public class ApplicationSummaryReportItemViewModel
    {
        public int ApplicationId { get; set; }

        public string ApplicationNumber { get; set; } = string.Empty;

        public string ApplicantName { get; set; } = string.Empty;

        public string IntakeName { get; set; } = string.Empty;

        public string AcademicYear { get; set; } = string.Empty;

        public string IntakePeriod { get; set; } = string.Empty;

        public string Status { get; set; } = string.Empty;

        public string? FirstChoiceProgramme { get; set; }

        public string? SecondChoiceProgramme { get; set; }

        public DateTime? SubmittedAtUtc { get; set; }

        public DateTime CreatedAtUtc { get; set; }
    }


    public class ApplicationSummaryStatusViewModel
    {
        public string Status { get; set; } = string.Empty;

        public int Count { get; set; }
    }


    public class ApplicationSummaryProgrammeViewModel
    {
        public int ProgrammeId { get; set; }

        public string ProgrammeCode { get; set; } = string.Empty;

        public string ProgrammeName { get; set; } = string.Empty;

        public int FirstChoiceCount { get; set; }

        public int SecondChoiceCount { get; set; }

        public int TotalChoiceCount =>
            FirstChoiceCount + SecondChoiceCount;
    }


    public class ApplicationSummaryIntakeViewModel
    {
        public int IntakeId { get; set; }

        public string IntakeName { get; set; } = string.Empty;

        public string AcademicYear { get; set; } = string.Empty;

        public string IntakePeriod { get; set; } = string.Empty;
    }
}