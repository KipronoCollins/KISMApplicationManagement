using System.ComponentModel.DataAnnotations;

namespace KISMApplicationManagement.Models
{
    public class Subcounty
    {
        public int SubcountyId { get; set; }

        [Required]
        public int CountyId { get; set; }

        public County County { get; set; } = null!;

        [Required]
        [StringLength(100)]
        public string SubcountyName { get; set; } = string.Empty;

        public bool IsActive { get; set; } = true;

        public DateTime CreatedAtUtc { get; set; }

        public DateTime? UpdatedAtUtc { get; set; }

        public ICollection<Division> Divisions { get; set; } = new List<Division>();
    }
}