using System.ComponentModel.DataAnnotations;
using KISMApplicationManagement.Constants;

namespace KISMApplicationManagement.Models
{
    public class Application
    {
        public int ApplicationId { get; set; }

        [Required]
        [StringLength(30)]
        [Display(Name = "Application Number")]
        public string ApplicationNumber { get; set; } = string.Empty;

        [Required]
        public int ApplicantProfileId { get; set; }

        public ApplicantProfile ApplicantProfile { get; set; } = null!;

        [Required]
        public int IntakeId { get; set; }

        public Intake Intake { get; set; } = null!;

        [Required]
        [StringLength(30)]
        public string Status { get; set; } = ApplicationStatuses.Draft;

        public DateTime? SubmittedAtUtc { get; set; }

        public string? SubmittedByUserId { get; set; }

        public ApplicationUser? SubmittedByUser { get; set; }

        public DateTime CreatedAtUtc { get; set; }

        public DateTime? UpdatedAtUtc { get; set; }

        public ICollection<ApplicationProgrammeChoice> ProgrammeChoices { get; set; }
            = new List<ApplicationProgrammeChoice>();
    }
}