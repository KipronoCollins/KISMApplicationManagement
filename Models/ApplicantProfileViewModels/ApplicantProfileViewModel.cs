using System.ComponentModel.DataAnnotations;

namespace KISMApplicationManagement.Models.ApplicantProfileViewModels
{
    public class ApplicantProfileViewModel
    {
        public int ApplicantProfileId { get; set; }

        // =========================================================
        // Account / Personal Information
        // Stored in ApplicationUser
        // =========================================================

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
        [Phone]
        [StringLength(30)]
        [Display(Name = "Phone Number")]
        public string PhoneNumber { get; set; } = string.Empty;

        [Display(Name = "Email")]
        public string Email { get; set; } = string.Empty;

        // =========================================================
        // Applicant Profile Information
        // =========================================================

        [StringLength(100)]
        [Display(Name = "ID / Passport / Birth Certificate No.")]
        public string? IdPassportBirthCertificateNo { get; set; }

        [Phone]
        [StringLength(30)]
        [Display(Name = "Alternative Mobile Number")]
        public string? MobileNumber { get; set; }

        [StringLength(250)]
        [Display(Name = "Contact Address")]
        public string? ContactAddress { get; set; }

        [StringLength(20)]
        [Display(Name = "Address Code")]
        public string? AddressCode { get; set; }

        [StringLength(100)]
        [Display(Name = "Town")]
        public string? Town { get; set; }

        [EmailAddress]
        [StringLength(256)]
        [Display(Name = "Alternative Contact Email")]
        public string? ContactEmail { get; set; }

        [DataType(DataType.Date)]
        [Display(Name = "Date of Birth")]
        public DateTime? DateOfBirth { get; set; }

        [StringLength(150)]
        [Display(Name = "Place of Birth")]
        public string? PlaceOfBirth { get; set; }

        [Display(Name = "County")]
        public int? CountyId { get; set; }

        [Display(Name = "Subcounty")]
        public int? SubcountyId { get; set; }

        [Display(Name = "Division")]
        public int? DivisionId { get; set; }

        [Display(Name = "Location")]
        public int? LocationId { get; set; }

        [Display(Name = "Sublocation")]
        public int? SublocationId { get; set; }

        [StringLength(50)]
        [Display(Name = "Marital Status")]
        public string? MaritalStatus { get; set; }

        [StringLength(200)]
        [Display(Name = "Parent / Guardian Name")]
        public string? ParentGuardianName { get; set; }

        [Phone]
        [StringLength(30)]
        [Display(Name = "Parent / Guardian Mobile")]
        public string? ParentGuardianMobile { get; set; }

        [Phone]
        [StringLength(30)]
        [Display(Name = "Alternative Contact")]
        public string? AlternativeContact { get; set; }

        // =========================================================
        // Dropdown Data
        // =========================================================

        public List<CountyOptionViewModel> Counties { get; set; } = new();
    }

    public class CountyOptionViewModel
    {
        public int CountyId { get; set; }

        public string CountyName { get; set; } = string.Empty;
    }
}