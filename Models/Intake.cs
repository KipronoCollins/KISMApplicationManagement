using System.ComponentModel.DataAnnotations;

namespace KISMApplicationManagement.Models
{
    public class Intake
    {
        public int IntakeId { get; set; }

        [Required]
        [StringLength(100)]
        [Display(Name = "Intake Name")]
        public string IntakeName { get; set; } = string.Empty;

        [Required]
        [StringLength(20)]
        [Display(Name = "Academic Year")]
        public string AcademicYear { get; set; } = string.Empty;

        [Required]
        [StringLength(30)]
        [Display(Name = "Intake Period")]
        public string IntakePeriod { get; set; } = string.Empty;

        [DataType(DataType.Date)]
        [Display(Name = "Application Opening Date")]
        public DateTime ApplicationOpeningDate { get; set; }

        [DataType(DataType.Date)]
        [Display(Name = "Application Closing Date")]
        public DateTime ApplicationClosingDate { get; set; }

        [DataType(DataType.Date)]
        [Display(Name = "Admission Start Date")]
        public DateTime? AdmissionStartDate { get; set; }

        [DataType(DataType.Date)]
        [Display(Name = "Admission End Date")]
        public DateTime? AdmissionEndDate { get; set; }

        [StringLength(1000)]
        public string? Description { get; set; }

        [Display(Name = "Display Order")]
        public int DisplayOrder { get; set; }

        public bool IsActive { get; set; } = true;

        [Display(Name = "Applications Open")]
        public bool ApplicationsOpen { get; set; } = false;

        public DateTime CreatedAtUtc { get; set; }

        public DateTime? UpdatedAtUtc { get; set; }
    }
}