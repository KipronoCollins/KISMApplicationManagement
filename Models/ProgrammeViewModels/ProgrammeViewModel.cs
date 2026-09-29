using System.ComponentModel.DataAnnotations;

namespace KISMApplicationManagement.Models.ProgrammeViewModels
{
    public class ProgrammeViewModel
    {
        public int ProgrammeId { get; set; }

        [Required]
        [StringLength(30)]
        [Display(Name = "Programme Code")]
        public string ProgrammeCode { get; set; } = string.Empty;

        [Required]
        [StringLength(200)]
        [Display(Name = "Programme Name")]
        public string ProgrammeName { get; set; } = string.Empty;

        [Required]
        [StringLength(50)]
        [Display(Name = "Award Type")]
        public string AwardType { get; set; } = string.Empty;

        [Required]
        [Range(
            1,
            10,
            ErrorMessage = "Duration must be between 1 and 10 years.")]
        [Display(Name = "Duration (Years)")]
        public int DurationYears { get; set; }

        [StringLength(2000)]
        public string? Description { get; set; }

        [Range(0, 10000)]
        [Display(Name = "Display Order")]
        public int DisplayOrder { get; set; }

        [Display(Name = "Active")]
        public bool IsActive { get; set; } = true;
    }
}