using System.ComponentModel.DataAnnotations;

namespace KISMApplicationManagement.Models.ProgrammeRequirementViewModels
{
    public class ProgrammeRequirementViewModel
    {
        public int ProgrammeRequirementId { get; set; }

        [Required]
        public int ProgrammeRequirementGroupId { get; set; }

        [Required]
        [StringLength(50)]
        [Display(Name = "Requirement Type")]
        public string RequirementType { get; set; } = string.Empty;

        [Required]
        [StringLength(150)]
        [Display(Name = "Requirement Name")]
        public string RequirementName { get; set; } = string.Empty;

        [StringLength(100)]
        [Display(Name = "Subject")]
        public string? SubjectName { get; set; }

        [StringLength(20)]
        [Display(Name = "Comparison Operator")]
        public string? ComparisonOperator { get; set; }

        [StringLength(50)]
        [Display(Name = "Required Value")]
        public string? RequiredValue { get; set; }

        [StringLength(500)]
        public string? Description { get; set; }

        [Range(0, 10000)]
        [Display(Name = "Display Order")]
        public int DisplayOrder { get; set; }

        [Display(Name = "Active")]
        public bool IsActive { get; set; } = true;

        public string GroupName { get; set; } = string.Empty;
    }
}