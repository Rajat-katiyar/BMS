using System.ComponentModel.DataAnnotations;

namespace BusinessManagementSystem.Models
{
    public class Employee
    {
        [Key]
        public int EmployeeId { get; set; }

        [Required]
        [StringLength(50)]
        public string EmployeeCode { get; set; } = string.Empty; // e.g., EMP001

        [Required]
        [StringLength(150)]
        public string EmployeeName { get; set; } = string.Empty;

        [StringLength(150)]
        [EmailAddress]
        public string? Email { get; set; }

        [StringLength(20)]
        public string? Phone { get; set; }

        [StringLength(100)]
        public string? Designation { get; set; }

        [Range(0, 999999999)]
        public decimal Salary { get; set; }

        public bool IsActive { get; set; } = true;

        public DateTime CreatedOn { get; set; } = DateTime.Now;
    }
}
