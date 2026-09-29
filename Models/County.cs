using System.ComponentModel.DataAnnotations;

namespace KISMApplicationManagement.Models
{
    public class County
    {
        public int CountyId { get; set; }

        [Required]
        [Range(1, 47, ErrorMessage = "County number must be between 1 and 47.")]
        public int CountyNumber { get; set; }

        [Required]
        [StringLength(100)]
        public string CountyName { get; set; } = string.Empty;

        public bool IsActive { get; set; } = true;

        public DateTime CreatedAtUtc { get; set; }

        public DateTime? UpdatedAtUtc { get; set; }

        public ICollection<Subcounty> Subcounties { get; set; } = new List<Subcounty>();
    }
}