using System.ComponentModel.DataAnnotations;

namespace BusinessManagementSystem.Models
{
    public class LeaveType
    {
        [Key]
        public int LeaveTypeId { get; set; }

        [Required]
        [StringLength(50)]
        public string LeaveName { get; set; } = string.Empty; // Casual, Sick, Paid

        public int MaxDays { get; set; }
    }
}
