namespace KISMApplicationManagement.Models.UserManagementViewModels
{
    public class UserDetailsViewModel
    {
        public string UserId { get; set; } = string.Empty;

        public string FirstName { get; set; } = string.Empty;

        public string? MiddleName { get; set; }

        public string LastName { get; set; } = string.Empty;

        public string UserName { get; set; } = string.Empty;

        public string Email { get; set; } = string.Empty;

        public string PhoneNumber { get; set; } = string.Empty;

        public bool EmailConfirmed { get; set; }

        public bool IsActive { get; set; }

        public List<string> Roles { get; set; } = new();

        public DateTime CreatedAtUtc { get; set; }

        public DateTime? UpdatedAtUtc { get; set; }

        public DateTime? LastLoginAtUtc { get; set; }
    }
}