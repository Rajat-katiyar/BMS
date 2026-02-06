using System.ComponentModel.DataAnnotations;

namespace BusinessManagementSystem.Models
{
    public class Purchase
    {
        [Key]
        public int PurchaseId { get; set; }

        [Required]
        public int VendorId { get; set; }

        [Required]
        public int ProductId { get; set; }

        [Required]
        public int Quantity { get; set; }

        [Required]
        public decimal UnitPrice { get; set; }

        public decimal TotalAmount { get; set; }

        [Required]
        public DateTime PurchaseDate { get; set; } = DateTime.Now;

        [StringLength(100)]
        public string? InvoiceNumber { get; set; }

        [StringLength(500)]
        public string? Notes { get; set; }

        // Navigation properties
        public virtual Vendor? Vendor { get; set; }
        public virtual Product? Product { get; set; }

        public DateTime CreatedDate { get; set; } = DateTime.Now;
    }
}
