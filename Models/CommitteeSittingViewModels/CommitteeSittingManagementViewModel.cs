using KISMApplicationManagement.Constants;

namespace KISMApplicationManagement.Models.CommitteeSittingViewModels
{
    public class CommitteeSittingManagementViewModel
    {
        public int CommitteeSittingId { get; set; }

        public int IntakeId { get; set; }

        public string IntakeName { get; set; } = string.Empty;

        public string AcademicYear { get; set; } = string.Empty;

        public string IntakePeriod { get; set; } = string.Empty;

        public DateTime SittingDate { get; set; }

        public string ReferenceNumber { get; set; } = string.Empty;

        public string Status { get; set; } = string.Empty;

        public string? Remarks { get; set; }

        public int ApplicationCount { get; set; }

        public List<CommitteeSittingApplicationItemViewModel> SittingApplications { get; set; }
            = new List<CommitteeSittingApplicationItemViewModel>();

        public List<EligibleCommitteeApplicationViewModel> EligibleApplications { get; set; }
            = new List<EligibleCommitteeApplicationViewModel>();
    }

    public class CommitteeSittingApplicationItemViewModel
    {
        public int CommitteeSittingApplicationId { get; set; }

        public int ApplicationId { get; set; }

        public string ApplicationNumber { get; set; } = string.Empty;

        public string ApplicantName { get; set; } = string.Empty;

        public int DisplayOrder { get; set; }

        public string? Decision { get; set; }

        public bool HasDecision { get; set; }
    }

    public class EligibleCommitteeApplicationViewModel
    {
        public int ApplicationId { get; set; }

        public string ApplicationNumber { get; set; } = string.Empty;

        public string ApplicantName { get; set; } = string.Empty;

        public string Status { get; set; } = ApplicationStatuses.PendingReview;

        public DateTime? SubmittedAtUtc { get; set; }

        public List<CommitteeApplicationProgrammeChoiceViewModel> ProgrammeChoices { get; set; }
            = new List<CommitteeApplicationProgrammeChoiceViewModel>();
    }

    public class CommitteeApplicationProgrammeChoiceViewModel
    {
        public int ChoiceNumber { get; set; }

        public string ProgrammeCode { get; set; } = string.Empty;

        public string ProgrammeName { get; set; } = string.Empty;
    }

    public class AddCommitteeApplicationsViewModel
    {
        public int CommitteeSittingId { get; set; }

        public List<int> ApplicationIds { get; set; }
            = new List<int>();
    }
}