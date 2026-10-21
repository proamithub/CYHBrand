using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CYHBrandData.Entities
{
    [Table("AuditLog")]
    public class AuditLog
    {
        [Key]
        public long AuditId { get; set; }
        public int? DoctorId { get; set; }
        public string? Action { get; set; }
        public string? Description { get; set; }
        public string? IpAddress { get; set; }
        public string? UserAgent { get; set; }
        public DateTime CreatedDate { get; set; } = DateTime.UtcNow;
    }
}
