using System.ComponentModel.DataAnnotations;

namespace KISMApplicationManagement.Models
{
    public class Sublocation
    {
        public int SublocationId { get; set; }

        [Required]
        public int LocationId { get; set; }

        public Location Location { get; set; } = null!;

        [Required]
        [StringLength(100)]
        public string SublocationName { get; set; } = string.Empty;

        public bool IsActive { get; set; } = true;

        public DateTime CreatedAtUtc { get; set; }

        public DateTime? UpdatedAtUtc { get; set; }
    }
}