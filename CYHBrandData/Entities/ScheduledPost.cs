using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CYHBrandData.Entities
{
    [Table("ScheduledPost")]
    public class ScheduledPost
    {
        [Key]
        public int PostId { get; set; }
        public int? DoctorId { get; set; }
        public int? DesignId { get; set; }
        public string? Platform { get; set; }           // Instagram | Facebook | LinkedIn
        public string? Caption { get; set; }
        public string Status { get; set; } = "Pending"; // Pending | Scheduled | Published | Failed | Cancelled
        public DateTime? ScheduledAt { get; set; }
        public DateTime? PublishedAt { get; set; }
        public string? PlatformPostId { get; set; }
        public string? ErrorMessage { get; set; }
        public bool IsDeleted { get; set; } = false;
        public DateTime CreatedDate { get; set; } = DateTime.UtcNow;
    }
}
