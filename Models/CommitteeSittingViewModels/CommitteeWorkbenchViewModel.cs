using System.ComponentModel.DataAnnotations;

namespace KISMApplicationManagement.Models.CommitteeSittingViewModels
{
    public class CommitteeWorkbenchViewModel
    {
        public int CommitteeSittingId { get; set; }

        public string ReferenceNumber { get; set; } = string.Empty;

        public string IntakeName { get; set; } = string.Empty;

        public DateTime SittingDate { get; set; }

        public int CurrentPosition { get; set; }

        public int TotalApplications { get; set; }

        public bool HasPrevious { get; set; }

        public bool HasNext { get; set; }

        public int CommitteeSittingApplicationId { get; set; }

        public int ApplicationId { get; set; }

        public string ApplicationNumber { get; set; } = string.Empty;

        public ApplicantCommitteeInformationViewModel Applicant { get; set; }
            = new ApplicantCommitteeInformationViewModel();

        public List<CommitteeQualificationViewModel> Qualifications { get; set; }
            = new List<CommitteeQualificationViewModel>();

        public List<CommitteeProgrammeChoiceViewModel> ProgrammeChoices { get; set; }
            = new List<CommitteeProgrammeChoiceViewModel>();

        public CommitteeDecisionEntryViewModel Decision { get; set; }
            = new CommitteeDecisionEntryViewModel();

        public List<CommitteeProgrammeOptionViewModel> AvailableProgrammes { get; set; }
            = new List<CommitteeProgrammeOptionViewModel>();
    }

    public class ApplicantCommitteeInformationViewModel
    {
        public string FullName { get; set; } = string.Empty;

        public string? IdPassportBirthCertificateNo { get; set; }

        public string? MobileNumber { get; set; }

        public string? ContactEmail { get; set; }

        public DateTime? DateOfBirth { get; set; }

        public string? PlaceOfBirth { get; set; }

        public string? ContactAddress { get; set; }

        public string? Town { get; set; }

        public string? CountyName { get; set; }
    }

    public class CommitteeQualificationViewModel
    {
        public int ApplicantQualificationId { get; set; }

        public string QualificationRoute { get; set; } = string.Empty;

        public int QualificationYear { get; set; }

        public string? ExaminationIndexNumber { get; set; }

        public string? MeanGrade { get; set; }

        public string? QualificationType { get; set; }

        public string? InstitutionName { get; set; }

        public string? FieldOfStudy { get; set; }

        public string? CertificateNumber { get; set; }

        public string? Description { get; set; }

        public string VerificationStatus { get; set; } = string.Empty;

        public List<CommitteeQualificationSubjectViewModel> Subjects { get; set; }
            = new List<CommitteeQualificationSubjectViewModel>();
    }

    public class CommitteeQualificationSubjectViewModel
    {
        public string SubjectName { get; set; } = string.Empty;

        public string? SubjectCode { get; set; }

        public string Grade { get; set; } = string.Empty;

        public int DisplayOrder { get; set; }
    }

    public class CommitteeProgrammeChoiceViewModel
    {
        public int ChoiceNumber { get; set; }

        public int ProgrammeId { get; set; }

        public string ProgrammeCode { get; set; } = string.Empty;

        public string ProgrammeName { get; set; } = string.Empty;

        public string AwardType { get; set; } = string.Empty;

        public int DurationYears { get; set; }
    }

    public class CommitteeProgrammeOptionViewModel
    {
        public int ProgrammeId { get; set; }

        public string ProgrammeCode { get; set; } = string.Empty;

        public string ProgrammeName { get; set; } = string.Empty;

        public string AwardType { get; set; } = string.Empty;
    }

    public class CommitteeDecisionEntryViewModel
    {
        [Required(ErrorMessage = "Please select a committee decision.")]
        public string Decision { get; set; } = string.Empty;

        public int? AssignedProgrammeId { get; set; }

        [StringLength(2000)]
        public string? Remarks { get; set; }
    }
}