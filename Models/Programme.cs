using System.ComponentModel.DataAnnotations;

namespace KISMApplicationManagement.Models
{
    public class Programme
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

        [Range(1, 10)]
        [Display(Name = "Duration (Years)")]
        public int DurationYears { get; set; }

        [StringLength(2000)]
        public string? Description { get; set; }

        [Display(Name = "Display Order")]
        public int DisplayOrder { get; set; }

        public bool IsActive { get; set; } = true;

        public DateTime CreatedAtUtc { get; set; }

        public DateTime? UpdatedAtUtc { get; set; }

        public ICollection<ProgrammeRequirementGroup> RequirementGroups { get; set; }
            = new List<ProgrammeRequirementGroup>();
    }
}