using System.ComponentModel.DataAnnotations;

namespace KISMApplicationManagement.Models
{
    public class ApplicantQualification
    {
        public int ApplicantQualificationId { get; set; }

        // -----------------------------------------------------
        // Applicant
        // -----------------------------------------------------

        [Required]
        public string UserId { get; set; } = string.Empty;

        public ApplicationUser User { get; set; } = null!;

        // -----------------------------------------------------
        // Qualification Route
        // -----------------------------------------------------

        [Required]
        [StringLength(20)]
        [Display(Name = "Qualification Route")]
        public string QualificationRoute { get; set; } = string.Empty;

        // Supported routes:
        // KCSE
        // KNEC

        // -----------------------------------------------------
        // General Qualification Information
        // -----------------------------------------------------

        [Range(1900, 2100)]
        [Display(Name = "Year")]
        public int QualificationYear { get; set; }

        [StringLength(100)]
        [Display(Name = "Examination Index Number")]
        public string? ExaminationIndexNumber { get; set; }

        [StringLength(100)]
        [Display(Name = "Mean Grade")]
        public string? MeanGrade { get; set; }

        // -----------------------------------------------------
        // KNEC Qualification Information
        // -----------------------------------------------------

        [StringLength(150)]
        [Display(Name = "Qualification Type")]
        public string? QualificationType { get; set; }

        [StringLength(200)]
        [Display(Name = "Institution")]
        public string? InstitutionName { get; set; }

        [StringLength(200)]
        [Display(Name = "Field of Study")]
        public string? FieldOfStudy { get; set; }

        [StringLength(150)]
        [Display(Name = "Certificate Number")]
        public string? CertificateNumber { get; set; }

        // -----------------------------------------------------
        // Additional Information
        // -----------------------------------------------------

        [StringLength(2000)]
        public string? Description { get; set; }

        // -----------------------------------------------------
        // Verification
        // -----------------------------------------------------

        [Required]
        [StringLength(30)]
        [Display(Name = "Verification Status")]
        public string VerificationStatus { get; set; } = "Pending";

        public DateTime? VerifiedAtUtc { get; set; }

        public string? VerifiedByUserId { get; set; }

        public ApplicationUser? VerifiedByUser { get; set; }

        // -----------------------------------------------------
        // Audit
        // -----------------------------------------------------

        public DateTime CreatedAtUtc { get; set; }

        public DateTime? UpdatedAtUtc { get; set; }

        // -----------------------------------------------------
        // KCSE Subject Grades
        // -----------------------------------------------------

        public ICollection<ApplicantQualificationSubject> Subjects { get; set; }
            = new List<ApplicantQualificationSubject>();
    }
}