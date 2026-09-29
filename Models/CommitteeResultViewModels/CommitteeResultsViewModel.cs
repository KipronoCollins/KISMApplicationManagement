using System;
using System.Collections.Generic;

namespace KISMApplicationManagement.Models.CommitteeResultViewModels
{
    public class CommitteeResultsViewModel
    {
        public int CommitteeSittingId { get; set; }

        public string ReferenceNumber { get; set; } = string.Empty;

        public string IntakeName { get; set; } = string.Empty;

        public string AcademicYear { get; set; } = string.Empty;

        public string IntakePeriod { get; set; } = string.Empty;

        public DateTime SittingDate { get; set; }

        public string Status { get; set; } = string.Empty;

        public DateTime? CompletedAtUtc { get; set; }

        public int TotalApplications { get; set; }

        public int RecommendedCount { get; set; }

        public int NotRecommendedCount { get; set; }

        public int DeferredCount { get; set; }

        public string? Remarks { get; set; }

        public List<CommitteeResultApplicationViewModel> Applications { get; set; }
            = new List<CommitteeResultApplicationViewModel>();
    }

    public class CommitteeResultApplicationViewModel
    {
        public int CommitteeSittingApplicationId { get; set; }

        public int ApplicationId { get; set; }

        public string ApplicationNumber { get; set; } = string.Empty;

        public string ApplicantName { get; set; } = string.Empty;

        public int DisplayOrder { get; set; }

        public string Decision { get; set; } = string.Empty;

        public int? AssignedProgrammeId { get; set; }

        public string? AssignedProgrammeCode { get; set; }

        public string? AssignedProgrammeName { get; set; }

        public string? AssignedProgrammeAwardType { get; set; }

        public string? CommitteeRemarks { get; set; }

        public DateTime DecisionDateUtc { get; set; }

        public string RecordedByName { get; set; } = string.Empty;
    }
}