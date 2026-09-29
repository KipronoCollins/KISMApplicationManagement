using System.ComponentModel.DataAnnotations;
using KISMApplicationManagement.Constants;

namespace KISMApplicationManagement.Models
{
    public class CommitteeDecision
    {
        public int CommitteeDecisionId { get; set; }

        [Required]
        public int CommitteeSittingApplicationId { get; set; }

        public CommitteeSittingApplication CommitteeSittingApplication { get; set; }
            = null!;

        [Required]
        [StringLength(50)]
        [Display(Name = "Decision")]
        public string Decision { get; set; } = string.Empty;

        [Display(Name = "Assigned Programme")]
        public int? AssignedProgrammeId { get; set; }

        public Programme? AssignedProgramme { get; set; }

        [StringLength(2000)]
        public string? Remarks { get; set; }

        [Required]
        public DateTime DecisionDateUtc { get; set; }

        [Required]
        public string RecordedByUserId { get; set; } = string.Empty;

        public ApplicationUser RecordedByUser { get; set; } = null!;

        public DateTime CreatedAtUtc { get; set; }

        public DateTime? UpdatedAtUtc { get; set; }
    }
}