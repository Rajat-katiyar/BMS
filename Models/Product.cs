using System.ComponentModel.DataAnnotations;

namespace BusinessManagementSystem.Models
{
    public class Product
    {
        [Key]
        public int ProductId { get; set; }

        [Required]
        [StringLength(100)]
        public string ProductName { get; set; } = string.Empty;

        [StringLength(500)]
        public string? Description { get; set; }

        [Required]
        [StringLength(50)]
        public string SKU { get; set; } = string.Empty;

        [Required]
        public decimal Price { get; set; }

        public int CurrentStock { get; set; } = 0;

        public int MinimumStockLevel { get; set; } = 10;

        [StringLength(50)]
        public string? Category { get; set; }

        [StringLength(50)]
        public string? Unit { get; set; }

        // Navigation properties
        public virtual ICollection<Purchase> Purchases { get; set; } = new List<Purchase>();
        public virtual ICollection<StockLedger> StockLedgers { get; set; } = new List<StockLedger>();

        public DateTime CreatedDate { get; set; } = DateTime.Now;

        public bool IsActive { get; set; } = true;
    }
}
