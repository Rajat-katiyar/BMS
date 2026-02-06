using System.ComponentModel.DataAnnotations;

namespace BusinessManagementSystem.Models
{
    public class User
    {
        [Key]
        public int UserId { get; set; }

        [Required]
        [StringLength(100)]
        public string Username { get; set; } = string.Empty;

        [Required]
        [StringLength(255)]
        public string Password { get; set; } = string.Empty;

        [Required]
        [StringLength(100)]
        public string Email { get; set; } = string.Empty;

        [StringLength(50)]
        public string? Phone { get; set; }

        public int RoleId { get; set; }

        public int? EmployeeId { get; set; }

        // Navigation property
        public virtual Role? Role { get; set; }

        [System.ComponentModel.DataAnnotations.Schema.ForeignKey("EmployeeId")]
        public virtual Employee? Employee { get; set; }

        public DateTime CreatedDate { get; set; } = DateTime.Now;

        public bool IsActive { get; set; } = true;
    }
}
