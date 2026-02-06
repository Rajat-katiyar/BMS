using System.ComponentModel.DataAnnotations;

namespace BusinessManagementSystem.Models
{
    public class Vendor
    {
        [Key]
        public int VendorId { get; set; }

        [Required]
        [StringLength(100)]
        public string VendorName { get; set; } = string.Empty;

        [StringLength(100)]
        public string? ContactPerson { get; set; }

        [StringLength(50)]
        public string? Phone { get; set; }

        [StringLength(100)]
        public string? Email { get; set; }

        [StringLength(500)]
        public string? Address { get; set; }

        [StringLength(100)]
        public string? City { get; set; }

        [StringLength(100)]
        public string? Country { get; set; }

        [StringLength(20)]
        public string? PostalCode { get; set; }

        // Navigation property
        public virtual ICollection<Purchase> Purchases { get; set; } = new List<Purchase>();

        public DateTime CreatedDate { get; set; } = DateTime.Now;

        public bool IsActive { get; set; } = true;
    }
}
