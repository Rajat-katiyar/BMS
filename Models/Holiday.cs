using System.ComponentModel.DataAnnotations;

namespace BusinessManagementSystem.Models
{
    public class Holiday
    {
        [Key]
        public int HolidayId { get; set; }

        [Required]
        public DateTime HolidayDate { get; set; }

        [Required]
        [StringLength(150)]
        public string HolidayName { get; set; } = string.Empty;

        public bool IsActive { get; set; } = true;

        public DateTime CreatedOn { get; set; } = DateTime.Now;
    }
}
