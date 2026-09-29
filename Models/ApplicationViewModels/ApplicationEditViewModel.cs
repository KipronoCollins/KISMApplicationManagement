using System.ComponentModel.DataAnnotations;

namespace KISMApplicationManagement.Models.ApplicationViewModels
{
    public class ApplicationEditViewModel
    {
        public int ApplicationId { get; set; }

        [Required]
        [Display(Name = "First Programme Choice")]
        public int FirstChoiceProgrammeId { get; set; }

        [Required]
        [Display(Name = "Second Programme Choice")]
        public int SecondChoiceProgrammeId { get; set; }
    }
}