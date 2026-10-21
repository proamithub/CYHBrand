using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CYHBrandData.Entities
{
    [Table("BannerTemplate")]
    public class BannerTemplate
    {
        [Key]
        public int TemplateId { get; set; }
        public int? CategoryId { get; set; }
        public string? TemplateName { get; set; }
        public string? Description { get; set; }
        public string? Tags { get; set; }
        public string? ThumbnailPath { get; set; }
        public string? TemplateJson { get; set; }
        public string? Orientation { get; set; }   // Square | Landscape | Portrait
        public int? WidthPx { get; set; }
        public int? HeightPx { get; set; }
        public bool IsPremium { get; set; } = false;
        public bool IsActive { get; set; } = true;
        public int UsageCount { get; set; } = 0;
        public DateTime CreatedDate { get; set; } = DateTime.UtcNow;
        public int? CreatedBy { get; set; }
    }
}
