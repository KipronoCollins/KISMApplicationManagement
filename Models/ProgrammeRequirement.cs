using System.ComponentModel.DataAnnotations;

namespace KISMApplicationManagement.Models
{
    public class ProgrammeRequirement
    {
        public int ProgrammeRequirementId { get; set; }

        [Required]
        public int ProgrammeRequirementGroupId { get; set; }

        public ProgrammeRequirementGroup RequirementGroup { get; set; }
            = null!;

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
        [Display(Name = "Operator")]
        public string? ComparisonOperator { get; set; }

        [StringLength(50)]
        [Display(Name = "Required Value")]
        public string? RequiredValue { get; set; }

        [StringLength(500)]
        public string? Description { get; set; }

        [Display(Name = "Display Order")]
        public int DisplayOrder { get; set; }

        public bool IsActive { get; set; } = true;

        public DateTime CreatedAtUtc { get; set; }

        public DateTime? UpdatedAtUtc { get; set; }
    }
}