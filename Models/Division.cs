using System.ComponentModel.DataAnnotations;

namespace KISMApplicationManagement.Models
{
    public class Division
    {
        public int DivisionId { get; set; }

        [Required]
        public int SubcountyId { get; set; }

        public Subcounty Subcounty { get; set; } = null!;

        [Required]
        [StringLength(100)]
        public string DivisionName { get; set; } = string.Empty;

        public bool IsActive { get; set; } = true;

        public DateTime CreatedAtUtc { get; set; }

        public DateTime? UpdatedAtUtc { get; set; }

        public ICollection<Location> Locations { get; set; } = new List<Location>();
    }
}