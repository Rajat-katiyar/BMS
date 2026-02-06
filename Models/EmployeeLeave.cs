using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BusinessManagementSystem.Models
{
    public class EmployeeLeave
    {
        [Key]
        public int LeaveId { get; set; }

        public int EmployeeId { get; set; }

        public int LeaveTypeId { get; set; }

        [Required]
        public DateTime FromDate { get; set; }

        [Required]
        public DateTime ToDate { get; set; }

        public int TotalDays { get; set; }

        [StringLength(300)]
        public string? Reason { get; set; }

        [StringLength(20)]
        public string Status { get; set; } = "Pending"; // Pending, Approved, Rejected

        public DateTime AppliedOn { get; set; } = DateTime.Now;

        // Navigation properties
        [ForeignKey("EmployeeId")]
        public virtual Employee? Employee { get; set; }

        [ForeignKey("LeaveTypeId")]
        public virtual LeaveType? LeaveType { get; set; }
    }
}
