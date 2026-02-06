using System.ComponentModel.DataAnnotations;

namespace BusinessManagementSystem.Models
{
    public class StockLedger
    {
        [Key]
        public int LedgerId { get; set; }

        [Required]
        public int ProductId { get; set; }

        [Required]
        [StringLength(20)]
        public string TransactionType { get; set; } = string.Empty; // "IN" or "OUT"

        [Required]
        public int Quantity { get; set; }

        public int BalanceQuantity { get; set; }

        [StringLength(200)]
        public string? Reference { get; set; }

        [StringLength(500)]
        public string? Remarks { get; set; }

        [Required]
        public DateTime TransactionDate { get; set; } = DateTime.Now;

        public int? CreatedBy { get; set; }

        // Navigation property
        public virtual Product? Product { get; set; }

        public DateTime CreatedDate { get; set; } = DateTime.Now;
    }
}
