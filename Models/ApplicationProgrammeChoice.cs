using System.ComponentModel.DataAnnotations;

namespace KISMApplicationManagement.Models
{
    public class ApplicationProgrammeChoice
    {
        public int ApplicationProgrammeChoiceId { get; set; }

        [Required]
        public int ApplicationId { get; set; }

        public Application Application { get; set; } = null!;

        [Required]
        public int ProgrammeId { get; set; }

        public Programme Programme { get; set; } = null!;

        [Range(1, 2)]
        [Display(Name = "Choice")]
        public int ChoiceNumber { get; set; }

        public DateTime CreatedAtUtc { get; set; }
    }
}