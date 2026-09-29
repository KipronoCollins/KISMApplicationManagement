using System.ComponentModel.DataAnnotations;

namespace KISMApplicationManagement.ViewModels.ApplicantQualifications
{
    public class ApplicantQualificationSubjectViewModel
    {
        // =========================================================
        // Identity
        // =========================================================

        public int ApplicantQualificationSubjectId { get; set; }

        public int ApplicantQualificationId { get; set; }

        // =========================================================
        // Subject Information
        // =========================================================

        [Required(ErrorMessage = "Please enter the subject name.")]
        [StringLength(150)]
        [Display(Name = "Subject Name")]
        public string SubjectName { get; set; } = string.Empty;

        [StringLength(30)]
        [Display(Name = "Subject Code")]
        public string? SubjectCode { get; set; }

        [Required(ErrorMessage = "Please select or enter the subject grade.")]
        [StringLength(10)]
        [Display(Name = "Grade")]
        public string Grade { get; set; } = string.Empty;

        [Display(Name = "Display Order")]
        public int DisplayOrder { get; set; }
    }
}