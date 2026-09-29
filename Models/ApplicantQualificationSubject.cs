using System.ComponentModel.DataAnnotations;

namespace KISMApplicationManagement.Models
{
    public class ApplicantQualificationSubject
    {
        public int ApplicantQualificationSubjectId { get; set; }

        // -----------------------------------------------------
        // Qualification
        // -----------------------------------------------------

        [Required]
        public int ApplicantQualificationId { get; set; }

        public ApplicantQualification Qualification { get; set; }
            = null!;

        // -----------------------------------------------------
        // Subject Information
        // -----------------------------------------------------

        [Required]
        [StringLength(150)]
        [Display(Name = "Subject Name")]
        public string SubjectName { get; set; } = string.Empty;

        [StringLength(30)]
        [Display(Name = "Subject Code")]
        public string? SubjectCode { get; set; }

        [Required]
        [StringLength(10)]
        public string Grade { get; set; } = string.Empty;

        [Display(Name = "Display Order")]
        public int DisplayOrder { get; set; }

        // -----------------------------------------------------
        // Audit
        // -----------------------------------------------------

        public DateTime CreatedAtUtc { get; set; }

        public DateTime? UpdatedAtUtc { get; set; }
    }
}