using System.ComponentModel.DataAnnotations;

namespace KISMApplicationManagement.Models
{
    public class ProgrammeRequirementGroup
    {
        public int ProgrammeRequirementGroupId { get; set; }

        [Required]
        public int ProgrammeId { get; set; }

        public Programme Programme { get; set; } = null!;

        // =========================================================
        // Parent / Child Group Hierarchy
        // =========================================================

        /// <summary>
        /// Null for a top-level requirement group.
        /// A value means this group is a child of another
        /// requirement group.
        /// </summary>
        public int? ParentGroupId { get; set; }

        public ProgrammeRequirementGroup? ParentGroup { get; set; }

        public ICollection<ProgrammeRequirementGroup> ChildGroups { get; set; }
            = new List<ProgrammeRequirementGroup>();

        // =========================================================
        // Group Information
        // =========================================================

        [Required]
        [StringLength(150)]
        [Display(Name = "Group Name")]
        public string GroupName { get; set; } = string.Empty;

        [StringLength(500)]
        public string? Description { get; set; }

        // =========================================================
        // Selection Logic
        // =========================================================

        /// <summary>
        /// Determines how this group's child groups or requirements
        /// are evaluated.
        ///
        /// ALL = every child must be satisfied.
        /// ANY = at least the configured minimum number must be satisfied.
        /// </summary>
        [Required]
        [StringLength(20)]
        [Display(Name = "Selection Rule")]
        public string SelectionRule { get; set; } = "ALL";

        /// <summary>
        /// Minimum number of child groups/requirements that must
        /// be satisfied when SelectionRule is ANY.
        ///
        /// For ALL groups this is normally 1 and is not used
        /// as the selection threshold.
        /// </summary>
        [Range(1, 100)]
        [Display(Name = "Minimum Required")]
        public int MinimumRequired { get; set; } = 1;

        // =========================================================
        // Ordering / Status
        // =========================================================

        [Display(Name = "Display Order")]
        public int DisplayOrder { get; set; }

        public bool IsActive { get; set; } = true;

        // =========================================================
        // Audit
        // =========================================================

        public DateTime CreatedAtUtc { get; set; }

        public DateTime? UpdatedAtUtc { get; set; }

        // =========================================================
        // Requirements
        // =========================================================

        public ICollection<ProgrammeRequirement> Requirements { get; set; }
            = new List<ProgrammeRequirement>();
    }
}