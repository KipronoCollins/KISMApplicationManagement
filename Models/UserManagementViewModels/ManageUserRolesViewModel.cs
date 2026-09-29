using System.ComponentModel.DataAnnotations;

namespace KISMApplicationManagement.Models.UserManagementViewModels
{
    public class ManageUserRolesViewModel
    {
        public string UserId { get; set; } = string.Empty;

        public string UserDisplayName { get; set; } = string.Empty;

        public string UserName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Please select at least one role.")]
        [Display(Name = "Roles")]
        public List<string> SelectedRoles { get; set; } = new();

        public List<RoleSelectionViewModel> AvailableRoles { get; set; } = new();
    }

    public class RoleSelectionViewModel
    {
        public string RoleName { get; set; } = string.Empty;

        public bool Selected { get; set; }
    }
}