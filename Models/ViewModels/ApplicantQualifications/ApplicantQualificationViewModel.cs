using System.ComponentModel.DataAnnotations;

namespace KISMApplicationManagement.ViewModels.ApplicantQualifications
{
    public class ApplicantQualificationViewModel
    {
        // =========================================================
        // Identity
        // =========================================================

        public int ApplicantQualificationId { get; set; }

        // =========================================================
        // Qualification Route
        // =========================================================

        [Required(ErrorMessage = "Please select a qualification route.")]
        [Display(Name = "Qualification Route")]
        public string QualificationRoute { get; set; } = string.Empty;

        // Supported routes:
        // KCSE
        // KNEC

        // =========================================================
        // General Qualification Information
        // =========================================================

        [Required(ErrorMessage = "Please enter the qualification year.")]
        [Range(1900, 2100, ErrorMessage = "Please enter a valid year.")]
        [Display(Name = "Year")]
        public int QualificationYear { get; set; }

        [StringLength(100)]
        [Display(Name = "Examination Index Number")]
        public string? ExaminationIndexNumber { get; set; }

        [StringLength(100)]
        [Display(Name = "Mean Grade")]
        public string? MeanGrade { get; set; }

        // =========================================================
        // KNEC Qualification Information
        // =========================================================

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

        // =========================================================
        // Additional Information
        // =========================================================

        [StringLength(2000)]
        public string? Description { get; set; }

        // =========================================================
        // Status
        // =========================================================

        public string VerificationStatus { get; set; } = "Pending";

        // =========================================================
        // KCSE Subject Grades
        // =========================================================

        public List<ApplicantQualificationSubjectViewModel> Subjects { get; set; }
            = new List<ApplicantQualificationSubjectViewModel>();
    }
}