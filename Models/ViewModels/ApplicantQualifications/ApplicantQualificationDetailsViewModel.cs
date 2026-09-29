namespace KISMApplicationManagement.ViewModels.ApplicantQualifications
{
    public class ApplicantQualificationDetailsViewModel
    {
        // =========================================================
        // Identity
        // =========================================================

        public int ApplicantQualificationId { get; set; }

        // =========================================================
        // Qualification Information
        // =========================================================

        public string QualificationRoute { get; set; } = string.Empty;

        public int QualificationYear { get; set; }

        public string? ExaminationIndexNumber { get; set; }

        public string? MeanGrade { get; set; }

        // =========================================================
        // KNEC Information
        // =========================================================

        public string? QualificationType { get; set; }

        public string? InstitutionName { get; set; }

        public string? FieldOfStudy { get; set; }

        public string? CertificateNumber { get; set; }

        // =========================================================
        // Additional Information
        // =========================================================

        public string? Description { get; set; }

        // =========================================================
        // Verification
        // =========================================================

        public string VerificationStatus { get; set; } = "Pending";

        public DateTime? VerifiedAtUtc { get; set; }

        // =========================================================
        // KCSE Subjects
        // =========================================================

        public List<ApplicantQualificationSubjectViewModel> Subjects { get; set; }
            = new List<ApplicantQualificationSubjectViewModel>();

        // =========================================================
        // Audit
        // =========================================================

        public DateTime CreatedAtUtc { get; set; }

        public DateTime? UpdatedAtUtc { get; set; }
    }
}