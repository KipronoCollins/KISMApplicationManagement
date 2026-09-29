using System.ComponentModel.DataAnnotations;

namespace KISMApplicationManagement.Models.ApplicationViewModels
{
    public class ApplicationListViewModel
    {
        public int ApplicationId { get; set; }

        [Display(Name = "Application Number")]
        public string ApplicationNumber { get; set; } = string.Empty;

        [Display(Name = "Intake")]
        public string IntakeName { get; set; } = string.Empty;

        [Display(Name = "Academic Year")]
        public string AcademicYear { get; set; } = string.Empty;

        [Display(Name = "Intake Period")]
        public string IntakePeriod { get; set; } = string.Empty;

        public string Status { get; set; } = string.Empty;

        [Display(Name = "First Choice")]
        public string? FirstChoiceProgramme { get; set; }

        [Display(Name = "Second Choice")]
        public string? SecondChoiceProgramme { get; set; }

        [Display(Name = "Submitted")]
        public DateTime? SubmittedAtUtc { get; set; }

        [Display(Name = "Created")]
        public DateTime CreatedAtUtc { get; set; }
    }
}