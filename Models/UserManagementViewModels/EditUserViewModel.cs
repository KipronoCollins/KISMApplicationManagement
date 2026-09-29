using System.ComponentModel.DataAnnotations;

namespace KISMApplicationManagement.Models.UserManagementViewModels
{
    public class EditUserViewModel
    {
        public string UserId { get; set; } = string.Empty;

        [Required]
        [StringLength(100)]
        [Display(Name = "First Name")]
        public string FirstName { get; set; } = string.Empty;

        [StringLength(100)]
        [Display(Name = "Middle Name")]
        public string? MiddleName { get; set; }

        [Required]
        [StringLength(100)]
        [Display(Name = "Last Name")]
        public string LastName { get; set; } = string.Empty;

        [Required]
        [StringLength(50)]
        [Display(Name = "Username")]
        public string UserName { get; set; } = string.Empty;

        [Required]
        [EmailAddress]
        [StringLength(256)]
        public string Email { get; set; } = string.Empty;

        [Phone]
        [StringLength(30)]
        [Display(Name = "Phone Number")]
        public string? PhoneNumber { get; set; }

        [Display(Name = "Email Confirmed")]
        public bool EmailConfirmed { get; set; }

        [Display(Name = "Active")]
        public bool IsActive { get; set; }
    }
}