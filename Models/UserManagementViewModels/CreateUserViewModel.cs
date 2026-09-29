using System.ComponentModel.DataAnnotations;

namespace KISMApplicationManagement.Models.UserManagementViewModels
{
    public class CreateUserViewModel
    {
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

        [Required]
        [StringLength(100, MinimumLength = 8)]
        [DataType(DataType.Password)]
        public string Password { get; set; } = string.Empty;

        [Required]
        [Compare(
            nameof(Password),
            ErrorMessage = "The passwords do not match.")]
        [DataType(DataType.Password)]
        [Display(Name = "Confirm Password")]
        public string ConfirmPassword { get; set; } = string.Empty;

        [Required(ErrorMessage = "Please select at least one role.")]
        [Display(Name = "Roles")]
        public List<string> SelectedRoles { get; set; } = new();

        [Display(Name = "Active")]
        public bool IsActive { get; set; } = true;
    }
}