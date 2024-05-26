using System.ComponentModel.DataAnnotations;

namespace SupportApp.Models
{
    public class TaskItem
    {
        [Key]
        public int Id { get; set; }
        [Required]
        [StringLength(1000)]
        public string TaskItemTitle { get; set; }

        public string? AssignedTo { get; set; }

        public string? Status { get; set; }

        public DateTime? CreatedAt { get; set; }

        public DateTime? UpdatedAt { get; set; }

        public string? CreatedBy { get; set; }

        public string? UpdatedBy { get; set; }

        [StringLength(100)]
        public string? Remarks { get; set; }
    }
}
