namespace KISMApplicationManagement.Models.ApplicantDashboardViewModels
{
    public class ApplicantDashboardViewModel
    {
        public string FirstName { get; set; } = string.Empty;

        public string LastName { get; set; } = string.Empty;

        public string Email { get; set; } = string.Empty;

        public string? PhoneNumber { get; set; }

        public bool ProfileComplete { get; set; }

        public string FullName =>
            string.Join(
                " ",
                new[]
                {
                    FirstName,
                    LastName
                }
                .Where(value =>
                    !string.IsNullOrWhiteSpace(value)));
    }
}