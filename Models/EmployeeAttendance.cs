using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BusinessManagementSystem.Models
{
    public class EmployeeAttendance
    {
        [Key]
        public int AttendanceId { get; set; }

        public int EmployeeId { get; set; }

        [Required]
        public DateTime AttendanceDate { get; set; }

        public DateTime? CheckInTime { get; set; }

        public DateTime? CheckOutTime { get; set; }

        [Column(TypeName = "decimal(5, 2)")]
        public decimal? WorkingHours { get; set; }

        [StringLength(20)]
        public string? Status { get; set; } // Present, Absent, HalfDay

        public DateTime CreatedOn { get; set; } = DateTime.Now;

        // Navigation property
        [ForeignKey("EmployeeId")]
        public virtual Employee? Employee { get; set; }
    }
}
