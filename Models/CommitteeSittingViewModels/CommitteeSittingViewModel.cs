using System.ComponentModel.DataAnnotations;

namespace KISMApplicationManagement.Models.CommitteeSittingViewModels
{
    public class CommitteeSittingViewModel
    {
        public int CommitteeSittingId { get; set; }

        [Required]
        [Display(Name = "Intake")]
        public int IntakeId { get; set; }

        [Required]
        [DataType(DataType.Date)]
        [Display(Name = "Sitting Date")]
        public DateTime SittingDate { get; set; }

        [Required]
        [StringLength(100)]
        [Display(Name = "Reference Number")]
        public string ReferenceNumber { get; set; } = string.Empty;

        [StringLength(2000)]
        public string? Remarks { get; set; }

        [Display(Name = "Status")]
        public string Status { get; set; } = string.Empty;

        public List<IntakeSelectItemViewModel> Intakes { get; set; }
            = new List<IntakeSelectItemViewModel>();
    }

    public class IntakeSelectItemViewModel
    {
        public int IntakeId { get; set; }

        public string IntakeName { get; set; } = string.Empty;

        public string AcademicYear { get; set; } = string.Empty;

        public string IntakePeriod { get; set; } = string.Empty;
    }
}