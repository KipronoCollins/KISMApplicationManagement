using System.ComponentModel.DataAnnotations;

namespace KISMApplicationManagement.Models.ProgrammeRequirementViewModels
{
    public class ProgrammeRequirementGroupViewModel
    {
        public int ProgrammeRequirementGroupId { get; set; }

        [Required]
        public int ProgrammeId { get; set; }

        [Required]
        [StringLength(150)]
        [Display(Name = "Group Name")]
        public string GroupName { get; set; } = string.Empty;

        [StringLength(500)]
        public string? Description { get; set; }

        [Required]
        [Display(Name = "Selection Rule")]
        public string SelectionRule { get; set; } = "ALL";

        [Required]
        [Range(
            1,
            100,
            ErrorMessage =
                "Minimum required must be between 1 and 100.")]
        [Display(Name = "Minimum Required")]
        public int MinimumRequired { get; set; } = 1;

        [Range(0, 10000)]
        [Display(Name = "Display Order")]
        public int DisplayOrder { get; set; }

        [Display(Name = "Active")]
        public bool IsActive { get; set; } = true;

        public string ProgrammeName { get; set; } = string.Empty;
    }
}