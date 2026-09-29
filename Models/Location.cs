using System.ComponentModel.DataAnnotations;

namespace KISMApplicationManagement.Models
{
    public class Location
    {
        public int LocationId { get; set; }

        [Required]
        public int DivisionId { get; set; }

        public Division Division { get; set; } = null!;

        [Required]
        [StringLength(100)]
        public string LocationName { get; set; } = string.Empty;

        public bool IsActive { get; set; } = true;

        public DateTime CreatedAtUtc { get; set; }

        public DateTime? UpdatedAtUtc { get; set; }

        public ICollection<Sublocation> Sublocations { get; set; } = new List<Sublocation>();
    }
}