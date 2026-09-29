using System.ComponentModel.DataAnnotations;

namespace KISMApplicationManagement.Models
{
    public class CommitteeSittingApplication
    {
        public int CommitteeSittingApplicationId { get; set; }

        [Required]
        public int CommitteeSittingId { get; set; }

        public CommitteeSitting CommitteeSitting { get; set; } = null!;

        [Required]
        public int ApplicationId { get; set; }

        public Application Application { get; set; } = null!;

        [Range(1, int.MaxValue)]
        [Display(Name = "Display Order")]
        public int DisplayOrder { get; set; }

        [Required]
        public DateTime AddedAtUtc { get; set; }

        [Required]
        public string AddedByUserId { get; set; } = string.Empty;

        public ApplicationUser AddedByUser { get; set; } = null!;

        public CommitteeDecision? Decision { get; set; }
    }
}