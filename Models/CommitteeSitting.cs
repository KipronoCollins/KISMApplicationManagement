using System.ComponentModel.DataAnnotations;
using KISMApplicationManagement.Constants;

namespace KISMApplicationManagement.Models
{
    public class CommitteeSitting
    {
        public int CommitteeSittingId { get; set; }

        [Required]
        public int IntakeId { get; set; }

        public Intake Intake { get; set; } = null!;

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

        [Required]
        [StringLength(30)]
        public string Status { get; set; } = CommitteeSittingStatuses.Scheduled;

        public DateTime CreatedAtUtc { get; set; }

        [Required]
        public string CreatedByUserId { get; set; } = string.Empty;

        public ApplicationUser CreatedByUser { get; set; } = null!;

        public DateTime? StartedAtUtc { get; set; }

        public DateTime? CompletedAtUtc { get; set; }

        public ICollection<CommitteeSittingApplication> Applications { get; set; }
            = new List<CommitteeSittingApplication>();
    }
}