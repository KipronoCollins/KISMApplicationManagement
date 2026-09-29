using System.ComponentModel.DataAnnotations;

namespace KISMApplicationManagement.Models
{
    public class ApplicantProfile
    {
        public int ApplicantProfileId { get; set; }

        [Required]
        public string UserId { get; set; } = string.Empty;

        public ApplicationUser User { get; set; } = null!;

        [Required]
        [StringLength(200)]
        public string FullName { get; set; } = string.Empty;

        [StringLength(100)]
        public string? IdPassportBirthCertificateNo { get; set; }

        [Phone]
        [StringLength(30)]
        public string? MobileNumber { get; set; }

        [StringLength(250)]
        public string? ContactAddress { get; set; }

        [StringLength(20)]
        public string? AddressCode { get; set; }

        [StringLength(100)]
        public string? Town { get; set; }

        [EmailAddress]
        [StringLength(256)]
        public string? ContactEmail { get; set; }

        [DataType(DataType.Date)]
        public DateTime? DateOfBirth { get; set; }

        [StringLength(150)]
        public string? PlaceOfBirth { get; set; }

        public int? CountyId { get; set; }
        public County? County { get; set; }

        public int? SubcountyId { get; set; }
        public Subcounty? Subcounty { get; set; }

        public int? DivisionId { get; set; }
        public Division? Division { get; set; }

        public int? LocationId { get; set; }
        public Location? Location { get; set; }

        public int? SublocationId { get; set; }
        public Sublocation? Sublocation { get; set; }

        [StringLength(50)]
        public string? MaritalStatus { get; set; }

        [StringLength(200)]
        public string? ParentGuardianName { get; set; }

        [Phone]
        [StringLength(30)]
        public string? ParentGuardianMobile { get; set; }

        [Phone]
        [StringLength(30)]
        public string? AlternativeContact { get; set; }

        public DateTime CreatedAtUtc { get; set; }

        public DateTime? UpdatedAtUtc { get; set; }
    }
}